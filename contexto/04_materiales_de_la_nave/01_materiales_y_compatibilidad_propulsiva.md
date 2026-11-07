# 01. Materiales de la Nave y Compatibilidad Propulsiva

Este documento establece la selección de materiales estructurales para la simulación, equilibrando el rigor físico con la jugabilidad y la claridad didáctica.

---

## 1. Filosofía de Simulación: "Similitud y Comprensión"

La simulación no busca ser un software de ingeniería de elementos finitos milimétrico, sino un **entorno simulado accesible**:
* Las leyes físicas se respetan conceptualmente (fricción relativista, radiación de aniquilación, estabilidad de curvatura).
* Existe una **tolerancia o margen de desvarianza** (la nave no explota al mínimo error, sino que cuenta con márgenes de seguridad razonables).
* El usuario debe poder entender inmediatamente qué material necesita cada nave y por qué.

---

## 2. Los Tres Tiers de Materiales

Para no saturar con docenas de aleaciones complejas, el sistema se sintetiza en **tres categorías de materiales claras**:

```
                       SELECCIÓN DE MATERIALES DEL CASCO
                       
   [ TIER 1: AEROESPACIAL ]       [ TIER 2: BLINDADO Y HTS ]       [ TIER 3: EXÓTICO / METAMATERIAL ]
   Aleación Aluminio-Titanio      Tungsteno + Polímeros + HTS      Grafeno Cuántico + Metamaterial
   • Ideal para: Química          • Ideal para: Antimateria        • Ideal para: Alcubierre Warp
   • Límite: < 0.001c             • Límite: hasta 0.50c            • Límite: Superlumínico aparente
   • Cero radiación               • Bloquea rayos gamma y gas ISM  • Soporta gradiente de curvatura
```

---

### Tier 1: Aleación Aeroespacial Estándar (Aluminio-Titanio / CFRP)
* **Composición conceptual:** Aleación ligera de Aluminio-Litio reforzada con largueros de Titanio y paneles de fibra de carbono.
* **Propulsión nativa:** **🚀 Propulsión Química Convencional**.
* **Propiedades físicas en la simulación:**
  * **Masa estructural:** Liviana ($1.000\text{ kg}$ base).
  * **Resistencia térmica:** Hasta $3.500\text{ K}$ con losetas cerámicas estándar.
  * **Velocidad máxima segura:** Hasta $300\text{ km/s}$ ($0.001c$).
  * **Capacidad de blindaje:** Muy baja contra radiación nuclear o impacto atómico interestelar.
* **Uso en juego:** Es la opción económica y clásica. Excelente para salir de la Tierra y viajar por el sistema solar interior a velocidades newtonianas.

---

### Tier 2: Blindaje de Densidad y Superconductores (Tungsteno-Berilio / HTS)
* **Composición conceptual:** Casco compuesto con escudo frontal de Berilio/Grafeno (escudo de ablación contra el gas interestelar) y un mamparo interior de Tungsteno y polietileno borado (*Shadow Shield*), equipado con bobinas superconductoras de alta temperatura (HTS).
* **Propulsión nativa:** **⚛️ Propulsión por Antimateria (Beam-Core)**.
* **Propiedades físicas en la simulación:**
  * **Masa estructural:** Media-Alta ($2.500\text{ kg}$ base por el peso del tungsteno).
  * **Blindaje de radiación:** Absorbe hasta el $99.9\%$ de los rayos gamma de $187\text{ MeV}$ producidos por la cascada de piones $\pi^0$.
  * **Resistencia a colisión interestelar:** El escudo frontal disipa el impacto continuo de partículas de gas interestelar hasta $0.50c$ ($150.000\text{ km/s}$).
  * **Velocidad máxima segura:** Hasta $0.50c$.
* **Uso en juego:** Es el chasis blindado obligatorio para misiones relativistas.

---

### Tier 3: Metamateriales Resonantes y Grafeno Cuántico
* **Composición conceptual:** Estructura en panal de abeja de Nanotubos de Carbono (CNT) ultraligeros, combinada con cavidades microresonadoras Casimir y metamateriales hiperbólicos con permitividad/permeabilidad negativa efectiva.
* **Propulsión nativa:** **🌌 Curvatura Espacio-Temporal (Alcubierre Warp)**.
* **Propiedades físicas en la simulación:**
  * **Masa estructural:** Ultraligera ($800\text{ kg}$ base).
  * **Resistencia a deformación de métrica:** Canaliza los gradientes de curvatura extrínseca ($K_{ij}$) y distribuye la densidad de energía exótica ($\rho < 0$) alrededor de la nave sin sufrir tensiones inerciales internas ($0.00g$).
  * **Deflexión electromagnética de horizonte:** Desvía el gas interestelar acumulado en el frente de la burbuja.
  * **Velocidad máxima segura:** Superlumínica aparente ilimitada ($10c - 100c$).
* **Uso en juego:** La cúspide de la tecnología teórica. Permite cruzar el sistema solar en minutos y llegar a Próxima Centauri en meses.

---

## 3. Matriz de Compatibilidad y Comportamiento Cruzado

¿Qué ocurre si el jugador combina un motor con un material no diseñado para él?

| Material Seleccionado | Con Motor Químico | Con Motor Antimateria | Con Motor Alcubierre Warp |
| :--- | :--- | :--- | :--- |
| **Tier 1 (Aeroespacial Estándar)** | **Óptimo (100% Eficiencia).** Vuelo nominal perfecto. | **Fallo Crítico:** Fuga de radiación gamma por falta de blindaje de tungsteno. Daño a la tripulación. | **Fallo de Activación:** El material no canaliza energía exótica; la burbuja colapsa. |
| **Tier 2 (Blindaje Tungsteno / HTS)** | **Vuelo posible pero ineficiente:** La nave es demasiado pesada; gasta el doble de combustible químico. | **Óptimo (100% Eficiencia).** Protección completa a $0.20c - 0.50c$. | **Incompatible:** Exceso de masa inercial bloquea la geometría de la burbuja. |
| **Tier 3 (Metamaterial Cuántico)** | **Funcional pero innecesario:** Resiste el vuelo pero encarece los recursos. | **Riesgo:** Si no tiene escudo frontal de Berilio, la fricción con gas interestelar desgasta el casco a alta velocidad. | **Óptimo (100% Eficiencia).** Estabilización perfecta de la burbuja sin fuerzas G. |

