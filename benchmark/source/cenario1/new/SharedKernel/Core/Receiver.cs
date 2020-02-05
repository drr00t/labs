using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace SharedKernel.Core
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
    
    public interface IReceiver<THandleStrategy>: IDisposable
    {
        void Receive();
    }

    public class ReceiverReqReply<THandleStrategy>: IReceiver<THandleStrategy> where THandleStrategy:IHandler
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

                try
                {

                    Action receiver = () =>
                    {
                        while (true)
                        {
                            var message = server.ReceiveMultipartMessage();
                            _messageBuffer.Enqueue(message);
                        }
                    };

                    Action processor = async () =>
                    {
                        NetMQMessage msg;
                        while (_messageBuffer.TryDequeue(out msg))
                        {
                            var responseData = await _handleStrategy.Handle(msg);

                            var messageToRouter = new NetMQMessage();
                            messageToRouter.Append(msg[Constants.ENVELOP_ROUTE_ID]);
                            messageToRouter.AppendEmptyFrame();
                            messageToRouter.Append(responseData);

                            server.SendMultipartMessage(messageToRouter);
                        }
                    };
                    
                    Parallel.Invoke(receiver,processor);
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        public void Dispose()
        {
            NetMQConfig.Cleanup();
        }
    }
}