using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherDataLab3.Models
{
    public class WeatherRecord
    {
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public string Plats { get; set; } = string.Empty;
        public double? Temp { get; set; }
        public double? Luftfuktighet { get; set; }
        public double Moldrisk
        {
            get
            {
                // Kontrollera om Temp och Luftfuktighet har värden
                if (!Temp.HasValue || !Luftfuktighet.HasValue)
                {
                    return 0;
                }
                // Beräkna mögelrisk baserat på temperatur och luftfuktighet
                double t = Temp.Value;
                double rh = Luftfuktighet.Value;

                // Kritisk relativ fuktighet för mögelbildning
                double rhCrit = 80 + 0.25 * (20 - t);

                // Begränsa rhCrit till intervallet 75% - 95%
                rhCrit = Math.Clamp(rhCrit, 75, 95);

                // Om relativ fuktighet är under kritisk nivå eller temperaturen är under 5°C, är risken 0
                if (rh < rhCrit || t < 5)
                {
                    return 0;
                }
                // Beräkna mögelrisken
                double risk = (rh - rhCrit) * t / 100;
                return Math.Round(risk, 2);
            }
        }
    }
}
