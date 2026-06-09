using Microsoft.Extensions.Configuration;
using RIVS.ASAK.ARFA.Application;
using RIVS.ASAK.Core.Contract;
using RIVS.ASAK.Core.Contract.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using System.Xml.Linq;
using RIVS.ASAK.Core.Contract.Enums;
using RIVS.ASAK.Core.Tools;
using RIVS.ASAK.Core.Tools.Serialization;

namespace RIVS.ASAK.ARFA.GUI.Services
{
    public sealed class ArfaConfigurationResolver : IArfaConfigurationResolver
    {
        // логер
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public ArfaConfigurationResolver(ILoggerFactory loggerFactory,
            IConfiguration connfig)
        {
            _logger = loggerFactory.Create(GetType());
            _configuration = connfig;
            _elements = new List<ElementDTO>();

            //_repers = new List<ReperDTO>();
            _lastIterationRepers = new List<ReperDTO>();

            _repers1DataQueue = new RepersDataQueue(RepersQueueLengthLimit);
            _repers2DataQueue = new RepersDataQueue(RepersQueueLengthLimit);

            _isUseAp = true;
        }

        //public static ArfaConfigurationResolver Instance = new ArfaConfigurationResolver();

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public Guid UserId { get; set; }

        // Держим не более 2 последних данных по реперам
        const int RepersQueueLengthLimit = 2;
        readonly RepersDataQueue _repers1DataQueue;
        readonly RepersDataQueue _repers2DataQueue;


        public string Apfile { get; set; }

        public string Festa { get; set; }

        public string Opc { get; set; }

        public bool Mode { get; set; }

        public string Port { get; set; }

        public string Ip { get; set; }

        public string ArfaNumber { get; set; }

        public int CuvetteCount { get; set; }

        public bool IsNeedSaveRepersSpectrum { get; set; }
        public bool IsNeedSaveMeasSpectrum { get; set; }

        /// <summary>
        /// Данные из таблицы Elements (арфа содержит один экземпляр элементс - арфа1 - elements1 и т.д.)
        /// </summary>
        private IEnumerable<ElementDTO> _elements;

        /// <summary>
        /// Данные из таблицы Reper
        /// </summary>
        //private static IEnumerable<ReperDTO> _repers;

        /// <summary>
        /// Данные реперов, полученные на последней итерации
        /// </summary>
        private IEnumerable<ReperDTO> _lastIterationRepers;

        /// <summary>
        /// Данные по аналитической программе
        /// первый раз мы получаем при инициализации
        /// </summary>
        private AnalyticProgramDTO _analyticProgram;

        private XDocument _docAnalyticProgram;

        private XElement _arfaDeviceManagerConfiguration;

        private XElement _spectrBlockDeviceManagerConfiguration;

        private bool _isUseAp;

        //private ARFAInitInfo()
        //{
        //    _elements = new List<ElementDTO>();
        //    _repers = new List<ReperDTO>();
        //    _lastIterationRepers = new List<ReperDTO>();
        //}

        //public SubjectSystemType GetSubjectSystemTypeByArfaNumber()
        //{
        //    Debug.Assert(!string.IsNullOrEmpty(ArfaNumber));
        //    switch (ArfaNumber)
        //    {
        //        case "1":
        //        {
        //            return SubjectSystemType.Arfa1;
        //        }
        //        case "2":
        //        {
        //            return SubjectSystemType.Arfa2;
        //        }
        //        case "3":
        //        {
        //            return SubjectSystemType.Arfa3;
        //        }
        //        case "4":
        //        {
        //            return SubjectSystemType.Arfa4;
        //        }
        //        default:
        //        {
        //            throw new ArgumentException("unknown parameter name");
        //        }
        //    }
        //}

        public void SetUserData(string login, Guid roleId, Guid userId)
        {
            Login = login;
            RoleId = roleId;
            UserId = userId;
        }

        public void SetElements(IEnumerable<ElementDTO> elements)
        {
            var lst = elements.Select(element => new ElementDTO()
            {
                Id = element.Id,
                Name = element.Name,
                Start = element.Start,
                Stop = element.Stop,
                R01 = element.R01,
                R02 = element.R02,
                Line = element.Line,
                Flag = element.Flag,
                CeN = element.CeN
            })
                .ToList();

            _elements = lst;
        }

        public ElementDTO GetElementsById(int id)
        {
            return _elements.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Добавление реперов, полученных из БД во время начальной инициализации
        /// </summary>
        /// <param name="initRepersDto"></param>
        public void SetRepersFromDb(IEnumerable<ReperDTO> initRepersDto)
        {
            if (initRepersDto == null)
            {
                //тут надо сообщить, что в БД не найдены данные по реперам
                return;
            }
            var lst = initRepersDto
                .Where(x => x != null)
                .Select(reper => new ReperDataItem
                {
                    MeasDt = reper.Meas_DT,
                    Pribor = reper.Pribor,
                    Cuvet = reper.Cuvet,
                    ReperHashKey = reper.ReperHashKey,
                    ElementJaItems = reper.ElementJas?.Select(CreateElementJaItem),
                })
                .ToList();

            if (!lst.Any())
            {
                return;
            }

            foreach (var item in lst)
            {
                AddReperDataItem(item);
            }

        }

        private static ElementJaItem CreateElementJaItem(ElementJaDTO elementJaDto)
        {
            return elementJaDto != null ?
                new ElementJaItem() { Id = elementJaDto.Id, Name = elementJaDto.Name, Ja = elementJaDto.Ja }
                : /*null*/ new ElementJaItem() { Id = 0, Name = string.Empty, Ja = 0.0f };
        }

        public float GetJaFromRepers(int id, int cuvetNumber)
        {
            float result = 0.0f;
            ReperDataItem reperDataItem = null;
            if (cuvetNumber == 1)
            {
                reperDataItem = _repers1DataQueue.GetLastItem();
            }
            else if (cuvetNumber == 2)
            {
                reperDataItem = _repers2DataQueue.GetLastItem();
            }
            else
            {
                return result;
            }

            if (reperDataItem == null || reperDataItem.ElementJaItems == null)
            {
                return result;
            }

            foreach (var element in reperDataItem.ElementJaItems)
            {
                if (element.Id != id)
                {
                    continue;
                }

                result = element.Ja;
                break;
            }

            return result;
        }

        public int GetArfaNumber()
        {
            return Convert.ToInt32(ArfaNumber);
        }

        public int GetCuvetteCount()
        {
            return CuvetteCount;
        }

        public int GetAnalyticProgramId()
        {
            return _analyticProgram == null ? 0 : _analyticProgram.Id;
        }

        /// <summary>
        /// Добавление реперов, полученных из БД во время начальной инициализации
        /// </summary>
        /// <param name="initRepers"></param>
        public void SetRepers(IEnumerable<ReperDTO> initRepers)
        {
            _lastIterationRepers = initRepers;
        }


        public void SetConfigData(bool mode, string festa, string opc,
            string arfaNumber, int cuvetteCount, string ip, string port,
            bool isNeedSaveRepersSpectrum, bool isNeedSaveMeasSpectrum)
        {
            Mode = mode;
            Festa = festa;
            Opc = opc;
            ArfaNumber = arfaNumber;
            Ip = ip;
            Port = port;
            CuvetteCount = cuvetteCount;
            IsNeedSaveRepersSpectrum = isNeedSaveRepersSpectrum;
            IsNeedSaveMeasSpectrum = isNeedSaveMeasSpectrum;
        }


        public void SetAnalyticProgram(AnalyticProgramDTO analyticProgramDto)
        {
            _analyticProgram = analyticProgramDto;

            if (_analyticProgram != null && !string.IsNullOrEmpty(_analyticProgram.XML_TXT))
            {
                _docAnalyticProgram = null;
                try
                {
                    _docAnalyticProgram = JsonConvert.DeserializeXNode(_analyticProgram.XML_TXT);
                    //Encoding sourceTextFormat = Encoding.UTF8;
                    //_docAnalyticProgram = XDocument.Load(new MemoryStream(sourceTextFormat.GetBytes(_analyticProgram.XML_TXT.Replace(@"\", @"\\"))));
                }
                catch (Exception ex)
                {
                    _logger?.Error("ArfaConfigurationResolver.SetAnalyticProgram Exception ", ex);
                }

            }
            //TEST
            //var deviceNumber = GetArfaNumber();
            //var cuvet = 9;
            //var prodN = -1;
            //foreach (XElement prod in _docAnalyticProgram.Elements("Project").Elements("Products").Elements("Product"))
            //{
            //    //System.Windows.MessageBox.Show("pribor N = "+prod.Attribute("pribor").ToString()+"  prod="+prod.ToString());

            //    if (deviceNumber == Convert.ToInt32(prod.Attribute("pribor").Value))
            //    {
            //        // test= Convert.ToInt32(prod.Attribute("kuvet").Value);
            //        // System.Windows.MessageBox.Show("kuvet =" + test.ToString()+" Cuvet="+(Cuvet-2).ToString());
            //        if ((cuvet - 2) == Convert.ToInt32(prod.Attribute("kuvet").Value))
            //        {
            //            //var trueAp = true;
            //            prodN = Convert.ToInt32(prod.Attribute("number").Value);
            //            //idAp = Convert.ToInt32(prod.Attribute("idAP").Value);
            //        }
            //    }
            //}

        }


        public XDocument GetAnalyticProgram()
        {
            return _docAnalyticProgram;
        }

        public IEnumerable<int> GetUsedCuvettesFromAnalyticProgram()
        {
            var lst = new List<int>();
            if (_docAnalyticProgram == null)
            {
                return lst;
            }

            foreach (var prod in _docAnalyticProgram.Elements("Project").Elements("Products").Elements("Product"))
            {
                if (Convert.ToInt16(prod.Attribute("pribor").Value) == GetArfaNumber())
                {
                    lst.Add(Convert.ToInt16(prod.Attribute("kuvet").Value));
                }
            }

            return lst;
        }

        /// <summary>
        /// Получение данных из АП для покраски участков спектра с указанием элементов
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ArfaGraphElement> GetGraphElementFromAnalyticProgram()
        {
            var lst = new List<ArfaGraphElement>();
            if (_docAnalyticProgram == null)
            {
                return lst;
            }

            foreach (var elem in _docAnalyticProgram.Elements("Project").Elements("Channels").Elements("Channel"))
            {
                if (elem != null && elem.HasAttributes && elem.Attribute("atn") != null)
                {
                    int elemId = Convert.ToInt32(elem.Attribute("atn").Value);
                    var element = GetElementsById(elemId);
                    string line = element.Line;
                    if (element.Line == "Ka")
                    {
                        line = ("Kα");
                    }

                    if (line == "La")
                    {
                        line = ("Lα");
                    }

                    var arfaGraphElement = new ArfaGraphElement(element.Id, element.Name, element.Start, element.Stop, line);
                    lst.Add(arfaGraphElement);
                }
            }

            return lst;
        }

        public (string, DateTime) GetAnalyticProgramInfo()
        {
            if (_docAnalyticProgram == null)
            {
                return ("Не найдена АП", DateTime.Now);
            }

            var name = _docAnalyticProgram.Element("Project").Attribute("name").Value;
            var activedTime = _analyticProgram.ActivedDT;

            return (name, activedTime);
        }

        public void AddReperDataItem(ReperDataItem reperDataItem)
        {
            if (reperDataItem.Cuvet == 1)
            {
                _repers1DataQueue.Enqueue(reperDataItem);
            }
            else if (reperDataItem.Cuvet == 2)
            {
                _repers2DataQueue.Enqueue(reperDataItem);
            }
        }

        /// <summary>
        /// Получаем последний добавленный репер
        /// в данном случае кювета не важна
        /// </summary>
        /// <returns></returns>
        public int GetLastReperHashKey()
        {
            var itemReper1 = _repers1DataQueue.GetLastItem();
            var itemReper2 = _repers2DataQueue.GetLastItem();
            if (itemReper1 != null && itemReper2 != null)
            {
                return itemReper2.MeasDt > itemReper1.MeasDt ? itemReper2.ReperHashKey : itemReper1.ReperHashKey;
            }

            if (itemReper1 == null && itemReper2 == null)
            {
                return 0;
            }

            if (itemReper1 == null && itemReper2 != null)
            {
                return itemReper2.ReperHashKey;
            }

            if (itemReper1 != null && itemReper2 == null)
            {
                return itemReper1.ReperHashKey;
            }

            return 0;
        }

        public bool GetIsNeedSaveRepersSpectrum()
        {
            return IsNeedSaveRepersSpectrum;
        }

        public bool GetIsNeedSaveMeasSpectrum()
        {
            return IsNeedSaveMeasSpectrum;
        }

        public XElement GetDeviceManagerConfiguration()
        {
            return _arfaDeviceManagerConfiguration;
        }

        public XElement GetSpectrBlockDeviceManagerConfiguration()
        {
            return _spectrBlockDeviceManagerConfiguration;
        }

        public bool IsUseAP
        {
            get { return _isUseAp; }
            set { _isUseAp = value; }
        }

        /// <summary>
        /// Создание конфигурации
        /// </summary>
        public void CreateArfaDeviceManagerConfiguration()
        {
            var deviceSection = _configuration.GetSection("devices");

            //конфига для плюсового DeviceManager
            var spectrElements = new List<XElement>();

            //конфига для шарпового DeviceManager
            var arfaElements = new List<XElement>();

            foreach (var item in deviceSection.GetChildren())
            {
                var configSectionsDictionary = item.GetChildren().ToDictionary(x => x.Key, x => x.Value);
                if (!configSectionsDictionary.ContainsKey("DeviceType"))
                {
                    continue;
                }

                IDeviceOptions deviceOptions = null;
                ArfaDeviceType arfaDeviceType = 0;
                var strDeviceType = configSectionsDictionary["DeviceType"];
                if (!string.IsNullOrEmpty(strDeviceType) && Enum.IsDefined(typeof(ArfaDeviceType), strDeviceType))
                {
                    arfaDeviceType = (ArfaDeviceType)Enum.Parse(typeof(ArfaDeviceType), strDeviceType);
                }

                if (arfaDeviceType == ArfaDeviceType.Tube)
                {
                    deviceOptions = item.Get<TubeDeviceOptions>();
                    if (deviceOptions == null)
                    {
                        continue;
                    }
                    try
                    {
                        var xElement = SerializeToXml(deviceOptions);
                        if (xElement != null)
                        {
                            spectrElements.Add(xElement);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Logger.ErrorException("Ошибка при создании конфигурации компоненты '" + deviceOptions + "'", ex);
                    }
                }
                else if (arfaDeviceType == ArfaDeviceType.Detector)
                {
                    deviceOptions = item.Get<DetectorDeviceOptions>();
                    if (deviceOptions == null)
                    {
                        continue;
                    }
                    try
                    {
                        var xElement = SerializeToXml(deviceOptions);
                        if (xElement != null)
                        {
                            //ВАЖНО!!!
                            // старый детектор действительно добавлялся в шарповый DeviceManager
                            //arfaElements.Add(xElement);
                            spectrElements.Add(xElement);

                        }
                    }
                    catch (Exception ex)
                    {
                        //Logger.ErrorException("Ошибка при создании конфигурации компоненты '" + deviceOptions + "'", ex);
                    }
                }
                else if (arfaDeviceType == ArfaDeviceType.Motor)
                {
                    deviceOptions = item.Get<MotorDeviceOptions>();
                    if (deviceOptions == null)
                    {
                        continue;
                    }
                    try
                    {
                        var xElement = SerializeToXml(deviceOptions);
                        if (xElement != null)
                        {
                            //ВАЖНО!!!
                            // старый привод Festo добавлялся в шарповый DeviceManager
                            arfaElements.Add(xElement);
                            //spectrElements.Add(xElement);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Logger.ErrorException("Ошибка при создании конфигурации компоненты '" + deviceOptions + "'", ex);
                    }
                }

            }

            _spectrBlockDeviceManagerConfiguration = new XElement(ArfaDeviceManagerNames.Elements.Root,
                new XElement(CommonNames.Elements.Commands,
                    new XElement(CommonNames.Elements.CommandCreate, spectrElements.Select(el => el)),
                    new XElement(CommonNames.Elements.CommandUpdate),
                    new XElement(CommonNames.Elements.CommandDestroy)
                ));

            //для шарпового DeviceManager
            _arfaDeviceManagerConfiguration = new XElement(ArfaDeviceManagerNames.Elements.Root,
                new XElement(CommonNames.Elements.Commands,
                    new XElement(CommonNames.Elements.CommandCreate, arfaElements.Select(el => el)),
                    new XElement(CommonNames.Elements.CommandUpdate),
                    new XElement(CommonNames.Elements.CommandDestroy)
                ));

            _logger.Trace($"ArfaDeviceManagerConfiguration\n: {_arfaDeviceManagerConfiguration}");
            _logger.Trace($"SpectrBlockDeviceManagerConfiguration\n: {_spectrBlockDeviceManagerConfiguration}");
        }

        private static XElement SerializeToXml(IDeviceOptions deviceOptions)
        {
            if (deviceOptions is TubeDeviceOptions)
            {
                return ArfaEntitySerializer.SerializeToXml(deviceOptions as TubeDeviceOptions);
            }

            if (deviceOptions is DetectorDeviceOptions)
            {
                return ArfaEntitySerializer.SerializeToXml(deviceOptions as DetectorDeviceOptions);
            }

            if (deviceOptions is MotorDeviceOptions)
            {
                return ArfaEntitySerializer.SerializeToXml(deviceOptions as MotorDeviceOptions);
            }

            Debug.Assert(false, "Сущность " + deviceOptions + " не используется в ArfaDeviceManager");
            return null;
        }
    }
}
