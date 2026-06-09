using System.Collections.Generic;
using System.Threading.Tasks;
using RIVS.ASAK.Core.Contract.DTO;

namespace RIVS.ASAK.Core.Contract
{
    public interface IConfigContext
    {
        Task<bool> CreateConfigContext();

        IEnumerable<DeviceParamsDTO> DeviceParams();

        IEnumerable<DeviceLinkDTO> DeviceLinks();

        IEnumerable<CyclogramDTO> Сyclograms();

        IEnumerable<ProductDTO> Products();

        IEnumerable<ContourDeviceDTO> ContourDevice();

        IEnumerable<ContourDTO> Contour();

        IEnumerable<ChangeOperationalDTO> ChangeOperational();
    }
}
