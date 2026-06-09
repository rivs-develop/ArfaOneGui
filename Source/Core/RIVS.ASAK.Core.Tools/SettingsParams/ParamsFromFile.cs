using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace RIVS.ASAK.Core.Tools.SettingsParams
{
    /// <summary>
    /// Чтение и сохранение параметров в конфигурационном xml-файле 
    /// </summary>
    class ParamsFromFile : IParamsFromStore
    {
        #region Constants

        public const string RIVSASAKConfigRoot = "RIVSASAKConfig";
        public const string Separator = "\\"; 

        /// <summary>
        /// Имя файла конфигурации
        /// </summary>
        public const string ConfigFileName = "RIVSASAK.config";

        /// <summary>
        /// Путь к папке с данными пользователей
        /// </summary>
        public const string RIVSASAKBehaviour = /*RIVSASAKConfigRoot + Separator + */"Behaviour";

        /// <summary>
        /// Путь к папке с отдадочными параметрами
        /// </summary>
        public const string RIVSASAKDebug = /*RIVSASAKConfigRoot + Separator + */"Debug";

        #endregion

        // локер
        private readonly ReaderWriterLockSlim _fileLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// Файл параметров
        /// </summary>
        private XElement _cfgRoot;

        //мгновенное перечитывание файла конфигурации во время его обновления мешает обновлению инфрмаци в файле - 
        //при записи файл сначала усекается, а топом в него попанадю данные.
        //событие же стандартного FileSystemWatcher срабатывает уже после усечения, и если сразу за этим начнется чтение, то та запись, которую мы запустили
        //из какого-то другого места, обломится, а мы тут получим значения по-умолчанию.
        //если же обрабатывать это событие чуть погодя, то такая коллизия не произойдет.
        //для этого и используется FileSystemWatcher
        private ConfigFileWatcher _fileWatcher;


        public ParamsFromFile()
        {
            LoadFile();

            // если прочитать не удалось - файла нет на диске - создаем новый
            if (_cfgRoot == null)
            {
                _cfgRoot = new XElement(RIVSASAKConfigRoot);
            }

            _fileWatcher = new ConfigFileWatcher();
            _fileWatcher.Changed += OnFileChanged;
        }

        void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            LoadFile();
        }


        internal void LoadFile()
        {
            _fileLock.EnterWriteLock();
            try
            {
                 // пробуем прочитать несколько раз, т.к. файл может быть занят по причине его чтения другим процессом
                const int loadAttemptsCount = 5;
                for (int i = 0; i < loadAttemptsCount; i++)
                {
                    try
                    {
                        _cfgRoot = XElement.Load(Path.Combine(RIVSASAKParams.RIVSASAKExecutionPath, ConfigFileName));
                    }
                    catch (Exception ex)
                    {
                        if (i > 1)
                        {
                            //Logger.WarnException("Не смогли прочитать файл RIVSASAK.config со второй и более попытки.", ex);
                            //Logger.Warn(ex, "Не смогли прочитать файл RIVSASAK.config со второй и более попытки.", ex);
                        }
                        Thread.Sleep(50);
                    }
                }
            }
            finally
            {
                _fileLock.ExitWriteLock();
            }
        }

        #region IParamsFromStore

        /// <summary>
        /// Получение значение ключа.
        /// </summary>
        /// <param name="valueName">Имя значения ключа.</param>
        /// <returns>Возвращает значение или null, если ключ не найден.</returns>
        public string GetParameter(string valueName)
        {
            return GetValue(RIVSASAKBehaviour, valueName);
        }

        /// <summary>
        /// Получение значение ключа, используемого при отладке.
        /// </summary>
        /// <param name="valueName">Имя значения ключа.</param>
        /// <returns>Возвращает значение или null, если ключ не найден.</returns>
        public string GetDebugParameter(string valueName)
        {
            return GetValue(RIVSASAKDebug, valueName);
        }

        /// <summary>
        /// Задание значение ключа.
        /// </summary>
        /// <param name="valueName">Имя значения ключа.</param>
        /// <param name="value">Значение ключа</param>
        public void SetParameter(string valueName, object value)
        {
            SetValue(RIVSASAKBehaviour, valueName, value);
        }

        /// <summary>
        /// Задание значение отладочного параметра.
        /// </summary>
        /// <param name="valueName">Имя значения параметра.</param>
        /// <param name="value">Значение параметра</param>
        public void SetDebugParameter(string valueName, object value)
        {
            SetValue(RIVSASAKDebug, valueName, value);
        }

        #endregion

        #region Private methods

        //static private readonly NLog.Logger Logger = NLog.LogManager.GetLogger("swswsw");

        /// <summary>
        /// Получение значение ключа.
        /// </summary>
        /// <param name="key">Имя ключа.</param>
        /// <param name="valueName">Имя значения ключа.</param>
        /// <returns>Возвращает значение или null, если ключ не найден.</returns>
        private string GetValue(string key, string valueName)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(valueName))
                throw new ArgumentNullException("valueName");

            _fileLock.EnterReadLock();

            try
            {
                var xpath = key + Separator +valueName;
                var elements = xpath.Split(Separator[0]);
                var node = _cfgRoot;
                foreach (var element in elements)
                {
                    node = node.Elements(element).FirstOrDefault();
                    if (node == null)
                    {
                        return null;
                    }
                }
                return node.Value;
            }
            finally
            {
                _fileLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Установить значение ключа
        /// </summary>
        /// <param name="key">Имя ключа.</param>
        /// <param name="valueName">Имя значения ключа.</param>
        /// <param name="value">Значение.</param>
        private void SetValue(string key, string valueName, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (string.IsNullOrEmpty(valueName))
                throw new ArgumentNullException("valueName");
            if (value == null)
                throw new ArgumentNullException("value");

            _fileLock.EnterWriteLock();

            try
            {
                var xpath = key + Separator + valueName;
                var elements = xpath.Split(Separator[0]);
                var node = _cfgRoot;
                foreach (var element in elements)
                {
                    var nextNode = node.Elements(element).FirstOrDefault();
                    if (nextNode == null)
                    {
                        nextNode = new XElement(element);
                        node.Add(nextNode);
                    }
                    node = nextNode;
                }
                node.Value = value.ToString();

                _cfgRoot.Save(Path.Combine(RIVSASAKParams.RIVSASAKExecutionPath, ConfigFileName));
            }
            finally
            {
                _fileLock.ExitWriteLock();
            }
            
        }
      
        #endregion
    }
}