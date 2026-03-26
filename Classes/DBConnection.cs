using Microsoft.EntityFrameworkCore;

namespace GameAPI.Classes
{
    public class DBConnection : DbContext
    {
        // Пример таблицы пользователей
        public DbSet<User> Users { get; set; }

        public DatabaseConnection()
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
                "server=127.0.0.1;port=3306;uid=root;pwd=;database=GameDatabase",
                new MySqlServerVersion(new Version(8, 0, 11)));
        }

        /// <summary>
        /// Конфигурация моделей базы данных
        /// </summary>
        /// <param name="modelBuilder">Билдер моделей</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Пример настройки таблицы Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
