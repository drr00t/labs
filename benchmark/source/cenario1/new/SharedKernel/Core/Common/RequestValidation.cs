using System.Text.Json;
using NetMQ;
using NetMQ.Sockets;

namespace SharedKernel.Core
{
    public class RequestValidation<TData> where TData:class, IRequestValidation<TData>
    {
        public string Name { get; }

        public RequestValidation(string validatorEndPoint)
        {
            Name = validatorEndPoint;
        }
        
        public ValidationResult Validate(TData data)
        {
            using (var client = new RouterSocket())
            {
                client.Connect(Name);
                
                // FIXME: implementar suporte a Cancelamento via token
                
                // TODO: serialize and send parameter 
                var msg = new NetMQMessage();
                string readyData;

                var sData = JsonSerializer.Serialize(data);
                                   
                
                // convert result to NetMQMessage
                msg.AppendEmptyFrame();
                msg.Append(sData);

                client.SendMultipartMessage(msg);

                var result = client.ReceiveMultipartMessage();
                
                return JsonSerializer.Deserialize<ValidationResult>(result[2].ConvertToString());

            }
        }
    }
}