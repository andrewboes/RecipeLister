using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class UserSubmittedImageRepository : IUserSubmittedImageRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<UserSubmittedImage> All
        {
            get { return context.UserSubmittedImages; }
        }

        public IQueryable<UserSubmittedImage> AllIncluding(params Expression<Func<UserSubmittedImage, object>>[] includeProperties)
        {
            IQueryable<UserSubmittedImage> query = context.UserSubmittedImages;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public UserSubmittedImage Find(int id)
        {
            return context.UserSubmittedImages.Find(id);
        }

        public void InsertOrUpdate(UserSubmittedImage usersubmittedimage)
        {
            if (usersubmittedimage.Id == default(int)) {
                // New entity
                context.UserSubmittedImages.Add(usersubmittedimage);
            } else {
                // Existing entity
                context.Entry(usersubmittedimage).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var usersubmittedimage = context.UserSubmittedImages.Find(id);
            context.UserSubmittedImages.Remove(usersubmittedimage);
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

    public interface IUserSubmittedImageRepository : IDisposable
    {
        IQueryable<UserSubmittedImage> All { get; }
        IQueryable<UserSubmittedImage> AllIncluding(params Expression<Func<UserSubmittedImage, object>>[] includeProperties);
        UserSubmittedImage Find(int id);
        void InsertOrUpdate(UserSubmittedImage usersubmittedimage);
        void Delete(int id);
        void Save();
    }
}