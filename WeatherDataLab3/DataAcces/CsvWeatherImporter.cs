using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using WeatherDataLab3.Models;

namespace WeatherDataLab3.DataAcces
{
    public class CsvWeatherImporter
    {
        public static List<WeatherRecord> ReadCsv(string filePath)
        {
            // Kontrollera om filen finns på angiven sökväg
            if (!File.Exists(filePath))
                throw new FileNotFoundException("CSV-filen saknas", filePath);

            // Konfigurera CsvHelper för att läsa CSV-filen
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ","
            };
            // Skapa en CsvReader för att läsa filen
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            // Mappar kolumnerna till WeatherRecord-egenskaper med hjälp av WeatherRecordMap
            csv.Context.RegisterClassMap<WeatherRecordMap>();
            return csv.GetRecords<WeatherRecord>().ToList();
        }
        public static void Import(WeatherDBContext db, string filePath)
        {
            Console.WriteLine("Läser CSV-filen...");

            var records = ReadCsv(filePath);

            // räkna insatta och överhoppade poster
            int inserted = 0;
            int skipped = 0;

            foreach (var r in records)
            {
                // Validera posten
                if (!Validate(r))
                {
                    skipped++;
                    continue;
                }

                // Undvik dubbletter: Datum + Plats
                bool exists = db.WeatherRecords.Any(x =>
                    x.Datum == r.Datum &&
                    x.Plats == r.Plats);

                if (!exists)
                {
                    db.WeatherRecords.Add(r);
                    inserted++;
                }
                else
                {
                    skipped++;
                }
            }
            // Spara ändringarna i databasen
            db.SaveChanges();

            Console.WriteLine();
            Console.WriteLine($"CSV-import klar!");
            Console.WriteLine($"Importerade: {inserted}");
            Console.WriteLine($"Överhoppade: {skipped}");
        }
        // hjälpmetod för validering av enskilda poster
        private static bool Validate(WeatherRecord r)
        {
            if (r == null) return false;
            if (r.Datum == default) return false;
            if (string.IsNullOrWhiteSpace(r.Plats)) return false;
            
            return true;
        }
    }
}
