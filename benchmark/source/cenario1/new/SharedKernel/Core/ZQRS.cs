using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace SharedKernel.Core
{
    public interface IParameter
    {
        public ulong Id { get;}
    }

    #region command side

    public class CommandParameter:IParameter
    {
        public CommandParameter(ulong id)
        {
            Id = id;
        }

        public ulong Id { get;}
    }
    
    public interface ICommandHandler<TCommand> where TCommand:CommandParameter
    {
        Task Execute(TCommand parameter);
    }
    
    #endregion
    
    #region query side
    
    
    public class QueryFilter:IParameter
    {
        public QueryFilter(ulong id)
        {
            Id = id;
        }

        public ulong Id { get;}
        
    }

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
   
    public interface IQueryHandler<TResult, TFilter> 
    {
        Task<QueryResult<TResult>> Execute(TFilter filter);
    }

    public abstract class QueryHandler<TResult, TFilter> : IQueryHandler<TResult, TFilter>
        where TResult: class 
        where TFilter:QueryFilter
    {
        public async Task<QueryResult<TResult>> Execute(TFilter filter)
        {
            return await ExecuteQuery(filter);
        }

        protected abstract Task<QueryResult<TResult>> ExecuteQuery(TFilter filter);
    }
    
    #endregion
    
    #region persistence

    public interface IEventSubmitter
    {
        void Submit<TData>(string connectionString, TData data);
    }
    
   #region write model
    
   public interface IEventStore
   {
       void AppendEvent();
       void TakeSnapshot();
       void Replicate();
   }

   public interface IEventStoreExecutor
   {
       void Store();
   }
    
    #endregion
    
    #region read model
    
    
    #endregion
    
    #endregion
}