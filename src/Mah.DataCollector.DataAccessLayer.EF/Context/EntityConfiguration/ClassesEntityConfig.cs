using Mah.DataCollector.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mah.DataCollector.DataAccessLayer.EF.Context.EntityConfiguration
{
    public class ClassesEntityConfig : EntityTypeConfiguration<Classes>
    {
        public ClassesEntityConfig():base()
        {
            this.ToTable("TB_CLASSES");

            this.Property(a => a.RequieredPhoto).HasColumnName("REQUIERED_PHOTO");

            this.Property(a => a.HasFlow).HasColumnName("HAS_FLOW");

            this.Property(a => a.ClassOrder).HasColumnName("CLASS_ORDER");

            this.Property(a => a.UserId).HasColumnName("USER_ID").HasMaxLength(50);

            this.Property(a => a.SupervisorDateOfField).HasColumnName("SUPERVISOR_DATEOF_FIELD").HasMaxLength(50);

            this.Property(a => a.SupervisorField).HasColumnName("SUPERVISOR_FIELD").HasMaxLength(50);

            this.Property(a => a.DateOf).HasColumnName("DATEOF").HasMaxLength(50);

            this.Property(a => a.TimeOf).HasColumnName("TIMEOF").HasMaxLength(50);  

            this.Property(a => a.AdressField).HasColumnName("ADRESS_FIELD").HasMaxLength(50);

            this.Property(a => a.UniqueField).HasColumnName("UNIQUE_FIELD").HasMaxLength(50);

            this.Property(a => a.StrokWidth).HasColumnName("STROK_WIDTH");

            this.Property(a => a.Width).HasColumnName("WIDTHOF");

            this.Property(a => a.StrokColor).HasColumnName("STROK_COLOR").HasMaxLength(50);

            this.Property(a => a.FillColor).HasColumnName("FILL_COLOR").HasMaxLength(50);

            this.Property(a => a.Scale).HasColumnName("SCALE");

            this.Property(a => a.SpatialRefrence).HasColumnName("SRID").HasMaxLength(50).IsRequired();
           
            this.Property(a => a.Class_type).HasColumnName("CLASS_TYPE").HasMaxLength(50).IsRequired();

            this.Property(a => a.Caption).HasColumnName("CAPTION").HasMaxLength(50).IsRequired();

            this.Property(a => a.Class_name).HasColumnName("CLASS_NAME").HasMaxLength(50).IsRequired();

            this.HasKey(a => a.ID);
        }
    }
}
