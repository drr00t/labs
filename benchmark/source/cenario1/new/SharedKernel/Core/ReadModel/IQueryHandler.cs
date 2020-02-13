using System.Threading.Tasks;

namespace SharedKernel.Core
{
    public interface IQueryHandler<TResult, TFilter> 
    {
        Task<QueryResult<TResult>> Execute(TFilter filter);
    }
}