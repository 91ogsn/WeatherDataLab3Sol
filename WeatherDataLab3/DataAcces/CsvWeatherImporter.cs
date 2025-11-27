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

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            // Mappar kolumnerna till WeatherRecord-egenskaper med hjälp av WeatherRecordMap
            csv.Context.RegisterClassMap<WeatherRecordMap>();
            return csv.GetRecords<WeatherRecord>().ToList();
        }
    }
}
