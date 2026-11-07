using System;
using UnityEngine;

namespace Alcubierre
{
    public enum PropulsionMode
    {
        Chemical,       // Cohete Químico (Línea base Tsiolkovsky)
        Antimatter,     // Aniquilación Antimateria Relativista (Beam-Core)
        WarpDrive       // Curvatura Espacio-Temporal (Métrica de Alcubierre)
    }

    /// <summary>
    /// Resultados analíticos y matemáticos de un viaje espacial según el paradigma de propulsión.
    /// </summary>
    [Serializable]
    public struct FlightCalculation
    {
        public PropulsionMode mode;
        public string modeName;
        public double travelTimeEarthSeconds;     // Tiempo medido en la Tierra
        public double travelTimeShipSeconds;      // Tiempo propio medido por la tripulación (tau)
        public double maxVelocityKms;             // Velocidad máxima alcanzada (km/s)
        public double velocityFractionC;          // Velocidad en fracción de c (v/c o Factor Warp W)
        public double lorentzGamma;               // Factor de Lorentz gamma
        public double feltGForce;                 // Fuerzas G sentidas por la tripulación (g)
        public double fuelMassKg;                 // Masa de combustible requerida
        public double antimatterMassKg;           // Masa de antimateria pura requerida
        public double normalMatterMassKg;         // Masa de materia normal requerida
        public double exoticEnergyJoules;         // Energía exótica requerida (Warp)
        public string physicalPrinciple;          // Descripción teórica del modo
        public string timeDilationSummary;        // Resumen del efecto temporal
    }

    /// <summary>
    /// Motor matemático que implementa las fórmulas físicas exactas de la carpeta contexto.
    /// Basado en Tsiolkovsky (Química), Relatividad Especial (Antimateria Beam-Core) y Relatividad General (Alcubierre).
    /// </summary>
    public static class PropulsionPhysics
    {
        // Masa en seco estándar de la nave científica (1 tonelada = 1000 kg)
        public const double DryMassKg = 1000.0;

        /// <summary>
        /// Calcula el perfil analítico de vuelo para una distancia dada en kilómetros,
        /// incorporando masa seca personalizada según el material y porcentaje de carga de combustible.
        /// </summary>
        public static FlightCalculation CalculateTrip(PropulsionMode mode, double distanceKm, double customDryMassKg = DryMassKg, float fuelLoadingPct = 100f)
        {
            FlightCalculation calc = new FlightCalculation();
            calc.mode = mode;

            double c_kms = SpaceConstants.SpeedOfLight_kms;
            double effectiveDryMass = Math.Max(100.0, customDryMassKg);
            double fuelFactor = Math.Max(0.1, fuelLoadingPct / 100.0);

            switch (mode)
            {
                case PropulsionMode.Chemical:
                    {
                        calc.modeName = "🚀 Propulsión Química (Hydrolox LOX/LH2)";
                        calc.physicalPrinciple = "Combustión redox molecular exotérmica. Ecuación clásica de Tsiolkovsky (Isp = 452 s, ve = 4.434 km/s).";
                        
                        // Velocidad media de crucero heliocéntrico de escape (Línea base histórica: New Horizons / Voyager)
                        double v_cruise = 17.0; // km/s
                        calc.maxVelocityKms = v_cruise;
                        calc.velocityFractionC = v_cruise / c_kms;
                        calc.lorentzGamma = 1.0;
                        calc.feltGForce = 3.5; // Durante el quemado inicial inercial

                        calc.travelTimeEarthSeconds = distanceKm / v_cruise;
                        calc.travelTimeShipSeconds = calc.travelTimeEarthSeconds; // gamma = 1

                        // Tsiolkovsky: m0 / mf = exp(deltaV / ve)
                        // Para acelerar a velocidad crucero de 17 km/s:
                        double deltaV_ms = v_cruise * 1000.0;
                        double ve_ms = 4434.0;
                        double massRatio = Math.Exp(deltaV_ms / ve_ms);
                        double nominalFuel = effectiveDryMass * (massRatio - 1.0);

                        calc.fuelMassKg = nominalFuel * fuelFactor;
                        calc.antimatterMassKg = 0.0;
                        calc.normalMatterMassKg = calc.fuelMassKg;
                        calc.exoticEnergyJoules = 0.0;
                        calc.timeDilationSummary = "Inapreciable (gamma = 1.000000). El tiempo transcurre idéntico a la Tierra.";
                    }
                    break;

                case PropulsionMode.Antimatter:
                    {
                        calc.modeName = "⚛️ Propulsión por Antimateria (Beam-Core)";
                        calc.physicalPrinciple = "Aniquilación pura p + p̄ -> pi± + pi0 -> 2mc^2 (ve = 0.50c). Dinámica relativista.";

                        // Velocidad de crucero relativista objetivo:
                        // 0.20c como estándar interplanetario y de misiones interestelares (según contexto),
                        // subiendo a 0.50c para distancias interestelares profundas.
                        double targetFractionC = distanceKm > (SpaceConstants.KmPerLightYear * 0.5) ? 0.35 : 0.20;
                        double v_kms = targetFractionC * c_kms;
                        calc.maxVelocityKms = v_kms;
                        calc.velocityFractionC = targetFractionC;

                        // Factor de Lorentz: gamma = 1 / sqrt(1 - (v/c)^2)
                        double beta = targetFractionC;
                        calc.lorentzGamma = 1.0 / Math.Sqrt(1.0 - (beta * beta));
                        calc.feltGForce = 1.0; // 1.0 g constante para confort de la tripulación

                        // Tiempo en la Tierra: t_tierra = d / v
                        calc.travelTimeEarthSeconds = distanceKm / v_kms;
                        // Tiempo propio en la nave: dtau = dt / gamma
                        calc.travelTimeShipSeconds = calc.travelTimeEarthSeconds / calc.lorentzGamma;

                        // Ecuación de cohete relativista de antimateria con desaceleración completa en destino:
                        // (m0 / mf)_completa = ((1 + beta) / (1 - beta))^(c / ve)
                        // Con ve = 0.5c -> c / ve = 2.0
                        double ratioOneWay = (1.0 + beta) / (1.0 - beta);
                        double massRatioComplete = ratioOneWay * ratioOneWay;
                        double nominalPropellantKg = effectiveDryMass * (massRatioComplete - 1.0);

                        double totalPropellantKg = nominalPropellantKg * fuelFactor;
                        calc.fuelMassKg = totalPropellantKg;
                        calc.antimatterMassKg = totalPropellantKg * 0.5; // 50% antimateria
                        calc.normalMatterMassKg = totalPropellantKg * 0.5; // 50% materia
                        calc.exoticEnergyJoules = 0.0;
                        calc.timeDilationSummary = $"Dilatación Temporal activa (gamma = {calc.lorentzGamma:F3}). La tripulación envejece más lento ({calc.travelTimeShipSeconds < calc.travelTimeEarthSeconds}).";
                    }
                    break;

                case PropulsionMode.WarpDrive:
                    {
                        calc.modeName = "🌌 Curvatura Espacio-Temporal (Alcubierre Warp)";
                        calc.physicalPrinciple = "Deformación métrica de Alcubierre (contracción frontal theta < 0, expansión posterior theta > 0). Caída libre inercial pura (a^mu = 0).";

                        // Factor Warp W según escala de la misión:
                        // W = 10c para Sistema Solar y Próxima Centauri (según contexto: Luna en 0.13s, Marte en 1.25min, Próxima en 5.1 meses)
                        // W = 100c para espacio interestelar medio, W = 1000c para galáctico
                        double warpFactor;
                        if (distanceKm <= SpaceConstants.KmPerAU * 100.0)
                        {
                            warpFactor = 10.0; // 10c dentro del Sistema Solar
                        }
                        else if (distanceKm <= SpaceConstants.KmPerLightYear * 10.0)
                        {
                            warpFactor = 10.0; // 10c en el vecindario estelar (Próxima Centauri)
                        }
                        else if (distanceKm <= SpaceConstants.KmPerLightYear * 1000.0)
                        {
                            warpFactor = 100.0; // 100c
                        }
                        else
                        {
                            warpFactor = 1000.0; // 1000c para Centro Galáctico y Andrómeda
                        }

                        double apparentSpeedKms = warpFactor * c_kms;
                        calc.maxVelocityKms = apparentSpeedKms;
                        calc.velocityFractionC = warpFactor; // Factor W
                        calc.lorentzGamma = 1.0; // En la burbuja plana no hay factor Lorentz clásico
                        calc.feltGForce = 0.0;   // 0.00 g inercial absoluto (a^mu = 0)

                        // Tiempo coordenado y tiempo propio son estrictamente IDÉNTICOS: dtau = dt
                        calc.travelTimeEarthSeconds = distanceKm / apparentSpeedKms;
                        calc.travelTimeShipSeconds = calc.travelTimeEarthSeconds;

                        calc.fuelMassKg = 0.0;
                        calc.antimatterMassKg = 0.0;
                        calc.normalMatterMassKg = 0.0;

                        // Energía exótica calculada según optimización de Van Den Broeck y Harold White:
                        // E ~ -1.2 * 10^20 Joules (equivalente a ~ -1335 kg de masa exótica según E = -mc^2)
                        calc.exoticEnergyJoules = -1.2e20;
                        calc.timeDilationSummary = "CERO DILATACIÓN (dtau = dt). Tiempo sincronizado con la Tierra y 0.00 g inerciales absolutos.";
                    }
                    break;
            }

            return calc;
        }

        /// <summary>
        /// Evalúa la función de forma métrica de Alcubierre f(rs) en un radio rs.
        /// </summary>
        public static float EvaluateAlcubierreShape(float rs, float R = 2.0f, float sigma = 1.5f)
        {
            double num = Math.Tanh(sigma * (rs + R)) - Math.Tanh(sigma * (rs - R));
            double den = 2.0 * Math.Tanh(sigma * R);
            return (float)(num / den);
        }

        /// <summary>
        /// Evalúa la tasa de expansión/contracción theta(x, y) del espacio-tiempo en torno a la nave.
        /// </summary>
        public static float EvaluateSpacetimeExpansion(Vector2 posRelativeToShip, float warpSpeedFraction)
        {
            float rs = posRelativeToShip.magnitude;
            if (rs < 0.001f) return 0f;

            float R = 2.0f;
            float sigma = 1.5f;

            // Derivada f'(rs)
            double t1 = Math.Tanh(sigma * (rs + R));
            double t2 = Math.Tanh(sigma * (rs - R));
            double sech2_1 = 1.0 - (t1 * t1);
            double sech2_2 = 1.0 - (t2 * t2);
            double f_prime = (sigma * (sech2_1 - sech2_2)) / (2.0 * Math.Tanh(sigma * R));

            // theta = (vs/c) * (x/rs) * f'(rs)
            float theta = (float)(warpSpeedFraction * (posRelativeToShip.x / rs) * f_prime);
            return theta;
        }
    }
}
