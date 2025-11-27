using Microsoft.EntityFrameworkCore;
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
                    Console.WriteLine("1. Utomhus - Medeltemperatur för valt datum");
                    Console.ReadKey();
                    break;
                case "2":
                    Console.WriteLine("2. Utomhus - Sortering av varmaste till kallaste dagen enligt medeltemperatur per dag");
                    Console.ReadKey();
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
                    Console.WriteLine("7.  Inomhus - Medeltemperatur för valt datum");
                    Console.ReadKey();
                    break;
                case "8":
                    Console.WriteLine("8.  Inomhus - Sortering av varmaste till kallaste dagen enligt medeltemperatur per dag");
                    Console.ReadKey();
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
}