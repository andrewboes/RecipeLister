using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class ShoppingListRepository : IShoppingListRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<ShoppingList> All
        {
            get { return context.ShoppingLists; }
        }

        public IQueryable<ShoppingList> AllIncluding(params Expression<Func<ShoppingList, object>>[] includeProperties)
        {
            IQueryable<ShoppingList> query = context.ShoppingLists;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ShoppingList Find(int id)
        {
            return context.ShoppingLists.Find(id);
        }

        public void InsertOrUpdate(ShoppingList shoppinglist)
        {
            if (shoppinglist.Id == default(int))
            {
	            shoppinglist.Created = DateTime.Now;
              context.ShoppingLists.Add(shoppinglist);
            } else {
                // Existing entity
                context.Entry(shoppinglist).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var shoppinglist = context.ShoppingLists.Find(id);
            context.ShoppingLists.Remove(shoppinglist);
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

    public interface IShoppingListRepository : IDisposable
    {
        IQueryable<ShoppingList> All { get; }
        IQueryable<ShoppingList> AllIncluding(params Expression<Func<ShoppingList, object>>[] includeProperties);
        ShoppingList Find(int id);
        void InsertOrUpdate(ShoppingList shoppinglist);
        void Delete(int id);
        void Save();
    }
}