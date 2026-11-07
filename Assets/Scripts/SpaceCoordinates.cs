using System;
using UnityEngine;

namespace Alcubierre
{
    /// <summary>
    /// Constantes astronómicas y funciones de conversión para navegación a escala real.
    /// </summary>
    public static class SpaceConstants
    {
        // Velocidad de la luz en el vacío (m/s y km/s)
        public const double SpeedOfLight_ms = 299792458.0;
        public const double SpeedOfLight_kms = 299792.458;

        // Unidades astronómicas en kilómetros
        public const double KmPerAU = 149597870.7;           // 1 UA en km
        public const double KmPerLightYear = 9460730472580.8; // 1 año luz en km
        public const double AUPerLightYear = KmPerLightYear / KmPerAU; // ~63241.077 UA/ly

        // Aceleración de gravedad estándar
        public const double StandardGravity = 9.80665; // m/s^2

        // Escalas visuales normalizadas para representación en Unity 2D
        // 1 Unidad de Unity = 1 UA en escala del Sistema Solar Interior
        public const float SolarSystemVisualScale = 1.0f; // 1 UA = 4 unidades Unity

        /// <summary>
        /// Mapea una distancia real en AU a coordenadas visuales 2D con compresión logarítmica
        /// para permitir navegar simultáneamente desde la órbita de la Luna hasta Próxima Centauri.
        /// </summary>
        public static float RealDistanceToVisualUnits(double distanceAU)
        {
            if (distanceAU <= 0.0001) return 0f;

            // Escala por segmentos suave para visualización continua:
            // 0 - 2 UA: Lineal suave (Tierra - Marte)
            if (distanceAU <= 2.0)
            {
                return (float)(distanceAU * 5.0);
            }
            // 2 - 40 UA: Sistema Solar exterior (Júpiter - Neptuno)
            else if (distanceAU <= 40.0)
            {
                return (float)(10.0 + Math.Log10(distanceAU / 2.0) * 15.0);
            }
            // 40 - 2000 UA: Cinturón de Kuiper y Oort interior
            else if (distanceAU <= 2000.0)
            {
                return (float)(25.0 + Math.Log10(distanceAU / 40.0) * 18.0);
            }
            // > 2000 UA: Espacio interestelar (Próxima Centauri, etc.)
            else
            {
                return (float)(43.0 + Math.Log10(distanceAU / 2000.0) * 22.0);
            }
        }

        public static string FormatDistance(double distanceKm)
        {
            if (distanceKm < 1000000.0)
            {
                return $"{distanceKm:N0} km";
            }
            else if (distanceKm < KmPerAU * 100.0)
            {
                double au = distanceKm / KmPerAU;
                return $"{distanceKm / 1e6:F2} millones km ({au:F3} UA)";
            }
            else if (distanceKm < KmPerLightYear * 0.1)
            {
                double au = distanceKm / KmPerAU;
                return $"{au:N1} UA";
            }
            else
            {
                double ly = distanceKm / KmPerLightYear;
                return $"{ly:F3} años luz ({distanceKm / KmPerAU:N0} UA)";
            }
        }

        public static string FormatTime(double totalSeconds)
        {
            if (totalSeconds < 60.0)
            {
                return $"{totalSeconds:F1} seg";
            }
            else if (totalSeconds < 3600.0)
            {
                double min = totalSeconds / 60.0;
                double sec = totalSeconds % 60.0;
                return $"{min:0} min {sec:0} s";
            }
            else if (totalSeconds < 86400.0)
            {
                double hrs = totalSeconds / 3600.0;
                return $"{hrs:F2} horas";
            }
            else if (totalSeconds < 86400.0 * 365.25)
            {
                double days = totalSeconds / 86400.0;
                return $"{days:F1} días";
            }
            else
            {
                double years = totalSeconds / (86400.0 * 365.25);
                return $"{years:F2} años";
            }
        }
    }
}
