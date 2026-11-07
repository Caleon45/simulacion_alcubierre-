# 01. Relatividad General y Estructura del Espacio-Tiempo

## 1. Introducción y Fundamentos Físicos

La física moderna descansa sobre dos grandes pilares formulados por Albert Einstein: la **Relatividad Especial (1905)** y la **Relatividad General (1915)**. Comprender el mecanismo de propulsión por distorsión (*Warp Drive*) formulado por Miguel Alcubierre exige desglosar con precisión matemática cómo la relatividad concibe el espacio, el tiempo y la gravitación.

### 1.1 El Límite Local de la Velocidad de la Luz ($c$)
En la Relatividad Especial, ningún cuerpo con masa inercial positiva puede acelerarse hasta alcanzar o superar la velocidad de la luz en el vacío ($c \approx 299.792.458 \text{ m/s}$). Si un objeto se mueve respecto a un sistema de referencia inercial local, su energía relativista diverge:

$$E = \gamma m_0 c^2 = \frac{m_0 c^2}{\sqrt{1 - \frac{v^2}{c^2}}}$$

A medida que $v \to c$, $\gamma \to \infty$, requiriendo energía infinita.

> **Distinción Fundamental en Relatividad General:**
> La restricción $v < c$ es una ley **estrictamente local**. Establece que ningún objeto material puede superar a un rayo de luz en su vecindad inmediata (dentro del cono de luz local). Sin embargo, la Relatividad General no impone límite alguno a la velocidad con la que el **propio tejido del espacio-tiempo puede expandirse, contraerse o deformarse**. El ejemplo cosmológico paradigmático es la inflación cósmica y la expansión del universo observable: galaxias distantes se alejan unas de otras a velocidades aparentes superiores a $c$ debido a la expansión métrica del espacio, sin que ninguna galaxia viole la relatividad especial localmente.

---

## 2. Geometría Riemanniana y el Tensor Métrico

En cuatro dimensiones (espacio-tiempo continuo), los eventos se parametrizan por cuatro coordenadas contravariantes $x^\mu = (x^0, x^1, x^2, x^3) = (c t, x, y, z)$, donde los índices griegos van de $0$ a $3$ y los índices latinos de $1$ a $3$.

### 2.1 Espacio Plano de Minkowski
En ausencia de gravedad y aceleración, el intervalo espacio-temporal cuadrático $ds^2$ está gobernado por la métrica de Minkowski $\eta_{\mu\nu} = \text{diag}(-1, +1, +1, +1)$:

$$ds^2 = \eta_{\mu\nu} dx^\mu dx^\nu = -c^2 dt^2 + dx^2 + dy^2 + dz^2$$

- Si $ds^2 < 0$: Intervalo de tipo temporal (*timelike*), trayectoria permitida para partículas masivas.
- Si $ds^2 = 0$: Intervalo de tipo nulo (*null* / *lightlike*), trayectoria de fotones y ondas gravitacionales.
- Si $ds^2 > 0$: Intervalo de tipo espacial (*spacelike*), inaccesible para señales causales estándar.

### 2.2 Espacio Curvo General y Métrica $g_{\mu\nu}$
En presencia de curvatura, el elemento de línea infinitesimal se generaliza a:

$$ds^2 = g_{\mu\nu}(x) dx^\mu dx^\nu$$

El tensor métrico $g_{\mu\nu}$ es una matriz simétrica $4 \times 4$ covariante que contiene toda la información geométrica, define los productos escalares entre cuadrivectores y determina cómo se miden distancias y lapsos temporales.

---

## 3. Ecuaciones de Campo de Einstein (EFE)

Publicadas en noviembre de 1915, las Ecuaciones de Campo de Einstein relacionan la geometría del espacio-tiempo con el contenido material y energético del universo:

$$G_{\mu\nu} \equiv R_{\mu\nu} - \frac{1}{2} R g_{\mu\nu} = \frac{8\pi G}{c^4} T_{\mu\nu}$$

Donde:
- **$G_{\mu\nu}$**: Tensor de Einstein, representa la curvatura del espacio-tiempo.
- **$R_{\mu\nu}$**: Tensor de curvatura de Ricci, obtenido por contracción del tensor de curvatura de Riemann:
  $$R_{\mu\nu} = R^\rho_{\ \mu\rho\nu} = \partial_\rho \Gamma^\rho_{\mu\nu} - \partial_\nu \Gamma^\rho_{\mu\rho} + \Gamma^\rho_{\rho\lambda}\Gamma^\lambda_{\mu\nu} - \Gamma^\rho_{\nu\lambda}\Gamma^\lambda_{\mu\rho}$$
- **$R$**: Escalar de curvatura de Ricci (curvatura escalar):
  $$R = g^{\mu\nu} R_{\mu\nu}$$
- **$T_{\mu\nu}$**: Tensor de energía-impulso, describe la densidad de masa-energía, el flujo de momento y las tensiones/presiones de la materia presente.
- **$G$**: Constante de gravitación universal de Newton ($6.67430 \times 10^{-11} \text{ m}^3 \text{kg}^{-1} \text{s}^{-2}$).
- **$c$**: Velocidad de la luz en el vacío ($2.99792458 \times 10^8 \text{ m/s}$).
- El coeficiente de acoplamiento es $\kappa = \frac{8\pi G}{c^4} \approx 2.076 \times 10^{-43} \text{ s}^2 \text{m}^{-1} \text{kg}^{-1}$.

### 3.1 El Método Inverso ("Ingeniería del Espacio-Tiempo")
En física gravitacional ordinaria, el método habitual es **directo**:
$$\text{Distribución de materia conocida } T_{\mu\nu} \longrightarrow \text{Resolver las EFE} \longrightarrow \text{Métrica } g_{\mu\nu}$$

Miguel Alcubierre adoptó el **enfoque inverso**:
1. Postular matemáticamente una geometría del espacio-tiempo $g_{\mu\nu}$ con las propiedades cinemáticas deseadas (una burbuja que viaje a velocidad arbitraria $v_s(t)$).
2. Calcular analíticamente su tensor de Einstein $G_{\mu\nu}$.
3. Invertir las ecuaciones de Einstein para deducir qué distribución de energía-impulso $T_{\mu\nu} = \frac{c^4}{8\pi G} G_{\mu\nu}$ exige la naturaleza para generar y sostener dicha burbuja.

---

## 4. El Formalismo 3+1 de Arnowitt-Deser-Misner (ADM)

Para analizar dinámicamente geometrías arbitrarias y plantear simulaciones numéricas de relatividad, el formalismo **3+1 ADM** descompone la variedad tetradimensional $\mathcal{M}$ en una familia monoparamétrica de hipersuperficies espaciales tridimensionales $\Sigma_t$ parametrizadas por el tiempo coordenado $t$.

El elemento de línea en el formalismo ADM se escribe de forma canónica como:

$$ds^2 = - (\alpha^2 - \beta_i \beta^i) c^2 dt^2 + 2 \beta_i c dt dx^i + \gamma_{ij} dx^i dx^j$$

Donde:
1. **Función Lapso ($\alpha$)**: Escalar estrictamente positivo que mide la razón de cambio del tiempo propio $\tau$ medido por un observador euleriano perpendicular a $\Sigma_t$ con respecto al tiempo coordenado $t$:
   $$d\tau = \alpha dt$$
2. **Vector de Desplazamiento ($\beta^i$)**: Vector espacial tridimensional que describe el arrastre o cambio relativo de coordenadas espaciales entre hipersuperficies temporales contiguas:
   $$\beta_i = \gamma_{ij} \beta^j$$
3. **Métrica Espacial Inducida ($\gamma_{ij}$)**: Tensor simétrico $3 \times 3$ que describe la geometría puramente espacial dentro de cada rebanada $\Sigma_t$.

### 4.2 Vectores Normales y Observadores Eulerianos
El vector unitario normal a la hipersuperficie $\Sigma_t$ se define covariante y contravariante como:

$$n_\mu = (-\alpha c, 0, 0, 0)$$
$$n^\mu = \frac{1}{\alpha} \left( \frac{1}{c}, -\beta^i \right)$$

Satisfaciendo la condición de normalización de tipo temporal:
$$n_\mu n^\mu = -1$$

Los observadores con cuadrivelocidad $u^\mu = n^\mu$ se denominan **observadores eulerianos** o en reposo respecto a la foliación espacial.

### 4.3 Curvatura Extrínseca ($K_{ij}$)
La curvatura intrínseca de cada hipersuperficie espacial viene dada por el tensor de Riemann tridimensional derivado de $\gamma_{ij}$. Sin embargo, la manera en que $\Sigma_t$ se dobla e incrusta dentro del espacio-tiempo de cuatro dimensiones está gobernada por el tensor de **curvatura extrínseca** $K_{ij}$:

$$K_{ij} = -\frac{1}{2\alpha} \left( \partial_t \gamma_{ij} - D_i \beta_j - D_j \beta_i \right)$$

Donde $D_i$ es la derivada covariante tridimensional asociada a la 3-métrica $\gamma_{ij}$.

La traza de la curvatura extrínseca:
$$K \equiv \gamma^{ij} K_{ij}$$

Tiene un significado físico trascendental: representa la **tasa de expansión o contracción del volumen propio** medido por observadores eulerianos. Como se verá en la métrica de Alcubierre, esta traza es exactamente la que genera la contracción al frente y la expansión detrás de la burbuja warp.
