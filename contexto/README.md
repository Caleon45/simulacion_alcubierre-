# COMPENDIO DE INVESTIGACIÓN: SISTEMAS DE PROPULSIÓN ESPACIAL AVANZADA

> **Carpeta de Contextualización Teórica, Matemática y Comparativa para la Simulación**  
> **Proyecto:** Alcubierre & Advanced Space Propulsion Simulation  
> **Asignatura:** Entornos y Simulación  

---

## 🌌 Visión General

Este compendio de investigación estructura y formaliza el marco teórico, matemático y computacional para simular tres paradigmas de propulsión espacial capaces de desplazar una nave hacia puntos lejanos del cosmos:

1. **Propulsión por Curvatura Espacio-Temporal (Warp Drive de Alcubierre):** Modificación no inercial de la métrica del espacio-tiempo mediante contracción y expansión espacial.
2. **Propulsión por Aniquilación de Antimateria (Régimen Relativista):** Conversión 100% eficiente de masa en reposo a energía cinética de piones relativistas mediante tobera magnética.
3. **Propulsión Química Convencional (Línea Base):** Motores cohete termodinámicos basados en combustión molecular redox.

---

## 📁 Mapa de Navegación de la Carpeta `contexto/`

```
contexto/
│
├── propuestas_de_propulsion.md                  <-- [DOCUMENTO COMPARATIVO PRINCIPAL]
│   ├── Matriz multidimensional: Química vs Antimateria vs Warp
│   ├── Escenarios de misión calculados: Luna, Marte, Oort y Próxima Centauri
│   └── Arquitectura unificada para la simulación multimodo en Unity
│
├── 01_curvatura_espaciotiempo/                  <-- [RAMA 1: WARP DRIVE DE ALCUBIERRE]
│   ├── 01_relatividad_general/
│   │   └── relatividad_y_espaciotiempo.md       (Einstein, tensor métrico y formalismo 3+1 ADM)
│   ├── 02_metrica_alcubierre/
│   │   └── metrica_de_alcubierre_1994.md        (Elemento de línea, función f(rs), contracción y expansión)
│   ├── 03_energia_exotica/
│   │   └── energia_exotica_y_condiciones_de_energia.md (Deducción de T⁰⁰, ρ ≤ 0 y Efecto Casimir)
│   ├── 04_ecuaciones_matematicas/
│   │   └── derivaciones_y_calculo_tensorial.md  (Christoffel, Riemann, Ricci, Einstein y geodésicas)
│   ├── 05_nave_y_propulsion/
│   │   └── fisica_de_la_nave_y_geometria.md     (0g inercial, ausencia de dilatación y anillo toroidal)
│   ├── 06_avances_y_otros_cientificos/
│   │   └── cientificos_y_evolucion_teorica.md   (Van Den Broeck, White, Bobrick & Martire, Lentz)
│   └── 07_bases_para_simulacion/
│       └── modelo_computacional_y_discretizacion.md (Algoritmos discretos para Unity y deformación de malla)
│
├── 02_propulsion_antimateria/                   <-- [RAMA 2: ANIQULACIÓN DE ANTIMATERIA]
│   ├── 01_fisica_aniquilacion/
│   │   └── aniquilacion_materia_antimateria.md  (Física subatómica p-p̄, cascada de piones y CERN)
│   ├── 02_arquitecturas_propulsion/
│   │   └── motores_de_antimateria.md            (Beam-Core, Térmico de H₂ y Fusión catalizada ACMF/ICAN-II)
│   ├── 03_ecuaciones_relativistas/
│   │   └── dinamica_y_calculo_relativista.md    (Ecuación cohete relativista, razón de masa y dilatación)
│   └── 04_bases_simulacion_antimateria/
│       └── modelo_computacional_antimateria.md  (Ecuaciones diferenciales en Unity, Doppler y aberración)
│
├── 03_propulsion_quimica_convencional/          <-- [RAMA 3: LÍNEA BASE Y TERMODINÁMICA]
│   └── cohetes_quimicos_y_linea_base.md         (Ecuación clásica Tsiolkovsky, Hydrolox y límites físicos)
│
├── 04_materiales_de_la_nave/                    <-- [RAMA 4: CIENCIA DE MATERIALES Y LÍMITES FÍSICOS]
│   ├── 01_materiales_y_compatibilidad_propulsiva.md (Tiers 1, 2 y 3, compatibilidad por motor y selección)
│   └── 02_estados_limite_y_fallos_criticos.md       (Tolerancias, desvarianza, niveles de alerta y fallos)
│
├── 05_diseno_cabinas_y_entorno_3d/              <-- [RAMA 5: INTERACCIÓN POV Y ENTORNO VISUAL]
│   ├── 01_conceptos_tres_cabinas.md                 (Tres cabinas únicas: Química, Antimateria y Warp)
│   └── 02_entorno_exterior_optica_doppler_y_despegue.md (Doppler estelar, aberración, halo warp y despegue)
│
├── 06_rutas_radiacion_y_combustible/            <-- [RAMA 6: RUTAS, RADIACIÓN Y FÍSICA DE CARGA]
│   ├── 01_modelo_matematico_radiacion_y_rutas.md    (Flujo solar 1/r², Van Allen, Júpiter, dosis y Beer-Lambert)
│   ├── 02_mecanica_combustible_masa_y_almacenamiento.md (Tiranía del cohete, boil-off y fuga de antimateria)
│   └── 03_modelo_computacional_y_algoritmos_vuelo.md (Bucle numérico, pseudocódigo y diales con inercia)
│
└── 07_auditoria_matematica_y_comprobaciones/    <-- [RAMA 7: AUDITORÍA INTERNA Y DOMINIOS NUMÉRICOS]
    └── auditoria_ecuaciones_y_simulacion_numerica.md (Guardas de no-negatividad, comprobación en 5 misiones)
```

---

## 🔬 Referencias Científicas Oficiales

1. **Alcubierre, M.** (1994). *The warp drive: hyper-fast travel within general relativity*. Class. Quantum Grav. 11, L73.
2. **Forward, R. L.** (1982). *Antimatter propulsion*. Journal of the British Interplanetary Society, 35, 391–395.
3. **Frisbee, E. F.** (2003). *Advanced Space Propulsion for the 21st Century*. Journal of Propulsion and Power, 19(6), 1129–1154.
4. **Smith, G. A., et al.** (1996). *Antiproton-catalyzed microfission/fusion propulsion (ICAN-II)*. Pennsylvania State University / AIAA Paper 96-3068.
5. **White, H. G.** (2011). *Warp Field Mechanics 101*. NASA Johnson Space Center, AIAA Paper 2011-5917.
6. **Bobrick, A., & Martire, G.** (2021). *Introducing physical warp drives*. Class. Quantum Grav. 38, 105009.
7. **Lentz, E. W.** (2021). *Breaking the warp barrier: hyper-fast solitons in Einstein-Maxwell-plasma theory*. Class. Quantum Grav. 38, 075015.
