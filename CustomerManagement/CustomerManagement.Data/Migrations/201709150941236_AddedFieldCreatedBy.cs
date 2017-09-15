namespace CustomerManagement.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFieldCreatedBy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customer", "CreatedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customer", "CreatedBy");
        }
    }
}
