using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class FoodRepository : IFoodRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<Food> All
        {
            get { return context.Foods; }
        }

        public IQueryable<Food> AllIncluding(params Expression<Func<Food, object>>[] includeProperties)
        {
            IQueryable<Food> query = context.Foods;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Food Find(int id)
        {
            return context.Foods.Find(id);
        }

        public void InsertOrUpdate(Food food)
        {
            if (food.Id == default(int)) {
                // New entity
                context.Foods.Add(food);
            } else {
                // Existing entity
                context.Entry(food).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var food = context.Foods.Find(id);
            context.Foods.Remove(food);
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

    public interface IFoodRepository : IDisposable
    {
        IQueryable<Food> All { get; }
        IQueryable<Food> AllIncluding(params Expression<Func<Food, object>>[] includeProperties);
        Food Find(int id);
        void InsertOrUpdate(Food food);
        void Delete(int id);
        void Save();
    }
}