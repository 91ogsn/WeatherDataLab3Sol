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

            RunApplication();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Något gick fel, vi avbryter denna sändning...");
            Console.WriteLine(ex);
            Console.ReadKey();
        }
               
    }

    
    // ==== Metoder ==== \\
    private static void InitializeData(string csvPath)
    {
        Console.WriteLine("Initierar databas...");

        using var db = new WeatherDBContext();

        // Applicera migrations skapar databasen om den inte finns
        db.Database.Migrate();

        // Läs in CSV endast om tabellen är tom
        if (!db.WeatherRecords.Any())
        {
            Console.WriteLine("Databasen är tom. Importerar CSV...");
            //CsvWeatherImporter.Import(db, csvPath);
        }

        Console.WriteLine("Databasen är redo.");
    }

    private static void RunApplication()
    {
        Console.WriteLine("Fixa! Ska starta meny");
        Console.ReadKey();
    }
}