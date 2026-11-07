# 07. Modelo Computacional y Discretización Numérica para la Simulación

Este documento extrae y sintetiza de forma operativa los algoritmos matemáticos y ecuaciones exactas necesarias para implementar la simulación computacional de la métrica de Alcubierre en un motor gráfico (como Unity).

---

## 1. Parámetros Fundamentales del Sistema

Para simular computacionalmente la burbuja de Alcubierre con rigor físico, definimos el siguiente vector de estado y parámetros de configuración:

```csharp
// Parámetros Físicos de la Burbuja
float R = 5.0f;          // Radio de la burbuja (metros o unidades de simulación)
float sigma = 1.2f;      // Parámetro de grosor de pared (Delta ~ 1 / sigma)
float c = 10.0f;         // Velocidad de la luz normalizada en el entorno virtual
float vs = 15.0f;        // Velocidad aparente de la nave (vs > c para régimen superlumínico)

// Variables de Estado de la Nave
Vector3 shipPosition;    // Posición del centro de la burbuja xs(t)
Vector3 shipVelocity;    // Vector velocidad de avance vs(t)
float properTime;        // Tiempo propio acumulado por la tripulación (tau)
float coordinateTime;    // Tiempo coordenado del universo exterior (t)
```

---

## 2. Algoritmos Analíticos de Evaluación Local

Para cualquier punto del espacio $\vec{p} = (x, y, z)$ en la simulación, el sistema calcula en tiempo real las siguientes funciones escalares y tensoriales:

### 2.1 Distancia Radial Relativa $r_s$
$$r_s(\vec{p}, t) = \|\vec{p} - \vec{x}_s(t)\| = \sqrt{(x - x_s)^2 + (y - y_s)^2 + (z - z_s)^2}$$

### 2.2 Función de Forma Suave $f(r_s)$
$$f(r_s) = \frac{\tanh(\sigma (r_s + R)) - \tanh(\sigma (r_s - R))}{2 \tanh(\sigma R)}$$

### 2.3 Derivada Radial Analítica $f'(r_s) = \frac{df}{dr_s}$
Puesto que $\frac{d}{du}\tanh(u) = \text{sech}^2(u) = 1 - \tanh^2(u)$:

$$f'(r_s) = \frac{\sigma \left[ \left(1 - \tanh^2(\sigma(r_s + R))\right) - \left(1 - \tanh^2(\sigma(r_s - R))\right) \right]}{2 \tanh(\sigma R)}$$

### 2.4 Vector de Desplazamiento Espacial $\vec{\beta}(\vec{p})$
$$\vec{\beta}(\vec{p}) = -\frac{\vec{v}_s}{c} f(r_s)$$

---

## 3. Capas de Representación Visual Fidedigna

Para que la simulación refleje con exactitud la física de la Relatividad General, se establecen tres capas visuales simultáneas en tiempo real:

### 3.1 Capa A: Deformación de la Malla del Espacio-Tiempo (Curvatura)
En la Game View, el tejido del espacio-tiempo se representa mediante una grilla poligonal plana $xy$ cuyos vértices se desplazan verticalmente en el eje $z$ (o perpendicularmente) en función del **escalar de expansión $\theta$**:

$$\Delta h(x, y) = K_{\text{visual}} \cdot \theta(x, y)$$
$$\theta(x, y) = \frac{v_s}{c} \frac{x - x_s}{r_s} f'(r_s)$$

- **Frente a la nave ($x > x_s$):** $\theta < 0 \implies$ Depresión / Pozo gravitacional de contracción (color azul cian). El espacio se contrae trayendo los objetos hacia la nave.
- **Detrás de la nave ($x < x_s$):** $\theta > 0 \implies$ Elevación / Cresta de expansión (color rojo magenta). El espacio se expande empujando la nave hacia adelante.
- **Centro de la nave ($r_s < R$):** $\theta = 0 \implies$ Malla perfectamente horizontal y plana.

### 3.2 Capa B: Halo de Densidad de Energía Exótica $\rho$
La energía negativa necesaria para mantener la burbuja se calcula en cada punto del toroide ecuatorial:

$$\rho(x, y, z) = -\frac{c^2 v_s^2}{32 \pi G} \left( \frac{(y - y_s)^2 + (z - z_s)^2}{r_s^2} \right) \left( f'(r_s) \right)^2$$

- Se mapea visualmente mediante un sistema de partículas o shader volumétrico con brillo incandescente (luz violeta / ultravioleta de energía de punto cero) concentrado en el anillo transversal a la velocidad de la nave.

### 3.3 Capa C: Arrastre de Partículas Libres y Desviación Geodésica
Para representar polvo estelar, fotones o escombros en el camino de la burbuja, sus posiciones se actualizan bajo el arrastre de coordenadas:

$$\vec{v}_{\text{aparente}} = \vec{v}_{\text{propia}} + c \vec{\beta} = \vec{v}_{\text{propia}} - \vec{v}_s f(r_s)$$

- Las partículas que tocan la pared frontal no colisionan con el parabrisas de la nave; en su lugar, la geometría del espacio-tiempo las desvía alrededor del perímetro de la burbuja, expulsándolas por la popa (lente gravitacional y flujo de desvío inercial).

---

## 4. Cinemática de la Nave y Relatividad del Tiempo

En cada paso de integración temporal $\Delta t$:

1. **Posición de la nave:**
   $$\vec{x}_s(t + \Delta t) = \vec{x}_s(t) + \vec{v}_s(t) \Delta t$$
2. **Tiempo del Universo (Coordenado):**
   $$t_{\text{universo}} \leftarrow t_{\text{universo}} + \Delta t$$
3. **Tiempo de la Tripulación (Propio):**
   $$\tau_{\text{nave}} \leftarrow \tau_{\text{nave}} + \Delta t \quad (d\tau = dt)$$
4. **Fuerzas G Sentidas por los Astronautas:**
   $$g_{\text{inercial}} = 0.00 \, g \quad (\text{Caída libre pura en reposo local})$$
5. **Velocidad Aparente:**
   $$\text{Factor Warp } W = \frac{\|\vec{v}_s\|}{c}$$
   - Si $W < 1$: Régimen sublumínico (viable con energía positiva ordinaria según Bobrick & Martire).
   - Si $W = 1$: Velocidad de la luz ($c$).
   - Si $W > 1$: Régimen superlumínico (viaje hiperlumínico de Alcubierre).
