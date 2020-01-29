using RedisWebApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RedisWebApplication.Services
{
    public interface IMongoService<T> where T : BidEntity
    {
        //Task<T> Get(int bidId);
        Task<T> Get(Expression<Func<T, bool>> expression);
        Task Add(T element);
        Task Add(List<T> elements);
        Task Delete(int bidId);
    }
}
