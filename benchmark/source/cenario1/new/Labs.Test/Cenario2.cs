using System;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Xunit;
using Xunit.Abstractions;

namespace Labs.Test
{
    public class Cenario2
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Cenario2(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ClientDealer_VentilatorRouter_ProcessorsRouter()
        {
            var endpointCommand = "inproc://127.0.0.1:5000";
            var endpointQuery = "inproc://127.0.0.1:5001";
            
            var endpointVentilator = "inproc://127.0.0.1:5100";
            
            // _testOutputHelper.WriteLine("New connection: {0}", args.Socket.Options.Identity.ToString());

            using (var runtime = new NetMQRuntime())
            {
                runtime.Run(ServerAsync(), ClientAsync());
            }
        }

        async Task ServerAsync()
        {
            using (var server = new RouterSocket())
            {
                server.Options.Identity = Encoding.UTF8.GetBytes("inproc://async");
                server.Bind("inproc://async");
                _testOutputHelper.WriteLine("ready to process");
                
                // for (int i = 0; i < 1000; i++)
                // {
                    var (routingKey, more) = await server.ReceiveRoutingKeyAsync();
                    var (message, _) = await server.ReceiveFrameStringAsync();

                    // TODO: process message
                    _testOutputHelper.WriteLine("New connection: {0} {1}", routingKey.ToString(),message);

                    await Task.Delay(100);
                    server.SendMoreFrame(routingKey);
                    server.SendFrame("Welcome");
                // }
            }
        }

        async Task ClientAsync()
        {
            Task.Delay(2000);
            
            using (var client = new DealerSocket())
            {
                client.Connect("inproc://async");
                
                for (int i = 0; i < 1000; i++)
                {
                    client.SendFrame("Hello");
                    var (message, more) = await client.ReceiveFrameStringAsync();

                    // TODO: process reply
                    _testOutputHelper.WriteLine("response: {0}", message);
                    
                    await Task.Delay(100);
                }
            }
        }
    }
}
