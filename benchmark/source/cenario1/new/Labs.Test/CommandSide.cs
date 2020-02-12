using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedKernel.Core;
using Xunit;

namespace Labs.Test
{
    public class CommandSideTests
    {
        class MyParameter: CommandParameter
        {
            public String Name { get;}
            
            public MyParameter(ulong id, string name) : base(id)
            {
                Name = name;
            }
        }

        class TestCommandHandler : CommandHandler<MyParameter>
        {
            public TestCommandHandler(string endPoint, IValidator validator) : base(endPoint, validator)
            {
            }
            
            protected override async Task ExecuteCommand(MyParameter command)
            {
                await Task.FromResult(new List<MyParameter>());
                
                this.Post(command);
            }
        }
            
        [Fact]
        public void CommandParameter_Values()
        {
            ulong id = 121212L;
            var name = "name";
            
            var parameter = new MyParameter(id,name);
            Assert.True(parameter.Id == id);
            Assert.True(parameter.Name == name);
        }
        
        [Fact]
        public async void TestQueryHandler_simpleTest()
        {
            ulong id = 121212L;
            var name = "name";
            
            var parameter = new MyParameter(id,name);
            
        }
    }
}
