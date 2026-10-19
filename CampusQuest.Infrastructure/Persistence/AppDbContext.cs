using Microsoft.EntityFrameworkCore;
using CampusQuest.Domain.Entities;

namespace CampusQuest.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Mission> Missions { get; set; }
    public DbSet<UserMission> UserMissions { get; set; }
    // Add Users DbSet so repositories can access users via the context
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Başlangıç Görev Verileri (Seed Data)
        modelBuilder.Entity<Mission>().HasData(
            new Mission
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Title = "Merkezi Kütüphane",
                Description = "Kütüphanede sessizce çalış ve QR kodu okut.",
                XpReward = 100,
                Latitude = 37.0592,
                Longitude = 35.3552,
                RadiusInMeters = 20.0,
                QrCodeSecret = "CAMPUSQUEST_KUTUPHANE"
            },
            new Mission
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Title = "Öğrenci Yemekhanesi",
                Description = "Günün menüsünü tat ve enerjini topla.",
                XpReward = 150,
                Latitude = 37.0580,
                Longitude = 35.3540,
                RadiusInMeters = 20.0,
                QrCodeSecret = "CAMPUSQUEST_YEMEKHANE"
            },
            new Mission
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Title = "Rektörlük Binası",
                Description = "Kampüsün yönetim merkezini ziyaret et.",
                XpReward = 200,
                Latitude = 37.0610,
                Longitude = 35.3570,
                RadiusInMeters = 20.0,
                QrCodeSecret = "CAMPUSQUEST_REKTORLUK"
            },
            new Mission
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Title = "Mühendislik Fakültesi",
                Description = "Geleceğin teknolojilerini yerinde incele.",
                XpReward = 120,
                Latitude = 37.0565,
                Longitude = 35.3520,
                RadiusInMeters = 20.0,
                QrCodeSecret = "CAMPUSQUEST_MUHENDISLIK"
            },
            new Mission
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Title = "Spor Salonu",
                Description = "Sağlam kafanın sağlam vücutta bulunduğunu kanıtla.",
                XpReward = 100,
                Latitude = 37.0630,
                Longitude = 35.3590,
                RadiusInMeters = 20.0,
                QrCodeSecret = "CAMPUSQUEST_SPOR"
            }
        );
    }
}