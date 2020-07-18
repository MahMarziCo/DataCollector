// Generated Time: 07/18/2020 21:09:29
// Generated By: mah

namespace System.Data.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRequiredFieldToClass : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Classes", newName: "TB_CLASSES");
            RenameColumn(table: "dbo.TB_CLASSES", name: "SpatialRefrence", newName: "SRID");
            RenameColumn(table: "dbo.TB_CLASSES", name: "FillColor", newName: "FILL_COLOR");
            RenameColumn(table: "dbo.TB_CLASSES", name: "StrokColor", newName: "STROK_COLOR");
            RenameColumn(table: "dbo.TB_CLASSES", name: "Width", newName: "WIDTHOF");
            RenameColumn(table: "dbo.TB_CLASSES", name: "StrokWidth", newName: "STROK_WIDTH");
            RenameColumn(table: "dbo.TB_CLASSES", name: "UniqueField", newName: "UNIQUE_FIELD");
            RenameColumn(table: "dbo.TB_CLASSES", name: "AdressField", newName: "ADRESS_FIELD");
            RenameColumn(table: "dbo.TB_CLASSES", name: "SupervisorField", newName: "SUPERVISOR_FIELD");
            RenameColumn(table: "dbo.TB_CLASSES", name: "SupervisorDateOfField", newName: "SUPERVISOR_DATEOF_FIELD");
            RenameColumn(table: "dbo.TB_CLASSES", name: "UserId", newName: "USER_ID");
            RenameColumn(table: "dbo.TB_CLASSES", name: "ClassOrder", newName: "CLASS_ORDER");
            RenameColumn(table: "dbo.TB_CLASSES", name: "HasFlow", newName: "HAS_FLOW");
            AddColumn("dbo.TB_CLASSES", "REQUIERED_PHOTO", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TB_CLASSES", "REQUIERED_PHOTO");
            RenameColumn(table: "dbo.TB_CLASSES", name: "HAS_FLOW", newName: "HasFlow");
            RenameColumn(table: "dbo.TB_CLASSES", name: "CLASS_ORDER", newName: "ClassOrder");
            RenameColumn(table: "dbo.TB_CLASSES", name: "USER_ID", newName: "UserId");
            RenameColumn(table: "dbo.TB_CLASSES", name: "SUPERVISOR_DATEOF_FIELD", newName: "SupervisorDateOfField");
            RenameColumn(table: "dbo.TB_CLASSES", name: "SUPERVISOR_FIELD", newName: "SupervisorField");
            RenameColumn(table: "dbo.TB_CLASSES", name: "ADRESS_FIELD", newName: "AdressField");
            RenameColumn(table: "dbo.TB_CLASSES", name: "UNIQUE_FIELD", newName: "UniqueField");
            RenameColumn(table: "dbo.TB_CLASSES", name: "STROK_WIDTH", newName: "StrokWidth");
            RenameColumn(table: "dbo.TB_CLASSES", name: "WIDTHOF", newName: "Width");
            RenameColumn(table: "dbo.TB_CLASSES", name: "STROK_COLOR", newName: "StrokColor");
            RenameColumn(table: "dbo.TB_CLASSES", name: "FILL_COLOR", newName: "FillColor");
            RenameColumn(table: "dbo.TB_CLASSES", name: "SRID", newName: "SpatialRefrence");
            RenameTable(name: "dbo.TB_CLASSES", newName: "Classes");
        }
    }
}
