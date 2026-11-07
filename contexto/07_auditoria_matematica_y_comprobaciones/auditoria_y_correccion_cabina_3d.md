# Auditoría Técnica y Corrección Integral de las Cabinas 3D (Cockpit POV)

Este documento detalla el diagnóstico del fallo visual reportado (el "punto gris" flotante), la solución de ingeniería aplicada a la jerarquía de cámaras, la auditoría y enriquecimiento del modelado 3D de las tres cabinas (Química, Antimateria y Warp), y la verificación de coherencia con las especificaciones de la carpeta `contexto/`.

---

## 1. Diagnóstico de Causa Raíz: ¿Por qué se veía un "punto gris" flotando en el espacio?

En la captura de pantalla (`media_1793576100127_3a3afd71.png`), la simulación mostraba una pequeña caja rectangular gris en medio del vacío.

### Causa Matemática y de Ciclo de Vida en Unity:
1. **Desfase en la captura de posición (`Awake` vs `Start`):**
   * En `SimulationManager.Start()`, se creaba el objeto `CockpitManager_Root` en `Z = -250f`.
   * Al ejecutarse `AddComponent<CockpitManager>()`, Unity disparó inmediatamente `Awake()`, el cual invocó `exteriorView.Initialize(transform, povCamera)`.
   * En ese instante, la cámara principal (`Camera.main`) aún se encontraba en el espacio global en `(0, 0, -10)` y **no había sido emparentada** a la cabina.
   * `CockpitExteriorView` guardó `originalCameraPos = povCamera.transform.localPosition` $\implies (0, 0, -10)$.
2. **Sobrescritura continua en el bucle `Update()`:**
   * Posteriormente, `CockpitManager` emparentaba la cámara a la cabina en `baseCameraPosition = (0f, 0.05f, 0.45f)`.
   * Sin embargo, cada fotograma en `CockpitExteriorView.UpdateCockpitShake()`, el script ejecutaba:
     $$p_{\text{cam}} = \text{originalCameraPos} \implies (0, 0, -10)$$
   * Esto forzaba a la cámara a posicionarse **10 metros por detrás de la cabina** dentro de la jerarquía local.
   * La cabina mide $3.2 \times 2.5 \times 3.0\text{ m}$ (ubicada entre $Z = -0.2$ y $Z = 2.5$). Al situar al observador en $Z = -10.0$ mirando hacia $+Z$, la cámara quedaba fuera de la nave, observando el mamparo posterior externo desde lejos. Por eso la cabina se apreciaba como una pequeña caja gris flotando en el espacio exterior.

### Solución Implementada:
* Se retiró cualquier manipulación directa de la posición de la cámara desde `CockpitExteriorView`.
* Ahora `CockpitExteriorView` únicamente expone el desplazamiento inercial de vibración:
  $$\vec{\delta}_{\text{shake}} \in \mathbb{R}^3, \quad \|\vec{\delta}\| \le 0.015\text{ m}$$
* `CockpitManager` es la **única autoridad** sobre la cámara, fijándola de forma estricta en la posición de los ojos del piloto:
  $$\vec{r}_{\text{cam}}(t) = \vec{r}_{\text{base}} + \vec{\delta}_{\text{shake}}(t), \quad \vec{r}_{\text{base}} = (0.0\text{ m}, +0.05\text{ m}, +0.45\text{ m})$$

---

## 2. Enriquecimiento del Modelado 3D por Paradigma

Siguiendo las especificaciones de `contexto/05_diseno_cabinas_y_entorno_3d/`:

```
                           ESQUEMA DE LA CABINA (POV)
                           
               Techo / Cúpula Protectora
       ┌──────────────────────────────────────┐
       │   [Parabrisas con Cristales 3D]      │  --> Campo Estelar Relativista
       │     (Espesor real, antirreflejo)     │      Halo Toroidal Warp / Doppler
       ├──────────────────────────────────────┤
       │ [Consola Izq]  [Pantalla HUD]  [Der] │  --> Diales, G-Meter, Dosis
       │  Combustibles   Radar Táctico  Leds  │      Contador Geiger, Palancas
       ├──────────────────────────────────────┤
       │       [Mamparo Frontal Inferior]     │  --> Sellado hermético al suelo
       │        (Sin vacíos hacia afuera)     │
       └──────────────────────────────────────┘
                  [Asiento Ergonómico]           --> Ubicado bajo la cámara (0, 0.05, 0.45)
```

### 2.1 Cabina 1: Propulsión Química (Industrial / Apollo / Starship)
* **Materiales:** Aleación de Aluminio-Litio (Al-Li 2195, plateado satinado), titanio oscuro en consolas y vidrio reforzado multicapa.
* **Geometría y Ergonomía:**
  * **Asiento de Vuelo:** Sillón ergonómico acolchado bajo el piloto con base, respaldo reclinado ($12^\circ$), reposacabezas y reposabrazos dobles.
  * **Mamparo Inferior de Cierre:** Sellado bajo el salpicadero que elimina cualquier hueco hacia el exterior.
  * **Parabrisas 3D con Espesor:** Se reemplazaron los planos 2D (*Quads*) por prismas 3D delgados de $2\text{ cm}$ de espesor con marcos estructurales pesados y poste central de refuerzo.
  * **Consola Central e Instrumentos:**
    * Pantalla de Navegación Táctica con **textura procedural de radar astronómico y retícula vectorial**.
    * Tanque izquierdo de Hidrógeno Líquido ($LH_2$) con columna azul eléctrica animada según consumo.
    * Tanque derecho de Oxígeno Líquido ($LOX$) con columna turquesa criogénica.
    * Manómetros circulares de presión en cabeza de tanques y dial de temperatura de tobera ($3.500\text{ K}$).
    * Acelerómetro mecánico ($G$-Meter) con aguja roja móvil ($0$ a $6g$).
    * Palanca de aceleración física (*Throttle*) e interruptores de armamento de ignición/RCS.
    * Luces LED de estado nominal (verde), advertencia (ámbar) y crítico (rojo).

### 2.2 Cabina 2: Propulsión por Antimateria (Valkyrie / Relativista)
* **Materiales:** Tungsteno denso antirradiación (grafito mate), fibra de carbono y cristal con polarizado dorado/ámbar atenuador de fotones gamma.
* **Geometría y Ergonomía:**
  * **Asiento de Alta-$G$:** Couch reclinado a $15^\circ$ con controles de vuelo integrados en los reposabrazos (*HOTAS*).
  * **Reubicación de la Trampa Penning:** Se trasladó a la **consola izquierda** para despejar completamente la línea de visión frontal a través del parabrisas.
  * **Núcleo de Plasma Pulsante:** Esfera de antiprotones ($p + \bar{p}$) levitando en el centro magnético con luz puntual violeta ($3.2\text{ m}$ de radio) y anillos superconductores YBCO en rotación activa.
  * **Instrumentación Relativista:** Monitor de integridad del escudo deflector de Berilio frente al medio interestelar (ISM), contador Geiger digital y monitor de velocidad en fracción de $c$.

### 2.3 Cabina 3: Curvatura Espacio-Temporal (Alcubierre / NASA IXS Enterprise)
* **Materiales:** Titanio satinado blanco perla, polímeros ergonómicos y cristal panorámico ultraclaro.
* **Geometría y Ergonomía:**
  * **Sillón de Mando Futurista:** Estructura en titanio perla con mandos táctiles hápticos en reposabrazos.
  * **Ventanal Panorámico Continuo:** Cristal envolvente de visión continua sin postes intrusivos.
  * **Resonador Casimir Central:** Desplazado a la consola izquierda, emitiendo brillo cian pulsante en función del factor Warp ($W = 10c \to 100c$).
  * **Proyector Holográfico 3D:** Esfera deformada que proyecta la métrica de Alcubierre $f(r_s)$ con contracción frontal y expansión posterior.
  * **Monitor Inercial Cero-$G$:** Indicador fijo permanentemente en **$0.00\text{ g}$** con LED verde de confirmación geodésica.
  * **Reloj Dual Cuántico:** Muestra de forma directa la igualdad de tiempos propios y terrestres ($d\tau = dt$).

---

## 3. Entorno Exterior Relativista y Campo Estelar

1. **Visibilidad de Estrellas:**
   * Se ajustó el tamaño de las partículas del campo estelar de $0.04\text{ m}$ a un rango visible y nítido de **$0.14\text{ m} - 0.32\text{ m}$**, distribuidas en el cono visual frontal ($Z = 4\text{ m}$ a $48\text{ m}$).
2. **Efecto Doppler y Aberración:**
   * En crucero relativista ($\beta = v/c > 0.05$), las estrellas frontales experimentan corrimiento al azul (cian brillante/azul eléctrico), mientras que las periféricas se corren al rojo anaranjado.
3. **Halo Toroidal de Curvatura:**
   * En régimen Warp, un anillo luminoso cian translúcido rodea el contorno del parabrisas exterior evidenciando la lente gravitacional de la densidad de energía exótica negativa ($\rho < 0$).
4. **Facsímil Planetario:**
   * Al alcanzar el $\ge 65\%$ del trayecto hacia la Luna, Marte, Júpiter o Próxima Centauri, emerge suavemente en el centro del parabrisas la esfera 3D estilizada del astro objetivo, escalando con la distancia real restante.

---

## 4. Matriz de Auditoría de Código y Verificación

| Componente | Archivo Fuente | Estado Previo | Corrección Aplicada | Resultado |
| :--- | :--- | :--- | :--- | :--- |
| **Cámara POV** | `CockpitManager.cs` | Sujeta a sobrescritura externa | Autoridad única en `(0, 0.05, 0.45)` | Vista interior auténtica en primera persona |
| **Vibración / Shake** | `CockpitExteriorView.cs` | Modificaba `povCamera.localPosition` | Exporta `CurrentShakeOffset` | Vibración física sin expulsar la cámara |
| **Parabrisas** | `*Cockpit.cs` | `Quad` plano monocara | Prismas `Cube` 3D ($2\text{ cm}$) | Refracción y visibilidad bidireccional limpia |
| **Asientos** | `*Cockpit.cs` | Ausentes (vacío bajo la cámara) | Sillón ergonómico por paradigma | Inmersión completa al mirar abajo con clic derecho |
| **Consola Táctica** | `CockpitMaterialPalette.cs` | Color plano unlit | Textura HUD procedural de radar 256x256 | Salpicadero iluminado con instrumentación real |
| **Compilación** | `Alcubierre.Scripts.csproj` | OK | OK | **0 Errores, 0 Advertencias** |

