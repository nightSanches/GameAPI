using GameAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Classes
{
    public class DBConnection : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserBonuses> UserBonuses { get; set; }
        public DbSet<UserCosmetics> UserCosmetics { get; set; }
        public DbSet<UserGifts> UserGifts { get; set; }
        public DbSet<UserScores> UserScores { get; set; }
        public DbSet<UserUpgrades> UserUpgrades { get; set; }

        public DBConnection()
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// Конфигурация подключения к базе данных MySQL
        /// </summary>
        /// <param name="optionsBuilder">Билдер опций контекста БД</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=127.0.0.1;port=3316;uid=root;pwd=;database=GameDatabase",
                new MySqlServerVersion(new Version(8, 0, 11)));
        }

        /// <summary>
        /// Конфигурация моделей базы данных
        /// </summary>
        /// <param name="modelBuilder">Билдер моделей</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уникальность никнейма
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Nickname)
                .IsUnique();

            // Уникальность email (может быть NULL, поэтому уникальность работает корректно)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserBonuses>()
                .HasIndex(e => e.UserId)
                .IsUnique();

            modelBuilder.Entity<UserGifts>()
                .HasIndex(e => e.UserId)
                .IsUnique();

            modelBuilder.Entity<UserScores>()
                .HasIndex(e => e.UserId)
                .IsUnique();

            modelBuilder.Entity<UserUpgrades>()
               .HasIndex(e => e.UserId)
               .IsUnique();
        }
    }
}
