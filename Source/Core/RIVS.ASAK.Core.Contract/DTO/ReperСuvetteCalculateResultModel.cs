using System;
using System.Collections.Generic;
using System.Linq;
using RIVS.ASAK.Core.Contract.Enums;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public interface IMeasuredData : ICloneable
    {

    }

    public class ElementJaModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Ja { get; set; }
    }

    public class ReperKzModel
    {
        public int KzState { get; set; }
        public string ParameterName { get; set; }
    }
    public class MeasureСriteriaResult : ICloneable
    {
        /// <summary>
        /// Общезначимое состояние (например вода/пусто)
        /// </summary>
        public MeasureStates GeneralState { get; set; }

        /// <summary>
        /// Результат по каждому элементу
        /// </summary>
        public class СriteriaResultForElement
        {
            public СriteriaResultForElement(int id, string name, float value, float сomparedValue, float сriteria, MeasureStates elementalState)
            {
                Id = id;
                Name = name;
                Value = value;
                СomparedValue = сomparedValue;
                Сriteria = сriteria;
                ElementalState = elementalState;
            }

            public int Id { get; set; }
            public string Name { get; set; }

            /// <summary>
            /// текущее значение
            /// </summary>
            public float Value { get; set; }

            /// <summary>
            /// значение с которым сравнивается
            /// </summary>
            public float СomparedValue { get; set; }

            /// <summary>
            /// Критерий
            /// </summary>
            public float Сriteria { get; set; }

            /// <summary>
            /// состояние поэлементно
            /// </summary>
            public MeasureStates ElementalState { get; set; }
        }

        public List<СriteriaResultForElement> СriteriaResultForElements = new List<СriteriaResultForElement>();

        public MeasureСriteriaResult()
        {
        }

        public void AddElement(int id, string name, float value, float сomparedValue, float сriteria, MeasureStates elementalState)
        {
            СriteriaResultForElements.Add(new СriteriaResultForElement(id, name, value, сomparedValue, сriteria, elementalState));
        }


        public object Clone()
        {
            var clone = new MeasureСriteriaResult
            {
                GeneralState = GeneralState
            };

            clone.СriteriaResultForElements = new List<СriteriaResultForElement>();

            СriteriaResultForElements.ForEach((item) =>
            {
                clone.СriteriaResultForElements
                .Add(new СriteriaResultForElement(item.Id, item.Name, item.Value, item.СomparedValue, item.Сriteria, item.ElementalState));
            });

            return clone;
        }
    }

    public class ReperСuvetteResultModel : IMeasuredData
    {
        public DateTime MeasureTime { get; set; }
        public int Pribor { get; set; }
        public int Cuvet { get; set; }
        public int Kz { get; set; }
        public int ReperHashKey { get; set; }
        public IEnumerable<ElementJaModel> ElementJas { get; set; }
        public MeasureСriteriaResult MeasureСriteriaResult { get; set; }
        public string Spectrum { get; set; }
        public double LiveTime { get; set; }
        public object Clone()
        {
            var clone = new ReperСuvetteResultModel
            {
                MeasureTime = MeasureTime,
                Pribor = Pribor,
                Cuvet = Cuvet,
                Kz = Kz,
                ReperHashKey = ReperHashKey,
                Spectrum = string.IsNullOrEmpty(Spectrum) ? null : (string)Spectrum.Clone(),
                LiveTime = LiveTime,
                ElementJas = ElementJas?.Select(x => new ElementJaModel() { Id = x.Id, Name = x.Name, Ja = x.Ja }),
                MeasureСriteriaResult = (MeasureСriteriaResult)MeasureСriteriaResult.Clone()
            };
            return clone;
        }
    }

    /// <summary>
    /// Обобщенная структура данных для РЕПЕРОВ после калькуляции
    /// содержит как данные отправляемые в Диспетчер на сохранение,
    /// так и данные для обновления ОПС
    /// </summary>
    public class ReperСuvetteCalculateResultModel
    {
        public IMeasuredData MeasuredData { get; set; }

        public string ItOpc { get; set; }
        public string JaOpc { get; set; }
        public string SpectrumOpc { get; set; }

        public ReperСuvetteCalculateResultModel()
        {
        }
    }
}
