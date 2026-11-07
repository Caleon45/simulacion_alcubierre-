# 01. Modelo Matemático de Radiación Ambiental y Rutas de Navegación

Este documento formaliza la física de los entornos radiactivos interplanetarios e interestelares, el cálculo analítico de dosis acumulada a lo largo de una trayectoria y el modelado de eventos astronómicos perturbadores (Cinturones de Van Allen, magnetosfera de Júpiter, radiación solar y rayos cósmicos).

---

## 1. Fuentes de Radiación en el Espacio y Dependencia Espacial

Una nave que se desplaza desde un punto $A$ hasta un punto $B$ utilizando propulsión física (química o antimateria) atraviesa tres fuentes principales de radiación ionizante:

```
                  FUENTES DE RADIACIÓN EN EL ESPACIO INTERPLANETARIO
                  
   [ VIENTO SOLAR Y CMEs ]       [ CINTURONES PLANETARIOS ]      [ RAYOS CÓSMICOS (GCR) ]
   Flujo radial dependiente      Magnetosferas atrapadas         Flujo isótropo galáctico
   de la distancia: 1 / r²       (Tierra, Júpiter, Saturno)      ultrapenetrante (> 1 GeV)
   Dosis letal en llamaradas     Dosis extrema concentrada       Dosis crónica continua
```

---

### 1.1 Radiación Solar y Flujo de Protones ($F_{\text{solar}}$)
La radiación electromagnética y el flujo de partículas del viento solar decaen con el cuadrado de la distancia radial heliocéntrica $r$ al Sol:

$$\dot{D}_{\text{solar}}(r) = \dot{D}_0 \left(\frac{r_0}{r}\right)^2$$

Donde:
* $r_0 = 1.0\text{ UA} = 1.496 \times 10^8\text{ km}$ (órbita terrestre).
* $\dot{D}_0$: Tasa de dosis en calma a 1 UA $\approx 0.05 - 0.10\text{ mSv/día}$.
* **Eyecciones de Masa Coronal (CMEs):** Durante un evento eruptivo solar, el flujo de protones de alta energía ($10 - 100\text{ MeV}$) se incrementa en factores de $10^3$ a $10^5$, elevando la tasa a:
  $$\dot{D}_{\text{CME}}(r) = \dot{D}_{\text{pico}} \left(\frac{r_0}{r}\right)^2 \quad \text{con } \dot{D}_{\text{pico}} \sim 50 - 500\text{ mSv/h}$$

---

### 1.2 Rayos Cósmicos Galácticos (GCR - *Galactic Cosmic Rays*)
Procedentes de supernovas y núcleos galácticos activos fuera del sistema solar. Son partículas de energía colosal ($> 1\text{ GeV}$), difíciles de apantallar completamente:

$$\dot{D}_{\text{GCR}} \approx 1.5 - 2.0\text{ mSv/día} = \text{constante en todo el espacio interplanetario}$$

---

### 1.3 Zonas Críticas: Magnetosferas y Cinturones de Radiación
Al trazar una ruta de navegación manual, cruzar las magnetosferas de ciertos cuerpos celestes multiplica exponencialmente la dosis:

1. **Cinturones de Van Allen (Tierra):**
   * Cinturón Interior ($r \sim 1.2 - 2.0 \, R_\oplus$): Protones de alta energía ($10 - 100\text{ MeV}$). Dosis sin blindaje: $\sim 20\text{ mSv/h}$.
   * Cinturón Exterior ($r \sim 3.0 - 6.0 \, R_\oplus$): Electrones relativistas ($0.1 - 10\text{ MeV}$).
2. **Magnetosfera de Júpiter (La trampa radiactiva más letal del Sistema Solar):**
   * El campo magnético joviano es $20.000$ veces más potente que el terrestre.
   * En la órbita de Ío / Europa, la tasa de radiación asciende a:
     $$\dot{D}_{\text{Jupiter}} \approx 10 - 50\text{ Sieverts/día} \quad (10.000 - 50.000\text{ mSv/día})$$
   * Una nave química o de antimateria que realice asistencia gravitatoria (*slingshot*) alrededor de Júpiter debe calcular con precisión el perijove para no esterilizar a la tripulación ni dañar la aviónica.

---

## 2. Ecuación Integral de Dosis Acumulada a lo Largo de la Ruta

Sea una trayectoria de vuelo parametrizada por el vector de posición $\vec{r}(t)$ desde el tiempo de despegue $t = 0$ hasta el arribo $t = t_f$.

La **dosis total acumulada en el exterior del casco** $D_{\text{exterior}}$ es la integral de línea en el tiempo:

$$D_{\text{exterior}} = \int_0^{t_f} \left[ \dot{D}_{\text{GCR}} + \dot{D}_0 \left(\frac{r_0}{\|\vec{r}(t) - \vec{r}_\odot\|}\right)^2 + \sum_i \dot{D}_{\text{mag}, i}(\|\vec{r}(t) - \vec{r}_i\|) \right] dt$$

Donde:
* $\vec{r}_\odot$ es la posición del Sol.
* $\vec{r}_i$ es la posición del planeta $i$ que posea cinturón magnético.
* $\dot{D}_{\text{mag}, i}(d)$ es el perfil gaussiano radial de intensidad magnética del planeta $i$:
  $$\dot{D}_{\text{mag}, i}(d) = \dot{D}_{\text{max}, i} \cdot \exp\left( - \frac{(d - R_{\text{cinturon}, i})^2}{2 \sigma_i^2} \right)$$

---

## 3. Atenuación del Blindaje: Ley de Beer-Lambert Exponencial

La dosis que efectivamente penetra en la cabina y es recibida por la tripulación depende del espesor $x$ y el coeficiente de atenuación lineal $\mu$ del material seleccionado:

$$D_{\text{interior}} = D_{\text{exterior}} \cdot e^{-\mu \cdot \rho \cdot x} = D_{\text{exterior}} \cdot 2^{-\frac{x}{\text{HVL}}}$$

Donde:
* $\rho$: Densidad del material ($\text{g/cm}^3$).
* $\text{HVL}$ (*Half-Value Layer* / Espesor de Semi-Reducción): Espesor requerido para reducir la radiación a la mitad:

| Material | Densidad ($\text{g/cm}^3$) | HVL para Rayos Gamma ($1\text{ MeV}$) | HVL para Protones ($100\text{ MeV}$) |
| :--- | :--- | :--- | :--- |
| **Aluminio-Litio (Tier 1)** | $2.60$ | $4.50\text{ cm}$ | $3.80\text{ cm}$ |
| **Polietileno Borado** | $0.95$ | $9.20\text{ cm}$ | $1.90\text{ cm}$ (Excelente contra neutrones) |
| **Tungsteno ($W$ - Tier 2)** | $19.25$ | **$0.75\text{ cm}$** | **$0.90\text{ cm}$** (Blindaje ultra-denso) |
| **Plomo ($Pb$)** | $11.34$ | $1.20\text{ cm}$ | $1.40\text{ cm}$ |

---

## 4. Escala de Toxicidad y Efectos Médicos de la Dosis Acumulada

El monitor biológico de la cabina evalúa la dosis efectiva absorbida según estándares de medicina aeroespacial (NASA / ESA):

$$\text{Dosis Equivalente } H = D \times Q \quad [\text{Sieverts, Sv}]$$

* **$< 0.25\text{ Sv} \ (250\text{ mSv})$:** Nivel Seguro. Sin síntomas clínicos ni degradación de rendimiento.
* **$0.25 - 1.0\text{ Sv}$:** Alerta Médica Nivel 1. Mareo leve, reducción del conteo de glóbulos blancos. La interfaz emite advertencia amarilla.
* **$1.0 - 3.5\text{ Sv}$:** Estado Crítico Nivel 2. Náuseas severas, fatiga, síndrome de radiación aguda. Las alarmas de cabina pasan a rojo estroboscópico.
* **$> 5.0\text{ Sv}$:** Dosis Letal $LD_{50/30}$. Fallo biológico irreversible y colapso de aviónica no reforzada si no se evacúa o apantalla inmediatamente.

