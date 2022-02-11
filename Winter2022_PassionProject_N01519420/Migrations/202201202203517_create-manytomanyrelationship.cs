namespace Winter2022_PassionProject_N01519420.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createmanytomanyrelationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MovieActors",
                c => new
                    {
                        Movie_MovieID = c.Int(nullable: false),
                        Actor_ActorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Movie_MovieID, t.Actor_ActorID })
                .ForeignKey("dbo.Movies", t => t.Movie_MovieID, cascadeDelete: true)
                .ForeignKey("dbo.Actors", t => t.Actor_ActorID, cascadeDelete: true)
                .Index(t => t.Movie_MovieID)
                .Index(t => t.Actor_ActorID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MovieActors", "Actor_ActorID", "dbo.Actors");
            DropForeignKey("dbo.MovieActors", "Movie_MovieID", "dbo.Movies");
            DropIndex("dbo.MovieActors", new[] { "Actor_ActorID" });
            DropIndex("dbo.MovieActors", new[] { "Movie_MovieID" });
            DropTable("dbo.MovieActors");
        }
    }
}
