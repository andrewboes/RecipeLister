using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShoppingNut.Models
{ 
    public class InstructionRepository : IInstructionRepository
    {
        ShoppingNutContext context = new ShoppingNutContext();

        public IQueryable<Instruction> All
        {
            get { return context.Instructions; }
        }

        public IQueryable<Instruction> AllIncluding(params Expression<Func<Instruction, object>>[] includeProperties)
        {
            IQueryable<Instruction> query = context.Instructions;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Instruction Find(int id)
        {
            return context.Instructions.Find(id);
        }

        public void InsertOrUpdate(Instruction instruction)
        {
            if (instruction.Id == default(int)) {
                // New entity
                context.Instructions.Add(instruction);
            } else {
                // Existing entity
                context.Entry(instruction).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var instruction = context.Instructions.Find(id);
            context.Instructions.Remove(instruction);
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

    public interface IInstructionRepository : IDisposable
    {
        IQueryable<Instruction> All { get; }
        IQueryable<Instruction> AllIncluding(params Expression<Func<Instruction, object>>[] includeProperties);
        Instruction Find(int id);
        void InsertOrUpdate(Instruction instruction);
        void Delete(int id);
        void Save();
    }
}