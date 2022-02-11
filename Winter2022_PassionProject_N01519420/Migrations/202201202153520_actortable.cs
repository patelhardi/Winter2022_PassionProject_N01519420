namespace Winter2022_PassionProject_N01519420.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class actortable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Actors",
                c => new
                    {
                        ActorID = c.Int(nullable: false, identity: true),
                        ActorFirstName = c.String(),
                        ActorLastName = c.String(),
                    })
                .PrimaryKey(t => t.ActorID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Actors");
        }
    }
}
