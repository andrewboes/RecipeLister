namespace ShoppingNut.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedImageToOwnTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecipeImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipeId = c.Int(nullable: false),
                        UserSubmittedImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Recipes", t => t.RecipeId, cascadeDelete: true)
                .ForeignKey("dbo.UserSubmittedImages", t => t.UserSubmittedImageId, cascadeDelete: true)
                .Index(t => t.RecipeId)
                .Index(t => t.UserSubmittedImageId);
            
            CreateTable(
                "dbo.UserSubmittedImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Image = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Recipes", "Image");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "Image", c => c.Binary());
            DropIndex("dbo.RecipeImages", new[] { "UserSubmittedImageId" });
            DropIndex("dbo.RecipeImages", new[] { "RecipeId" });
            DropForeignKey("dbo.RecipeImages", "UserSubmittedImageId", "dbo.UserSubmittedImages");
            DropForeignKey("dbo.RecipeImages", "RecipeId", "dbo.Recipes");
            DropTable("dbo.UserSubmittedImages");
            DropTable("dbo.RecipeImages");
        }
    }
}
