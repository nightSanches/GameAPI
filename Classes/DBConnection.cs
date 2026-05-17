using GameAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameAPI.Classes
{
    public class DBConnection : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Bonus> Bonuses { get; set; }
        public DbSet<Upgrade> Upgrades { get; set; }
        public DbSet<UserBonus> UserBonuses { get; set; }
        public DbSet<UserGift> UserGifts { get; set; }
        public DbSet<UserScore> UserScores { get; set; }
        public DbSet<UserUpgrade> UserUpgrades { get; set; }
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<UpgradesCost> UpgradesCosts { get; set; }
        public DbSet<UserStats> UserStatss { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }

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
            base.OnModelCreating(modelBuilder);

            // --- Users ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Nickname).IsUnique();
                entity.Property(e => e.Nickname).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Role).HasDefaultValue("player");
                entity.Property(e => e.RegistrationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EmailConfirmed).HasDefaultValue(false);
            });

            // --- Bonuses ---
            modelBuilder.Entity<Bonus>(entity =>
            {
                entity.ToTable("Bonuses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PriceMoney).HasDefaultValue(0);
            });

            // --- Upgrades ---
            modelBuilder.Entity<Upgrade>(entity =>
            {
                entity.ToTable("Upgrades");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // --- Upgrades_cost ---
            modelBuilder.Entity<UpgradesCost>(entity =>
            {
                entity.ToTable("Upgrades_cost");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UpgradeId).IsRequired();
                entity.Property(e => e.Level).HasDefaultValue(1);
                entity.Property(e => e.PriceMoney).HasDefaultValue(0);

                entity.HasOne<Upgrade>()
                      .WithMany()
                      .HasForeignKey(e => e.UpgradeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Users_bonuses ---
            modelBuilder.Entity<UserBonus>(entity =>
            {
                entity.ToTable("Users_bonuses");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Bonuses)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Bonus)
                      .WithMany()
                      .HasForeignKey(e => e.BonusId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Quantity).HasDefaultValue(0);
            });

            // --- Users_gifts ---
            modelBuilder.Entity<UserGift>(entity =>
            {
                entity.ToTable("Users_gifts");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasOne(e => e.User)
                      .WithOne(u => u.Gift)
                      .HasForeignKey<UserGift>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Users_scores ---
            modelBuilder.Entity<UserScore>(entity =>
            {
                entity.ToTable("Users_scores");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.DistrictId }).IsUnique();
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Scores)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.District)
                      .WithMany()
                      .HasForeignKey(e => e.DistrictId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.BestScore).IsRequired();
            });

            // --- Users_upgrades ---
            modelBuilder.Entity<UserUpgrade>(entity =>
            {
                entity.ToTable("Users_upgrades");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Upgrades)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Upgrade)
                      .WithMany()
                      .HasForeignKey(e => e.UpgradeId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Level).HasDefaultValue(0);
            });

            // --- Users_wallet ---
            modelBuilder.Entity<UserWallet>(entity =>
            {
                entity.ToTable("Users_wallet");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasOne(e => e.User)
                      .WithOne(u => u.Wallet)
                      .HasForeignKey<UserWallet>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Money).HasDefaultValue(0);
                entity.Property(e => e.Reputation).HasDefaultValue(0);
            });

            // --- Users_stats ---
            modelBuilder.Entity<UserStats>(entity =>
            {
                entity.ToTable("Users_stats");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasOne(e => e.User)
                      .WithOne()
                      .HasForeignKey<UserStats>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.GamesPlayedCount).HasDefaultValue(0);
                entity.Property(e => e.BlocksPlacedCount).HasDefaultValue(0);
                entity.Property(e => e.IBlocksPlacedCount).HasDefaultValue(0);
            });

            // --- Districts ---
            modelBuilder.Entity<District>(entity =>
            {
                entity.ToTable("Districts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnlockRepReq).HasDefaultValue(0);
                entity.Property(e => e.DifficultyMultiplier).HasDefaultValue(1.00m);
                entity.Property(e => e.SortOrder).HasDefaultValue(0);
            });

            // --- Achievements ---
            modelBuilder.Entity<Achievement>(entity =>
            {
                entity.ToTable("Achievements");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.ConditionType).IsRequired();
                entity.Property(e => e.ConditionValue).HasDefaultValue(0);
                entity.Property(e => e.RewardRep).HasDefaultValue(0);
                
                entity.HasOne(e => e.District)
                      .WithMany()
                      .HasForeignKey(e => e.DistrictId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Users_achievements ---
            modelBuilder.Entity<UserAchievement>(entity =>
            {
                entity.ToTable("Users_achievements");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Achievement)
                      .WithMany()
                      .HasForeignKey(e => e.AchievementId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.CurrentProgress).HasDefaultValue(0);
                entity.Property(e => e.IsUnlocked).HasDefaultValue(false);
            });
        }
    }
}
