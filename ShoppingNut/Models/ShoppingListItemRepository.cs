using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class ShoppingListItemRepository : IShoppingListItemRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<ShoppingListItem> All
        {
            get { return context.ShoppingListItems; }
        }

        public IQueryable<ShoppingListItem> AllIncluding(params Expression<Func<ShoppingListItem, object>>[] includeProperties)
        {
            IQueryable<ShoppingListItem> query = context.ShoppingListItems;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ShoppingListItem Find(int id)
        {
            return context.ShoppingListItems.Find(id);
        }

        public void InsertOrUpdate(ShoppingListItem shoppinglistitem)
        {
            if (shoppinglistitem.Id == default(int)) {
                // New entity
                context.ShoppingListItems.Add(shoppinglistitem);
            } else {
                // Existing entity
                context.Entry(shoppinglistitem).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var shoppinglistitem = context.ShoppingListItems.Find(id);
            context.ShoppingListItems.Remove(shoppinglistitem);
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

    public interface IShoppingListItemRepository : IDisposable
    {
        IQueryable<ShoppingListItem> All { get; }
        IQueryable<ShoppingListItem> AllIncluding(params Expression<Func<ShoppingListItem, object>>[] includeProperties);
        ShoppingListItem Find(int id);
        void InsertOrUpdate(ShoppingListItem shoppinglistitem);
        void Delete(int id);
        void Save();
    }
}