using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WeatherDataLab3.Models;

namespace WeatherDataLab3.DataAcces;

public class WeatherDBContext : DbContext
{
    // Connection string to the lokala SQL Server databasen
    private const string connectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=EFCore;Trusted_Connection=True;";

    // Konfigurera DbContext att använda SQL Server med angiven connection string

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString);
    }
    // Konfigurera modellens entiteter och tabeller
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherRecord>().ToTable("WeatherData");
    }


    // Definiera DbSet för WeatherRecord entiteten
    public DbSet<WeatherRecord> WeatherRecords { get; set; }
}
