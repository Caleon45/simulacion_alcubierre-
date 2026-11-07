# 02. Estados Límite, Tolerancias y Fallos Críticos

Este documento modela la respuesta dinámica de la nave ante condiciones extremas de vuelo, estableciendo un sistema de advertencias graduales con tolerancias físicas realistas pero permisivas.

---

## 1. Filosofía de Tolerancias y Desvarianzas

En la ingeniería real existen factores de seguridad ($SF \approx 1.5 - 2.0$). En nuestra simulación:
* **No hay fallo instantáneo al superar un umbral:** Superar un parámetro no destruye la nave de inmediato.
* **Margen de desvarianza ($\pm 20\%$):** El sistema permite al piloto maniobrar, reducir potencia o activar contramedidas antes de llegar al punto de no retorno.
* **Feedback multisensorial dentro de la cabina:** El piloto se entera del peligro a través de instrumentos visuales, alarmas luminosas en el salpicadero y vibración de cabina.

---

## 2. Variables de Estado Monitoreadas en Tiempo Real

Durante cualquier vuelo se evalúan cuatro variables de supervivencia en la consola:

```
  ┌────────────────────────────────────────────────────────────────────────┐
  │                   TELEMETRÍA DE ESTADO DEL VEHÍCULO                   │
  │                                                                        │
  │  [ INTEGRIDAD DEL CASCO ]    [ RADIACIÓN EN CABINA ]  [ ESTRÉS TÉRMICO ]
  │  ████████████████░░░░ 82%    ░░░░░░░░░░░░░░░░ 0.2 mSv ▓▓▓▓▓▓░░░░░░ 450°C
  │  Nominal                     Segura                   Estable          │
  └────────────────────────────────────────────────────────────────────────┘
```

1. **Integridad Estructural del Casco ($0\% - 100\%$):**
   * Disminuye si la nave viaja a velocidades relativistas sin escudo deflector de impacto interestelar (choque con micropartículas y átomos de hidrógeno del medio interestelar).
2. **Nivel de Radiación Interna ($0 - 10\text{ Sieverts}$):**
   * Aumenta si el reactor de antimateria opera sin suficiente blindaje de tungsteno. El límite de seguridad de la tripulación es de $0.5\text{ Sv}$ por misión.
3. **Estrés Térmico del Reactor / Tobera ($0\% - 100\%$):**
   * Aumenta durante quemados continuos de propulsión química a máxima aceleración o por calentamiento del confinamiento magnético de piones.
4. **Estabilidad de la Burbuja Warp ($0\% - 100\%$):**
   * Evalúa la coherencia cuántica de la densidad de energía exótica negativa. Si las fluctuaciones del vacío rompen el $15\%$, la burbuja colapsa.

---

## 3. Escala de Estados de la Nave

La cabina responde dinámicamente a través de cuatro niveles de estado:

| Nivel de Estado | Rango de Salud | Comportamiento Visual y Auditivo en Cabina | Consecuencia en el Vuelo |
| :--- | :--- | :--- | :--- |
| **🟢 1. Nominal** | $100\% - 65\%$ | Iluminación de cabina blanca/azul suave. Instrumentos en verde. Vuelo sereno. | Rendimiento óptimo al $100\%$. |
| **🟡 2. Advertencia** | $65\% - 30\%$ | Luces de salpicadero parpadean en amarillo ámbar. Pitido intermitente de alerta. Mensaje en pantalla: *"Aviso: Incremento de fricción/temperatura"*. | El sistema avisa al piloto. Reducir la velocidad o estabilizar estabilizadores mitiga el daño. |
| **🔴 3. Crítico** | $30\% - 1\%$ | Iluminación de emergencia roja estroboscópica. Sirena de alarma continua. Micro-vibraciones en la cámara de cabina. **Aparecen micro-fisuras visuales en el cristal del parabrisas**. | La integridad se deteriora rápidamente si no se corta la propulsión. |
| **💥 4. Estado Límite / Fallo** | $0\%$ | Ruptura estructural. Fogonazo blanco en el parabrisas y despresurización o desintegración de la nave. Pantalla final de telemetría de misión fallida. | Vuelo abortado. Opción de reiniciar o regresar al hangar. |

---

## 4. Escenarios Específicos de Fallo Relativista y Cuántico

### A. Fallo por Fricción Interestelar Extrema ($v \to c$)
* **Causa física:** La nave alcanza $0.50c$ o más con un casco estándar de Tier 1 sin escudo deflector frontal de Berilio.
* **Fenómeno:** El gas interestelar choca como un chorro de plasma abrasivo.
* **Secuencia en cabina:**
  1. A $0.10c$: Las partículas estelares impactan el cristal generando chispas luminosas.
  2. A $0.30c$: El indicador de temperatura se dispara a la zona roja. Alarma acústica: *"Casco frontal superando límite de ablación"*.
  3. A $0.50c$: Si el usuario no desacelera, el parabrisas cede con una fractura visual y se produce la explosión cinemática.

### B. Fallo por Radiación de Aniquilación
* **Causa física:** Selección de propulsión por antimateria sin blindaje de tungsteno.
* **Secuencia en cabina:** Los instrumentos digitales sufren interferencias electromagnéticas (glitches visuales momentáneos) y el contador Geiger de cabina se satura, provocando parada de emergencia del reactor.

### C. Fallo por Inestabilidad de Curvatura (Alcubierre Warp)
* **Causa física:** Pérdida de confinamiento de energía negativa $\rho < 0$.
* **Secuencia en cabina:** El halo toroidal del parabrisas oscila violentamente de azul a blanco cegador y la nave es expulsada violentamente de la burbuja, retornando instantáneamente a velocidad cero en el espacio normal sin sufrir daños inerciales corporales (fiel a la física de caída libre $a^\mu = 0$).

