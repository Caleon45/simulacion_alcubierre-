# 02. La Métrica de Alcubierre (1994)

## 1. Contexto Histórico

En mayo de 1994, el físico teórico mexicano **Miguel Alcubierre Moya** (entonces investigador en la Universidad de Gales, Cardiff) publicó en la prestigiosa revista *Classical and Quantum Gravity* el artículo seminal:

> **"The warp drive: hyper-fast travel within general relativity"**  
> *Class. Quantum Grav. 11 (1994) L73–L77.*

Inspirado en la idea de ciencia ficción de la propulsión por curvatura (*Warp Drive* de *Star Trek*), Alcubierre se preguntó si las ecuaciones de la Relatividad General de Einstein permitían concebir una configuración del espacio-tiempo donde una nave pudiera viajar entre dos puntos distantes a velocidades efectivas arbitrariamente altas ($v > c$), sin violar el principio relativista de que nada puede viajar más rápido que la luz localmente.

La respuesta matemática fue afirmativa: mediante una deformación geométrica asimétrica del espacio-tiempo, es teóricamente posible desplazar una región de espacio plano donde viaja la nave.

---

## 2. El Elemento de Línea de Alcubierre

En coordenadas cartesianas $(t, x, y, z)$, la métrica de Alcubierre adopta la forma elegante y compacta:

$$ds^2 = -c^2 dt^2 + \left[ dx - v_s(t) f(r_s) dt \right]^2 + dy^2 + dz^2$$

Desarrollando el binomio al cuadrado:

$$ds^2 = - \left( c^2 - v_s^2(t) f^2(r_s) \right) dt^2 - 2 v_s(t) f(r_s) dx dt + dx^2 + dy^2 + dz^2$$

### 2.1 Mapeo al Formalismo 3+1 ADM
Comparando con la forma general del elemento de línea ADM:
$$ds^2 = - (\alpha^2 - \beta_i \beta^i) c^2 dt^2 + 2 \beta_i c dt dx^i + \gamma_{ij} dx^i dx^j$$

Identificamos inmediatamente los tres componentes estructurales:
1. **Función Lapso ($\alpha$):**
   $$\alpha = 1$$
   La función lapso es constante y unitaria en todo el espacio-tiempo.
2. **Vector de Desplazamiento ($\vec{\beta}$):**
   $$\beta^x = -\frac{v_s(t)}{c} f(r_s), \quad \beta^y = 0, \quad \beta^z = 0$$
   $$\beta_x = \gamma_{xx} \beta^x = -\frac{v_s(t)}{c} f(r_s)$$
3. **Métrica Espacial Inducida ($\gamma_{ij}$):**
   $$\gamma_{ij} = \delta_{ij} = \begin{pmatrix} 1 & 0 & 0 \\ 0 & 1 & 0 \\ 0 & 0 & 1 \end{pmatrix}$$

> **Propiedad Geométrica Extraordinaria:**
> La métrica espacial tridimensional $\gamma_{ij}$ es **estrictamente euclidiana y plana en todas partes**. Toda la dinámica gravitacional, la aparente velocidad hiperlumínica y las fuerzas de marea provienen exclusivamente del **vector de desplazamiento espacial $\beta^i$**, es decir, del arrastre dinámico del sistema de coordenadas.

---

## 3. Coordenadas y Parámetros de la Burbuja

Sea una nave situada en el centro de la burbuja, cuya posición sobre el eje $x$ como función del tiempo coordenado es $x_s(t)$. La velocidad aparente de la nave es:

$$v_s(t) = \frac{dx_s(t)}{dt}$$

La distancia euclidiana radial $r_s$ desde cualquier punto del espacio $(x, y, z)$ al centro de la nave es:

$$r_s(t, x, y, z) = \sqrt{(x - x_s(t))^2 + y^2 + z^2}$$

### 3.1 La Función de Forma (*Shape Function*) $f(r_s)$
Para aislar la nave dentro de una región protegida de espacio-tiempo plano y asegurar que lejos de la nave el espacio-tiempo retorne a la normalidad de Minkowski, Alcubierre introdujo la función de forma analítica:

$$f(r_s) = \frac{\tanh\left(\sigma (r_s + R)\right) - \tanh\left(\sigma (r_s - R)\right)}{2 \tanh(\sigma R)}$$

Donde:
- **$R > 0$**: Radio interior de la burbuja warp (el volumen seguro que aloja a la nave).
- **$\sigma > 0$**: Parámetro de grosor de pared. El grosor efectivo de la pared de la burbuja es $\Delta \approx \frac{1}{\sigma}$.
- En el límite cuando $\sigma \to \infty$, la función de forma se convierte en la función escalón de Heaviside $\Theta(R - r_s)$ (*Top-Hat profile*).

#### Comportamiento Asintótico de $f(r_s)$:
1. **Interior de la burbuja ($r_s \ll R$):**
   $$f(r_s) \approx 1, \quad \frac{df}{dr_s} \approx 0$$
   El elemento de línea se reduce a:
   $$ds^2 = -c^2 dt^2 + (dx - v_s dt)^2 + dy^2 + dz^2$$
   Para un observador en la nave donde $x(t) = x_s(t)$, $dx = v_s dt$, por lo que $(dx - v_s dt) = 0$, resultando en $ds^2 = -c^2 dt^2$.
2. **Exterior lejano ($r_s \gg R$):**
   $$f(r_s) \approx 0, \quad \frac{df}{dr_s} \approx 0$$
   El elemento de línea se reduce a la métrica estándar de Minkowski plana:
   $$ds^2 = -c^2 dt^2 + dx^2 + dy^2 + dz^2$$
3. **Pared de la burbuja ($r_s \approx R$):**
   Transición suave con pendientes pronunciadas $\frac{df}{dr_s} < 0$. Aquí es donde se concentran los gradientes gravitacionales y la energía exótica.

---

## 4. Dinámica de Contracción y Expansión: El Escalar $\theta$

En el formalismo ADM con $\alpha = 1$ y $\gamma_{ij} = \delta_{ij}$, el tensor de curvatura extrínseca se calcula como:

$$K_{ij} = -\frac{1}{2} (\partial_i \beta_j + \partial_j \beta_i)$$

Calculando explícitamente:
- Para $K_{xx}$:
  $$K_{xx} = -\partial_x \beta_x = \frac{v_s}{c} \partial_x [f(r_s)] = \frac{v_s}{c} \frac{df}{dr_s} \frac{\partial r_s}{\partial x} = \frac{v_s}{c} \frac{x - x_s}{r_s} \frac{df}{dr_s}$$
- Para $K_{yy} = K_{zz} = 0$, puesto que $\beta_y = \beta_z = 0$.
- Para componentes cruzadas:
  $$K_{xy} = -\frac{1}{2} \partial_y \beta_x = \frac{v_s}{2c} \frac{y}{r_s} \frac{df}{dr_s}$$
  $$K_{xz} = -\frac{1}{2} \partial_z \beta_x = \frac{v_s}{2c} \frac{z}{r_s} \frac{df}{dr_s}$$

### 4.1 La Traza de la Curvatura Extrínseca (Escalar de Expansión $\theta$)
El escalar de expansión volumétrica medido por observadores eulerianos es la traza $\theta = \text{Tr}(K) = \gamma^{ij} K_{ij}$:

$$\theta = K_{xx} + K_{yy} + K_{zz} = \frac{v_s}{c} \frac{x - x_s}{r_s} \frac{df}{dr_s}$$

Puesto que $f(r_s)$ decae monótonamente hacia afuera, su derivada es estrictamente no positiva en la pared:
$$\frac{df}{dr_s} \le 0$$

Analicemos los signos de $\theta$:
1. **Frente a la nave ($x > x_s$):**
   $$(x - x_s) > 0 \implies \theta = \frac{v_s}{c} \cdot (+) \cdot (-) < 0$$
   Un valor de $\theta < 0$ significa **CONTRACCIÓN ESPACIAL**. El volumen de los elementos de espacio en la proa se comprime a un ritmo proporcional a $v_s$.
2. **Detrás de la nave ($x < x_s$):**
   $$(x - x_s) < 0 \implies \theta = \frac{v_s}{c} \cdot (-) \cdot (-) > 0$$
   Un valor de $\theta > 0$ significa **EXPANSIÓN ESPACIAL**. El volumen de espacio en la popa se dilata continuamente.
3. **Centro de la nave ($r_s = 0$) y Exterior ($r_s \gg R$):**
   $$\frac{df}{dr_s} = 0 \implies \theta = 0$$
   No hay expansión ni contracción.

```
       Detrás de la nave                     Interior                   Frente a la nave
       (x < x_s)                          (r_s < R)                    (x > x_s)
   =========================          =================          =========================
   ESPACIO EN EXPANSIÓN (θ > 0)          ESPACIO PLANO (θ = 0)      ESPACIO EN CONTRACCIÓN (θ < 0)
   Empuja la burbuja hacia adelante      Nave en reposo local       Atrae el destino hacia la nave
   =========================          =================          =========================
                                             🚀
```

---

## 5. Ausencia de Fuerzas Inerciales y Dilatación Temporal

### 5.1 Tiempo Propio de la Tripulación
Consideremos la trayectoria de la nave situada en el origen de la burbuja: $x_s(t)$, $y = 0$, $z = 0$.
A lo largo de su línea de universo, $dx = \dot{x}_s dt = v_s dt$.

Sustituyendo en el elemento de línea con $r_s = 0$ y $f(0) = 1$:
$$ds^2 = -c^2 dt^2 + [v_s dt - v_s (1) dt]^2 + 0 + 0 = -c^2 dt^2$$

Dado que para cualquier observador con masa $ds^2 = -c^2 d\tau^2$:
$$-c^2 d\tau^2 = -c^2 dt^2 \implies d\tau = dt$$

> **Resultado Trascendental:**
> **No existe dilatación temporal relativista para los ocupantes de la nave**. Un segundo dentro de la nave equivale exactamente a un segundo para un observador estacionario en la Tierra o en el destino. No ocurre la paradoja de los gemelos propia del viaje a velocidades relativistas en espacio plano.

### 5.2 Aceleración Inercial Cero (Caída Libre Permanente)
La cuadriaceleración de una partícula viene dada por la derivada covariante de su cuadrivelocidad:
$$a^\mu = u^\nu \nabla_\nu u^\mu = \frac{du^\mu}{d\tau} + \Gamma^\mu_{\alpha\beta} u^\alpha u^\beta$$

Para la trayectoria de la nave en el centro de la burbuja, los símbolos de Christoffel $\Gamma^\mu_{\alpha\beta}$ calculados en $r_s = 0$ son exactamente cero (debido a que $\frac{df}{dr_s}\Big|_{r_s=0} = 0$).

Por consiguiente:
$$a^\mu = 0$$

La nave viaja en **geodésica pura** (caída libre inercial). La tripulación no experimenta ninguna fuerza G, aceleración centrífuga, aplastamiento contra los mamparos ni tensiones mecánicas, incluso si la burbuja acelera instantáneamente de $0$ a $100c$.
