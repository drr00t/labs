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
    
    
    public interface ICommandHandler<TCommand>
    {
        Task Execute(TCommand parameter);
    }
    
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand:CommandParameter
    {
        
        public async Task Execute(TCommand command)
        {
            await ExecuteQuery(command);
        }
        
        private async Task ServerAsync()
        {
            using (var server = new RouterSocket("inproc://async"))
            {
                var (routingKey, more) = await server.ReceiveFrameStringAsync();
                var (message, _) = await server.ReceiveFrameStringAsync();

                // TODO: process message

                switch (true)
                {
                    
                }
                    
                server.SendMoreFrame(routingKey);
                server.SendFrame("Welcome");
            
            }
        }

        protected abstract Task ExecuteQuery(TCommand command);
        
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
        Task Submit<TData>(TData data);
    }

    public class EventSubmitter : IEventSubmitter
    {
        private readonly String[] _endPOints; 
        
        public EventSubmitter(String[] endPoints)
        {
            _endPOints = endPoints;
        }

        public async Task Submit<TData>(TData data)
        {
            throw new NotImplementedException();
        }

        private void Connect()
        {
           
        }
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