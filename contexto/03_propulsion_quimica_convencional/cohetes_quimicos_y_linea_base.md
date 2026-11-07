# 03. Propulsión Química Convencional y Línea Base

Este documento establece la línea base de la tecnología aeroespacial actual para contrastar sus límites termodinámicos infranqueables con la propulsión por antimateria y el motor warp de Alcubierre.

---

## 1. Principio Termodinámico de los Cohetes Químicos

Todos los cohetes espaciales operativos de la humanidad (Saturn V, Space Shuttle, Falcon 9, Starship, SLS) se basan en la combustión química exotérmica: una reacción redox entre un combustible y un oxidante rompe y reorganiza enlaces electrónicos moleculares de valencia.

La energía liberada por enlace es de unos pocos **electronvoltios ($\sim 1 - 5 \text{ eV}$ por molécula)**. Esta energía se transforma en calor, elevando la temperatura de los gases de escape en la cámara de combustión, los cuales se expanden a través de una tobera convergente-divergente de Laval para generar empuje.

### 1.1 Las Tres Mezclas Químicas Más Eficientes de la Historia
1. **Hidrógeno Líquido y Oxígeno Líquido (Hydrolox - $\text{H}_2 + \text{O}_2$):**
   - Utilizado en el motor RS-25 del Transbordador Espacial y SLS.
   - Es la reacción química de mayor velocidad de escape práctica lograda por la humanidad:
     $$v_e \approx 4.430 \text{ m/s} \quad (4.43 \text{ km/s}), \quad I_{sp} \approx 452 \text{ segundos}$$
2. **Metano Líquido y Oxígeno Líquido (Methalox - $\text{CH}_4 + \text{O}_2$):**
   - Utilizado en los motores Raptor de SpaceX (Starship).
   - $v_e \approx 3.730 \text{ m/s} \quad (3.73 \text{ km/s}), \quad I_{sp} \approx 380 \text{ segundos}$ en el vacío.
3. **Kerosene Refinado y Oxígeno Líquido (Kerolox - $\text{RP-1} + \text{O}_2$):**
   - Utilizado en el cohete lunar Saturn V (F-1) y Falcon 9 (Merlin 1D).
   - $v_e \approx 3.050 \text{ m/s} \quad (3.05 \text{ km/s}), \quad I_{sp} \approx 311 \text{ segundos}$ a nivel del mar.

---

## 2. La Ecuación Clásica de Tsiolkovsky (1903)

Formulada por el pionero ruso Konstantín Tsiolkovsky, dicta el cambio de velocidad ideal $\Delta v$ alcanzable por un cohete que expulsa masa a velocidad constante $v_e$ en ausencia de gravedad y fricción atmosférica:

$$\Delta v = v_e \ln \left( \frac{m_0}{m_f} \right) = I_{sp} g_0 \ln \left( \frac{m_0}{m_f} \right)$$

Despejando la **relación de masa requerida** ($R_m = m_0 / m_f$):

$$\frac{m_0}{m_f} = e^{\frac{\Delta v}{v_e}} = e^{\frac{\Delta v}{I_{sp} g_0}}$$

> **La Tiranía de la Ecuación del Cohete:**
> La masa de combustible requerida no crece de manera lineal con la velocidad deseada, sino de forma **exponencial**. Para aumentar la velocidad linealmente, la masa de combustible debe multiplicarse exponencialmente.

---

## 3. El Techo Termodinámico Infranqueable de la Química

La velocidad máxima teórica de escape de cualquier gas ideal en expansión adiabática completa viene limitada por la entalpía química específica de combustión $\Delta H / M$:

$$v_{e,\max} = \sqrt{2 \frac{\Delta H}{M}}$$

Para la reacción más energética que existe en la tabla periódica ($\text{H}_2 + \text{F}_2 \to 2\text{HF}$, flúor con hidrógeno):
$$v_{e,\max} \approx 5.320 \text{ m/s} \approx 0.0000177 c$$

**Ninguna reacción química en el universo puede superar este límite físico**, porque la energía contenida en los enlaces electrónicos de los átomos es millones de veces menor que la contenida en el núcleo atómico o en la aniquilación de la masa.

---

## 4. ¿Por qué los Cohetes Químicos son Inviables para Viajes Interestelares?

### 4.1 Cálculo para Alcanzar el 1% de la Velocidad de la Luz ($0.01c = 3.000 \text{ km/s}$)
Con el motor químico de hidrógeno líquido más avanzado de la NASA ($v_e = 4.5 \text{ km/s}$):

$$\frac{m_0}{m_f} = e^{\frac{3.000.000}{4.500}} = e^{666.67} \approx 3.7 \times 10^{289}$$

- Si la masa de la nave seca fuese de tan solo **$1 \text{ kg}$**, la masa de combustible de hidrógeno y oxígeno necesaria sería de:
  $$m_0 \approx 10^{289} \text{ kilogramos}$$
- La masa total de **todas las estrellas y galaxias de todo el universo observable** es de aproximadamente $10^{53} \text{ kg}$.
- El cohete químico requeriría $10^{236}$ veces más combustible que la masa del universo entero para acelerar 1 kilo al 1% de la luz.

### 4.2 Tiempo de Viaje a la Estrella Más Cercana (Próxima Centauri - $4.24 \text{ años luz}$)
Las sondas espaciales químicas más veloces lanzadas por la humanidad (Voyager 1 y New Horizons) viajan a una velocidad de crucero heliocéntrica de aproximadamente:
$$v_{\text{Voyager}} \approx 17.0 \text{ km/s} \approx 0.0000567 c$$

El tiempo requerido para cubrir los $4.24 \text{ años luz}$ ($4.01 \times 10^{13} \text{ km}$) es:

$$t = \frac{4.01 \times 10^{13} \text{ km}}{17.0 \text{ km/s}} \approx 2.36 \times 10^9 \text{ segundos} \approx \mathbf{74.800 \text{ años}}$$

Un viaje interestelar químico tardaría más de **2.500 generaciones humanas**, demostrando por qué la propulsión avanzada (antimateria) y la propulsión no inercial (curvatura de Alcubierre) son indispensables para la exploración estelar.
