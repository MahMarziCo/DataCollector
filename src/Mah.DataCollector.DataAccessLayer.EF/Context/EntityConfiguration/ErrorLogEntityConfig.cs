
using DataAccess.Entities.Logg;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mah.DataCollector.DataAccessLayer.EF.Context.EntityConfiguration
{
    public class ErrorLoggEntityConfig : EntityTypeConfiguration<ErrorLogg>
    {
        public ErrorLoggEntityConfig()
            : base()
        {
           
            this.ToTable("TB_APP_LOG");

            this.HasKey(a => a.Id);

            this.Property(a => a.Id).HasColumnName("ID");

            this.Property(a => a.DateOf).HasColumnName("DATE_OF");

            this.Property(a => a.LogLevel).HasColumnName("LOG_LEVEL").HasMaxLength(30);

            this.Property(a => a.Message).HasColumnName("MESSAGE").HasMaxLength(150);
            this.Property(a => a.Stack).HasColumnName("STACK").HasColumnType("varchar(MAX)");
        }
    }
}
