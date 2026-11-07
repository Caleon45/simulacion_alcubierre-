# 04. Ecuaciones Matemáticas y Cálculo Tensorial Completo

Este documento presenta el desglose matemático formal y riguroso de la métrica de Alcubierre, derivando paso a paso los tensores fundamentales requeridos para implementar simulaciones numéricas físicamente exactas.

---

## 1. El Tensor Métrico y su Inverso

En coordenadas cartesianas estándar $x^\mu = (x^0, x^1, x^2, x^3) = (c t, x, y, z)$, tomamos $x^0 = c t$ (o equivalentemente $t$ con $c=1$ para simplificar notación analítica; mantendremos $c$ explícita para rigor computacional).

### 1.1 Matriz del Tensor Métrico Covariante $g_{\mu\nu}$
A partir del elemento de línea:
$$ds^2 = - \left( c^2 - v_s^2 f^2 \right) dt^2 - 2 v_s f dx dt + dx^2 + dy^2 + dz^2$$

Escribiendo $g_{\mu\nu}$ en forma matricial con coordenadas $(t, x, y, z)$:

$$g_{\mu\nu} = \begin{pmatrix} 
-(c^2 - v_s^2 f^2) & -v_s f & 0 & 0 \\ 
-v_s f & 1 & 0 & 0 \\ 
0 & 0 & 1 & 0 \\ 
0 & 0 & 0 & 1 
\end{pmatrix}$$

### 1.2 Determinante de la Métrica
Calculamos el determinante mediante la expansión por menores:
$$\det(g) = g_{00} g_{11} - g_{01} g_{10} = -(c^2 - v_s^2 f^2)(1) - (-v_s f)(-v_s f)$$
$$\det(g) = -c^2 + v_s^2 f^2 - v_s^2 f^2 = -c^2$$

$$\det(g_{\mu\nu}) = -c^2 = \text{constante}$$

> **Propiedad Matemática Crucial:**
> El determinante métrico es **estrictamente constante en todo el espacio-tiempo**, idéntico al del espacio plano de Minkowski (para $c=1$, $g = -1$). Esto implica que el elemento de volumen covariante invariantivo es trivial:
> $$d^4 V = \sqrt{-g} \, d^4 x = c \, dt \, dx \, dy \, dz$$
> No hay singularidades de volumen ni compresión de coordenadas globales.

### 1.3 Matriz del Tensor Métrico Contravariante (Inverso) $g^{\mu\nu}$
La matriz inversa $g^{\mu\nu}$ que satisface $g^{\mu\alpha} g_{\alpha\nu} = \delta^\mu_\nu$ es:

$$g^{\mu\nu} = \begin{pmatrix} 
-\frac{1}{c^2} & -\frac{v_s f}{c^2} & 0 & 0 \\ 
-\frac{v_s f}{c^2} & 1 - \frac{v_s^2 f^2}{c^2} & 0 & 0 \\ 
0 & 0 & 1 & 0 \\ 
0 & 0 & 0 & 1 
\end{pmatrix}$$

Podemos comprobar inmediatamente la primera fila:
$$g^{00} g_{00} + g^{01} g_{10} = \left(-\frac{1}{c^2}\right)(-(c^2 - v_s^2 f^2)) + \left(-\frac{v_s f}{c^2}\right)(-v_s f) = \frac{c^2 - v_s^2 f^2 + v_s^2 f^2}{c^2} = 1 \quad \checkmark$$

---

## 2. Derivadas Parciales de la Métrica

Para abreviar, definimos las derivadas espaciales de la función de forma $f(r_s)$:
$$\partial_i f = \frac{df}{dr_s} \frac{\partial r_s}{\partial x^i} = \frac{x^i - x_s^i}{r_s} f'$$
Donde $f' \equiv \frac{df}{dr_s}$, y denotamos $x_s^1 = x_s$, $x_s^2 = 0$, $x_s^3 = 0$.

Para la derivada temporal $\partial_t f$:
$$\partial_t f = f' \frac{\partial r_s}{\partial t} = f' \frac{-(x - x_s)\dot{x}_s}{r_s} = -v_s \frac{x - x_s}{r_s} f' = -v_s \partial_x f$$

Derivadas de los componentes no nulos de $g_{\mu\nu}$:
$$\partial_i g_{00} = 2 v_s^2 f \partial_i f$$
$$\partial_t g_{00} = 2 v_s \dot{v}_s f^2 + 2 v_s^2 f \partial_t f = 2 v_s \dot{v}_s f^2 - 2 v_s^3 f \partial_x f$$
$$\partial_i g_{01} = -v_s \partial_i f$$
$$\partial_t g_{01} = -\dot{v}_s f - v_s \partial_t f = -\dot{v}_s f + v_s^2 \partial_x f$$
Todas las derivadas espaciales de $g_{ij}$ son idénticamente cero ($\partial_\mu g_{ij} = 0$).

---

## 3. Símbolos de Christoffel de Segunda Especie ($\Gamma^\lambda_{\mu\nu}$)

La conexión afín se define como:
$$\Gamma^\lambda_{\mu\nu} = \frac{1}{2} g^{\lambda\sigma} \left( \partial_\mu g_{\nu\sigma} + \partial_\nu g_{\mu\sigma} - \partial_\sigma g_{\mu\nu} \right)$$

Asumiendo velocidad constante de la burbuja ($\dot{v}_s = 0$) para aislar la geometría pura de viaje en régimen estacionario:

### 3.1 Componentes de $\Gamma^0_{\mu\nu}$:
- $\Gamma^0_{00} = -\frac{v_s^2 f}{c^2} \partial_x f$
- $\Gamma^0_{0x} = \Gamma^0_{x0} = -\frac{v_s}{2c^2} \partial_x (v_s f) \cdot \dots = 0$
- $\Gamma^0_{0y} = \Gamma^0_{y0} = -\frac{v_s}{2c^2} \partial_y f$
- $\Gamma^0_{0z} = \Gamma^0_{z0} = -\frac{v_s}{2c^2} \partial_z f$
- $\Gamma^0_{xx} = \Gamma^0_{yy} = \Gamma^0_{zz} = \Gamma^0_{jk} = 0$

### 3.2 Componentes de $\Gamma^x_{\mu\nu}$:
- $\Gamma^x_{00} = v_s^2 f \partial_x f \left(1 - \frac{v_s^2 f^2}{c^2}\right) \approx v_s^2 f \partial_x f$
- $\Gamma^x_{0y} = \Gamma^x_{y0} = \frac{1}{2} v_s \partial_y f \left(1 - \frac{v_s^2 f^2}{c^2}\right)$
- $\Gamma^x_{0z} = \Gamma^x_{z0} = \frac{1}{2} v_s \partial_z f \left(1 - \frac{v_s^2 f^2}{c^2}\right)$
- $\Gamma^x_{yy} = \Gamma^x_{zz} = 0$

### 3.3 Componentes de $\Gamma^y_{\mu\nu}$ y $\Gamma^z_{\mu\nu}$:
- $\Gamma^y_{00} = \frac{1}{2} v_s^2 \partial_y (f^2) = v_s^2 f \partial_y f$
- $\Gamma^y_{0x} = \Gamma^y_{x0} = -\frac{1}{2} v_s \partial_y f$
- $\Gamma^z_{00} = v_s^2 f \partial_z f$
- $\Gamma^z_{0x} = \Gamma^z_{x0} = -\frac{1}{2} v_s \partial_z f$
- El resto de términos espaciales puros son cero: $\Gamma^i_{jk} = 0$.

---

## 4. El Tensor de Einstein $G_{\mu\nu}$ y el Tensor de Energía-Impulso

Calculando el tensor de Ricci $R_{\mu\nu} = \partial_\rho \Gamma^\rho_{\mu\nu} - \partial_\nu \Gamma^\rho_{\mu\rho} + \Gamma^\rho_{\rho\lambda}\Gamma^\lambda_{\mu\nu} - \Gamma^\rho_{\nu\lambda}\Gamma^\lambda_{\mu\rho}$:

Para observadores eulerianos con vector normal $n^\mu$, el tensor de Einstein proyectado arroja el resultado clásico de Alcubierre:

$$G^{00} = -\frac{v_s^2}{4 c^2} \left( \frac{y^2 + z^2}{r_s^2} \right) \left( \frac{df}{dr_s} \right)^2$$

A través de la ecuación de Einstein $G^{\mu\nu} = \frac{8\pi G}{c^4} T^{\mu\nu}$, obtenemos las componentes principales del tensor de energía-impulso:

### 4.1 Componente de Densidad de Energía ($T^{00}$):
$$T^{00} = -\frac{c^2 v_s^2}{32 \pi G} \left( \frac{y^2 + z^2}{r_s^2} \right) \left( \frac{df}{dr_s} \right)^2$$

### 4.2 Componentes de Flujo de Momento ($T^{0x}, T^{0y}, T^{0z}$):
$$T^{0x} = v_s f T^{00}$$
$$T^{0y} = -\frac{c^4}{16 \pi G} \frac{v_s}{c^2} \left[ \partial_x \partial_y f \dots \right]$$

### 4.3 Presiones Principales ($T^{xx}, T^{yy}, T^{zz}$):
En la pared de la burbuja, además de la densidad de energía negativa $\rho < 0$, existe una enorme **tensión negativa (presión de compresión espacial hacia adentro en el frente y descompresión en la popa)** que equilibra la geometría:
$$p_x \approx T^{xx} \sim v_s^2 \left( \frac{df}{dr_s} \right)^2$$

---

## 5. Ecuaciones Geodésicas de la Simulación

Las partículas libres y la luz siguen geodésicas dictadas por:
$$\frac{d^2 x^\mu}{d\lambda^2} + \Gamma^\mu_{\alpha\beta} \frac{dx^\alpha}{d\lambda} \frac{dx^\beta}{d\lambda} = 0$$

Donde $\lambda$ es un parámetro afín (el tiempo propio $\tau$ para partículas con masa; $\lambda$ afín para fotones con $ds^2 = 0$).

### 5.1 Caso 1: La Nave en el Centro ($r_s = 0$)
En $r_s = 0$, $y=0$, $z=0$, y por simetría de la función de forma:
$$\left. \frac{df}{dr_s} \right|_{r_s = 0} = 0 \implies \Gamma^\mu_{\alpha\beta}(0) = 0$$

La ecuación geodésica se reduce a:
$$\frac{d^2 x^\mu}{d\tau^2} = 0$$
$$\frac{dx_s}{dt} = v_s(t) = \text{constante en cada instante}$$
La nave se encuentra en **movimiento inercial perfecto**, flotando libremente sin requerir propulsores convencionales una vez establecida la burbuja.

### 5.2 Caso 2: Rayos de Luz (Fotones y Horizonte Aparente)
Para rayos de luz incidentes que entran a la burbuja a lo largo del eje $x$ ($dy = dz = 0$):
$$ds^2 = 0 \implies -(c^2 - v_s^2 f^2) dt^2 - 2 v_s f dx dt + dx^2 = 0$$

Dividiendo entre $dt^2$, obtenemos una ecuación cuadrática para la velocidad de la luz coordenada $\frac{dx}{dt}$:
$$\left( \frac{dx}{dt} \right)^2 - 2 v_s f \left( \frac{dx}{dt} \right) - (c^2 - v_s^2 f^2) = 0$$

Resolviendo por la fórmula cuadrática:
$$\frac{dx}{dt} = \frac{2 v_s f \pm \sqrt{4 v_s^2 f^2 + 4(c^2 - v_s^2 f^2)}}{2} = v_s f(r_s) \pm c$$

> **Interpretación Física Fundamental para la Simulación:**
> - En el interior de la burbuja ($f = 1$): Los fotones se mueven hacia adelante a velocidad coordenada $v_s + c$, y hacia atrás a $v_s - c$.
> - Si la nave viaja a velocidad superlumínica ($v_s > c$), para fotones que viajan hacia atrás: $\frac{dx}{dt} = v_s - c > 0$. **¡Incluso la luz emitida hacia atrás es arrastrada hacia adelante!**
> - En la pared de la burbuja donde $v_s f(r_s) = c$, surge un **horizonte aparente de eventos (horizonte de Cauchy)** similar al de un agujero negro / agujero blanco.
