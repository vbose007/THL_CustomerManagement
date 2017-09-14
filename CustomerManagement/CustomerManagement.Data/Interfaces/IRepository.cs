using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CustomerManagement.Data.Interfaces
{
    public interface IRepository<TEntity, TModel> where TEntity : class where TModel : IDomainModel<TEntity>
    {
        ICollection<TModel> GetAll();

        ICollection<TModel> Get(Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, object>> orderBy = null, bool orderAsc = true, int pageSize = 10, int pageNumber=1);

        TModel Get(object key);
        
        int Count(Expression<Func<TEntity, bool>> where = null);

        TModel Add(TModel model);

        int Update(TModel model);

        int Delete(object key);

        int Delete(Expression<Func<TEntity, bool>> where);
    }
}