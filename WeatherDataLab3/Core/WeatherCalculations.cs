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
        public static double? AverageTemperatureForDate(
    IQueryable<WeatherRecord> records,
    DateTime date,
    string plats)
        // Använder Datum och Plats för att filtrera poster och efter om Temp har värde
        {
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
    }
}
