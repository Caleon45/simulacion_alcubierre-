# 01. Conceptos y Arquitectura de las Tres Cabinas 3D

Este documento define la estética, instrumentación y diseño visual en primera persona (**Cockpit POV**) para los tres paradigmas de propulsión. Cada cabina responde funcionalmente a las exigencias operativas y térmicas de su motor.

---

## 1. Cabina 1: Nave de Propulsión Química (Estilo Industrial / Analógico)

Inspirada en el diseño aeroespacial histórico y contemporáneo (Apollo, Space Shuttle, Crew Dragon y Starship).

```
   ┌────────────────────────────────────────────────────────────────────────┐
   │                         CABINA QUÍMICA (POV)                           │
   │                                                                        │
   │      ┌───────────────┐                  ┌───────────────┐              │
   │      │ Ventana Izq   │                  │ Ventana Der   │              │
   │      │ Vidrio Grueso │   [ Mamparo ]    │ Vidrio Grueso │              │
   │      └───────────────┘                  └───────────────┘              │
   │                                                                        │
   │   [Presión LOX]   [Acelerómetro G]   [Temp Tobera]   [Monitor 2D]      │
   │      ( (●) )          ( ( 3.5g ) )       ( (●) )     ┌──────────────┐  │
   │                                                      │ Pantalla Nav │  │
   │   [Palanca Empuje]   [Interruptores de Ignición]     └──────────────┘  │
   └────────────────────────────────────────────────────────────────────────┘
```

### Características de Diseño:
* **Estructura y Visibilidad:**
  * Ventanas rectangulares reforzadas con marcos gruesos y remaches visibles para resistir la máxima presión aerodinámica durante el ascenso atmosférico.
  * Mamparos en tonos grises industriales y titanio cepillado.
* **Instrumentación en Salpicadero:**
  * **Diales Analógicos:** Manómetros de presión de combustible criogénico ($LH_2$) y oxidante ($LOX$).
  * **Acelerómetro de Aguja (G-Force Meter):** Aguja móvil que oscila entre $1g$ y $3.5g$ durante el empuje inicial, cayendo a $0g$ en fase de crucero inercial.
  * **Termómetros de Tobera:** Indicador de temperatura de cámara de combustión ($3.500\text{ K}$).
  * **Palanca de aceleración:** Palanca de empuje físico en el pedestal central.
* **Ambiente e Iluminación:**
  * Luces interiores cálidas / blanco suave con botones retroiluminados en verde y ámbar.
  * Alarma sonora mecánica en caso de sobrecalentamiento.

---

## 2. Cabina 2: Nave de Antimateria (Estilo Relativista / Alta Densidad)

Diseñada para viajes interestelares a fracciones significativas de la luz ($0.10c - 0.50c$).

```
   ┌────────────────────────────────────────────────────────────────────────┐
   │                       CABINA DE ANTIMATERIA (POV)                      │
   │                                                                        │
   │               ┌──────────────────────────────────────┐                 │
   │               │   Parabrisas con Filtro Polarizado   │                 │
   │               │       Dorado (Bloqueo Gamma)         │                 │
   │               └──────────────────────────────────────┘                 │
   │                                                                        │
   │   [Trampa Penning]   [Flujo Piones]   [Velocidad Relativista]          │
   │     B-Field: 50 T       pi+ / pi-        v = 0.20c (gamma=1.02)        │
   │                                                                        │
   │   [Radiómetro Geiger]   [Escudo Ablación ISM]   [Pantalla Táctica 2D]  │
   │     0.02 mSv/h            Integridad: 100%      ┌────────────────────┐ │
   │                                                 │   Computador Nav   │ │
   │                                                 └────────────────────┘ │
   └────────────────────────────────────────────────────────────────────────┘
```

### Características de Diseño:
* **Estructura y Visibilidad:**
  * Ventana frontal angular y alargada con **tratamiento de cristal polarizado dorado/ámbar**, diseñado conceptualmente para atenuar los destellos de radiación Cerenkov y fotones gamma secundarios.
  * Mamparos densos de aleación compuesta oscura con textura de fibra de carbono y blindaje integrado.
* **Instrumentación en Salpicadero:**
  * **Monitor de Trampa de Confinamiento:** Estado del campo magnético superconductor que retiene la antimateria ($p + \bar{p}$).
  * **Display de Velocidad Relativista:** Velocidad en fracción de la luz ($\beta = v/c$) y factor de Lorentz calculado en vivo ($\gamma$).
  * **Contador de Radiación Geiger:** Medidor digital de dosis absorbida.
  * **Monitor de Integridad del Escudo Deflector:** Muestra el desgaste del escudo de Berilio/Grafeno al chocar contra el gas interestelar.
* **Ambiente e Iluminación:**
  * Tonalidades azul cian y violeta frío, con iluminación indirecta en tiras LED técnicas.

---

## 3. Cabina 3: Nave de Curvatura Warp (Estilo Alcubierre / Futurista Ergonómico)

Diseñada para vuelos superlumínicos mediante distorsión geométrica del espacio-tiempo.

```
   ┌────────────────────────────────────────────────────────────────────────┐
   │                       CABINA DE CURVATURA WARP (POV)                   │
   │                                                                        │
   │       . - ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ - .            │
   │     /         VENTANAL PANORÁMICO SEMICIRCULAR             \           │
   │    (    (Halo Toroidal de Curvatura Luminoso en el Borde)   )          │
   │     \                                                      /           │
   │       ~ - . _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ . - ~            │
   │                                                                        │
   │   [Métrica Alcubierre]   [G-Force Inercial]    [Reloj Dual Cuántico]   │
   │     W = 10.0 c             0.00 g (Absoluto)   Tierra:  01d 04h 12m    │
   │     f(rs) Estable          Geodésica pura      Nave:    01d 04h 12m    │
   │                                                (d tau = dt Exacto)     │
   │                                                                        │
   │   [Densidad Energía]     [Pantalla Holográfica 2D de Navegación]       │
   │     rho < 0 (Casimir)    ┌─────────────────────────────────────────┐   │
   │     -1.2 x 10^20 J       │  Mapa Orbital e Hitos Interestelares    │   │
   │                          └─────────────────────────────────────────┘   │
   └────────────────────────────────────────────────────────────────────────┘
```

### Características de Diseño:
* **Estructura y Visibilidad:**
  * **Ventanal panorámico continuo semicircular:** Máxima visibilidad frontal y lateral para contemplar la contracción del espacio exterior y el lente gravitacional de la burbuja.
  * Diseño limpio, minimalista y ergonómico sin aristas agresivas, construido en grafeno blanco y titanio satinado.
* **Instrumentación en Salpicadero:**
  * **Holograma de la Burbuja de Métrica:** Proyección 3D en miniatura que ilustra la deformación del espacio alrededor de la nave (azul al frente por contracción $\theta < 0$, rojo atrás por expansión $\theta > 0$).
  * **Dial de Fuerza G Sentida:** Indicador fijo e inmutable en **$0.00\text{ g}$**, recordándole al piloto que la nave no se mueve por el espacio, sino que el espacio se mueve alrededor de la nave en caída libre geodésica permanente ($a^\mu = 0$).
  * **Reloj Dual Cuántico:** Dos cronómetros sincronizados al milisegundo: uno para la Tierra y otro para la tripulación, demostrando visualmente que en el motor de Alcubierre **no existe dilatación temporal ($d\tau = dt$)**.
  * **Medidor de Energía Exótica:** Nivel del reservorio de fluctuaciones de Casimir dinámico ($\rho < 0$).
* **Ambiente e Iluminación:**
  * Iluminación ambiental suave en azul celeste luminiscente que pulsa sutilmente en armonía con la frecuencia de la burbuja Warp.

