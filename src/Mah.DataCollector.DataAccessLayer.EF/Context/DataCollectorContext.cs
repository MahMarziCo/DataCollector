using Mah.DataCollector.DataAccessLayer.EF.Context;
namespace Mah.DataCollector.Entity.Entities
{
    using DataAccess.Entities.Logg;
    using Mah.DataCollector.DataAccessLayer.EF.Context.EntityConfiguration;
    using System.Data.Entity;

    public partial class DataCollectorContext : DbContext
    {

        public DataCollectorContext(string connectionString)
            : base(connectionString)
        {
        }

        public virtual DbSet<Classes> Classes { get; set; }
        public virtual DbSet<Domain> Domain { get; set; }
        public virtual DbSet<DomainValue> DomainValue { get; set; }
        public virtual DbSet<Feature_Pic> Feature_Pic { get; set; }
        public virtual DbSet<Fields> Fields { get; set; }
        public virtual DbSet<UniqueStyle> UniqueStyles { get; set; }
        public virtual DbSet<TB_Setting> TB_Setting { get; set; }
        public virtual DbSet<Update_Log> Update_Log { get; set; }
        public virtual DbSet<User_location> User_location { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add<ErrorLogg>(new ErrorLoggEntityConfig());

            modelBuilder.Entity<Classes>()
                .HasMany(e => e.Fields)
                .WithOptional(e => e.Classes)
                .HasForeignKey(e => e.Class_ID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Classes>()
                .HasMany(e => e.UniqueStyles)
                .WithOptional(e => e.Classes)
                .HasForeignKey(e => e.CLASS_ID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Domain>()
                .HasMany(e => e.DomainValue)
                .WithOptional(e => e.Domain)
                .HasForeignKey(e => e.Domain_ID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Domain>()
                .HasMany(e => e.Fields)
                .WithOptional(e => e.Domain)
                .HasForeignKey(e => e.Domain_ID);

            modelBuilder.Entity<TB_Setting>()
                .Property(e => e.SET_Key)
                .IsUnicode(false);

            modelBuilder.Entity<TB_Setting>()
                .Property(e => e.User_name)
                .IsUnicode(false);

            modelBuilder.Entity<TB_Setting>()
                .Property(e => e.SET_Type)
                .IsUnicode(false);

            modelBuilder.Entity<Update_Log>()
                .Property(e => e.User_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<User_location>()
                .Property(e => e.User_name)
                .IsUnicode(false);
        }
    }
}
