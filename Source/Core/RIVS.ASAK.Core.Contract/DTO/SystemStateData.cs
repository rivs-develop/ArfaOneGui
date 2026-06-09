using System;
using Newtonsoft.Json;

namespace RIVS.ASAK.Core.Contract.DTO
{
    /// <summary>
    /// Контракт данных для передачи информации о состоянии системы
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SystemStateData
    {
        [JsonProperty]
        public DateTime Timestamp { get; set; }

        [JsonProperty]
        public string Message { get; set; }

        [JsonProperty]
        public int Status { get; set; }

        [JsonProperty]
        public string Details { get; set; }
    }

    /// <summary>
    /// Интерфейс для сервиса обмена данными
    /// </summary>
    public interface IDataExchangeService
    {
        void SendData(SystemStateData data);
    }
}
