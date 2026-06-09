using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    /// <summary>
    /// Класс типов сообщений.
    /// </summary>
    public static class EventTypes
    {

        #region 10хххх - Общие сообщения (зарезервировано)

        #endregion 10хххх - Общие сообщения (зарезервировано)

        #region 11хххх - Сообщения конфигурации

        /// <summary>
        /// (Код: 110001) Добавлено устройство в конфигурацию
        /// </summary>
        public static Guid DeviceAdded = new Guid("9070DDB2-B8F5-488C-B129-E8CCA88E5005");

        /// <summary>
        /// (Код: 110002) Удалено устройство из конфигурации
        /// </summary>
        public static Guid DeviceDeleted = new Guid("EF8A1C80-56B1-4A5D-8131-36A2416CF26E");

        /// <summary>
        /// (Код: 110003) Изменены свойства устройства
        /// </summary>
        public static Guid DevicePropertyChanged = new Guid("37D1472D-2984-4CA1-9612-8989A851E805");

        /// <summary>
        /// (Код: 110004) Добавлена циклограмма в конфигурацию
        /// </summary>
        public static Guid CyclogramAdded = new Guid("0A46631A-19CC-41AF-822D-CA6E34B7AEBA");

        /// <summary>
        /// (Код: 110005) Удалена циклограмма из конфигурации
        /// </summary>
        public static Guid CyclogramDeleted = new Guid("78D95B07-EC27-4232-AC13-F60DA81BACC1");

        /// <summary>
        /// (Код: 110006) Изменены свойства циклограммы
        /// </summary>
        public static Guid CyclogramPropertyChanged = new Guid("36F925D8-E07A-4EC3-AB8B-4B9069CD8F67");

        #endregion 11хххх - Сообщения конфигурации

        #region 12хххх - Cообщения от устройств

        /// <summary>
        /// (Код: 120001) Успешный старт работы драйвера привода
        /// </summary>
        public static Guid MotorStartSuccessful = new Guid("C6F5F7C7-9FFE-49BA-A98C-7D032728B97D");

        /// <summary>
        /// (Код: 120002) Успешный стоп работы драйвера привода
        /// </summary>
        public static Guid MotorStopSuccessful = new Guid("A67D1129-EB93-4711-9FF4-AC41361EA9B0");

        /// <summary>
        /// (Код: 120003) Начало отработки команды приводу на перемещение к кювете
        /// </summary>
        public static Guid MotorMoveToTablePositionStart = new Guid("9334F9A2-6930-4DF2-A487-B50D583CBBF5");

        /// <summary>
        /// (Код: 120004) Завершение отработки команды приводу на перемещение к кювете
        /// </summary>
        public static Guid MotorMoveToTablePositionFinish = new Guid("19B001DB-0526-4728-8515-B734162C71F1");

        /// <summary>
        /// (Код: 120005) Начало отработки команды приводу GoHome
        /// </summary>
        public static Guid MotorGoHomeStart = new Guid("9BD309AD-FC00-4D61-AB91-C4E82DFD92BE");

        /// <summary>
        /// (Код: 120006) Завершение отработки команды приводу GoHome
        /// </summary>
        public static Guid MotorGoHomeFinish = new Guid("5923E6E3-A498-409F-AF72-52B5FCBF3DCC");




        public static Guid DppStartSuccessful = new Guid("0ACC2062-8282-4043-B1CD-17E7896C2710");

        public static Guid DppStopSuccessful = new Guid("C8004901-4C1B-4B71-878D-F2D926A474EB");

        //пока статус измерения не определяется
        //public static Guid DppMeasureSuccessful = new Guid("305BD5E2-8CF9-4A30-992D-43489737B1E0");

        public static Guid DppMeasureFinished = new Guid("1F465B65-627E-45BE-82E2-E46BEBFFBED4");

        //public static Guid DppGetDeviceStateSuccessful = new Guid("809D7126-02E5-4DB5-A9DF-750CFC7ABA90");


        public static Guid XRayStartSuccessful = new Guid("49617AEF-A873-415F-81B4-F965D3E4C605");

        public static Guid XRayStartFail = new Guid("1C0999E3-D286-49D7-9C38-D13F13569DAB");


        public static Guid XRayStopSuccessful = new Guid("7C5C329F-75BA-4713-808C-5F349CC6053F");

        //public static Guid XRayGetDeviceStateSuccessful = new Guid("C680C79B-8AD5-430D-85CD-AE2965D8C7BB");


        #endregion 12хххх - Cообщения от устройств

        #region 50ххх – Общие ошибки

        /// <summary>
        /// (Код: 50001) Неизвестная ошибка
        /// </summary>
        public static Guid Error = new Guid("A4EB4D35-4D16-4D5F-9494-1BF7FC35D202");

        #endregion 50ххх – Общие ошибки

        #region 501хх – Ошибки привода

        /// <summary>
        /// (Код: 50101) Ошибка старта привода
        /// </summary>
        public static Guid MotorStartError = new Guid("13F718FE-D5D1-437C-9F5E-F72480F75CA9");

        /// <summary>
        /// (Код: 50102) Ошибка стопа привода
        /// </summary>
        public static Guid MotorStopError = new Guid("48AF72C5-A390-4B5E-A4B6-6B388D1D0385");

        /// <summary>
        /// (Код: 50103) Ошибка коннекта привода
        /// </summary>
        public static Guid MotorStartConnectError = new Guid("83F31283-F38F-4813-9DA6-E8CD5FFF78A8");

        /// <summary>
        /// (Код: 50104) Ошибка при подготовке контроллера привода к работе
        /// </summary>
        public static Guid MotorStartPrepareError = new Guid("129313A5-EE99-48F2-BDEC-B98E2BB7A1B2");

        /// <summary>
        /// (Код: 50105) Ошибка при выполнении команды позиционирования привода в позицию по таблице
        /// </summary>
        public static Guid MotorMoveToTablePositionError = new Guid("5250AE2C-2841-4CA3-BB5F-2ADB37ACE8DA");

        /// <summary>
        /// (Код: 50106) Ошибка при выполнении команды GoHome
        /// </summary>
        public static Guid MotorGoHomeError = new Guid("5250AE2C-2841-4CA3-BB5F-2ADB37ACE8DA");

        #endregion 501хх – Ошибки привода

        #region 502хх – Ошибки DPP

        /// <summary>
        /// (Код: 50201) Ошибка старта DPP
        /// </summary>
        public static Guid DppStartError = new Guid("2FEF792D-60EA-48C6-81F8-D1AA8F8C8C83");

        /// <summary>
        /// (Код: 50202) Ошибка стопа DPP
        /// </summary>
        public static Guid DppStopError = new Guid("54429CB6-6AA4-4BC2-8B40-6F2FAB7CB81D");

        /// <summary>
        /// (Код: 50203) Ошибка измерений DPP
        /// </summary>
        public static Guid DppMeasureError = new Guid("FCFF76A7-17C0-4732-9524-A69F67ADF2D3");

        /// <summary>
        /// (Код: 50204) Ошибка получение состояния DPP
        /// </summary>
        //public static Guid DppGetDeviceStateError = new Guid("0222D103-9DD2-4D13-8468-2464B48DE224");

        #endregion 502хх – Ошибки DPP

        #region 503хх – Ошибки XRay

        /// <summary>
        /// (Код: 50301) Ошибка старта XRay
        /// </summary>
        public static Guid XRayStartError = new Guid("BCC9FBD2-C555-498E-81D1-6541695D9E24");

        /// <summary>
        /// (Код: 50302) Ошибка стопа XRay
        /// </summary>
        public static Guid XRayStopError = new Guid("2D2F43BB-C35E-4B87-84B9-144F57C0F981");

        /// <summary>
        /// (Код: 50303) Ошибка получения статуса XRay
        /// </summary>
        //public static Guid XRayGetDeviceStateError = new Guid("935F93D9-95F5-4E87-A7C4-71B5ED54C1B6");

        #endregion 503хх – Ошибки XRay

        #region 51ххх – Ошибки конфигурации

        /// <summary>
        /// (Код: 51001) Ошибка в конфигурации устройства
        /// </summary>
        public static Guid DeviceConfigurationError = new Guid("9CBA1AFC-340B-4B9D-91BA-CE827A460B19");

        /// <summary>
        /// (Код: 51002) Ошибка в конфигурации цмклограммы
        /// </summary>
        public static Guid CyclogramConfigurationError = new Guid("D5D55FA0-BC8B-4E61-99B2-155BC2226A9C");

        #endregion 51ххх – Ошибки конфигурации

        //TODO потом определить категорию событий

        /// <summary>
        /// (Код: XXXXX) Изменение состояния XRay
        /// </summary>
        public static Guid XRayChangeState = new Guid("25345BE6-4343-449A-B77D-63E06C504FCF");

        /// <summary>
        /// (Код: XXXXX) Изменение состояния переменных OPC
        /// </summary>
        //public static Guid OpcChangeState = new Guid("60E8DF75-ECBA-428C-AE24-148254C86E72");

        /// <summary>
        /// (Код: XXXXX) Зафиксирована нехватка ресурсов 
        /// </summary>
        public static Guid NotEnoughResources = new Guid("AD55E2BA-7760-4827-958A-B1C38D84D41D");

        /// <summary>
        /// Новые данные измерений, для отображения в таблице интерфейса Арфы
        /// </summary>
        public static Guid AddNewMeasureDataToTable = new Guid("D4C793E6-234D-4105-8D0A-B074C410F5D3");

        /// <summary>
        /// Новые данные, для отображения на графике в UI
        /// </summary>
        public static Guid ArfaUpdateGraph = new Guid("84D06E8A-C623-4406-86DA-A3C66AA9BB30");

        /// <summary>
        /// Сообщения, для отображения в UI ARFA
        /// </summary>
        public static Guid ArfaLogMessage = new Guid("E5F9AA09-2B24-4E59-9231-34534BCCF39E");

        /// <summary>
        /// Сообщения, для отображения в UI CSU
        /// </summary>
        public static Guid CsuLogMessage = new Guid("BCF74EB3-002E-4A5D-AFA2-4F219C9B60B6");

        /// <summary>
        /// Установленно или изменено состояние сущности. 
        /// Значение композитного состояния является параметром события.
        /// </summary>
        public static Guid StateChanged = new Guid("D67BFBB2-7814-4B96-8D87-1D0F4D9E6F32");

        /// <summary>
        /// Сброшено состояние сущности.
        /// Значение композитного состояния является параметром события.
        /// </summary>
        public static Guid StateDroped = new Guid("C02CE0C1-9715-478D-8638-8AE812F2A761");

        ///<summary>
        /// Пользователь отправил команду.
        ///</summary>
        public static Guid UserCommandRequest = new Guid("55A5E78E-0DB4-4898-8D72-AC20F8CDA0F3");


        public static Guid CsuArfaOpcResult = new Guid("93779AEF-E95C-4461-9570-3E8253FB1BD6");

        public static Guid CsuAkcpOpcResult = new Guid("F90782A7-E9DA-4347-8DF5-986B555F9971");

        /// <summary>
        /// Изменилась переменная AutoMode в ОПС сервере
        /// </summary>
        public static Guid OPCAutoModeChanges = new Guid("B4E7BE02-58D7-4714-918C-4F0231697ACF");



        #region 52ххх – Данные по устройствам (ЦСУ)

        public static Guid AkpDeviceData = new Guid("05053D87-A2EA-44BD-B74E-E1C572596498");

        public static Guid AkcpDeviceData = new Guid("045EA599-9C54-46A2-AE6C-4381EE8B3327");

        public static Guid ArfaDeviceData = new Guid("AC9FBE09-0FD6-4CCB-9F48-342517FA7E46");

        #endregion

    }
}
