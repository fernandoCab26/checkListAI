### 🚀 Checklist para TypeScript / JavaScript

Esta guía se enfoca en las mejores prácticas del lenguaje para escribir un código moderno, seguro y mantenible.

* **Declaración de Variables:**
    * **Prefiere `const` y `let` sobre `var`**. `const` para variables que no serán reasignadas y `let` para las que sí. Esto evita problemas de *hoisting* y alcance (*scope*) que `var` puede causar.
    * **Asegura que todas las variables estén declaradas** antes de su uso para evitar errores en tiempo de ejecución.

* **Calidad y Seguridad del Código:**
    * **Utiliza el operador de igualdad estricta (`===`) en lugar del operador de igualdad flexible (`==`)**. El `===` compara tanto el valor como el tipo, evitando conversiones inesperadas que pueden llevar a errores sutiles.
    * **Valida siempre los posibles valores nulos o indefinidos** antes de intentar acceder a sus propiedades para prevenir errores de `TypeError`.
    * **Evita el uso de `eval()` y el constructor `new Function()`**. Estas funciones pueden ejecutar código arbitrario y representan un riesgo de seguridad significativo (XSS).
    * **Maneja los errores de forma explícita.** Todo bloque `try` debe tener un bloque `catch` con lógica para gestionar el error (registrarlo, notificar al usuario, etc.), en lugar de dejarlo vacío.
    * **Elimina código de depuración antes de pasar a producción.** Asegúrate de que no queden instrucciones como `console.log`, `debugger` o `alert` en el código final.

* **Operaciones Asíncronas y APIs:**
    * **Utiliza los verbos HTTP correctos** en las peticiones a APIs (`GET`, `POST`, `PUT`, `DELETE`, etc.) según la acción que se desea realizar.
    * **Proporciona retroalimentación al usuario durante las operaciones asíncronas.** Muestra un indicador de carga (*loader* o *spinner*) mientras se espera la respuesta de una API para mejorar la experiencia de usuario.

---

### 🅰️ Checklist Específico para Angular

Buenas prácticas para desarrollar aplicaciones robustas, eficientes y escalables con Angular.

* **Gestión de Componentes y Ciclo de Vida:**
    * **Desuscríbete de los Observables para evitar fugas de memoria.** En el método `ngOnDestroy` de cada componente, cancela todas las suscripciones activas para liberar recursos cuando el componente es destruido.
    * **Evita tener lógica de aplicación en `main.ts`**. Este archivo debe usarse únicamente para el arranque de la aplicación.
    * **Resetea el estado de componentes reutilizables (como modales o *drawers*) al cerrarlos.** Esto garantiza que la próxima vez que se abran, aparezcan en su estado inicial.
    * **Asegúrate de que todas las variables y métodos usados en la plantilla HTML estén definidos en el archivo TypeScript (`.ts`) del componente**.

* **Rendimiento y Optimización:**
    * **Usa `trackBy` con la directiva `*ngFor`** al iterar sobre colecciones. Esto ayuda a Angular a identificar qué elementos han cambiado, mejorando significativamente el rendimiento al evitar la re-renderización de toda la lista.
    * **Implementa *Lazy Loading* (carga perezosa) para los módulos de funcionalidades.** Esto divide la aplicación en trozos más pequeños que se cargan solo cuando son necesarios, mejorando drásticamente el tiempo de carga inicial.
    * **Utiliza `ChangeDetectorRef` con precaución y solo cuando sea necesario** para controlar manualmente el ciclo de detección de cambios de Angular.

* **Arquitectura y Organización del Código:**
    * **Organiza los archivos por tipo de funcionalidad.** No mezcles servicios, componentes, interfaces y clases en la misma carpeta. Cada uno debe residir en su ubicación correspondiente.
    * **Centraliza las funcionalidades compartidas** (componentes, directivas, pipes) en un `SharedModule` para poder importarlo en otros módulos de la aplicación.
    * **Crea un módulo (`NgModule`) para cada grupo de funcionalidades** o característica del sistema para mantener el código encapsulado y organizado.
    * **Utiliza directivas para la manipulación del DOM** y **pipes para la transformación de datos** en las plantillas.
    * **Asegura que las variables de entorno se agreguen a todos los archivos `environment` relevantes** (development, testing, production).

---

### 🎨 Checklist para CSS y Estilos

Principios para escribir CSS escalable, mantenible y de alto rendimiento.

* **Metodología y Estructura:**
    * **Adopta una metodología de nombrado como BEM (Block, Element, Modifier)** para crear selectores de CSS predecibles y sin conflictos de especificidad.
    * **Implementa variables CSS (*custom properties*)** para centralizar valores reutilizables como colores, tipografías y espaciados. Esto facilita la creación de temas y cambios de diseño globales.
    * **Incluye un `reset` o `normalize` CSS** al inicio de tu proyecto para estandarizar los estilos por defecto de los navegadores y evitar inconsistencias visuales.

* **Buenas Prácticas y Mantenibilidad:**
    * **Evita el uso de `!important`**. Su uso rompe la cascada natural de CSS y dificulta la depuración. Es preferible aumentar la especificidad de los selectores si es necesario.
    * **No utilices estilos en línea (atributo `style` en el HTML)**. Mantén la separación de responsabilidades moviendo todos los estilos a archivos `.css` externos.
    * **Evita selectores demasiado específicos o anidados.** Selectores más simples son más reutilizables y tienen mejor rendimiento.
    * **Añade comentarios para explicar secciones complejas** o decisiones de diseño importantes en tu código CSS.

* **Rendimiento y Diseño Adaptativo:**
    * **Utiliza unidades relativas (`rem`, `em`) en lugar de unidades absolutas** para tipografías y espaciados. Esto permite que los elementos escalen de manera proporcional y mejora la accesibilidad.
    * **Emplea `Flexbox` o `Grid` para la maquetación de diseños complejos.** Estas herramientas modernas ofrecen un control mucho más potente y flexible que los métodos antiguos como los flotados.
    * **Comprime las imágenes y utiliza formatos recomendados para la web** como JPEG y PNG para reducir el tiempo de carga de la página.
    * **Utiliza prefijos de navegador (`-webkit-`, `-moz-`, etc.) para propiedades CSS que no son totalmente compatibles** con todos los navegadores.
    
    ### Documentación y Comentarios:
    * **Las clases, métodos y funciones públicas deben tener documentación en formato JSDoc (`/** ... */`)**. Esta debe explicar su propósito y utilizar etiquetas (`tags`) para describir los parámetros (`@param`), el valor de retorno (`@returns`) y los errores que puede lanzar (`@throws`).
    * **Revisar que la documentación, comentarios y valores de strings no contengan errores ortográficos y correcto uso de acentos y puntuaciones**.
