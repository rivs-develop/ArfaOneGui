using System;
using System.Globalization;
using System.Diagnostics;

namespace RIVS.ASAK.Core.Tools.SettingsParams
{

    /// <summary>
    /// Атрибут, задающий параметры для поведенческих настроек RIVSASAKConfig
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class BehaviourParamsAttribute : Attribute
    {
        public object DefaultValue;
        public string Description;
    }


    /// <summary>
    /// Параметры RIVSASAK
    /// Параметры поведения и отладочные параметры.
    /// </summary>
    public static class RIVSASAKParams
    {  
        /// <summary>
        /// Хранилище параметров в xml файле
        /// </summary>
        private static readonly ParamsFromFile FileStore = new ParamsFromFile();

        internal static void UpdateFileStore()
        {
            FileStore.LoadFile();
        }

        #region Рабочие параметры        

        /// <summary>
        /// Рабочая директория RIVSASAK
        /// </summary>
        public static string RIVSASAKExecutionPath
        {
            get
            {
                // значение по умолчанию - AppDomain.CurrentDomain.BaseDirectory
                return AppDomain.CurrentDomain.BaseDirectory;
            }           
        }


        /// <summary>
        /// Считать поведенческий флаг
        /// </summary>
        /// <param name="parameter">Имя параметра</param>
        /// <returns>Значение</returns>
        public static bool GetBehaviourBoolParameter(BehaviourParams parameter)
        {
            var result = false;
            try
            {
            var value = FileStore.GetParameter(parameter.ToString());
            // читаем дефолтные значения
            if (string.IsNullOrEmpty(value) || !bool.TryParse(value, out result))
            {
                var defValueAttr = EnumHelper.GetAttribute<BehaviourParamsAttribute, BehaviourParams>(parameter);                
                if (defValueAttr != null)
                {
                    result = (bool)defValueAttr.DefaultValue;
                }
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine("Tool: GetBehaviourBoolParameter: все идет не так как надо! {0}", exc.Message);
            }
            return result;
        }

        /// <summary>
        /// Считать поведенческий числовой параметр
        /// </summary>
        /// <param name="parameter">Имя параметра</param>
        /// <returns>Значение</returns>
        public static int GetBehaviourIntParameter(BehaviourParams parameter)
        {
            var value = FileStore.GetParameter(parameter.ToString());
            var result = 0;
            // читаем дефолтные значения
            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out result))
            {
                var defValueAttr = EnumHelper.GetAttribute<BehaviourParamsAttribute, BehaviourParams>(parameter);
                if (defValueAttr != null)
                {
                    result = (int)defValueAttr.DefaultValue;
                }
            }
            return result;
        }

        /// <summary>
        /// Считать поведенческий строковый параметр
        /// </summary>
        /// <param name="parameter">Имя параметра</param>
        /// <returns>Значение</returns>
        public static string GetBehaviourStringParameter(BehaviourParams parameter)
        {
            var value = FileStore.GetParameter(parameter.ToString());
            // читаем дефолтные значения
            if (value == null)
            {
                var defValueAttr = EnumHelper.GetAttribute<BehaviourParamsAttribute, BehaviourParams>(parameter);
                if (defValueAttr != null)
                {
                    value = (string)defValueAttr.DefaultValue;
                }
            }
            return value;
        }

        /// <summary>
        /// Установить поведенческий параметр
        /// </summary>
        /// <param name="parameter">Имя параметра</param>
        /// <param name="value">значение</param>
        public static void SetBehaviourParameter(BehaviourParams parameter, object value)
        {
            FileStore.SetParameter(parameter.ToString(), value);
        }


        #endregion
        
        #region Отладочные параметры

        ///// <summary>
        ///// Флаг процесса - ожидание Attach.
        ///// </summary>
        //public static IEnumerable<Guid> WaitForDebug
        //{
        //    get
        //    {
        //        try
        //        {
        //            var value = FileStore.GetDebugParameter(DebugParams.WaitForDebug.ToString());
        //            return value.Split('|').Select(el => new Guid(el)).ToList();
        //        }
        //        catch (Exception )
        //        {
        //            return new List<Guid>();
        //        }
        //    }
        //    set 
        //    { 
        //        FileStore.SetDebugParameter(DebugParams.WaitForDebug.ToString(), string.Join("|", value));
        //    }
        //}

        /// <summary>
        /// Флаг, показывающий необходимость пинга сетевых машин
        /// </summary>
        public static bool IsNeedTestPing
        {
            get
            {
                return GetDebugBoolParameter(DebugParams.IsNeedTestPing);
            }
            set
            {
                FileStore.SetParameter(DebugParams.IsNeedTestPing.ToString(), value);
            }
        }

      
        /// <summary>
        /// Считать отладочный флаг
        /// </summary>
        /// <param name="parameter">Имя параметра</param>
        /// <returns>Значение</returns>
        public static bool GetDebugBoolParameter(DebugParams parameter)
        {
            var value = FileStore.GetDebugParameter(parameter.ToString());
            var result = false;
            // читаем дефолтные значения
            if (string.IsNullOrEmpty(value) || !bool.TryParse(value, out result))
            {
                var defValueAttr = EnumHelper.GetAttribute<BehaviourParamsAttribute, DebugParams>(parameter);
                if (defValueAttr != null)
                {
                    try
                    {
                    result = (bool)defValueAttr.DefaultValue;
                }
                    catch (Exception)
                    {
                        result = false;
                    }
                    
                }
            }
            return result;
        }

        /// <summary>
        /// Считать числовое значение
        /// </summary>
        /// <param name="parameter">Имя параметра</param>
        /// <returns>Значение</returns>
        public static int GetDebugIntParameter(DebugParams parameter)
        {
            var value = FileStore.GetDebugParameter(parameter.ToString());
            var result = 0;
            // читаем дефолтные значения
            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out result))
            {
                var defValueAttr = EnumHelper.GetAttribute<BehaviourParamsAttribute, DebugParams>(parameter);
                if (defValueAttr != null)
                {
                    result = (int)defValueAttr.DefaultValue;
                }
            }
            return result;
        }

        /// <summary>
        /// Считать числовое значение
        /// </summary>
        /// <param name="parameter">Имя параметра</param>
        /// <returns>Значение</returns>
        public static double GetDebugDoubleParameter(DebugParams parameter)
        {
            var value = FileStore.GetDebugParameter(parameter.ToString());
            var result = 0.0;
            // читаем дефолтные значения
            if (string.IsNullOrEmpty(value) || !double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                var defValueAttr = EnumHelper.GetAttribute<BehaviourParamsAttribute, DebugParams>(parameter);
                if (defValueAttr != null)
                {
                    result = (double)defValueAttr.DefaultValue;
                }
            }
            return result;
        }

        /// <summary>
        /// Установить отладочный параметр
        /// </summary>
        /// <param name="parameter">Имя параметра</param>
        /// <param name="value">значение</param>
        public static void SetDebugParameter(DebugParams parameter, object value)
        {
            FileStore.SetDebugParameter(parameter.ToString(), value);
        }


        /// <summary>
        /// Получить значение временного отладочного параметра
        /// </summary>
        /// <param name="paramName">имя параметра</param>
        /// <returns></returns>
        public static string GetCustomParam(string paramName)
        {
            return FileStore.GetDebugParameter(paramName);
        }

        /// <summary>
        /// Задать значение временного отладочного параметра
        /// </summary>
        /// <param name="paramName">имя параметра</param>
        /// <param name="paramValue">значение параметра </param>
        public static void SetCustomParam(string paramName, object paramValue)
        {
            FileStore.SetDebugParameter(paramName, paramValue);
        }

        #endregion

    }
}
