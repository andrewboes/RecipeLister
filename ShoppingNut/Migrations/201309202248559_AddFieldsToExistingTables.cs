using ShoppingNut.Models;

namespace ShoppingNut.Migrations
{
	using System;
	using System.Data.Entity.Migrations;
	

	public partial class AddFieldsToExistingTables : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Ingredients", "Notes", c => c.String());
			AddColumn("dbo.Recipes", "Url", c => c.String());
			AddColumn("dbo.Recipes", "ActiveTime", c => c.String());
			AddColumn("dbo.Recipes", "TotalTime", c => c.String());
			AddColumn("dbo.Recipes", "Created", c => c.DateTime(nullable: false));
			AddColumn("dbo.Recipes", "Image", c => c.Binary());
			AddColumn("dbo.Recipes", "Notes", c => c.String());
			AddColumn("dbo.ShoppingLists", "Created", c => c.DateTime(nullable: false));
			AlterColumn("dbo.Recipes", "Servings", c => c.Double());
		}

		public override void Down()
		{
			AlterColumn("dbo.Recipes", "Servings", c => c.Double(nullable: false));
			DropColumn("dbo.ShoppingLists", "Created");
			DropColumn("dbo.Recipes", "Notes");
			DropColumn("dbo.Recipes", "Image");
			DropColumn("dbo.Recipes", "Created");
			DropColumn("dbo.Recipes", "TotalTime");
			DropColumn("dbo.Recipes", "ActiveTime");
			DropColumn("dbo.Recipes", "Url");
			DropColumn("dbo.Ingredients", "Notes");
		}
	}
}
