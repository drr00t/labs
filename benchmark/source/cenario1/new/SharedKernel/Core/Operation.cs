using System.Text.Json;
using NetMQ;
using NetMQ.Sockets;

namespace SharedKernel.Core
{
    public class Operation : IOperation
    {
        private string _endPoint;
        public Operation(string endPoint)
        {
            _endPoint = endPoint;
        }
        public async void Post<TParameter>(TParameter parameter)
        {
            using (var server = new RouterSocket(_endPoint))
            {
                var (routingKey, more) = await server.ReceiveFrameStringAsync();
                var (message, _) = await server.ReceiveFrameStringAsync();

                // TODO: serialize and send parameter 
                var msg = new NetMQMessage();

                var data = JsonSerializer.Serialize(parameter);
                msg.Append(data);
                
                server.SendMultipartMessage(msg);

                var result = server.ReceiveMultipartMessage();

            }
        }
    }
}