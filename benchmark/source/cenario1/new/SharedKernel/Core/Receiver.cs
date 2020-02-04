using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace SharedKernel.Core
{
    public interface IHandler
    {
        NetMQFrame Handle(NetMQMessage data);
    }
    
    public interface IReceiver<THandleStrategy>: IDisposable
    {
        void Receive();
    }

    public class ReceiverReqReply<THandleStrategy>: IReceiver<THandleStrategy> where THandleStrategy:IHandler
    {
        private readonly THandleStrategy _handleStrategy;
        private readonly String _endPoint;

        public ReceiverReqReply(String endPoint, THandleStrategy handleStrategy)
        {
            _endPoint = endPoint;
            _handleStrategy = handleStrategy;

            var server = new Task(() => Receive(),
                TaskCreationOptions.LongRunning);
                
            server.Start();
        }
        
        public void Receive()
        {
            using (var server = new RouterSocket())
            {
                server.Options.Identity = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
                server.Options.RouterMandatory = true;
                server.Bind($"{_endPoint}");

                try
                {
                    while (true)
                    {
                        var message = server.ReceiveMultipartMessage();
                             
                        var responseData = _handleStrategy.Handle(message); 
                        
                        var messageToRouter = new NetMQMessage();
                        messageToRouter.Append(message[0]);
                        messageToRouter.AppendEmptyFrame();
                        messageToRouter.Append(responseData);

                        server.SendMultipartMessage(messageToRouter);
                    }
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