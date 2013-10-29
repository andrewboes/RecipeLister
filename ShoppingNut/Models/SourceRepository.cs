using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class SourceRepository : ISourceRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<Source> All
        {
            get { return context.Sources; }
        }

        public IQueryable<Source> AllIncluding(params Expression<Func<Source, object>>[] includeProperties)
        {
            IQueryable<Source> query = context.Sources;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Source Find(int id)
        {
            return context.Sources.Find(id);
        }

        public void InsertOrUpdate(Source source)
        {
            if (source.Id == default(int)) {
                // New entity
                context.Sources.Add(source);
            } else {
                // Existing entity
                context.Entry(source).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var source = context.Sources.Find(id);
            context.Sources.Remove(source);
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

    public interface ISourceRepository : IDisposable
    {
        IQueryable<Source> All { get; }
        IQueryable<Source> AllIncluding(params Expression<Func<Source, object>>[] includeProperties);
        Source Find(int id);
        void InsertOrUpdate(Source source);
        void Delete(int id);
        void Save();
    }
}