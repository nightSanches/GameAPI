using Newtonsoft.Json;
using System.Collections.Generic;

/// запрос на сохранение прогресса достижений (при выходе через паузу)
[System.Serializable]
public class SaveAchievementsRequest
{
    [JsonProperty("districtId")]
    public int DistrictId { get; set; }

    /// <summary>
    /// Прогресс достижений для различных типов условий.
    /// Ключ - тип условия (например, "max_floor", "perfect_streak"),
    /// Значение - текущее значение прогресса.
    /// </summary>
    [JsonProperty("achievementProgresses")]
    public Dictionary<string, int> AchievementProgresses { get; set; } = new();

    /// <summary>
    /// ID достижений, которые были выполнены в этой сессии.
    /// </summary>
    [JsonProperty("newlyUnlockedIds")]
    public List<int> NewlyUnlockedIds { get; set; } = new();
    /// <summary>
    /// Использованные бонусы за игровую сессию (bonusId -> количество использований).
    /// </summary>
    [JsonProperty("usedBonuses")]
    public Dictionary<int, int> UsedBonuses { get; set; } = new();
}
