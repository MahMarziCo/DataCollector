using Mah.DataCollector.Entity.Entities;
using System.Data.Entity.Infrastructure;

namespace Mah.DataCollector.DataAccessLayer.EF.Context
{
    public class MigrationsContextFactory : IDbContextFactory<DataCollectorContext>
    {
        public DataCollectorContext Create()
        {
            return new DataCollectorContext("LogicDb");
        }
    }
}
