using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class QuantityTypeRepository : IQuantityTypeRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<QuantityType> All
        {
            get { return context.QuantityTypes; }
        }

        public IQueryable<QuantityType> AllIncluding(params Expression<Func<QuantityType, object>>[] includeProperties)
        {
            IQueryable<QuantityType> query = context.QuantityTypes;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public QuantityType Find(int id)
        {
            return context.QuantityTypes.Find(id);
        }

        public void InsertOrUpdate(QuantityType quantitytype)
        {
            if (quantitytype.Id == default(int)) {
                // New entity
                context.QuantityTypes.Add(quantitytype);
            } else {
                // Existing entity
                context.Entry(quantitytype).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var quantitytype = context.QuantityTypes.Find(id);
            context.QuantityTypes.Remove(quantitytype);
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

    public interface IQuantityTypeRepository : IDisposable
    {
        IQueryable<QuantityType> All { get; }
        IQueryable<QuantityType> AllIncluding(params Expression<Func<QuantityType, object>>[] includeProperties);
        QuantityType Find(int id);
        void InsertOrUpdate(QuantityType quantitytype);
        void Delete(int id);
        void Save();
    }
}