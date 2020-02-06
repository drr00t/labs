using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedKernel.Core;
using Xunit;

namespace Labs.Test
{
    public class ZQRS_QuerySide
    {
        class MyParameter: QueryFilter
        {
            public String Name { get;}
            
            public MyParameter(ulong id, string name) : base(id)
            {
                Name = name;
            }
        }

        class MyItem
        {
            public string name { get; set; }
        }

        class TestQueryFilter : QueryFilter
        {
            public string Name { get; }
            
            public TestQueryFilter(ulong id, string name) : base(id)
            {
                Name = name;
            }
        }

        class TestQueryHandler : QueryHandler<MyItem, TestQueryFilter>
        {
            protected override async Task<QueryResult<MyItem>> ExecuteQuery(TestQueryFilter filter)
            {
                var result = await Task.FromResult(new QueryResult<MyItem>(new []{new MyItem(),new MyItem()}, 2, 0));

                return result;
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

            var query = new TestQueryHandler();
            var result = await query.Execute(new TestQueryFilter(id, name));
            
            Assert.True(result.Count == 2);
            Assert.True(result.Page == 0);
        }
    }
}
