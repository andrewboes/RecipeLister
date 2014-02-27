using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class ShoppingListShoppingListItemRepository : IShoppingListShoppingListItemRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<ShoppingListShoppingListItem> All
        {
            get { return context.ShoppingListShoppingListItems; }
        }

        public IQueryable<ShoppingListShoppingListItem> AllIncluding(params Expression<Func<ShoppingListShoppingListItem, object>>[] includeProperties)
        {
            IQueryable<ShoppingListShoppingListItem> query = context.ShoppingListShoppingListItems;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ShoppingListShoppingListItem Find(int id)
        {
            return context.ShoppingListShoppingListItems.Find(id);
        }

        public void InsertOrUpdate(ShoppingListShoppingListItem shoppinglistshoppinglistitem)
        {
            if (shoppinglistshoppinglistitem.Id == default(int)) {
                // New entity
                context.ShoppingListShoppingListItems.Add(shoppinglistshoppinglistitem);
            } else {
                // Existing entity
                context.Entry(shoppinglistshoppinglistitem).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var shoppinglistshoppinglistitem = context.ShoppingListShoppingListItems.Find(id);
            context.ShoppingListShoppingListItems.Remove(shoppinglistshoppinglistitem);
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

    public interface IShoppingListShoppingListItemRepository : IDisposable
    {
        IQueryable<ShoppingListShoppingListItem> All { get; }
        IQueryable<ShoppingListShoppingListItem> AllIncluding(params Expression<Func<ShoppingListShoppingListItem, object>>[] includeProperties);
        ShoppingListShoppingListItem Find(int id);
        void InsertOrUpdate(ShoppingListShoppingListItem shoppinglistshoppinglistitem);
        void Delete(int id);
        void Save();
    }
}