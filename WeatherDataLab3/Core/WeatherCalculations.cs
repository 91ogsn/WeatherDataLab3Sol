using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherDataLab3.Models;

namespace WeatherDataLab3.Core
{
    // === Klass med Metoder för väderberäkningar === \\
    public static class WeatherCalculations
    {
        // === Metod för att beräkna medeltemperatur för en given dag och plats === \\
        public static double? AverageTemperatureForDate(IQueryable<WeatherRecord> records, DateTime date, string plats)  
        {
            // Använder Datum och Plats för att filtrera poster och efter om Temp har värde
            var query = records
                .Where(r => r.Datum.Date == date.Date &&
                            r.Plats == plats &&
                            r.Temp.HasValue)
                .Select(r => r.Temp.Value);
            // Hantera fallet där inga poster matchar
            if (!query.Any())
                return null;
            // Beräkna och returnera medelvärdet med LINQ
            return query.Average();
        }

        // === Sortering varmaste till kallaste dagen enligt medeltemperatur per dag === \\
        public static IEnumerable<(DateTime Datum, double MedelTemp)>SorteraVarmastTillKallast(IQueryable<WeatherRecord> records, string plats)
        {
            // Gruppera poster efter datum där platsen matchar och Temp har värde
            var query = records
                .Where(r => r.Plats == plats && r.Temp.HasValue)
                .GroupBy(r => r.Datum.Date)
                .Select(g => new
                {
                    Datum = g.Key,
                    MedelTemp = g.Average(x => x.Temp!.Value) // medelvärde av Temp för varje dag 
                })
                .OrderByDescending(x => x.MedelTemp) // sortera från varmaste till kallaste
                .ToList(); // exekvera frågan och hämta resultatet som lista
            
            return query.Select(x => (x.Datum, x.MedelTemp));
        }
    }
}
