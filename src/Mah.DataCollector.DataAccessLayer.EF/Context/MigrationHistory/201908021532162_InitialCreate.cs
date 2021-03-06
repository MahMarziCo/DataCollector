// Generated Time: 08/02/2019 20:02:16
// Generated By: MAH

namespace System.Data.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UniqueStyles", "CLASS_ID", "dbo.Classes");
            DropForeignKey("dbo.Fields", "Class_ID", "dbo.Classes");
            DropForeignKey("dbo.Fields", "Domain_ID", "dbo.Domain");
            DropForeignKey("dbo.DomainValue", "Domain_ID", "dbo.Domain");
            DropIndex("dbo.UniqueStyles", new[] { "CLASS_ID" });
            DropIndex("dbo.DomainValue", new[] { "Domain_ID" });
            DropIndex("dbo.Fields", new[] { "Domain_ID" });
            DropIndex("dbo.Fields", new[] { "Class_ID" });
            DropTable("dbo.User_location");
            DropTable("dbo.Update_Log");
            DropTable("dbo.TB_Setting");
            DropTable("dbo.Feature_Pic");
            DropTable("dbo.UniqueStyles");
            DropTable("dbo.DomainValue");
            DropTable("dbo.Domain");
            DropTable("dbo.Fields");
            DropTable("dbo.Classes");
        }
    }
}
