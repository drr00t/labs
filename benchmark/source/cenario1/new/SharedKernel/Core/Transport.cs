using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace SharedKernel.Core
{
    public class Transport
    {
        public static class Constants
        {
            public static int ENVELOP_ROUTE_ID { private set; get; } = 0;
            public static int ENVELOP_COMMAND { private set; get; } = 1;
        }

        public interface IHandler
        {
            Task<NetMQFrame> Handle(NetMQMessage data);
        }

        public class Broker
        {
            private readonly string[] WORKER_ENDPOINT = {"inproc://WORKER:{0}"};
            private readonly IList<RouterSocket> _workers = new List<RouterSocket>();
            private readonly IList<string> _busyWorkers = new List<string>();

            
            public Broker()
            {
                using (var runtime = new NetMQRuntime())
                {
                    runtime.Run(WorkerAsync(), ReceiverAsync());
                }
            }

            async Task WorkerAsync()
            {
                using (var worker = new RouterSocket())
                {
                    var endPoint = String.Format(WORKER_ENDPOINT[0], 1);
                    worker.Options.Identity = Encoding.UTF8.GetBytes(endPoint);
                    worker.Bind(endPoint);
                    
                    while(true)
                    {
                        var (routingKey, more) = await worker.ReceiveRoutingKeyAsync();
                        var (command, _) = await worker.ReceiveFrameStringAsync();
                        await worker.SkipFrameAsync();
                        var (body, _) = await worker.ReceiveFrameStringAsync();
                        
                        
                    }
                }
            }
            
            void ConnectWorkers()
            {
                var worker = new RouterSocket();
                var endPoint = String.Format(WORKER_ENDPOINT[0], 1);
                worker.Connect(endPoint);
                _workers.Add(worker);
            }


            async Task ReceiverAsync()
            {
                // devo usar um timer aqui NetMQTimer com Pooller
                ConnectWorkers();
                using (var server = new RouterSocket())
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        var (routingKey, more) = await server.ReceiveRoutingKeyAsync();
                        var (message, _) = await server.ReceiveFrameStringAsync();

                        // TODO: process message

                        await Task.Delay(100);
                        server.SendMoreFrame(routingKey);
                        server.SendFrame("Welcome");
                    }
                }
            }

            async Task Publishersync()
            {
                using (var publisher = new XPublisherSocket())
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        publisher.SendFrame("Hello");
                        var (message, more) = await publisher.ReceiveFrameStringAsync();

                        // TODO: process reply

                        await Task.Delay(100);
                    }
                }
            }

        }
        
        public class Producer<TStrategy>
        {
            private readonly string _endPoint;

            public Producer(string endpoint)
            {
                _endPoint = endpoint;
                using (var runtime = new NetMQRuntime())
                {
                    runtime.Run(ClientAsync());
                }
            }

            async Task ClientAsync()
            {
                using (var client = new DealerSocket("inproc://async"))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        client.SendFrame("Hello");
                        var (message, more) = await client.ReceiveFrameStringAsync();

                        // TODO: process reply

                        await Task.Delay(100);
                    }
                }
            }
        }
        
        public class Consumer<TStrategy>
        {
            private readonly string _endPoint;
            
            public Consumer(string endpoint)
            {
                _endPoint = endpoint;
                using (var runtime = new NetMQRuntime())
                {
                    runtime.Run(ReceiverAsync());
                }   
            }
            
            async Task ReceiverAsync()
            {
                using (var server = new RouterSocket("inproc://async"))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        var (routingKey, more) = await server.ReceiveRoutingKeyAsync();
                        var (message, _) = await server.ReceiveFrameStringAsync();

                        // TODO: process message

                        await Task.Delay(100);
                        server.SendMoreFrame(routingKey);
                        server.SendFrame("Welcome");
                    }
                }
            }
        }
        
        public interface IConsumer<THandleStrategy> : IDisposable
        {
            void Receive();
        }

        public class ReceiverReqReply<THandleStrategy> //: IReceiver<THandleStrategy> where THandleStrategy : IHandler
        {
            private readonly THandleStrategy _handleStrategy;
            private readonly String _endPoint;
            private readonly ConcurrentQueue<NetMQMessage> _messageBuffer = new ConcurrentQueue<NetMQMessage>();

            public ReceiverReqReply(String endPoint, THandleStrategy handleStrategy)
            {
                _endPoint = endPoint;
                _handleStrategy = handleStrategy;
                var server = new Task(Receive, TaskCreationOptions.LongRunning);

                server.Start();
            }

            public async void Receive()
            {
                using (var server = new RouterSocket())
                {
                    server.Options.Identity = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
                    server.Options.RouterMandatory = true;
                    server.Bind($"{_endPoint}");
                }
            }
        }
    }
}