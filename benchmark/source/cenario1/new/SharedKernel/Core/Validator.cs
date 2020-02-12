using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace SharedKernel.Core
{
    public abstract class Validator<TData> : IValidator where TData:new() 
    {
        private string _endPoint;
        
        public string Name { get; }
        
        protected Validator(string endPoint)
        {
            _endPoint = endPoint;
            Name = endPoint;

            using (var runtime = new NetMQRuntime())
            {
                runtime.Run(StartServer());
            }
        }

        protected abstract Task<ValidationResult> ValidationRule();
        
        private async Task StartServer()
        {
            using (var server = new RouterSocket())
            {
                server.Options.Identity = Encoding.UTF8.GetBytes(Name);
                server.Options.RouterMandatory = true;
                server.Bind(Name);
                
                // FIXME: implementar suporte a Cancelamento via token
                
                while (true)
                {
                    var (routingKey, more) = await server.ReceiveFrameStringAsync();
                    var (message, _) = await server.ReceiveFrameStringAsync();

                    // TODO: serialize and send parameter 
                    var msg = new NetMQMessage();

                    var result = await ValidationRule();
                    
                    // convert result to NetMQMessage
                    msg.Append(routingKey);
                    msg.AppendEmptyFrame();
                    msg.Append( "result");
                    
                    server.SendMultipartMessage(msg);
                }
            }
        }
    }
}