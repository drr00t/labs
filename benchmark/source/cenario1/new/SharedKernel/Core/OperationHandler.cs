using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace SharedKernel.Core
{
    public abstract class OperationHandler<TCommand>
    {
        private readonly string _externalEndPoint;
        private readonly string _replicationEndPoint;
        
        protected OperationHandler(string externalEndPoint, string replicationEndPoint)
        {
            _externalEndPoint = externalEndPoint;
            _replicationEndPoint = replicationEndPoint;
            
            using (var runtime = new NetMQRuntime())
            {
                runtime.Run(ServerAsync()); //, ServerReplicationAsync());
            }            
        }
        
        private async Task ServerAsync()
        {
            using (var server = new RouterSocket())
            {
                server.Options.Identity = Encoding.UTF8.GetBytes(_externalEndPoint);
                server.Bind(_externalEndPoint);
                
                while (true)
                {
                    var (routingKey, more) = await server.ReceiveFrameStringAsync();
                    
                    var message = server.ReceiveMultipartMessage();
                
                    // execute something but not return anything, tipo salvar no banco o
                    // Store do event source será esse cara
                    
                    await ExecuteOperation(default(TCommand));
                    
                    // publicar a modifcação conforme regra, aqui pensei num timer que
                    // fazendo o diff ou alguma outra estratégia  
                
                    server.SendMoreFrame(routingKey);
                    server.SendFrame("Welcome");

                }
            }
        }

        private async Task ServerReplicationAsync()
        {
            using (var server = new XPublisherSocket())
            {
                server.Options.Identity = Encoding.UTF8.GetBytes(_replicationEndPoint);
                server.SetWelcomeMessage("OLLEH");
                server.Bind(_replicationEndPoint);
                
                while (true)
                {
                    // execute something but not return anything, tipo salvar no banco o
                    // Store do event source será esse cara
                    
                    await ExecuteOperation(default(TCommand));
                    
                    // publicar a modifcação conforme regra, aqui pensei num timer que
                    // fazendo o diff ou alguma outra estratégia  
                    var msg = new NetMQMessage();
                    
                    msg.Append("topic");
                    msg.AppendEmptyFrame();
                    
                    //msg.Append( data_will_be_here);
                    
                    server.SendMultipartMessage(msg);

                }
            }
        }

        protected abstract Task ExecuteOperation(TCommand command);
        
    }
}