using System;
using System.Text;
using System.Threading.Tasks;
using NBench;
using NetMQ;
using NetMQ.Sockets;
using Xunit;
using Xunit.Abstractions;

namespace Labs.Test
{
    public class SharedKernelTests
    {
        private Counter _requestCounter;
        private readonly ITestOutputHelper _testOutputHelper;

        public SharedKernelTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void Setup(BenchmarkContext context)
        {
            _requestCounter = context.GetCounter("Requests");
        }

        [PerfBenchmark(NumberOfIterations = 10, RunMode = RunMode.Throughput, RunTimeMilliseconds = 1000, TestMode = TestMode.Measurement)]
        [CounterMeasurement("Requests")]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public void BenchmarkMethod(BenchmarkContext context)
        {
            var bytes = new byte[1024];
            _requestCounter.Increment();

        }
    }
}
