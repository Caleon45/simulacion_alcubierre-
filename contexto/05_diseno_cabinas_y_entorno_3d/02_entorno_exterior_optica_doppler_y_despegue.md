# 02. Entorno Exterior, Óptica Relativista (Doppler) y Despegue Atmosférico

Este documento describe cómo se percibe visualmente el universo exterior desde el parabrisas de la cabina en primera persona (**POV**), detallando el efecto Doppler estelar, la aberración de la luz, el halo de curvatura Warp y la secuencia suave de despegue desde la Tierra.

---

## 1. El Entorno Estelar Exterior

Para no saturar el rendimiento del motor gráfico con geometrías lejanas invisibles, el espacio exterior se genera de forma procedural y eficiente:
* **Fondo Estelar Dinámico:** Un campo de estrellas infinito visible a través de los cristales de la cabina.
* **Profundidad y Capas:** Estrellas lejanas estáticas combinadas con partículas de polvo cósmico brillante que pasan velozmente al acelerar, otorgando una sensación intuitiva de velocidad y dirección.

---

## 2. Óptica Relativista del Observador: Efecto Doppler y Aberración

Cuando la nave acelera hacia fracciones apreciables de la luz ($0.10c \to 0.50c$), las ecuaciones de la Relatividad Especial transforman drásticamente lo que el ojo humano o los sensores de cabina observan a través del parabrisas:

```
                          EVOLUCIÓN VISUAL EN EL PARABRISAS
                          
       [ VELOCIDAD CRUCERO 0.001c ]           [ VELOCIDAD RELATIVISTA 0.50c ]
    ┌─────────────────────────────────┐    ┌─────────────────────────────────┐
    │     *             *        ·    │    │         \    |    /           │
    │               ·                 │    │     - - - ( O ) - - -       │
    │  ·       *             *        │    │         /    |    \           │
    └─────────────────────────────────┘    └─────────────────────────────────┘
    Cielo normal con estrellas             Efecto "Túnel Estirado" (Aberración)
    blancas y distribución uniforme.       + Estrellas azul-violeta (Blueshift)
                                           + Bordes rojizos oscuros (Redshift)
```

### 2.1 Aberración Relativista (Efecto Searchlight / Túnel Estirado)
* **Principio físico:** A velocidades relativistas, el cono de luz frontal se estrecha hacia la dirección del vector de avance:
  $$\cos \theta_{\text{obs}} = \frac{\cos \theta + \beta}{1 + \beta \cos \theta}$$
* **Efecto visual en cabina:** Las estrellas que en reposo se encontraban a los lados o en la periferia parecen "doblarse" y concentrarse en el centro del parabrisas.
* **Percepción de la tripulación:** Se crea una sensación inmersiva de **"túnel estirado"**, donde el cosmos parece converger en un punto focal frente a la proa de la nave.

### 2.2 Desplazamiento Doppler Estelar (Blueshift y Redshift)
* **Corrimiento al Azul (Blueshift Frontal):**  
  Las estrellas situadas frente a la nave emiten fotones que la nave intercepta con mayor frecuencia. Su espectro visible vira hacia el **azul eléctrico, cian y violeta brillante**, con picos de radiación de alta energía.
* **Corrimiento al Rojo (Redshift Periférico):**  
  Las estrellas situadas hacia los bordes y hacia atrás experimentan estiramiento de longitud de onda, virando hacia el **rojo anaranjado** y desvaneciéndose en la oscuridad al caer en el espectro infrarrojo.

### 2.3 Visualización del Régimen Alcubierre Warp
* En modo Warp no rige el Doppler clásico porque la nave no se desplaza a través del espacio, sino que el espacio se desplaza alrededor de ella.
* **Efecto visual exterior:**
  * Al activarse la burbuja, aparece un **halo luminoso toroidal** (anillo de luz refractada) rodeando el contorno del parabrisas, producto del lente gravitacional de la densidad de energía exótica negativa ($\rho < 0$).
  * El destino astronómico frontal parece "acercarse angularmente" de inmediato por efecto de la compresión del espacio ($\theta < 0$).

---

## 3. Secuencia de Despegue Atmosférico Simple

Para emular la salida de la Tierra sin saturar la simulación con cálculos aerodinámicos pesados, el despegue se representa mediante una **transición cromática y ambiental limpia**:

```
      ALTITUD                          COLOR DEL CIELO EXTERIOR                  SENSACIÓN EN CABINA
  ───────────────────────────────────────────────────────────────────────────────────────────────────
  0 a 10 km (Baja atmósfera)           Azul celeste brillante (Luz diurna)       Vibración suave de empuje
  10 a 50 km (Estratosfera)            Azul marino profundo                      Vibración decreciente
  50 a 100 km (Línea de Kármán)        Azul índigo / Violeta oscuro              Aparecen las primeras estrellas
  > 100 km (Espacio Orbital)           Negro espacial profundo absoluto          Silencio inercial / Microgravedad
```

### Pasos de la Secuencia:
1. **Inicio en Superficie Terrestre:**
   * A través de las ventanas de la cabina se aprecia la atmósfera terrestre iluminada en azul celeste diurno (efecto de dispersión de Rayleigh por el nitrógeno y oxígeno).
2. **Ascenso Gradual (15 a 20 segundos):**
   * Al iniciar la ignición, una suave sacudida mecánica transmite la potencia de despegue.
   * Conforme la altitud aumenta, el color de la atmósfera exterior se oscurece fluidamente: de azul celeste a azul cobalto, violeta intenso y finalmente negro espacial.
3. **Entrada al Vacío y Salida Orbital:**
   * Las estrellas y nebulosas del cosmos emergen nítidas una vez que la atmósfera ha desaparecido por completo.
   * La vibración de la nave cesa por completo, entrando en régimen de caída libre orbital y activando el monitor de navegación para elegir el trayecto final.

---

## 4. Facsímiles Planetarios por Proximidad

Para mantener una eficiencia extrema en el rendimiento:
* Durante el 90% del viaje, los planetas no se renderizan como mallas 3D complejas en el vacío exterior.
* **Aparición en Proximidad:** Al acercarse al destino (fase de frenado o llegada final), se proyecta en el centro del parabrisas una **esfera 3D limpia y estilizada** con la textura del planeta correspondiente (Tierra, Luna, Marte, Júpiter, etc.).
* La esfera crece suavemente a la vista del piloto conforme la distancia física en kilómetros desciende a cero, culminando con la confirmación de órbita o llegada exitosa en el ordenador de navegación.

