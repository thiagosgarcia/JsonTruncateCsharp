using System.Collections.Generic;
using System.Threading.Tasks;
using JsonTruncate;
using Newtonsoft.Json;
using Xunit;

namespace Tests
{
    public class UnitTests
    {
        private readonly object _obj = new
        {
            Id = 1,
            Reference = new
            {
                Id = 2,
                Reference = new
                {
                    Id = 3,
                    Reference = new
                    {
                        Id = 4
                    }
                }
            }
        };
        [Fact]
        public void TestDepth()
        {
            var result = _obj.SerializeObject(2);
            Assert.Equal(@"{""Id"":1,""Reference"":{""Id"":2,""Reference"":{}}}", result);
            
            // 0 depth resolves to nothing
            result = _obj.SerializeObject(0);
            Assert.Equal(@"{}", result);

            // negative depth resolves to full json
            result = _obj.SerializeObject(-1);
            Assert.Equal(@"{""Id"":1,""Reference"":{""Id"":2,""Reference"":{""Id"":3,""Reference"":{""Id"":4}}}}", result);
        }
        [Fact]
        public void TestListDepth()
        {
            var obj = new
            {
                Id = 1,
                Reference = new List<Obj>()
                {
                    new Obj()
                    {
                        Id = 2,
                        Reference = new Obj()
                        {
                             Id = 3,
                            Reference = new Obj()
                            {
                                Id = 4,
                                Reference = new Obj()
                            }
                        }
                    }
                }
            };
            var result = obj.SerializeObject(2);
            var result2 = obj.SerializeObject(3);
            Assert.Equal(@"{""Id"":1,""Reference"":[{""Id"":2,""Reference"":{}}]}", result);
            Assert.Equal(@"{""Id"":1,""Reference"":[{""Id"":2,""Reference"":{""Id"":3,""Reference"":{}}}]}", result2);

        }
        [Fact]
        public void TestSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MaxDepth = 2
            };

            var result = _obj.SerializeObject(settings);


            var obj2 = new Obj
            {
                Id = 1,
                Reference = new Obj
                {
                    Id = 2,
                    Reference = null
                }
            };
            var result2 = obj2.SerializeObject(settings);

            Assert.Equal(@"{""Id"":1,""Reference"":{""Id"":2,""Reference"":{}}}", result);
            Assert.Equal(@"{""Id"":1,""Reference"":{""Id"":2}}", result2);
        }
        [Fact]
        public void TestMultipleRunsSameInstance()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MaxDepth = 2
            };

            var txt1 = _obj.SerializeObject(settings);

            for(var i = 0; i < 10; i++)
            {
                var txt2 = _obj.SerializeObject(settings);
                Assert.Equal(txt1, txt2);
            }
        }
        [Fact]
        public void TestMultipleRunsSameInstanceParallel()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MaxDepth = 2
            };

            var txt1 = _obj.SerializeObject(settings);

            Parallel.For(0, 10, (_) =>
            {
                var txt2 = _obj.SerializeObject(settings);
                Assert.Equal(txt1, txt2);
            });
        }
    }


    internal class Obj
    {
        public int Id { get; set; }
        public Obj Reference { get; set; }
    }
}
