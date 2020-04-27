using System.Collections.Generic;
using System.Threading.Tasks;
using JsonTruncate;
using Newtonsoft.Json;
using Xunit;

namespace Tests
{
    public class DeserializeTests
    {
        private readonly string _str = @"{""Id"":1,""Reference"":{""Id"":2,""Reference"":{""Id"":3,""Reference"":{""Id"":4}}}}";
        private readonly object _obj = new Obj()
        {
            Id = 1,
            Reference = new Obj()
            {
                Id = 2,
                Reference = new Obj()
                {
                    Id = 3,
                    Reference = new Obj()
                    {
                        Id = 4,
                        Reference = null
                    }
                }
            }
        };
        private readonly object _obj2 = new Obj()
        {
            Id = 1,
            Reference = new Obj()
            {
                Id = 2,
                Reference = new Obj()
                {
                    Id = 0,
                    Reference = null
                }
            }
        };
        [Fact]
        public void TestDepth()
        {
            var settings = new JsonSerializerSettings()
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,

            };
            var result = _str.DeserializeObject<Obj>(2, settings);
            Assert.Equal(JsonConvert.SerializeObject(_obj2), JsonConvert.SerializeObject(result));

            //0 depth resolves to nothing
            result = _str.DeserializeObject<Obj>(0, settings);
            Assert.Equal(JsonConvert.SerializeObject(new Obj()), JsonConvert.SerializeObject(result));

            // negative depth resolves to full json
            result = _str.DeserializeObject<Obj>(-1, settings);
            Assert.Equal(JsonConvert.SerializeObject(_obj), JsonConvert.SerializeObject(result));
        }
        [Fact]
        public void TestListDepth()
        {
            var str5 = @"{""Id"":1,""Reference"":[{""Id"":2,""Reference"":{""Id"":3,""Reference"":{""Id"":4,""Reference"":{""Id"":5,""Reference"":null}}}}]}";

            var obj = new ObjL
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
            var obj2 = new ObjL
            {
                Id = 1,
                Reference = new List<Obj>()
                {
                    new Obj()
                    {
                        Id = 2,
                        Reference = new Obj()
                        {
                        }
                    }
                }
            };
            var obj3 = new ObjL
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
                            }
                        }
                    }
                }
            };
            var result2 = str5.DeserializeObject<ObjL>(2);
            var result3 = str5.DeserializeObject<ObjL>(3);
            Assert.Equal(JsonConvert.SerializeObject(obj2), JsonConvert.SerializeObject(result2));
            Assert.Equal(JsonConvert.SerializeObject(obj3), JsonConvert.SerializeObject(result3));

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

            var txt1 = _str.DeserializeObject<Obj>(settings);

            for (var i = 0; i < 10; i++)
            {
                var txt2 = _str.DeserializeObject<Obj>(settings);
                Assert.Equal(JsonConvert.SerializeObject(txt1), JsonConvert.SerializeObject(txt2));
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
            
            var txt1 = _str.DeserializeObject<Obj>(settings);

            Parallel.For(0, 10, (_) =>
            {
                var txt2 = _str.DeserializeObject<Obj>(settings);
                Assert.Equal(JsonConvert.SerializeObject(txt1), JsonConvert.SerializeObject(txt2));
            });
        }
    }

    public class ObjL
    {
        public int Id { get; set; }
        public List<Obj> Reference { get; set; }
    }
}
