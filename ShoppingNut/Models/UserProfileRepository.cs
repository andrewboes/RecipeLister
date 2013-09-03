using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class UserProfileRepository : IUserProfileRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<UserProfile> All
        {
            get { return context.UserProfiles; }
        }

        public IQueryable<UserProfile> AllIncluding(params Expression<Func<UserProfile, object>>[] includeProperties)
        {
            IQueryable<UserProfile> query = context.UserProfiles;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public UserProfile Find(int id)
        {
            return context.UserProfiles.Find(id);
        }

        public void InsertOrUpdate(UserProfile userprofile)
        {
            if (userprofile.UserId == default(int)) {
                // New entity
                context.UserProfiles.Add(userprofile);
            } else {
                // Existing entity
                context.Entry(userprofile).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var userprofile = context.UserProfiles.Find(id);
            context.UserProfiles.Remove(userprofile);
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

    public interface IUserProfileRepository : IDisposable
    {
        IQueryable<UserProfile> All { get; }
        IQueryable<UserProfile> AllIncluding(params Expression<Func<UserProfile, object>>[] includeProperties);
        UserProfile Find(int id);
        void InsertOrUpdate(UserProfile userprofile);
        void Delete(int id);
        void Save();
    }
}