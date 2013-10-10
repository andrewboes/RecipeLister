using System.Data.Entity.Infrastructure;
using System.Globalization;
using ShoppingNut.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace UsdaMigration
{
	class Program
	{
		private static Food UsdaToShoppingNut(ABBREV data, List<WEIGHT> weights, FOOD_DE desc, int foodGroupId)
		{
			//string name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase( data.Shrt_Desc.ToLower());

			Food f = new Food
			{
				Name = desc.Long_Desc,
				CommonName = desc.ComName,
				Ndb_No = data.NDB_No,
				Calories = data.Energ_Kcal.GetValueOrDefault(0),
				Water = data.Water__g_.GetValueOrDefault(0),
				Protein = data.Protein__g_.GetValueOrDefault(0),
				Fat = data.Lipid_Tot__g_.GetValueOrDefault(0),
				Carbohydrate = data.Carbohydrt__g_.GetValueOrDefault(0),
				Fiber = data.Fiber_TD__g_.GetValueOrDefault(0),
				Sugar = data.Sugar_Tot__g_.GetValueOrDefault(0),
				CalciumMg = data.Calcium__mg_.GetValueOrDefault(0),
				IronMg = data.Iron__mg_.GetValueOrDefault(0),
				MagnesiumMg = data.Magnesium__mg_.GetValueOrDefault(0),
				SodiumMg = data.Sodium__mg_.GetValueOrDefault(0),
				SaturatedFat = data.FA_Sat__g_.GetValueOrDefault(0),
				MonoFat = data.FA_Mono__g_.GetValueOrDefault(0),
				PolyFat = data.FA_Poly__g_.GetValueOrDefault(0),
				CholesteralMg = data.Cholestrl__mg_.GetValueOrDefault(0),
				VitaminCMg = data.Vit_C__mg_.GetValueOrDefault(0),
				DefaultGrams = data.GmWt_1.GetValueOrDefault(0),
				QuantityTypes = weights.Select(UsdaWeightToQuanityType).ToList(),
				FoodGroupId = foodGroupId
			};
			return f;
		}

		private static QuantityType UsdaWeightToQuanityType(WEIGHT x)
		{
			QuantityType qt = new QuantityType {Grams = x.Gm_Wgt.GetValueOrDefault(0), Name = x.Msre_Desc};
			return qt;
		}

		private static FoodGroup UsdaFoodGroupToFoodGroup(FD_GROUP group)
		{
			return new FoodGroup { Description = group.FdGrp_Desc };
		}

		static void Main(string[] args)
		{
			using (var context = new DataClasses1DataContext())
			{
				using (var v = new ShoppingNutContext("Data Source=.;Initial Catalog=ShoppingNutOne;Integrated Security=SSPI"))
				{
					var usdaFoods = context.ABBREVs;
					int count = 0;
					foreach (var s in usdaFoods)
					{
						var weights = context.WEIGHTs.Where(x => x.NDB_No == s.NDB_No).ToList();
						var desc = context.FOOD_DEs.Single(x => x.NDB_No == s.NDB_No);
						var groups = context.FD_GROUPs.Single(x => x.FdGrp_CD == desc.FdGrp_Cd);
						int foodGroupId = GetFoodGroupId(v, groups);
						Food f = UsdaToShoppingNut(s, weights, desc, foodGroupId);
						v.Foods.Add(f);
						if (count % 500 == 0)
							Console.WriteLine(count);
						count++;
						v.SaveChanges();
					}
					v.Database.ExecuteSqlCommand("INSERT INTO dbo.QuantityTypes ( Name, Grams, FoodId ) SELECT N'gram', 1, id FROM dbo.Foods");
					v.Database.ExecuteSqlCommand("INSERT INTO dbo.QuantityTypes ( Name, Grams, FoodId ) SELECT N'ounce (weight)', 28.3495, id FROM dbo.Foods");
					v.Database.ExecuteSqlCommand("INSERT INTO dbo.QuantityTypes ( Name, Grams, FoodId ) SELECT N'pound', 453.592, id FROM dbo.Foods");
				}
			}
		}

		private static int GetFoodGroupId(ShoppingNutContext ctx, FD_GROUP usdaGroup)
		{
			var groups = ctx.FoodGroups.Where(x => x.Description == usdaGroup.FdGrp_Desc);
			if (groups.Any())
				return groups.Single().Id;
			else
			{
				FoodGroup group = new FoodGroup {Description = usdaGroup.FdGrp_Desc};
				ctx.FoodGroups.Add(group);
				ctx.SaveChanges();
				return group.Id;
			}
		}
	}
}
