namespace Winter2022_PassionProject_N01519420.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedtwocolumnintoactortable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "ActorHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Actors", "PicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Actors", "PicExtension");
            DropColumn("dbo.Actors", "ActorHasPic");
        }
    }
}
