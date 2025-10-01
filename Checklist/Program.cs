using Checklist;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

// Fuerza UTF-8 como predeterminado para todo
Console.OutputEncoding = Encoding.UTF8;
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

string projectType = args.Length > 0 ? args[0] : "dotnet"; // default 
string commitPath = args.Length > 0 ? args[1] : @"C:\checkcommit";

string basePath = Path.Combine(commitPath,
    projectType
);

string checklistPath = Path.Combine(basePath, "checklist.md");
string reportPath = Path.Combine(basePath, "checkcommit_report.md");

if (!File.Exists(checklistPath))
{
    Console.WriteLine($"⚠️  No se encontró el checklist en {checklistPath}");
    return 1;
}


string? apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("❌ API key no encontrada. Configura GEMINI_API_KEY en las variables de entorno.");
    return 1;
}

var http = new HttpClient();

// 📌 1. Leer checklist.md
var checklist = File.ReadAllText(checklistPath,Encoding.UTF8);

// 📌 2. Obtener diff de Git (cambios staged)
var diff = RunGitCommand("diff --cached");

if (string.IsNullOrWhiteSpace(diff))
{
    Console.WriteLine("⚠️ No hay cambios staged para validar.");
    return 0;
}

// 2. Verificar si los cambios son solo comentarios
bool onlyComments = AreOnlyComments(diff, projectType);

string prompt = "";
if (onlyComments)
{
    prompt = $@"
Eres un revisor de código.
Se han detectado cambios SOLO en COMENTARIOS.
Por favor revisa únicamente la ortografía, acentos correctos en español,
y la claridad de los comentarios agregados o modificados.

Cambios en comentarios:
{diff}";
}
else
{

    // 📌 3. Construir prompt pidiendo formato claro
    prompt = $@"
Eres un revisor de código. 

Tarea:
- Evalúa los cambios en los archivos según el checklist.
- Da retroalimentación agrupada por archivo.
- Solo menciona los puntos que NO cumplen (✖).
- Si un archivo cumple con todo, no lo menciones.
- Sé determinista, intenta no variar la salida si el contenido no cambió.
- Usa saltos de línea para separar los archivos y títulos de los archivos con sus explicaciones.
- Revisa faltas de ortografía, acentos correctos en español en valores de cadenas.

Checklist:
{checklist}

Código modificado (git diff):
{diff}

Formato down de salida esperado (en markdown):
##Archivo: NOMBRE_DEL_ARCHIVO
✖ Punto incumplido 1
✖ Punto incumplido 2
---
#Archivo: OTRO_ARCHIVO
✖ Punto incumplido 1
";
}

var request = new
{
    contents = new[]
    {
        new {
            role = "user",
            parts = new[] { new { text = prompt } }
        }
    },
    generation_config = new
    {
        temperature = 0.3, // Set the desired temperature value (between 0.0 and 2.0)
    }
};

var response = await http.PostAsJsonAsync(
    $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={apiKey}",
    request
);

if (!response.IsSuccessStatusCode)
{
    string errorBody = await response.Content.ReadAsStringAsync();
    Console.WriteLine(errorBody);
    return 1;
}

var body = await response.Content.ReadAsStringAsync();


// 📌 4. Extraer texto de la respuesta JSON
using var doc = JsonDocument.Parse(body);
var rawText = doc.RootElement
    .GetSafe("candidates")?.GetFirst()?
    .GetSafe("content")?
    .GetSafe("parts")?.GetFirst()?
    .GetSafe("text")?
    .GetString();
try
{
    // 📌 5. Guardar salida completa
    File.WriteAllText(reportPath, rawText, Encoding.UTF8);

    // 📌 6. Mostrar salida formateada en consola
    Console.WriteLine($"\nResultados del checklist para {projectType}:\n");

}
catch (Exception ex)
{
    Console.WriteLine("Error:");
    return 1;
}

// 📌 7. Hacer fallar commit si hay incumplimientos
if (!string.IsNullOrWhiteSpace(rawText) && rawText.Contains('✖'))
{
    Console.WriteLine("\n❌ Se encontraron incumplimientos. Revisa checkcommit_report.md para más detalles.");
    return 1; // 🔴 Falla el pre-commit
}
else if(!string.IsNullOrWhiteSpace(rawText))
{
    Console.WriteLine("\n✅ Checklist cumplido. Puedes hacer commit.");
    return 0;
}
else
{
    Console.WriteLine("\nNo se detectaron cambios en el código. Puedes hacer commit.");
    return 0;
}

string RunGitCommand(string args)
{
    var psi = new System.Diagnostics.ProcessStartInfo("git", args)
    {
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };
    var process = System.Diagnostics.Process.Start(psi);
    return process.StandardOutput.ReadToEnd();
}


static bool AreOnlyComments(string diff, string projectType)
{
    // Regex para detectar líneas que NO sean comentarios
    Regex nonCommentPattern;

    if (projectType == "dotnet")
    {
        // En C#: líneas que no empiezan con // ni están dentro de /* */
        nonCommentPattern = new Regex(@"^[\+\-]\s*(?!//)(?!/\*).*\S", RegexOptions.Multiline);
    }
    else
    {
        // En Angular/TS: // o /* */ o <!-- -->
        nonCommentPattern = new Regex(@"^[\+\-]\s*(?!//)(?!/\*|<!--).*\S", RegexOptions.Multiline);
    }

    // Si encuentra al menos una línea que no es comentario → no es solo comentarios
    return !nonCommentPattern.IsMatch(diff);
}