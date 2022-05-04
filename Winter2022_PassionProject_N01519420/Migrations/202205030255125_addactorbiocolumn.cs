namespace Winter2022_PassionProject_N01519420.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addactorbiocolumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "Bio", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Actors", "Bio");
        }
    }
}
