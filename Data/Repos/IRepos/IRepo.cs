using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos.IRepos
{
    public interface IRepo<T> where T : class
    {
        List<T> GetAll();

        List<T> GetAll(params string[] tables);

        T GetById(int id);

        T First(Expression<Func<T, bool>> predicate);

        T Get(Expression<Func<T, bool>> match);

        void Add(T item);

        void Delete(T item);

        void SaveChanges();

    }
}
