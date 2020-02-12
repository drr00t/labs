using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKernel.Core
{
    public abstract class QueryHandler<TResult, TFilter> : IQueryHandler<TResult, TFilter>
        where TResult: class 
        where TFilter:QueryParameter
    {
        protected readonly ImmutableHashSet<TResult> _dataStore;
        protected readonly IQueryable<TResult> Context;

        public QueryHandler()
        {
            _dataStore = ImmutableHashSet<TResult>.Empty;
            
        }
        
        public async Task<QueryResult<TResult>> Execute(TFilter filter)
        {
            return await ExecuteQuery(filter);
        }

        protected abstract Task<QueryResult<TResult>> ExecuteQuery(TFilter filter);
    }
}