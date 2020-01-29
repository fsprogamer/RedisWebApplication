using RedisWebApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RedisWebApplication.Repositiries.Mongo
{
    public interface IMongoRepository<T> where T : BidEntity
    {
        //Task<List<T>> GetByBidId(T entity);
        Task<List<T>> Get(Expression<Func<T, bool>> filter);
        Task Insert(T entity);
        Task Insert(IEnumerable<T> values);
        Task<long> Delete(T entity);
        Task<long> Delete(IEnumerable<T> entityList);
    }
}
