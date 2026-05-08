namespace GameAPI.Models.Authentification
{
    public class FullLoginResponse
    {
        // Основные пользовательские данные
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime? RegistrationDate { get; set; }

        // Ресурсы
        public int Gold { get; set; }

        // Рекорд и место
        public int BestScore { get; set; }
        public int Rank { get; set; }  // место в dense rank

        // Статистика
        public int GamesPlayed { get; set; }
        public int BlocksPlaced { get; set; }
        public int PerfectBlocks { get; set; }

        // Владения
        public List<UserBonusDto> Bonuses { get; set; }
        public List<UserUpgradeDto> Upgrades { get; set; }

        // Подарок
        public int SecondsUntilNextGift { get; set; }
        public bool GiftAvailable { get; set; }

        // Конфигурация магазина
        public StoreConfigDto StoreConfig { get; set; }

        // Вспомогательные DTO для владения
        public class UserBonusDto
        {
            public int BonusId { get; set; }
            public int Quantity { get; set; }
        }

        public class UserUpgradeDto
        {
            public int UpgradeId { get; set; }
            public int Level { get; set; }
        }

        public class StoreConfigDto
        {
            public List<BonusConfigDto> Bonuses { get; set; }
            public List<UpgradeConfigDto> Upgrades { get; set; }
            public List<UpgradeLevelConfigDto> UpgradeLevels { get; set; }
        }

        public class BonusConfigDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int PriceGold { get; set; }
        }
        public class UpgradeConfigDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class UpgradeLevelConfigDto
        {
            public int UpgradeId { get; set; }
            public int Level { get; set; }
            public int PriceGold { get; set; }
        }
    }
}
