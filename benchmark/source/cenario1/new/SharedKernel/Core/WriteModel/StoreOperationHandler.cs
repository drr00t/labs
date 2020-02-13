using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace SharedKernel.Core
{
    public abstract class StoreOperationHandler: OperationHandler<StoreCommand>
    {
      
        public StoreOperationHandler(string externalEndPoint, string replicationEndPoint)
            :base(externalEndPoint,replicationEndPoint)
        {

        }

        protected override Task ExecuteOperation(StoreCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}