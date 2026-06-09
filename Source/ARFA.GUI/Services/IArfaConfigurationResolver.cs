using System;
using System.Collections.Generic;
using System.Xml.Linq;
using RIVS.ASAK.ARFA.Application;
using RIVS.ASAK.Core.Contract.DTO;

namespace RIVS.ASAK.ARFA.GUI.Services
{
    public interface IArfaConfigurationResolver
    {
        ElementDTO GetElementsById(int id);

        float GetJaFromRepers(int id, int cuvetNumber);

        int GetArfaNumber();

        int GetCuvetteCount();

        int GetAnalyticProgramId();

        IEnumerable<int> GetUsedCuvettesFromAnalyticProgram();

        IEnumerable<ArfaGraphElement> GetGraphElementFromAnalyticProgram();

        (string, DateTime) GetAnalyticProgramInfo();

        XDocument GetAnalyticProgram();

        void AddReperDataItem(ReperDataItem reperDataItem);

        int GetLastReperHashKey();

        //SubjectSystemType GetSubjectSystemTypeByArfaNumber();

        bool GetIsNeedSaveRepersSpectrum();

        bool GetIsNeedSaveMeasSpectrum();

        XElement GetDeviceManagerConfiguration();

        XElement GetSpectrBlockDeviceManagerConfiguration();

        bool IsUseAP { get; set; }

        void SetUserData(string login, Guid roleId, Guid userId);
    }
}
