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
        public static IEnumerable<(DateTime Datum, double MedelTemp)> SorteraVarmastTillKallast(IQueryable<WeatherRecord> records, string plats)
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

        // === Metod för sortering torraste till fuktigaste dagen enligt medelluftfuktighet per dag === \\
        public static IEnumerable<(DateTime Datum, double MedelFuktighet)> SorteraTorrastTillFuktigast(IQueryable<WeatherRecord> records, string plats)
        {
            // Filtrera bort alla rader som inte matchar platsen ("Ute"/"Inne")
            // och som saknar luftfuktighetsvärde (NULL).
            var query = records
                .Where(r => r.Plats == plats && r.Luftfuktighet.HasValue)

                // grupperar alla rader som tillhör samma datum så vi kan räkna medelvärdet per dag.
                .GroupBy(r => r.Datum.Date)

                // Beräkna dagens genomssnittliga luftfuktighet(vi vet att värdena inte är nulll pga HasValue ovan)
                .Select(g => new
                {
                    Datum = g.Key,
                    MedelFuktighet = g.Average(x => x.Luftfuktighet!.Value)
                })

                // Sortering stigande (torrast → fuktigast)
                .OrderBy(x => x.MedelFuktighet)
                .ToList();

            return query.Select(x => (x.Datum, x.MedelFuktighet));
        }

        // === Metod för sortering av minst till störst risk för mögel === \\
        public static IEnumerable<(DateTime Datum, double Moldrisk)> SorteraMogelRisk(IQueryable<WeatherRecord> records, string plats)
        {
            // Filtrera på vald plats ("Ute" eller "Inne")
            // och kräv att både temperatur och luftfuktighet finns.
            var query = records
                .Where(r => r.Plats == plats &&
                            r.Temp.HasValue &&
                            r.Luftfuktighet.HasValue)
                .AsEnumerable() // Gör resten av beräkningen i C#
                .GroupBy(r => r.Datum.Date)  // Gruppera per dag               
                .Select(g => new         
                {
                    Datum = g.Key,
                    MedelRisk = g.Average(x => x.Moldrisk)  // Beräkna dagens Mögelrisk som medelvärde.
                })                
                .OrderBy(x => x.MedelRisk)// Sortera stigande (minst → störst risk)
                .ToList();

            // Returnera datum och risk
            return query.Select(x => (x.Datum, x.MedelRisk));
        }
    }
}
