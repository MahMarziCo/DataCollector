using Mah.DataCollector.Interface.Dto.Feature;
using Mah.DataCollector.Interface.Interfaces.Features;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Mah.DataCollector.Service.Services.Features
{
    public class FeatureService: IFeatureService
    {
        private readonly string _GdbConnectionString;
        public FeatureService(string gdbConnection)
        {
            _GdbConnectionString = gdbConnection;
        }

        public async Task<IEnumerable<UserOperationReport>> GetUserReportOnLayer(string layerName,string userName, string userField, string fieldName)
        {
            List<UserOperationReport> list = new List<UserOperationReport>();
            using (SqlConnection cnn = new SqlConnection(_GdbConnectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"SELECT { fieldName } FIELD_NAME ,COUNT(*)  COUNT_OF FROM [{ layerName }] WHERE upper({ userField }) = upper('{ userName }') GROUP BY { fieldName}";
                    var dr = await cmd.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        var row = new UserOperationReport();
                        row.Domain = dr["FIELD_NAME"].ToString();
                        row.Count = Convert.ToInt32(dr["COUNT_OF"]);
                        list.Add(row);
                    }
                }
                cnn.Close();
            }
            return list;
        }
    }
}
