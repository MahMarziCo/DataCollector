namespace Mah.DataCollector.DataAccessLayer.EF.Context.MigrationHistory
{
    using Mah.Common.EntityFramework.Migration;
    using Mah.DataCollector.Entity.Entities;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DataCollectorContext>
    {
        public Configuration()
        {
            CodeGenerator = new MigrationUserAndTimeCodeGenerator();
            MigrationsNamespace = "System.Data.Entity.Migrations";

            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Context\MigrationHistory";
        }

        protected override void Seed(DataCollectorContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
