using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class RecipeRepository : IRecipeRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<Recipe> All
        {
            get { return context.Recipes; }
        }

        public IQueryable<Recipe> AllIncluding(params Expression<Func<Recipe, object>>[] includeProperties)
        {
            IQueryable<Recipe> query = context.Recipes;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Recipe Find(int id)
        {
            return context.Recipes.Find(id);
        }

        public void InsertOrUpdate(Recipe recipe)
        {
            if (recipe.Id == default(int)) {
                // New entity
                context.Recipes.Add(recipe);
            } else {
                // Existing entity
                context.Entry(recipe).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var recipe = context.Recipes.Find(id);
            context.Recipes.Remove(recipe);
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

    public interface IRecipeRepository : IDisposable
    {
        IQueryable<Recipe> All { get; }
        IQueryable<Recipe> AllIncluding(params Expression<Func<Recipe, object>>[] includeProperties);
        Recipe Find(int id);
        void InsertOrUpdate(Recipe recipe);
        void Delete(int id);
        void Save();
    }
}