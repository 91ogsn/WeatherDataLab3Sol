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

            Console.Write("\nVälj ett alternativ (Nummer + Enter): ");
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
                    Console.WriteLine("3. Utomhus - Sortering av torraste till fuktigaste dagen enligt medelluftfuktighet per dag");
                    Console.ReadKey();
                    break;
                case "4":
                    Console.WriteLine("4. Utomhus - Sortering av minst till störst risk för mögel");
                    Console.ReadKey();
                    break;
                case "5":
                    Console.WriteLine("5. Utomhus - Datum för meteorologisk Höst");
                    Console.ReadKey();
                    break;
                case "6":
                    Console.WriteLine("6. Utomhus - Datum för meteorologisk Vinter");
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
                    Console.WriteLine("9.  Inomhus - Sortering av torraste till fuktigaste dagen enligt medelluftfuktighet per dag");
                    Console.ReadKey();
                    break;
                case "10":
                    Console.WriteLine("10. Inomhus - Sortering av minst till störst risk för mögel");
                    Console.ReadKey();
                    break;


                case "0":
                    Console.WriteLine("Avslutar programmet...");
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
}