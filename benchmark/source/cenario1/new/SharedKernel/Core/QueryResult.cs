using System.Collections.Generic;

namespace SharedKernel.Core
{
    public class QueryResult<TData>
    {
        public ulong Page { get;}
        public ulong Count { get;}
        public IEnumerable<TData> Data { get;}

        public QueryResult(IEnumerable<TData> data, ulong count, ulong page)
        {
            Data = data;
            Count = count;
            Page = page;
        }
    }
}