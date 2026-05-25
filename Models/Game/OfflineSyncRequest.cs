using Newtonsoft.Json;

namespace GameAPI.Models.Game
{
    /// <summary>
    /// Модель запроса для синхронизации отложенных офлайн-запросов.
    /// Отправляется клиентом при восстановлении соединения.
    /// </summary>
    [System.Serializable]
    public class OfflineSyncRequest
    {
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("jsonBody")]
        public string JsonBody { get; set; }

        [JsonProperty("requestType")]
        public string RequestType { get; set; }
    }
}
