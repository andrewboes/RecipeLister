using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ShoppingNut.Models
{
	public class Instruction
	{
		public int Id { get; set; }
		public int Order { get; set; }
		public string Text { get; set; }

		[ForeignKey("Recipe")]
		public int RecipeId { get; set; }
		public virtual Recipe Recipe{ get; set; }
	}
}
