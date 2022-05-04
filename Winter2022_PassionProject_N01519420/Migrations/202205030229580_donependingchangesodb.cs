namespace Winter2022_PassionProject_N01519420.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class donependingchangesodb : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Actors", "ActorFirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Actors", "ActorLastName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Actors", "ActorLastName", c => c.String());
            AlterColumn("dbo.Actors", "ActorFirstName", c => c.String());
        }
    }
}
