# Simulación Alcubierre (Unity)

Proyecto configurado para la asignatura **Entornos y Simulación**.

## 📁 Estructura del Proyecto Unity
```
FINAL/
├── Assets/                    # Recursos, escenas y código de Unity
│   ├── Scripts/               # Código C# de la simulación
│   │   ├── Alcubierre.Scripts.asmdef  # Assembly Definition
│   │   ├── SimulationManager.cs       # Controlador principal (Start, Update, Simulate, DrawView)
│   │   └── SimulationEntity.cs        # Clase base para entidades/agentes
│   ├── Scenes/                # Escenas del proyecto (SampleScene.unity ya configurada)
│   ├── Prefabs/               # Prefabs de agentes y entorno
│   ├── Materials/             # Materiales visuales
│   ├── Settings/              # Configuración URP y de renderizado
│   └── UI/                    # Elementos de interfaz gráfica
├── Packages/
│   ├── manifest.json          # Dependencias y paquetes de Unity
│   └── packages-lock.json     # Árbol de paquetes resuelto
├── ProjectSettings/
│   ├── ProjectVersion.txt     # Versión de Unity (6000.3.24f1)
│   └── ProjectSettings.asset  # Configuración global del proyecto (Nombre: Alcubierre)
├── contexto/                  # Carpeta de investigación teórica, física y matemática (Intacta)
│   ├── 01_curvatura_espaciotiempo/
│   ├── 02_propulsion_antimateria/
│   ├── 03_propulsion_quimica_convencional/
│   └── propuestas_de_propulsion.md
├── Abrir_en_Unity.bat         # Lanzador directo para abrir con Unity 6
└── README.md
```

## ⚙️ Reglas de la Plantilla (Rúbrica de Clase)
El script [SimulationManager.cs](Assets/Scripts/SimulationManager.cs) implementa estrictamente los 4 pilares solicitados:
1. **`Start()`**: Define las condiciones iniciales, inicializa variables de ejecución y dibuja el ecosistema inicial.
2. **`Update()`**: Controla el temporizador de simulación (`secondsPerDay`) y ejecuta cada ciclo.
3. **`Simulate()`**: Aplica las reglas matemáticas y lógicas de evolución del sistema.
4. **`DrawView()`**: Representa visualmente en el Game View el estado en tiempo real.
5. **Visualización y Consola**: Reporta los cambios día a día tanto en la consola de Unity (`Debug.Log`) como en pantalla mediante interfaz gráfica (`OnGUI`).

## 🚀 Cómo Abrir y Ejecutar el Proyecto
### Opción 1: Directo con el lanzador (Recomendado)
- Haz doble clic en el archivo [Abrir_en_Unity.bat](file:///i:/Mi%20unidad/CAMILO%20DRIVE/simulacion/FINAL/Abrir_en_Unity.bat). Se abrirá directamente en Unity Editor 6000.3.24f1.

### Opción 2: Desde Unity Hub
1. Abre **Unity Hub**.
2. Haz clic en **Add** (o *Add project from disk*).
3. Selecciona esta carpeta: `FINAL`.
4. El proyecto aparecerá en la lista con el nombre **Alcubierre** y versión **6000.3.24f1**.
5. Ábrelo. La escena [SampleScene.unity](Assets/Scenes/SampleScene.unity) ya tiene el GameObject `SimulationManager` listo.
6. Presiona **Play ▶️** para ver la simulación ejecutándose en tiempo real.
