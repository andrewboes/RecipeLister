using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ShoppingNut.Models
{
	public class QuantityType
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public double Grams { get; set; }

		[ForeignKey("Food")]
		public int FoodId { get; set; }
		public virtual Food Food { get; set; }
	}
}
