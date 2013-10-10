using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class RecipeImageRepository : IRecipeImageRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<RecipeImage> All
        {
            get { return context.RecipeImages; }
        }

        public IQueryable<RecipeImage> AllIncluding(params Expression<Func<RecipeImage, object>>[] includeProperties)
        {
            IQueryable<RecipeImage> query = context.RecipeImages;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public RecipeImage Find(int id)
        {
            return context.RecipeImages.Find(id);
        }

        public void InsertOrUpdate(RecipeImage recipeimage)
        {
            if (recipeimage.Id == default(int)) {
                // New entity
                context.RecipeImages.Add(recipeimage);
            } else {
                // Existing entity
                context.Entry(recipeimage).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var recipeimage = context.RecipeImages.Find(id);
            context.RecipeImages.Remove(recipeimage);
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

    public interface IRecipeImageRepository : IDisposable
    {
        IQueryable<RecipeImage> All { get; }
        IQueryable<RecipeImage> AllIncluding(params Expression<Func<RecipeImage, object>>[] includeProperties);
        RecipeImage Find(int id);
        void InsertOrUpdate(RecipeImage recipeimage);
        void Delete(int id);
        void Save();
    }
}