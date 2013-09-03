using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingNut.Models
{
	public class Food
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public double Calories { get; set; }
		public double Protein { get; set; }
		public double Water { get; set; }
		public double Fat { get; set; }
		public double Carbohydrate { get; set; }
		public double Fiber { get; set; }
		public double Sugar { get; set; }
		public double CalciumMg { get; set; }
		public double IronMg { get; set; }
		public double MagnesiumMg { get; set; }
		public double SodiumMg { get; set; }
		public double SaturatedFat { get; set; }
		public double MonoFat { get; set; }
		public double PolyFat { get; set; }
		public double CholesteralMg { get; set; }
		public double VitaminCMg { get; set; }
		public double DefaultGrams { get; set; }

		public virtual List<QuantityType> QuantityTypes { get; set; }
	}
}
