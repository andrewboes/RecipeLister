using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class IngredientRepository : IIngredientRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<Ingredient> All
        {
            get { return context.Ingredients; }
        }

        public IQueryable<Ingredient> AllIncluding(params Expression<Func<Ingredient, object>>[] includeProperties)
        {
            IQueryable<Ingredient> query = context.Ingredients;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Ingredient Find(int id)
        {
            return context.Ingredients.Find(id);
        }

        public void InsertOrUpdate(Ingredient ingredient)
        {
            if (ingredient.Id == default(int)) {
                // New entity
                context.Ingredients.Add(ingredient);
            } else {
                // Existing entity
                context.Entry(ingredient).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var ingredient = context.Ingredients.Find(id);
            context.Ingredients.Remove(ingredient);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }
    }

    public interface IIngredientRepository : IDisposable
    {
        IQueryable<Ingredient> All { get; }
        IQueryable<Ingredient> AllIncluding(params Expression<Func<Ingredient, object>>[] includeProperties);
        Ingredient Find(int id);
        void InsertOrUpdate(Ingredient ingredient);
        void Delete(int id);
        void Save();
    }
}