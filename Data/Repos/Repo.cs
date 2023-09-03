using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repos.IRepos;
using Microsoft.EntityFrameworkCore;

namespace Data.Repos
{
    public class Repo<T> : IRepo<T> where T : class
    {
        private readonly AppDbContext _db;

        public Repo(AppDbContext db)
        {
            _db = db;
        }

        public void Add(T item)
        {
            _db.Set<T>().Add(item);
        }

        public void Delete(T item)
        {
            _db.Set<T>().Remove(item);
        }

        public T Get(Expression<Func<T, bool>> match)
        {
            return _db.Set<T>().FirstOrDefault(match);
        }

        public List<T> GetAll()
        {
            return _db.Set<T>().ToList();
        }

        public List<T> GetAll(params string[] tables)
        {
            IQueryable<T> q =  _db.Set<T>();
            foreach (var table in tables)
            {
                q =  q.Include(table);
            }
            return q.ToList();
        }

        public T GetById(int id)
        {
            return _db.Set<T>().Find(id);
        }

        public T First(Expression<Func<T, bool>> match)
        {
            return _db.Set<T>().FirstOrDefault(match);
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
    

}


