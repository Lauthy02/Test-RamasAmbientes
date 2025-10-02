# Práctica de Pipeline CI/CD con GitHub Actions y .NET

Este repositorio es un ejercicio práctico para demostrar la configuración de un flujo de trabajo completo de **Integración Continua y Despliegue Continuo (CI/CD)**. El proyecto utiliza GitHub Actions para automatizar el proceso de compilación, pruebas y despliegue de una aplicación de consola .NET a través de múltiples ambientes.

El objetivo es simular un entorno de desarrollo profesional, aplicando una estrategia de ramificación Git Flow simplificada y utilizando las funcionalidades de GitHub para garantizar la calidad y seguridad del código antes de que llegue a producción.

---

## 🚀 Habilidades y Conceptos Demostrados

Este proyecto pone en práctica los siguientes conceptos clave del mundo DevOps:

* **Control de Versiones con Git:** Creación y gestión de un flujo de trabajo con múltiples ramas (`develop`, `test`, `preprod`, `main`).
* **Estrategia de Ramificación:** Implementación de un flujo de promoción de código secuencial, asegurando que los cambios pasen por todas las etapas de validación.
* **CI/CD con GitHub Actions:** Creación de un pipeline de automatización desde cero utilizando archivos de flujo de trabajo en formato YAML.
* **Gestión de Ambientes en GitHub:** Configuración de ambientes (`Test`, `Pre-produccion`, `Produccion`) para asociar reglas de protección específicas a cada etapa del despliegue.
* **Reglas de Protección de Ramas:** Implementación de restricciones como:
    * Despliegues condicionales basados en la rama.
    * **Revisores obligatorios** para aprobar despliegues a producción, simulando un control de calidad final.
* **Automatización de Pruebas:** Integración del comando `dotnet test` en el pipeline para asegurar que ningún cambio que rompa el código existente sea promocionado.

---

##  workflow-diagram Flujo de Trabajo del Pipeline

El pipeline está diseñado para seguir un flujo estricto de promoción entre ambientes:

**`develop` ➡️ `test` ➡️ `preprod` ➡️ `main`**

1.  **Desarrollo (`develop`):** Todo el código nuevo se integra en esta rama. Un `push` a `develop` solo dispara el job de pruebas.
2.  **Pruebas (`test`):** El código se fusiona en `test`. Un `push` a esta rama ejecuta las pruebas y, si pasan, despliega al ambiente de **Test**.
3.  **Pre-producción (`preprod`):** Una vez validado en Test, el código se fusiona en `preprod`. Un `push` aquí ejecuta pruebas y despliega al ambiente de **Pre-producción**.
4.  **Producción (`main`):** Finalmente, el código se fusiona en `main`. Un `push` aquí ejecuta pruebas y lanza el despliegue a **Producción**, el cual queda **en pausa hasta que un revisor autorizado lo apruebe manualmente**.



---

<details>
<summary>Haga clic aquí para ver el ejercicio completo paso a paso</summary>

### Paso 1: Configuración Inicial del Repositorio y Ramas

1.  **Crear el Repositorio:** Se creó un repositorio público en GitHub con un archivo README inicial.
2.  **Clonar y Crear Ramas:** Se clonó el repositorio y se crearon las ramas `develop`, `test` y `preprod` localmente.
    ```bash
    git branch develop
    git branch test
    git branch preprod
    ```
3.  **Subir Ramas:** Se subieron todas las ramas al repositorio remoto.
    ```bash
    git push --all
    ```

### Paso 2: Configurar Ambientes y Restricciones

Se configuraron los siguientes ambientes en **Settings > Environments**:

* **`Test`**: Protegido para aceptar despliegues solo desde la rama `test`.
* **`Pre-produccion`**: Protegido para aceptar despliegues solo desde la rama `preprod`.
* **`Produccion`**: Protegido para aceptar despliegues solo desde `main` y con la regla **Required reviewers** activada.

### Paso 3: Crear una Aplicación Simple en C#

1.  **Crear Proyecto .NET:** En la rama `develop`, se creó una solución con una aplicación de consola y un proyecto de pruebas MSTest.
    ```bash
    dotnet new sln -n MiAppSolucion
    dotnet new console -n MiApp.App -o MiApp.App
    dotnet new mstest -n MiApp.Tests -o MiApp.Tests
    dotnet sln add MiApp.App/MiApp.App.csproj MiApp.Tests/MiApp.Tests.csproj
    dotnet add MiApp.Tests/MiApp.Tests.csproj reference MiApp.App/MiApp.App.csproj
    ```
2.  **Escribir Código y Pruebas:** Se implementó una función simple de suma y una prueba unitaria para validarla.

### Paso 4: Automatizar con GitHub Actions

Se creó el archivo `.github/workflows/despliegue-completo.yml` con el siguiente contenido para definir el pipeline:

```yaml
name: Pipeline de Despliegue Completo CI/CD con .NET

on:
  push:
    branches: [ "main", "preprod", "test", "develop" ]

jobs:
  test-code:
    name: 1. Probar Código .NET
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal

  deploy-test:
    name: 2. Desplegar a Test
    needs: test-code
    if: github.ref == 'refs/heads/test'
    runs-on: ubuntu-latest
    environment: Test
    steps:
      - run: echo "Desplegando a Test..."

  deploy-preprod:
    name: 3. Desplegar a Pre-producción
    needs: test-code
    if: github.ref == 'refs/heads/preprod'
    runs-on: ubuntu-latest
    environment: Pre-produccion
    steps:
      - run: echo "Desplegando a Pre-producción..."

  deploy-prod:
    name: 4. Desplegar a Producción
    needs: test-code
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    environment: Produccion
    steps:
      - run: echo "Desplegando a Producción..."
```

### Paso 5: Probar el Flujo Completo

Se simuló la promoción de un cambio fusionando las ramas en secuencia y subiendo los cambios a GitHub, verificando en la pestaña "Actions" que cada job se ejecutara correctamente y que el despliegue a producción esperara la aprobación manual.

```bash
# 1. Promocionar a Test
git checkout test
git merge develop
git push origin test

# 2. Promocionar a Pre-producción
git checkout preprod
git merge test
git push origin preprod

# 3. Promocionar a Producción
git checkout main
git merge preprod
git push origin main
```
