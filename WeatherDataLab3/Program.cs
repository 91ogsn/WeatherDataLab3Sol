using Microsoft.EntityFrameworkCore;
using WeatherDataLab3.Core;
using WeatherDataLab3.DataAcces;

public class Program
{
    private static void Main(string[] args)
    {
        // Filsökväg till csv- filen
        var filePath = "../../../TempFuktData.csv";

        try
        {
            InitializeData(filePath);

            RunAppMenu();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Något gick fel, vi avbryter denna sändning...");
            Console.WriteLine(ex.ToString());
            Console.ReadKey();
        }

    }


    // ==== Metoder ==== \\
    private static void InitializeData(string csvPath)
    {
        Console.WriteLine("Initierar databas...");

        using var db = new WeatherDBContext();

        // Applicera migrations skapar databasen om den inte finns och om tabeller saknas så kapas de
        db.Database.Migrate();

        // Läs in CSV endast om tabellen är tom
        if (!db.WeatherRecords.Any())
        {
            Console.WriteLine("Databasen är tom. Importerar CSV...");
            CsvWeatherImporter.Import(db, csvPath);
        }
        else
        {
            Console.WriteLine("Imorterar inte från CSV-fil, databasen innehåller redan data.");
        }

        Console.WriteLine("Databasen är redo.");
        Console.WriteLine("Tryck på valfri för att fortsätta till menyn...");
        Console.ReadKey();
    }

    public static void RunAppMenu()
    {
        while (true)
        {
            Console.Clear();
            PrintMenuChoices();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nVälj ett alternativ (Nummer + Enter): ");
            Console.ResetColor();
            var choice = Console.ReadLine();

            switch (choice)
            {
                // ===== Utomhus val ==== \\
                case "1":
                    MenuMedelTemp("Ute");
                    break;
                case "2":
                    MenuSorteraVarmastTillKallast("Ute");
                    break;
                case "3":
                    MenuSorteraTorrastTillFuktigast("Ute");
                    break;
                case "4":
                    MenuSorteraMogelRisk("Ute");
                    break;
                case "5":
                    //Console.WriteLine("5. Utomhus - Datum för meteorologisk Höst");
                    Console.ReadKey();
                    break;
                case "6":
                    //Console.WriteLine("6. Utomhus - Datum för meteorologisk Vinter");
                    Console.ReadKey();
                    break;

                // ===== Inomhus val ==== \\
                case "7":
                    MenuMedelTemp("Inne");
                    break;
                case "8":
                    MenuSorteraVarmastTillKallast("Inne");
                    break;
                case "9":
                    MenuSorteraTorrastTillFuktigast("Inne");
                    break;
                case "10":
                    MenuSorteraMogelRisk("Inne");
                    break;


                case "0":
                    Console.Clear();
                    Console.WriteLine("Tryck på en tangent för att avslutar programmet...");
                    Console.WriteLine("Made by Stefan, tack för din medverkan... May the code be with you...always");
                    Console.ReadKey();
                    return;

                default:
                    Console.WriteLine("Ogiltigt val!");
                    Console.ReadKey();
                    break;
            }
        }
    }
    // Skriv ut menyalternativ
    public static void PrintMenuChoices()
    {
        Console.WriteLine("\t============ VäderData Meny ============\n");
        Console.WriteLine("1. Utomhus - Medeltemperatur för valt datum");
        Console.WriteLine("2. Utomhus - Sortering av varmaste till kallaste dagen enligt medeltemperatur per dag");
        Console.WriteLine("3. Utomhus - Sortering av torraste till fuktigaste dagen enligt medelluftfuktighet per dag");
        Console.WriteLine("4. Utomhus - Sortering av minst till störst risk för mögel");
        Console.WriteLine("5. Utomhus - Datum för meteorologisk Höst");
        Console.WriteLine("6. Utomhus - Datum för meteorologisk Vinter\n");

        Console.WriteLine("7.  Inomhus - Medeltemperatur för valt datum");
        Console.WriteLine("8.  Inomhus - Sortering av varmaste till kallaste dagen enligt medeltemperatur per dag");
        Console.WriteLine("9.  Inomhus - Sortering av torraste till fuktigaste dagen enligt medelluftfuktighet per dag");
        Console.WriteLine("10. Inomhus - Sortering av minst till störst risk för mögel");
        Console.WriteLine("0.  Avsluta Programmet");

    }

    // Hämta medeltemperatur för ett valt datum och plats (inomhus/utomhus)
    private static void MenuMedelTemp(string plats)
    {
        Console.Clear();
        Console.WriteLine($"Medeltemperatur för {plats}\n");

        Console.Write("Ange datum (ÅÅÅÅ-MM-DD): ");
        string? input = Console.ReadLine();
        // Kontroleraa datumformat
        if (!DateTime.TryParse(input, out DateTime date))
        {
            Console.WriteLine("\nFelaktigt datumformat! Exempel: 2016-10-01");
            Console.ReadKey();
            return;
        }
        // Hämta medeltemperatur från metod i WeatherStatisticsService
        using var db = new WeatherDBContext();

        var avg = WeatherCalculations.AverageTemperatureForDate(
            db.WeatherRecords,
            date,
            plats
        );

        Console.WriteLine();
        // Skriv ut resultat till användaren
        if (avg.HasValue)
            Console.WriteLine($"Medeltemperatur {plats} den {date:yyyy-MM-dd}: {avg:F2} °C");
        else
            Console.WriteLine($"Ingen data hittades för {plats} den {date:yyyy-MM-dd}.");

        Console.WriteLine("\nTryck valfri tangent för att återgå...");
        Console.ReadKey();
    }

    // Sortera varmaste till kallaste dagen enligt medeltemperatur per dag
    private static void MenuSorteraVarmastTillKallast(string plats)
    {
        Console.Clear();
        Console.WriteLine($"Sortering: Varmast --> Kallast ({plats})\n");

        using var db = new WeatherDBContext();
        // Hämta sorterad lista från metod i WeatherCalculations
        var lista = WeatherCalculations.SorteraVarmastTillKallast(
            db.WeatherRecords,
            plats
        );
        Console.WriteLine(" Datum       Medeltemp (visar 30 dagar)");
        Console.WriteLine("------------------------");
        // Skriv ut resultat till användaren (första 30 dagarna)
        foreach (var dag in lista.Take(30)) 
        {
            Console.WriteLine($"{dag.Datum:yyyy-MM-dd}  {dag.MedelTemp:F2} °C");
        }

        Console.WriteLine("\nTryck valfri tangent för att återgå...");
        Console.ReadKey();
    }

    // Sortera torraste till fuktigaste dagen enligt medelluftfuktighet per dag
    private static void MenuSorteraTorrastTillFuktigast(string plats)
    {
        Console.Clear();
        Console.WriteLine($"Sortering: Torraste -> Fuktigaste ({plats})\n");

        // öppnar databasanslutningen (dispose automatiskt med 'using var').
        using var db = new WeatherDBContext();

        // Hämtar den aggregerade och sorterade listan från metoden i Core
        var lista = WeatherCalculations.SorteraTorrastTillFuktigast(
            db.WeatherRecords,
            plats
        );

        Console.WriteLine("Datum         Medelfuktighet (%)");
        Console.WriteLine("---------------------------------");

        // Visa topp 30 dagar
        foreach (var dag in lista.Take(30))
        {
            Console.WriteLine($"{dag.Datum:yyyy-MM-dd}    {dag.MedelFuktighet:F1}%");
        }

        Console.WriteLine("\nTryck valfri tangent för att koma tillbaks till meny...");
        Console.ReadKey();
    }

    // Sortera minst till störst risk för mögel
    private static void MenuSorteraMogelRisk(string plats)
    {
        Console.Clear();
        Console.WriteLine($"Sortering: Minst till Störst Mögelrisk ({plats})\n");

        using var db = new WeatherDBContext();

        // Hämta sorterad lista från metod i WeatherCalculations
        var lista = WeatherCalculations.SorteraMogelRisk(
            db.WeatherRecords,
            plats
        );

        Console.WriteLine("Datum         Mögelrisk");
        Console.WriteLine("------------------------");

        foreach (var dag in lista.Take(100)) // visa topp 100 till användaren
        {
            Console.WriteLine($"{dag.Datum:yyyy-MM-dd}    {dag.Moldrisk:F2}");
        }

        Console.WriteLine("\nTryck valfri tangent för att komma tillbaka till meny...");
        Console.ReadKey();
    }
}