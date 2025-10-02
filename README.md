
# 📝 Guía de Configuración de Git Hook con Gemini y Checklist

Este instructivo explica cómo configurar un **Git Hook (`commit-msg`)** que ejecuta un validador de checklist usando **Gemini API**.  

La validación es **opcional**: solo se activa si el mensaje del commit contiene la palabra clave `#checkList`.  
Si no está presente, el commit se realiza normalmente.

---

## 📂 Estructura esperada de archivos externos

Todos los archivos necesarios para el checklist estarán en una carpeta **fuera del repositorio**.  
Ejemplo:

```
D:/ChecklistCommit/
├── program/
│   └── Checklist.exe           # Ejecutable .NET que llama a Gemini
├── angular/
│   ├── checklist.md             # Checklist para Angular
│   └── checkcommit_report.md    # Reporte generado automáticamente
└── dotnet/
├── checklist.md             # Checklist para .NET
└── checkcommit_report.md    # Reporte generado automáticamente

```
---

## ⚙️ Configuración del Git Hook

### 1. Copiar el hook
- Copia el archivo `commit-msg` en la carpeta del repo:


tu-proyecto/.git/hooks/commit-msg



### 2. Contenido del hook (`.git/hooks/commit-msg`)

```sh
#!/bin/sh
# ==========================
# Git Hook: commit-msg
# Validación opcional con CheckCommit
# Se ejecuta SOLO si el mensaje contiene una palabra clave
# ==========================

# 🔧 Configura tus rutas aquí
CHECKCOMMIT_EXE="D:/ChecklistCommit/program/Checklist.exe"
CHECKLIST_DIR="D:/ChecklistCommit"
REPORT_PATH="D:\ChecklistCommit\dotnet\checkcommit_report.md"

# Palabra clave que activa la validación
KEYWORD="#checkList"

# Obtener el mensaje del commit
COMMIT_MSG_FILE=$1
COMMIT_MSG=$(cat "$COMMIT_MSG_FILE")

# Verificar si contiene la palabra clave
echo "$COMMIT_MSG" | grep -qi "$KEYWORD"
if [ $? -eq 0 ]; then
    echo "🔍 Validando checklist con Gemini..."
	# Cambiar dotnet o angular
    "$CHECKCOMMIT_EXE" dotnet D:/ChecklistCommit
    STATUS=$?

    if [ $STATUS -ne 0 ]; then
        echo "❌ Commit bloqueado: revisa el reporte en:"
        echo "📄 file:///$REPORT_PATH"
        # Abrir carpeta del reporte (Windows)
        explorer $REPORT_PATH
        exit 1
    fi

    echo "✅ Validación completada."
else
    echo "⚡ Checklist omitido (no se encontró la palabra clave '$KEYWORD')."
fi

exit 0

```

### 3. Dar permisos de ejecución

En Linux o macOS:

```sh
chmod +x .git/hooks/commit-msg
```

En Windows con Git Bash, este paso también es necesario.

---

## 🔑 Configuración de Gemini API Key

El ejecutable `Checklist.exe` requiere una **API Key de Gemini** configurada como variable de entorno.

En Windows (PowerShell):

```powershell
setx GEMINI_API_KEY "tu_api_key"
```

En Linux/macOS:

```sh
export GEMINI_API_KEY="tu_api_key"
```

---

## 🚀 Flujo de trabajo

1. Escribes un commit, por ejemplo:

   ```sh
   git commit -m "Fix en el login #checkList"
   ```

2. El hook se ejecuta:

   * Si detecta `#checkList` en el mensaje, valida el código contra el checklist correspondiente (Angular o .NET).
   * Si **no** está la palabra clave, el commit se omite de validación y se aplica normalmente.

3. Si hay observaciones:

   * El commit se bloquea.
   * Se genera el archivo `checkcommit_report.md` en la carpeta externa.
   * El hook abre la carpeta donde está el reporte.

4. Si todo pasa:

   * El commit se completa con éxito.
   * Se muestra la ruta al archivo de reporte.

---

## 📄 Ejemplo de salida

### Caso con errores:

```
🔍 Validando checklist con Gemini...
⚙️ Proyecto .NET detectado
❌ Commit bloqueado: revisa el reporte en:
📄 file:///D:/ChecklistCommit/dotnet/checkcommit_report.md
```

### Caso exitoso:

```
🔍 Validando checklist con Gemini...
📦 Proyecto Angular detectado
✅ Validación completada.
📄 file:///D:/ChecklistCommit/angular/checkcommit_report.md
```

### Caso sin validación:

```
⚡ Checklist omitido (no se encontró la palabra clave '#checkList').
```

---

## 🛠️ Notas importantes

* Los **checklists y reportes** están **fuera del repo** → no se versionan ni afectan `.gitignore`.
* El hook funciona en **Windows (Git Bash), Linux y macOS**.
* Puedes personalizar la palabra clave cambiando la variable:

  ```sh
  KEYWORD="#checkList"
  ```

---

