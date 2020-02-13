using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace SharedKernel.Core
{
    public abstract class CommandHandler<TCommand> : Operation, ICommandHandler<TCommand>
        where TCommand:CommandParameter
    {
        
        public CommandHandler(string endPoint, IValidator validator)
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
}