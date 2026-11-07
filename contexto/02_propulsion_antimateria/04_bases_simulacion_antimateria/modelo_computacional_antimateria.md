# 04. Modelo Computacional de la Nave de Antimateria para Unity

Este documento detalla la formulación matemática y algorítmica para programar el comportamiento físico de la nave de antimateria dentro del motor de simulación.

---

## 1. Parámetros y Variables de Estado

```csharp
// Constantes Físicas Normalizadas
float c = 100.0f;               // Velocidad de la luz en unidades de simulación (u/s)
float g0 = 9.81f;               // Aceleración gravitacional estándar

// Parámetros del Motor de Antimateria (Beam-Core)
float ve = 0.5f * c;            // Velocidad de escape de piones (50% de c)
float burnRate = 0.05f;         // Tasa de consumo de masa total por segundo (kg/s)
float dryMass = 1000.0f;        // Masa seca de la nave (kg)
float antimatterFuel = 250.0f;  // Masa de antimateria a bordo (kg)
float matterFuel = 250.0f;      // Masa de materia ordinaria de reacción (kg)

// Variables de Estado Dinámicas
Vector3 position = Vector3.zero;
float velocity = 0.0f;          // Velocidad escalar sobre el eje de avance
float currentMass;              // dryMass + antimatterFuel + matterFuel
float coordinateTime = 0.0f;    // Tiempo del observador en reposo (t)
float properTime = 0.0f;        // Tiempo propio de la nave (tau)
float gamma = 1.0f;             // Factor de Lorentz
float feltAccelerationG = 0.0f; // Fuerzas G sentidas por la tripulación
```

---

## 2. Ecuaciones Cinemáticas y Dinámica Relativista

En cada frame de simulación con paso temporal $\Delta t$:

### 2.1 Consumo de Combustible
Si aún queda antimateria disponible ($m_{\text{antimateria}} > 0$):
$$\Delta m = \dot{m} \Delta t$$
$$m_{\text{antimateria}} \leftarrow m_{\text{antimateria}} - \frac{\Delta m}{2}$$
$$m_{\text{materia}} \leftarrow m_{\text{materia}} - \frac{\Delta m}{2}$$
$$m(t) = m_{\text{seca}} + m_{\text{antimateria}} + m_{\text{materia}}$$

### 2.2 Fuerza y Aceleración Relativista Longitudinal
En relatividad especial, la segunda ley de Newton para aceleración en la dirección del movimiento es:
$$F = \frac{dp}{dt} = \frac{d}{dt}(\gamma m v) = \gamma^3 m \frac{dv}{dt}$$

El empuje generado por el cohete de antimateria es $F_{\text{empuje}} = \dot{m} v_e$. Igualando ambas expresiones:

$$\frac{dv}{dt} = \frac{\dot{m} v_e}{m(t)} \frac{1}{\gamma^3} = \frac{\dot{m} v_e}{m(t)} \left( 1 - \frac{v^2}{c^2} \right)^{3/2}$$

> **Propiedad Física Clave:**
> A medida que la nave se aproxima a la velocidad de la luz ($v \to c$), el término $(1 - v^2/c^2)^{3/2} \to 0$. La aceleración en el marco coordenado exterior disminuye suave y continuamente hacia cero, impidiendo que la nave jamás alcance o supere $c$.

### 2.3 Aceleración Propia Sentida por la Tripulación
La aceleración que sienten los astronautas en su propio sistema inercial acelerado (fuerza G sobre el cuerpo) es:
$$a_{\text{propia}} = \gamma^3 \frac{dv}{dt} = \frac{\dot{m} v_e}{m(t)}$$
$$G_{\text{tripulacion}} = \frac{a_{\text{propia}}}{g_0}$$

### 2.4 Factor de Lorentz y Dilatación Temporal
$$\gamma = \frac{1}{\sqrt{1 - \frac{v^2}{c^2}}}$$
$$t_{\text{universo}} \leftarrow t_{\text{universo}} + \Delta t$$
$$\tau_{\text{nave}} \leftarrow \tau_{\text{nave}} + \frac{\Delta t}{\gamma}$$

---

## 3. Efectos Visuales Relativistas en Game View

A diferencia de la nave warp (donde el espacio se deforma alrededor), la nave de antimateria experimenta los fenómenos visuales canónicos de la Relatividad Especial cuando viaja a velocidades $v \ge 0.1c$:

### 3.1 Efecto Doppler Relativista
La frecuencia observada de la luz estelar incidente cambia según el ángulo de incidencia $\theta$:

$$f_{\text{obs}} = f_0 \frac{\sqrt{1 - \frac{v^2}{c^2}}}{1 - \frac{v}{c} \cos\theta}$$

- **Hacia adelante ($\theta = 0$):**
  $$f_{\text{frente}} = f_0 \sqrt{\frac{1 + v/c}{1 - v/c}}$$
  Corrimiento al azul masivo (*Blueshift*). Las estrellas amarillas/rojas se vuelven azules, violetas y emiten en el ultravioleta y rayos X.
- **Hacia atrás ($\theta = \pi$):**
  $$f_{\text{popa}} = f_0 \sqrt{\frac{1 - v/c}{1 + v/c}}$$
  Corrimiento al rojo severo (*Redshift*). Las estrellas se desvanecen en infrarrojo y ondas de radio.

### 3.2 Aberración Relativista (Efecto Reflector / *Searchlight*)
El campo de visión se concentra hacia la proa de la nave. Las estrellas que se encuentran en los costados o atrás parecen desplazarse ópticamente hacia el frente debido a la suma relativista del ángulo del vector de onda fotónico:

$$\cos\theta_{\text{aparente}} = \frac{\cos\theta + \frac{v}{c}}{1 + \frac{v}{c} \cos\theta}$$

A $v = 0.5c$, todas las estrellas visibles en el hemisferio posterior se comprimen visualmente en un cono brillante frente a la nave.

### 3.3 Visualización del Haz de Piones de la Tobera
- Un sistema de partículas orientado hacia la popa emite un chorro hiperbólico de partículas violetas y azuladas de alta intensidad colimadas por el campo magnético, simulando la expulsión de piones relativistas $\pi^\pm$.
