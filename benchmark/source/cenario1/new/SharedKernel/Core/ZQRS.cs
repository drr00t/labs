using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace SharedKernel.Core
{
    public interface IParameter
    {
        public ulong Id { get;}
    }

    public interface IOperation
    {
        void Post<TParameter>(TParameter parameter);
    }

    public class Operation : IOperation
    {
        private string _endPoint;
        public Operation(string endPoint)
        {
            _endPoint = endPoint;
        }
        public async void Post<TParameter>(TParameter parameter)
        {
            using (var server = new RouterSocket("inproc://async"))
            {
                var (routingKey, more) = await server.ReceiveFrameStringAsync();
                var (message, _) = await server.ReceiveFrameStringAsync();

                // TODO: serialize and send parameter 
                var msg = new NetMQMessage();
                
                server.SendMultipartMessage(msg);
            
            }
        }
    }
    
    
    public interface IValidator<TData>
    {
        string Name { get; }      
    }
    
    public class Validator<TData> : IValidator<TData> where TData:new() 
    {
        private string _endPoint;
        

        public string Name { get; }
        
        public Validator(string endPoint)
        {
            _endPoint = endPoint;
            Name = endPoint;

            using (var runtime = new NetMQRuntime())
            {
                runtime.Run(StartServer());
            }
        }
        
        public async Task StartServer()
        {
            using (var server = new RouterSocket(Name))
            {
                /// implementar suporte a Cancelamento via token
                while (true)
                {
                    var (routingKey, more) = await server.ReceiveFrameStringAsync();
                    var (message, _) = await server.ReceiveFrameStringAsync();

                    // TODO: serialize and send parameter 
                    var msg = new NetMQMessage();
                
                    server.SendMultipartMessage(msg);

                }
            }
        }
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
    
    public abstract class CommandHandler<TCommand> : Operation, ICommandHandler<TCommand>
        where TCommand:CommandParameter
    {
        
        public CommandHandler(string endPoint, IValidator<int> validator)
            :base(endPoint)
        {
            
            using (var runtime = new NetMQRuntime())
            {
                runtime.Run(ServerAsync());
            }            
        }
        
        public async Task Execute(TCommand command)
        {
            await ExecuteCommand(command);
        }
        
        private async Task ServerAsync()
        {
            using (var server = new RouterSocket("inproc://async"))
            {
                var (routingKey, more) = await server.ReceiveFrameStringAsync();
                var (message, _) = await server.ReceiveFrameStringAsync();

                
                // TODO: se o COmmandSubmitter existir então deserializamos o comando aqui para processá-lo
                
                // await ExecuteCommand(message);
                
                var validatorsRouter = new RouterSocket();
                // validatorsRouter.Connect(");
                server.SendMoreFrame(routingKey);
                server.SendFrame("Welcome");
            
            }
        }

        protected abstract Task ExecuteCommand(TCommand command);
        
    }
    
    #endregion
    
    #region query side
    
    
    public class QueryParameter:IParameter
    {
        public QueryParameter(ulong id)
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
    
    #endregion
    
    #region persistence
    
    /*
     * message  fields
     *     route key
     *     operation string
     *     empty
     *     fragmented indicator boolean
     *     fragment number
     *     fraglimit
     *     data blob
     * 
     *     states
     *         hello
     *         ready
     *         processing
     *         failed
     *     
     */

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