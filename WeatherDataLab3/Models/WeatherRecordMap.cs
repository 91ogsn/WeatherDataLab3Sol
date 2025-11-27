using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WeatherDataLab3.Models;

public class WeatherRecordMap : ClassMap<WeatherRecord>
{
    public WeatherRecordMap()
    {
        Map(m => m.Datum)
            .Name("Datum")
            .TypeConverterOption.Format("yyyy-MM-dd");

        Map(m => m.Plats)
            .Name("Plats");

        Map(m => m.Temp)
            .Name("Temp")
            .TypeConverter<DoubleConverter>();

        Map(m => m.Luftfuktighet)
            .Name("Luftfuktighet")
            .TypeConverter<DoubleConverter>();
    }
}
// Anpassad typkonverterare för att hantera både punkt och komma som decimalavskiljare
public class DoubleConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;
        // Ta bort eventuella omgivande blanksteg
        text = text.Trim();

        // 1. Försök med invariant culture (punkt som decimal)
        if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            return value;

        // 2. Försök med svensk kultur (komma som decimal)
        if (double.TryParse(text, NumberStyles.Any, new CultureInfo("sv-SE"), out value))
            return value;

        // 3. Misslyckades båda → returnera null
        return null;
    }
}
