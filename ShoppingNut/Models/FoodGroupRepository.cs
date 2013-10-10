using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class FoodGroupRepository : IFoodGroupRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<FoodGroup> All
        {
            get { return context.FoodGroups; }
        }

        public IQueryable<FoodGroup> AllIncluding(params Expression<Func<FoodGroup, object>>[] includeProperties)
        {
            IQueryable<FoodGroup> query = context.FoodGroups;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public FoodGroup Find(int id)
        {
            return context.FoodGroups.Find(id);
        }

        public void InsertOrUpdate(FoodGroup foodgroup)
        {
            if (foodgroup.Id == default(int)) {
                // New entity
                context.FoodGroups.Add(foodgroup);
            } else {
                // Existing entity
                context.Entry(foodgroup).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var foodgroup = context.FoodGroups.Find(id);
            context.FoodGroups.Remove(foodgroup);
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

    public interface IFoodGroupRepository : IDisposable
    {
        IQueryable<FoodGroup> All { get; }
        IQueryable<FoodGroup> AllIncluding(params Expression<Func<FoodGroup, object>>[] includeProperties);
        FoodGroup Find(int id);
        void InsertOrUpdate(FoodGroup foodgroup);
        void Delete(int id);
        void Save();
    }
}