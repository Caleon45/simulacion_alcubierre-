# 03. Dinámica Relativista y Ecuaciones del Cohete de Antimateria

Para velocidades que superan el $1\%$ de la velocidad de la luz ($v > 0.01c$), la física newtoniana y la ecuación clásica de Tsiolkovsky dejan de ser válidas y sobreestiman groseramente el rendimiento. Es necesario aplicar la **Mecánica Relativista de Einstein**.

---

## 1. Deducción de la Ecuación del Cohete Relativista

Consideremos una nave espacial con masa instantánea $m$ y velocidad $v$ respecto a un observador inercial en reposo. En su propio sistema de referencia instantáneo en reposo, la nave expulsa una masa infinitesimal $dm$ a una velocidad de escape $v_e$.

### 1.1 Transformación Relativista de Velocidades y Rapidez
En relatividad especial, las velocidades no se suman linealmente. En su lugar, se utiliza el parámetro de **rapidez** $\theta$:

$$\theta = \operatorname{arctanh}\left(\frac{v}{c}\right) = \frac{1}{2} \ln \left( \frac{1 + v/c}{1 - v/c} \right)$$

Bajo transformaciones de Lorentz colineales, las rapideces se suman de forma estrictamente lineal:
$$d\theta = \frac{v_e}{c} \frac{dm}{m}$$

Integrando desde la masa inicial con combustible $m_0$ (velocidad $v=0 \implies \theta=0$) hasta la masa final seca $m_f$:

$$\int_0^\theta d\theta' = -\frac{v_e}{c} \int_{m_0}^{m_f} \frac{dm}{m}$$
$$\theta = \frac{v_e}{c} \ln \left( \frac{m_0}{m_f} \right)$$

### 1.2 Ecuación Relativista de Velocidad Final
Despejando la velocidad $\frac{v}{c} = \tanh(\theta)$:

$$\frac{v}{c} = \tanh \left[ \frac{v_e}{c} \ln \left( \frac{m_0}{m_f} \right) \right]$$

Utilizando la identidad exponencial de la tangente hiperbólica $\tanh(x) = \frac{e^{2x} - 1}{e^{2x} + 1}$:

$$\frac{v}{c} = \frac{1 - \left( \frac{m_f}{m_0} \right)^{\frac{2 v_e}{c}}}{1 + \left( \frac{m_f}{m_0} \right)^{\frac{2 v_e}{c}}}$$

> **Ecuación del Cohete Relativista:**
> Para cualquier relación de masa finita, $\frac{v}{c} < 1$ de forma natural e inviolable. A medida que la masa de combustible tiende a infinito ($m_0/m_f \to \infty$), $v \to c$ asintóticamente sin violar la velocidad límite de la luz.

---

## 2. Razón de Masa y Consumo Exacto de Antimateria

Invirtiendo la ecuación anterior para determinar cuánta masa inicial $m_0$ se requiere para acelerar una carga útil $m_f$ hasta una velocidad relativista $v$:

$$\frac{m_0}{m_f} = \left( \frac{1 + \frac{v}{c}}{1 - \frac{v}{c}} \right)^{\frac{c}{2 v_e}}$$

### 2.1 Caso Óptimo del Motor Beam-Core ($v_e = 0.5c$)
Para un motor de núcleo de haz de aniquilación $p + \bar{p}$ bien colimado, la velocidad efectiva de escape de los piones cargados es aproximadamente $v_e = 0.5 c$.

Sustituyendo $\frac{c}{2 v_e} = \frac{c}{2 (0.5c)} = 1$, la relación de masa se simplifica drásticamente a:

$$\frac{m_0}{m_f} = \frac{1 + \frac{v}{c}}{1 - \frac{v}{c}}$$

La masa total de combustible requerida es $m_{\text{propelente}} = m_0 - m_f$.  
Puesto que el propelente consta de partes iguales de materia y antimateria ($50\% \ p$ y $50\% \ \bar{p}$):

$$m_{\text{antimateria}} = \frac{1}{2} (m_0 - m_f) = \frac{m_f}{2} \left( \frac{m_0}{m_f} - 1 \right) = m_f \left( \frac{\frac{v}{c}}{1 - \frac{v}{c}} \right)$$

---

## 3. Tabla de Requerimientos Numéricos Exactos

Asumiendo una sonda interestelar científica moderna con una masa final útil de **$m_f = 1.000 \text{ kg}$ (1 tonelada)** y un motor *Beam-Core* ($v_e = 0.5c$):

| Velocidad Objetivo ($v/c$) | Razón de Masa ($m_0/m_f$) | Masa Antimateria ($kg$) | Masa Materia ($kg$) | Tiempo de Vuelo a Próxima Centauri (4.24 ly) |
| :--- | :--- | :--- | :--- | :--- |
| **0.05 c** (15.000 km/s) | $1.105$ | **$52.6 \text{ kg}$** | $52.6 \text{ kg}$ | $84.8 \text{ años}$ |
| **0.10 c** (30.000 km/s) | $1.222$ | **$111.1 \text{ kg}$** | $111.1 \text{ kg}$ | $42.4 \text{ años}$ |
| **0.20 c** (60.000 km/s) | $1.500$ | **$250.0 \text{ kg}$** | $250.0 \text{ kg}$ | $21.2 \text{ años}$ |
| **0.30 c** (90.000 km/s) | $1.857$ | **$428.6 \text{ kg}$** | $428.6 \text{ kg}$ | $14.1 \text{ años}$ |
| **0.50 c** (150.000 km/s) | $3.000$ | **$1.000.0 \text{ kg}$** | $1.000.0 \text{ kg}$ | $8.5 \text{ años}$ |
| **0.80 c** (240.000 km/s) | $9.000$ | **$4.000.0 \text{ kg}$** | $4.000.0 \text{ kg}$ | $5.3 \text{ años}$ |

> **Comparación Demoledora contra Cohetes Químicos:**
> Para alcanzar apenas el **$10\%$ de la velocidad de la luz ($0.1c$)** con el mejor cohete químico posible ($v_e = 4.5 \text{ km/s} = 0.000015c$):
> $$\frac{m_0}{m_f} = e^{\Delta v / v_e} = e^{30.000.000 / 4.500} = e^{6.666} \approx 10^{2.895}$$
> La masa de combustible requerida superaría por trillones de veces la masa total de todos los átomos del universo observable ($10^{80}$ átomos).  
> **Con antimateria, solo se requieren 111 kg.**

---

## 4. Viaje con Aceleración y Frenado en el Destino

Para una misión que deba entrar en órbita en Alfa Centauri o un exoplaneta, la nave debe acelerar durante la primera mitad del viaje y frenar (encender los motores en sentido opuesto) durante la segunda mitad.

La relación de masa para una misión de dos fases (acelerar a $+v$ y frenar a $0$) es el producto de las dos relaciones:

$$\left(\frac{m_0}{m_f}\right)_{\text{completa}} = \left( \frac{1 + \frac{v}{c}}{1 - \frac{v}{c}} \right)^{\frac{c}{v_e}}$$

Para $v = 0.2c$ con $v_e = 0.5c$:
$$\left(\frac{m_0}{m_f}\right)_{\text{completa}} = \left( 1.50 \right)^2 = 2.25$$

Para frenar una sonda de 1 tonelada al llegar a Próxima Centauri a $0.2c$, la masa inicial total en la Tierra es de solo **$2.250 \text{ kg}$** ($625 \text{ kg}$ de antimateria y $625 \text{ kg}$ de materia).

---

## 5. Dilatación Temporal Relativista de la Tripulación

A diferencia de la nave de Alcubierre (donde $d\tau = dt$ porque el espacio viaja con la nave), la nave de antimateria es un **vehículo inercial ordinario viajando a velocidades relativistas a través del espacio plano**.

Por tanto, está sujeta a la **dilatación del tiempo de la Relatividad Especial**:

$$d\tau = dt \sqrt{1 - \frac{v^2}{c^2}} = \frac{dt}{\gamma}$$

Para una velocidad crucero de $v = 0.5c$:
$$\gamma = \frac{1}{\sqrt{1 - 0.5^2}} = \frac{1}{\sqrt{0.75}} \approx 1.155$$

- Mientras en la Tierra transcurren $8.50 \text{ años}$, los astronautas dentro de la nave envejecen solo:
  $$\tau = \frac{8.50}{1.155} \approx 7.36 \text{ años}$$
- Si la nave alcanzara $0.99c$ ($\gamma \approx 7.09$), un viaje de 40 años medido desde la Tierra transcurriría en tan solo **$5.64 \text{ años}$** de tiempo propio para la tripulación.
