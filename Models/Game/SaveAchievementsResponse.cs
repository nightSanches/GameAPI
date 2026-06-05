using Newtonsoft.Json;
using System.Collections.Generic;
using static GameAPI.Models.Authentification.FullLoginResponse;

/// ответ сервера после сохранения прогресса достижений
[System.Serializable]
public class SaveAchievementsResponse
{
    [JsonProperty("achievements")]
    public List<UserAchievementDto> Achievements { get; set; }

    [JsonProperty("reputation")]
    public int Reputation { get; set; }
    /// <summary>
    /// Обновлённый список бонусов пользователя (после списания использованных).
    /// </summary>
    [JsonProperty("bonuses")]
    public List<UserBonusDto> Bonuses { get; set; }
}
