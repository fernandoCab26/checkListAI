# Checklist Mejorado para Revisión de Código C#

Este documento detalla las mejores prácticas y estándares a seguir en proyectos de C# y .NET. Su objetivo es garantizar la calidad, legibilidad, rendimiento y mantenibilidad del código.

---

### ✅ Principios Generales y Calidad del Código

* **Gestión de Dependencias:**
    * **Verificar que no existan referencias a ensamblados o paquetes NuGet que no se estén utilizando.** Esto reduce el tamaño del compilado y evita dependencias innecesarias.
    * **Eliminar las directivas `using` que no son necesarias en cada archivo.** Ayuda a mantener el código limpio y a entender rápidamente las dependencias reales de una clase.

* **Nomenclatura y Estándares:**
    * **Asegurar que los nombres de variables, métodos y clases sigan las convenciones de C#**.
        * `PascalCase` para nombres de clases, métodos, propiedades y eventos.
        * `camelCase` para variables locales y parámetros de métodos.
        * Nombres descriptivos y claros. Evitar abreviaturas ambiguas.
    * **Evitar el uso de nombres de variables en negativo.** Por ejemplo, usar `isValid` en lugar de `isNotValid`, ya que facilita la lectura en condicionales `if (isValid)`.

* **Estructura y Organización:**
    * **Cada archivo debe estar ubicado en la carpeta que le corresponde según su función dentro de la arquitectura del proyecto** (ej. `Controllers`, `Services`, `DTOs`, `Repositories`).
    * **Dentro de una clase, los métodos deben estar agrupados por funcionalidad.** Por ejemplo, métodos públicos primero, seguidos de los privados, o agrupados por la característica que implementan.
    * **Mantener una única instrucción por línea.** No se deben concatenar múltiples instrucciones usando punto y coma en la misma línea para mejorar la legibilidad y facilitar la depuración.

* **Documentación y Comentarios:**
    * **Las clases y métodos públicos deben tener documentación XML (`///`).** Esta debe explicar el propósito, los parámetros (`<param>`), el valor de retorno (`<returns>`) y las excepciones que puede lanzar (`<exception>`).
    * **Revisar que la documentación, comentarios y valores de cadena no contengan errores ortográficos y correcto uso de acentos y puntuaciones**.

* **Legibilidad y Mantenibilidad:**
    * **Garantizar la correcta indentación del código.** El código dentro de un bloque (clase, método, `if`, `for`, etc.) debe estar correctamente sangrado.
    * **Reemplazar "números mágicos" y "cadenas mágicas" por constantes con nombres descriptivos.** En lugar de `if (status == 2)`, usar `const int ApprovedStatus = 2;` y luego `if (status == ApprovedStatus)`.
    * **Aplicar el principio DRY (Don't Repeat Yourself).** Identificar y refactorizar bloques de código duplicados en métodos reutilizables.
    * **Usar el modificador de acceso más restrictivo posible (`private`, `protected`, `internal`).** Exponer públicamente (`public`) solo lo que es estrictamente necesario.

---

### ⚙️ Buenas Prácticas Específicas de C#

* **Manejo de Nulos y Cadenas:**
    * **Para tipos `nullable` (ej. `int?`, `DateTime?`), usar la propiedad `.HasValue` para verificar si tienen un valor,** en lugar de comparar con `null`. Es más explícito y claro en su intención.
    * **Para acceder al valor de un tipo `nullable` que ya ha sido verificado, usar la propiedad `.Value`**.
    * **Utilizar `string.IsNullOrWhiteSpace()` para validar cadenas de texto.** Este método es más robusto que `string.IsNullOrEmpty` o `cadena == ""`, ya que también detecta cadenas que solo contienen espacios en blanco.
    * **Al dividir cadenas con `String.Split`, usar la sobrecarga `StringSplitOptions.RemoveEmptyEntries`** para evitar elementos vacíos en el resultado.
    * **Preferir la interpolación de cadenas (`$"{variable}"`)** sobre la concatenación con `+` por ser más legible y eficiente.

* **Conversión de Tipos y Lógica de Control:**
    * **Usar `TryParse()` (ej. `int.TryParse()`) para convertir texto a tipos numéricos u otros tipos.** A diferencia de `Parse` o `Convert`, `TryParse` no lanza una excepción si la conversión falla, permitiendo un manejo de errores más limpio.
    * **Simplificar las condiciones booleanas**.
        * *Correcto:* `if (esValido)`
        * *Incorrecto:* `if (esValido == true)`
    * **Asegurar que cada bloque `case` en una instrucción `switch` termine con una sentencia de salida** (`break`, `return`, etc.).
    * **Verificar el uso correcto de los operadores lógicos `&&` (AND condicional) y `||` (OR condicional)**.

* **Manejo de Excepciones:**
    * **Implementar bloques `try-catch` para gestionar operaciones que puedan fallar,** como I/O, llamadas a APIs o interacciones con la base de datos.
    * **En los bloques `catch`, asegurar que el error se registre (log) o se relance a una capa superior** para ser gestionado adecuadamente, en lugar de silenciar la excepción.

---

### 🌐 Interacción con Base de Datos y LINQ

* **Rendimiento en Consultas:**
    * **Aplicar filtros (`.Where()`) lo antes posible en las consultas LINQ to Entities.** Esto asegura que el filtrado se realice en el motor de la base de datos y no en memoria, minimizando la cantidad de datos transferidos.
    * **Seleccionar únicamente las columnas necesarias usando `.Select()`.** Evitar traer entidades completas si solo se necesitan algunas de sus propiedades.
    * **Usar `.Any()` en lugar de `.Count() > 0` para verificar si existen registros que cumplan una condición.** `.Any()` es más rápido porque se detiene al encontrar el primer registro coincidente.

* **Operaciones y Transacciones:**
    * **Evitar realizar operaciones de base de datos (Insert, Update, Delete) dentro de un ciclo.** En su lugar, preparar una lista de entidades y realizar una operación masiva.
    * **No ejecutar métodos de C# personalizados dentro de una consulta LINQ que se vaya a traducir a SQL.** El proveedor de la base de datos no puede traducir código .NET a SQL.
    * **Asegurar que todas las operaciones de base de datos que forman una unidad de trabajo lógica estén agrupadas en una única transacción**.
    * **Verificar que toda transacción (`DbTransaction`) se cierre correctamente con un `Commit()` en caso de éxito**.

---

### 🚀 Prácticas para .NET Core / .NET

* **Inyección de Dependencias:**
    * **Registrar los servicios en el contenedor de dependencias (`Program.cs` o `Startup.cs`) utilizando el ciclo de vida apropiado:** `AddTransient`, `AddScoped` o `AddSingleton`.

* **Configuración:**
    * **Asegurar que las variables de configuración (cadenas de conexión, claves de API, etc.) existan en todos los archivos `appsettings.{Environment}.json`** (Development, Staging, Production).

* **Entity Framework:**
    * **Si se usan migraciones de base de datos, incluir siempre el archivo de `snapshot` actualizado en el control de versiones.** Este archivo es crucial para que EF Core entienda el estado actual del modelo de datos.

---

### 🔌 Controladores de API (.NET Core / .NET)

* **Diseño de Endpoints:**
    * **Utilizar el verbo HTTP correcto para cada acción:** `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`.
    * **Los métodos del controlador deben devolver DTOs (Data Transfer Objects) en lugar de las entidades de dominio directamente.** Esto desacopla la API de la estructura de la base de datos y evita exponer datos sensibles.
    * **Verificar que las rutas (`[Route]`) y los atributos de enlace de parámetros (`[FromBody]`, `[FromQuery]`, `[FromRoute]`) sean correctos y consistentes**.

* **Seguridad:**
    * **Proteger los endpoints con el atributo `[Authorize]` por defecto.** Solo aquellos que deban ser accesibles públicamente deben estar marcados con `[AllowAnonymous]`.
