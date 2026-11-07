# 01. Física de la Aniquilación Materia-Antimateria

## 1. Principio Fundamental y Densidad Energética

La propulsión por antimateria se fundamenta en la reacción física más energética permitida por las leyes del modelo estándar de la física de partículas: la **aniquilación mutua entre una partícula de materia y su correspondiente antipartícula**.

A diferencia de las reacciones químicas (que reorganizan enlaces electrónicos con eficiencias del $10^{-8}\%$) o las reacciones nucleares de fisión y fusión (que liberan entre el $0.1\%$ y el $0.7\%$ de la masa en reposo), la aniquilación materia-antimateria convierte el **100% de la masa en reposo en energía radiante y cinética pura**:

$$E = 2 m_{\text{antimateria}} c^2$$

Donde el factor $2$ proviene de que cada gramo de antimateria debe aniquilarse con un gramo idéntico de materia ordinaria.

### 1.1 Comparativa de Densidad de Energía Específica
| Reacción / Propelente | Densidad Energética Específica ($J/\text{kg}$) | Eficiencia de Conversión ($\Delta m / m$) |
| :--- | :--- | :--- |
| **Combustión Química** ($\text{H}_2 + \text{O}_2$) | $1.42 \times 10^7 \text{ J/kg}$ | $1.5 \times 10^{-10}$ |
| **Fisión Nuclear** ($^{235}\text{U}$) | $8.20 \times 10^{13} \text{ J/kg}$ | $0.091\%$ |
| **Fusión Nuclear** ($\text{D} + \text{T}$) | $3.37 \times 10^{14} \text{ J/kg}$ | $0.375\%$ |
| **Aniquilación Antimateria** ($p + \bar{p}$) | **$1.80 \times 10^{17} \text{ J/kg}$** | **$100.00\%$** |

> **Escala:** Un solo gramo ($0.001 \text{ kg}$) de antimateria aniquilado con un gramo de materia ordinaria libera:
> $$E = 2 \times 10^{-3} \text{ kg} \times (2.998 \times 10^8 \text{ m/s})^2 \approx 1.80 \times 10^{14} \text{ Joules} \approx 43 \text{ kilotones de TNT}$$
> *(Aproximadamente el triple de la energía liberada por la bomba de Hiroshima, en apenas 2 gramos de reactivos).*

---

## 2. Reacciones Subatómicas Específicas

Para el diseño de un motor espacial, la elección del tipo de antimateria determina si los productos de reacción pueden dirigirse electromagnéticamente para generar empuje o si se disipan inútilmente en forma de radiación gamma penetrante.

### 2.1 Aniquilación Electrón-Positrón ($e^- + e^+$)
La aniquilación entre un electrón y un positrón produce exclusivamente fotones gamma de alta energía:

$$e^- + e^+ \longrightarrow 2 \gamma \quad (E_\gamma = 511 \text{ keV cada uno})$$

- **Problema de Propulsión:** Los fotones gamma no poseen carga eléctrica ($q=0$). No pueden ser desviados ni confinados mediante campos magnéticos. Para utilizarlos como empuje se requerirían espejos reflectores de rayos gamma, los cuales no existen en la física del estado sólido (los rayos gamma atraviesan o ionizan la materia en lugar de reflejarse). Por ello, el positrón no es el candidato ideal para propulsión directa.

---

### 2.2 Aniquilación Protón-Antiprotón ($p + \bar{p}$)
Es el proceso clave para motores espaciales de alto rendimiento. Cuando un protón y un antiprotón colisionan a bajas energías cinéticas, la aniquilación no produce fotones directos, sino una cascada de **mesones piónicos ($\pi$)** intermediarios mediada por la fuerza nuclear fuerte:

$$p + \bar{p} \longrightarrow n_{\pi^\pm} (\pi^+ + \pi^-) + n_{\pi^0} \pi^0$$

En promedio, cada aniquilación genera aproximadamente **$5$ piones**:
- **Piones cargados ($\pi^+, \pi^-$):** Promedio de $3.0$ piones cargados por evento ($1.5 \ \pi^+$ y $1.5 \ \pi^-$).
- **Piones neutros ($\pi^0$):** Promedio de $1.9$ piones neutros por evento.

La energía total en reposo del sistema es $E_0 = 2 m_p c^2 = 1876.5 \text{ MeV}$. Esta energía se reparte en:

```
                            [ p + p̄ (1876.5 MeV) ]
                                      │
            ┌─────────────────────────┴─────────────────────────┐
            ▼                                                   ▼
   3.0 Piones Cargados (π⁺, π⁻)                        1.9 Piones Neutros (π⁰)
   Energía cinética ~ 378 MeV/pión                      Energía ~ 200 MeV/pión
   Carga eléctrica: ±e                                  Carga eléctrica: 0
   Velocidad relativista: v ≈ 0.94c                     Vida media: 8.5 × 10⁻¹⁷ s
            │                                                   │
            ▼                                                   ▼
   DESVIABLES CON TOBERA MAGNÉTICA                     DECAIMIENTO INMEDIATO
   (EMPUJE DIRECTO)                                    π⁰ ──> 2γ (Rayos Gamma)
                                                       (Pérdida térmica / blindaje)
```

---

## 3. Dinámica Relativista de los Piones Cargados

Para que los piones cargados puedan propulsar la nave, deben ser expulsados por la tobera magnética antes de desintegrarse.

### 3.1 Vida Media y Dilatación Temporal de los Piones
El pión cargado libre tiene una vida media en reposo de:
$$\tau_0 \approx 2.603 \times 10^{-8} \text{ segundos}$$

Dado que nacen con una energía cinética media de $E_{\text{cin}} \approx 238 \text{ MeV}$ y una masa en reposo de $m_\pi c^2 = 139.57 \text{ MeV}$:
$$\gamma = \frac{E}{m_\pi c^2} = \frac{139.57 + 238}{139.57} \approx 2.70$$

La velocidad de los piones cargados es altamente relativista:
$$v_\pi = c \sqrt{1 - \frac{1}{\gamma^2}} = c \sqrt{1 - \frac{1}{2.7^2}} \approx 0.928 c$$

Por **dilatación temporal relativista**, la vida media en el sistema de referencia de la nave se extiende a:
$$\tau = \gamma \tau_0 = 2.70 \times (2.603 \times 10^{-8} \text{ s}) \approx 7.03 \times 10^{-8} \text{ s}$$

La distancia media que recorre un pión cargado antes de decaer es:
$$d = v_\pi \tau = (0.928 \times 3 \times 10^8 \text{ m/s}) \times (7.03 \times 10^{-8} \text{ s}) \approx 19.6 \text{ metros}$$

> **Conclusión de Ingeniería:**
> Como una tobera magnética espacial mide típicamente entre $5$ y $10$ metros de longitud, **la gran mayoría de los piones cargados son expulsados al vacío exterior antes de decaer**, transfiriendo su momento lineal directamente a la nave a través de las líneas del campo magnético superconductor.

---

## 4. Estado de Producción y Confinamiento Real (CERN y Laboratorios)

### 4.1 Producción Actual en Aceleradores
- En el **CERN** (instalaciones *Antiproton Decelerator* - AD y el anillo *ELENA*), los antiprotones se producen disparando un haz de protones a $26 \text{ GeV}/c$ contra un blanco estacionario de iridio o tungsteno.
- La colisión genera pares protón-antiprotón mediante la conservación del número bariónico:
  $$p + p \longrightarrow p + p + p + \bar{p}$$
- La tasa de producción mundial combinada (CERN + Fermilab históricamente) es de aproximadamente **10 a 20 nanogramos por año**.
- Costo estimado actual: aproximadamente **$62.5 billones de dólares por gramo**.

### 4.2 Trampas Penning-Malmberg y Almacenamiento
Para evitar que la antimateria toque las paredes del contenedor y se aniquile instantáneamente:
1. **Trampas de Penning-Malmberg:** Utilizan un campo magnético axial estático uniforme ($B \approx 3 - 5 \text{ Tesla}$) que confina radialmente a los antiprotones en órbitas de ciclotrón, combinado con un potencial electrostático de barrera cuadrupolar en los extremos axiales.
2. **Antihidrógeno Neutro ($\bar{\text{H}} = \bar{p} + e^+$):** Experimentos como ALPHA en el CERN han logrado combinar antiprotones fríos con positrones para crear átomos de antihidrógeno neutro, atrapándolos mediante trampas magnéticas Ioffe-Pritchard que actúan sobre su momento dipolar magnético durante semanas a temperaturas criogénicas ($T < 0.5 \text{ K}$).
3. **Almacenamiento Macroscópico Futuro (Hielo de Antihidrógeno):** Para misiones espaciales se propone almacenar antimateria en estado sólido como micro-cristales de antihidrógeno molecular ($\bar{\text{H}}_2$), diamagnéticamente levitados en cámaras de ultra-alto vacío criogénico ($10^{-17} \text{ Torr}, 4 \text{ K}$).
