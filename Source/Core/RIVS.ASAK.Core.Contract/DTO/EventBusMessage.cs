using System;
using RIVS.ASAK.Core.Contract.Enums;
using RIVS.ASAK.Core.Contract.Values;

namespace RIVS.ASAK.Core.Contract.DTO
{
    /// <summary>
    /// Класс события представляет собой описание события, отправляемого по шине эвентов
    /// </summary>
    [Serializable]
    public class EventBusMessage
    {
        /// <summary>
        /// Время наступления события.
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Категория события. (ошибка, информация, тревога и т.п.)
        /// </summary>
        public EventCategoryType Category { get; set; }

        /// <summary>
        /// Тип события.
        /// Идентификатор однозначно идентифицирует тип события.
        /// </summary>
        public Guid EventType { get; private set; }

        /// <summary>
        /// Субъект события.
        /// Сущность системы, к которой относится сообщение (часть АСАК)
        /// </summary>
        public Guid SubjectType { get; private set; }

        /// <summary>
        /// Подтип субъекта события.
        /// Тип сущности системы, к которой относится сообщение.
        /// Например - АРФА субъект события, а Детектор - подтип этого субъекта.
        /// </summary>
        public Guid SubjectSubType { get; set; }

        /// <summary>
        /// Идентификатор сервиса.
        /// Например, необходим для определения exception в сервисе.
        /// </summary>
        public Guid ServiceId { get; set; }

        /// <summary>
        /// Имя субъекта события.
        /// Фактически замена полям SubjectSubType и ServiceId.
        /// </summary>
        /// <remarks>
        /// Иногда проще задать имя сущности, чем тип сущности
        /// </remarks>
        public string SubjectSubName { get; set; }

        /// <summary>
        /// Параметры события, которые используются при помещении события в БД событий.  
        /// Дополнительные, связанные с событием идентификаторы сущностей, строковые или числовые значения 
        /// </summary>
        public ParamValueCollection EventParams = new ParamValueCollection();

        /// <summary>
        /// Дополнительные данные события.
        /// Используются при обмене событиями между компонентами - в БД не пишется!
        /// </summary>
        public object ExtData { get; private set; }

        
        /// <summary>
        /// Флаг публикации события в БД событий
        /// Если он не выставлен, то событие не попадает в БД и используется только для оповещения компонент системы
        /// </summary>
        public bool PublishToDataBase { get; private set; }


        /// <summary>
        /// Конструктор класса с заданием обязательных параметров
        /// </summary>
        /// <param name="time"></param>
        /// <param name="category">Категория события (информация, ошибка)</param>
        /// <param name="eventType">Тип события</param>
        /// <param name="subjectType">Субъект события</param>
        /// <param name="extData">Дополнительные данные события</param>
        /// <param name="paramValue">Параметры события</param>
        //public EventBusMessage(
        //    DateTime time,
        //    EventCategoryType category,
        //    Guid eventType,
        //    Guid subjectType,
        //    ParamValue paramValue = null,
        //    object extData = null)
        //    : this(time, category, eventType, subjectType, Guid.Empty, Guid.Empty, paramValue != null ? new List<ParamValue> { paramValue } : null, string.Empty, extData)
        //{
        //}

        /// <summary>
        /// Конструктор класса с заданием обязательных параметров
        /// </summary>
        /// <param name="time"></param>
        /// <param name="category">Категория события (информация, ошибка)</param>
        /// <param name="eventType">Тип события</param>
        /// <param name="subjectType">Субъект события (носитель действия)</param>
        /// <param name="paramsList">Список параметров события</param>
        /// <param name="extData">Дополнительные данные события</param>
        //public EventBusMessage(
        //    DateTime time,
        //    EventCategoryType category,
        //    Guid eventType,
        //    Guid subjectType,
        //    IEnumerable<ParamValue> paramsList,
        //    object extData = null)
        //    : this(time, category, eventType, subjectType, Guid.Empty, Guid.Empty, paramsList, string.Empty, extData)
        //{
        //}


        /// <summary>
        /// Конструктор класса с заданием обязательных параметров.
        /// Задаются все параметры, включая флаг записи события в БД.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="category">Категория события (информация, ошибка)</param>
        /// <param name="eventType">Тип события</param>
        /// <param name="subjectType">Субъект события (носитель действия)</param>
        /// <param name="subjectSubType">Подтип субъект события</param>
        /// <param name="serviceId">Идентификатор сервиса</param>
        /// <param name="subjectSubName">Имя субъекта события</param>
        /// <param name="extData">Дополнительные данные события</param>
        /// <param name="paramsList">Список параметров события. Может быть null</param>
        /// <param name="publishToDataBase">Публикация в базе событий</param>
        //[JsonConstructor]
        //public EventBusMessage(
        //    DateTime time,
        //    EventCategoryType category, 
        //    Guid eventType,
        //    Guid subjectType,
        //    Guid subjectSubType,
        //    Guid serviceId,
        //    IEnumerable<ParamValue> paramsList,
        //    string subjectSubName,
        //    object extData,
        //    bool publishToDataBase = true)
        //{
        //    Time = time;
        //    Category = category;
        //    EventType = eventType;
        //    SubjectType = subjectType;
        //    SubjectSubType = subjectSubType;
        //    ServiceId = serviceId;
        //    SubjectSubName = subjectSubName;
        //    ExtData = extData;
        //    if (paramsList != null)
        //    {
        //        EventParams = new ParamValueCollection(paramsList);
        //    }
        //    PublishToDataBase = publishToDataBase;
        //}

        public CustomValue this[string name]
        {
            get { return GetParam(name)?.Value; }
            set { SetParam(name, value); }
        }

        public ParamValue GetParam(string name)
        {
            return EventParams[name];
        }

        public void SetParam(string name, CustomValue value)
        {
            EventParams.Set(new ParamValue(name, value));
        }

        public override string ToString()
        {
            return $"Event: category: {Category} type: {EventType} subject: {SubjectType}/{SubjectSubType}, data: {ExtData}, time: {Time.ToString("dd.MM.yyyy hh:mm:ss.ffff")}";
        }


        //public static string ToJson(EventBusMessage data)
        //{
        //    return data == null ? "" : JsonConvert.SerializeObject(data, new CustomJsonCustomValueConverter());
        //}


        //public static EventBusMessage FromJson(string text)
        //{
        //    return string.IsNullOrEmpty(text) 
        //        ? null 
        //        : JsonConvert.DeserializeObject<EventBusMessage>(text, new CustomJsonCustomValueConverter());
        //}

    }

}


