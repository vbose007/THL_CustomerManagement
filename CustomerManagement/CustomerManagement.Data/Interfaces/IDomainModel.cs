
using System;

namespace CustomerManagement.Data.Interfaces
{
    public interface IDomainModel<TEntity> where TEntity : class
    {
            TEntity ToModel();
            IDomainModel<TEntity> FromModel(TEntity model);
            TEntity UpdateModel(TEntity model);
            object GetKey();
        }
}