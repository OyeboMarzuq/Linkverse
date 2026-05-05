using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IServices
{
    public interface ICacheService
    {
        Task<T> GetOrSetAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan ttl,
            CancellationToken cancellationToken = default);

        T? Get<T>(string key);

        void Set<T>(string key, T value, TimeSpan ttl);

        void Remove(string key);
        void RemoveByPrefix(string prefix);
    }
}
