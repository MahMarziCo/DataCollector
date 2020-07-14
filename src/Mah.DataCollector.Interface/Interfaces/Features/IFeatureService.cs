using Mah.DataCollector.Interface.Dto.Feature;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mah.DataCollector.Interface.Interfaces.Features
{
    public interface IFeatureService
    {
        Task<IEnumerable<UserOperationReport>> GetUserReportOnLayer(string layerName, string userName, string userField, string fieldName);
    }
}
