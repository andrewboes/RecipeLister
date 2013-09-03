namespace ShoppingNut.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blah : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Foods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Calories = c.Double(nullable: false),
                        Protein = c.Double(nullable: false),
                        Water = c.Double(nullable: false),
                        Fat = c.Double(nullable: false),
                        Carbohydrate = c.Double(nullable: false),
                        Fiber = c.Double(nullable: false),
                        Sugar = c.Double(nullable: false),
                        CalciumMg = c.Double(nullable: false),
                        IronMg = c.Double(nullable: false),
                        MagnesiumMg = c.Double(nullable: false),
                        SodiumMg = c.Double(nullable: false),
                        SaturatedFat = c.Double(nullable: false),
                        MonoFat = c.Double(nullable: false),
                        PolyFat = c.Double(nullable: false),
                        CholesteralMg = c.Double(nullable: false),
                        VitaminCMg = c.Double(nullable: false),
                        DefaultGrams = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuantityTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Grams = c.Double(nullable: false),
                        FoodId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Foods", t => t.FoodId, cascadeDelete: true)
                .Index(t => t.FoodId);
            
            CreateTable(
                "dbo.Ingredients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Double(nullable: false),
                        QuantityTypeId = c.Int(),
                        RecipeId = c.Int(nullable: false),
                        FoodId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuantityTypes", t => t.QuantityTypeId)
                .ForeignKey("dbo.Recipes", t => t.RecipeId, cascadeDelete: true)
                .ForeignKey("dbo.Foods", t => t.FoodId, cascadeDelete: true)
                .Index(t => t.QuantityTypeId)
                .Index(t => t.RecipeId)
                .Index(t => t.FoodId);
            
            CreateTable(
                "dbo.Recipes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Servings = c.Double(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Instructions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Text = c.String(),
                        RecipeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Recipes", t => t.RecipeId, cascadeDelete: true)
                .Index(t => t.RecipeId);
            
            CreateTable(
                "dbo.ShoppingLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ShoppingListItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Quantity = c.Double(nullable: false),
                        QuantityType = c.String(),
                        PickedUp = c.Boolean(nullable: false),
                        ShoppingListId = c.Int(nullable: false),
                        FoodId = c.Int(),
                        QuantityTypeId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShoppingLists", t => t.ShoppingListId, cascadeDelete: true)
                .ForeignKey("dbo.Foods", t => t.FoodId)
                .ForeignKey("dbo.QuantityTypes", t => t.QuantityTypeId)
                .Index(t => t.ShoppingListId)
                .Index(t => t.FoodId)
                .Index(t => t.QuantityTypeId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ShoppingListItems", new[] { "QuantityTypeId" });
            DropIndex("dbo.ShoppingListItems", new[] { "FoodId" });
            DropIndex("dbo.ShoppingListItems", new[] { "ShoppingListId" });
            DropIndex("dbo.ShoppingLists", new[] { "UserId" });
            DropIndex("dbo.Instructions", new[] { "RecipeId" });
            DropIndex("dbo.Recipes", new[] { "UserId" });
            DropIndex("dbo.Ingredients", new[] { "FoodId" });
            DropIndex("dbo.Ingredients", new[] { "RecipeId" });
            DropIndex("dbo.Ingredients", new[] { "QuantityTypeId" });
            DropIndex("dbo.QuantityTypes", new[] { "FoodId" });
            DropForeignKey("dbo.ShoppingListItems", "QuantityTypeId", "dbo.QuantityTypes");
            DropForeignKey("dbo.ShoppingListItems", "FoodId", "dbo.Foods");
            DropForeignKey("dbo.ShoppingListItems", "ShoppingListId", "dbo.ShoppingLists");
            DropForeignKey("dbo.ShoppingLists", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.Instructions", "RecipeId", "dbo.Recipes");
            DropForeignKey("dbo.Recipes", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.Ingredients", "FoodId", "dbo.Foods");
            DropForeignKey("dbo.Ingredients", "RecipeId", "dbo.Recipes");
            DropForeignKey("dbo.Ingredients", "QuantityTypeId", "dbo.QuantityTypes");
            DropForeignKey("dbo.QuantityTypes", "FoodId", "dbo.Foods");
            DropTable("dbo.ShoppingListItems");
            DropTable("dbo.ShoppingLists");
            DropTable("dbo.Instructions");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Recipes");
            DropTable("dbo.Ingredients");
            DropTable("dbo.QuantityTypes");
            DropTable("dbo.Foods");
        }
    }
}
