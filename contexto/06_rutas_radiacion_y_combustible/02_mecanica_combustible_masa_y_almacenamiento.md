# 02. Mecánica del Combustible, Balances de Masa y Almacenamiento

Este documento formula matemáticamente la relación entre la masa seca de la nave, el blindaje, la carga de combustible óptima, las consecuencias del déficit o exceso de propelente, y la física del almacenamiento criogénico y magnético de antimateria.

---

## 1. La Tiranía de la Ecuación del Cohete: Balances de Masa

La masa total de despegue $m_0$ se desglosa rigurosamente en tres componentes:

$$m_0 = m_{\text{seco}} + m_{\text{blindaje}} + m_{\text{propelente}}$$

Donde:
* **$m_{\text{seco}}$:** Masa base de la estructura y cabina ($1.000\text{ kg}$).
* **$m_{\text{blindaje}}$:** Masa del escudo contra radiación y partículas:
  $$m_{\text{blindaje}} = \text{Área Superficie} \times \text{Espesor } x \times \text{Densidad } \rho$$
* **$m_{\text{propelente}}$:** Masa de combustible embarcado (Químico o Antimateria/Materia).

---

### 1.1 Penalización por Exceso de Combustible
Un error común en astronáutica es asumir que *"cuanto más combustible llevemos, más seguros estaremos"*. 

La ecuación de Tsiolkovsky demuestra que añadir combustible extra penaliza el rendimiento:

$$\Delta v = v_e \ln \left( \frac{m_0}{m_f} \right) = v_e \ln \left( 1 + \frac{m_{\text{propelente}}}{m_{\text{seco}} + m_{\text{blindaje}}} \right)$$

1. **Aceleración Inicial Reducida:**
   $$a_0 = \frac{F_{\text{empuje}}}{m_0}$$
   A mayor combustible, menor aceleración inicial. La nave tarda más tiempo en escapar de la gravedad terrestre o en alcanzar velocidad de crucero.
2. **Estrés Térmico Prolongado:**
   Para obtener el mismo cambio de velocidad, la tobera debe permanecer encendida durante un tiempo mucho mayor ($t_{\text{quemado}} = m_{\text{propelente}} / \dot{m}$), elevando el calentamiento acumulado del reactor.
3. **Consumo Exponencial de Retorno Decreciente:**
   Para transportar $1\text{ kg}$ adicional de combustible hasta Marte, se necesitan casi $3\text{ kg}$ de combustible adicional en la Tierra solo para acelerar ese primer kilo.

---

### 1.2 Penalización por Déficit de Combustible
* Si $m_{\text{propelente}} < m_{\text{mínimo}}$: La nave agota su reserva antes de completar la desaceleración en destino.
* **Consecuencia:** La nave pasa de largo del planeta destino en trayectoria hiperbólica de escape, quedando a la deriva en órbita heliocéntrica sin capacidad de frenado.

---

## 2. Almacenamiento de Antimateria: ¿Emite Radiación en Reposo?

**Respuesta física:** **SÍ**. Aunque la antimateria no es radiactiva por sí misma, su almacenamiento físico genera radiación continua debido al gas residual en las trampas de confinamiento magnético.

```
                  TRAMPA DE PENNING-MALMBERG CRIOGÉNICA
                  
    Bobinas Superconductoras              Átomos de Gas Residual
        [ Campo B: 50 T ]                        (n_gas)
    ┌──────────────────────────────────────────────────────────────┐
    │  S S S S S S S S S S S S S S S S S S S S S S S S S S S S S   │
    │                                                              │
    │         p̄     p̄     p̄  ──(Choque)──> Gas Residual        │
    │            (Nube de Antiprotones)              │             │
    │         p̄     p̄     p̄                          ▼             │
    │                                         2 Rayos Gamma        │
    │  N N N N N N N N N N N N N N N N N N    (187 MeV c/u)        │
    └──────────────────────────────────────────────────────────────┘
```

### 2.1 Aniquilación Parásita por Gas Residual
En el mejor vacío de laboratorio alcanzable a temperatura criogénica ($4\text{ K}$), la densidad del gas residual de fondo es $n_{\text{gas}} \approx 10^8 - 10^9\text{ partículas/m}^3$ (presión $P \sim 10^{-14}\text{ Torr}$).

Los antiprotones confinados chocan inevitablemente con los átomos de gas neutro residual con una sección eficaz de aniquilación $\sigma_{\text{ann}}$:

$$\dot{N}_{\text{leak}} = N_{\bar{p}} \cdot n_{\text{gas}} \cdot \sigma_{\text{ann}} \cdot v_{\text{rel}} \quad [\text{aniquilaciones/segundo}]$$

### 2.2 Potencia de Radiación y Calor Parásito ($P_{\text{leak}}$)
Cada aniquilación libera la energía de masa en reposo de un protón y un antiprotón ($2 m_p c^2 \approx 1.876\text{ GeV}$):

$$P_{\text{leak}} = \dot{N}_{\text{leak}} \times (2 m_p c^2) \quad [\text{Watts}]$$

> **Consecuencia Crucial para la Simulación:**
> * Si el jugador almacena una **cantidad excesiva de antimateria** (ej. $500\text{ kg}$ para un viaje a la Luna que solo requería gramos), el número de antiprotones $N_{\bar{p}}$ es tan gigantesco que la potencia de radiación de fuga $P_{\text{leak}}$ satura los sistemas de enfriamiento criogénico.
> * Esto eleva la temperatura del tanque, degrada la superconductividad de los imanes y genera una **fuga crónica de rayos gamma hacia la cabina**.

---

## 3. Pérdida por Ebullición Criogénica en Química (*Boil-off*)

El Hidrógeno Líquido ($LH_2$) se almacena a su punto de ebullición de **$20.28\text{ K}$ ($-252.87\text{ °C}$)**.
En misiones interplanetarias que duran meses (Tierra a Marte: $6 - 9\text{ meses}$):
* La radiación solar y la conducción térmica a través de los soportes transfieren calor continuo al tanque.
* A pesar del aislamiento multicapa (MLI), una fracción del hidrógeno se evapora y debe ser venteada para no reventar los tanques por sobrepresión:

$$m_{\text{fuel}}(t) = m_{\text{fuel}, 0} \cdot e^{-\kappa \cdot t}$$

Donde $\kappa$ es la tasa de ebullición diaria ($\kappa \approx 0.05\% - 0.15\%\text{ por día}$ en el espacio profundo).

---

## 4. Sistema Pedagógico de Asistencia al Jugador

Para permitir que el usuario experimente sin frustración, el ordenador de a bordo del astillero ofrece **tres datos de referencia**:

```
  ┌────────────────────────────────────────────────────────────────────────┐
  │                   ASISTENTE DE CONFIGURACIÓN DE MASA                   │
  │                                                                        │
  │  • Masa Mínima Teórica (Límite Físico):      142.5 kg                  │
  │  • Margen de Seguridad Recomendado (+15%):   164.0 kg                  │
  │  • Pérdida Estimada por Boil-off / Fuga:       4.2 kg                  │
  │                                                                        │
  │  [ CARGA SELECCIONADA POR EL PILOTO:  165.0 kg ]                       │
  │  Estado: ÓPTIMO (Margen suficiente sin sobrecargar inercia)            │
  └────────────────────────────────────────────────────────────────────────┘
```

1. **Masa Mínima Teórica ($m_{\text{mín}}$):** Calculada por Tsiolkovsky o la ecuación relativista para la distancia exacta seleccionada.
2. **Margen de Seguridad Óptimo ($+10\% \text{ a } +20\%$):** Protege contra desviaciones de trayectoria o correcciones orbitales intermedias.
3. **Zona de Sobrecarga ($> +50\%$):** El indicador alerta visualmente en amarillo: *"Advertencia: Exceso de peso reducirá aceleración y aumentará estrés radiológico"*.

