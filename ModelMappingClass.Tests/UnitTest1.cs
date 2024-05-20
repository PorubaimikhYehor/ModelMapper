using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace ModelMappingClass.Tests
{
    public class ModelMapperTests : IDisposable
    {
        private readonly ModelMapper _mapper;
        private readonly ITestOutputHelper _output;

        public ModelMapperTests(ITestOutputHelper output)
        {
            _mapper = new ModelMapper();
            _output = output;
        }

        [Fact]
        public void Map_SameType_ReturnsSameObject()
        {
            int source = 5;
            int result = _mapper.Map<int, int>(source);

            _output.WriteLine($"Source: {source}, Result: {result}");
            Assert.Equal(source, result);
        }

        [Fact]
        public void Map_PrimitiveTypes_ReturnsMappedObject()
        {
            string source = "Hello";
            string result = _mapper.Map<string, string>(source);

            _output.WriteLine($"Source: {source}, Result: {result}");
            Assert.Equal(source, result);
        }

        [Fact]
        public void Map_ListOfPrimitives_ReturnsMappedList()
        {
            var source = new List<int> { 1, 2, 3 };
            var result = _mapper.Map<List<int>, List<int>>(source);

            _output.WriteLine($"Source: {string.Join(", ", source)}, Result: {string.Join(", ", result)}");
            Assert.Equal(source, result);
        }
        [Fact]
        public void Map_ListOfString_ReturnsMappedList()
        {
            var source = new List<string> { "s1", "s2", "s3" };
            var result = _mapper.Map<List<string>, List<string>>(source);

            _output.WriteLine($"Source: {string.Join(", ", source)}, Result: {string.Join(", ", result)}");
            Assert.Equal(source, result);
        }

        [Fact]
        public void Map_ListOfString_ReturnsMappedList2()
        {
            var source = new List<string> { "" };
            var result = _mapper.Map<List<string>, List<string>>(source);

            _output.WriteLine($"Source: {string.Join(", ", source)}, Result: {string.Join(", ", result)}");
            Assert.Equal(source, result);
        }

        [Fact]
        public void Map_ObjectToString_EmptyString()
        {
            var source = new SourceClass() {Name = "test"};
            string result = _mapper.Map<SourceClass, string>(source);

            _output.WriteLine($"Source: {source.Name}, Result: {result}");
            Assert.Empty(result);
        }

        [Fact]
        public void Map_StringToObject_EmptyObject()
        {
            string source = "test";
            SourceClass result = _mapper.Map<string, SourceClass>(source);

            _output.WriteLine($"Source: {source}, Result: {result.ToString()}");
            Assert.NotNull(result);
            Assert.Equal(typeof(SourceClass), result.GetType());

        }
        [Fact]
        public void Map_Dictionary_ReturnsMappedDictionary()
        {
            var source = new Dictionary<string, int>
            {
                { "one", 1 },
                { "two", 2 }
            };
            var result = _mapper.Map<Dictionary<string, int>, Dictionary<string, int>>(source);

            _output.WriteLine($"Source: {string.Join(", ", source)}, Result: {string.Join(", ", result)}");
            Assert.Equal(source, result);
        }

        [Theory]
        [InlineData(SourceEnum.Two, DestinationEnum.Two)]
        [InlineData(SourceEnum.Three, DestinationEnum.One)]
        public void Map_Enum_ReturnsMappedEnum(SourceEnum source, DestinationEnum expected)
        {
            DestinationEnum result = _mapper.Map<SourceEnum, DestinationEnum>(source);

            _output.WriteLine($"Source: {source}, Result: {result}");
            Assert.Equal(expected, result);
        }
        [Theory]
        [InlineData("Two", DestinationEnum.Two)]
        [InlineData("Three", DestinationEnum.One)]
        public void Map_SyringToEnum_ReturnsMappedEnum(string source, DestinationEnum expected)
        {
            DestinationEnum result = _mapper.Map<string, DestinationEnum>(source);

            _output.WriteLine($"Source: {source}, Result: {result}");
            Console.WriteLine($"Source: {source}, Result: {result}");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Map_EnumToString_ReturnsString()
        {
            SourceEnum source = SourceEnum.One;
            string result = _mapper.Map<SourceEnum, string>(source);

            _output.WriteLine($"Source: {source}, Result: {result}");
            Assert.Equal(source.ToString(), result);
        }

        [Fact]
        public void Map_StringToEnum_ReturnsMappedEnum()
        {
            SourceEnum source = SourceEnum.One;
            DestinationEnum result = _mapper.Map<SourceEnum, DestinationEnum>(source);

            _output.WriteLine($"Source: {source}, Result: {result}");
            Assert.Equal(DestinationEnum.One, result);
        }

        [Fact]
        public void Map_Object_ReturnsMappedObject()
        {
            var source = new SourceClass
            {
                Id = 1,
                Name = "Source",
                SubClass = new SubClass { Value = "Sub" }
            };
            var result = _mapper.Map<SourceClass, DestinationClass>(source);

            _output.WriteLine($"Source: {source}, Result: {result}");
            Assert.NotNull(result);
            Assert.Equal(source.Id, result.Id);
            Assert.Equal(source.Name, result.Name);
            Assert.NotNull(result.SubClass);
            Assert.Equal(source.SubClass?.Value, result.SubClass?.Value);
        }

        [Theory]
        [InlineData(5, "")]
        [InlineData(0, "")]
        [InlineData(-1, "")]
        public void Map_DifferentPrimitiveTypes_ReturnsDefaultDestinationType(int source, string expected)
        {
            string result = _mapper.Map<int, string>(source);

            _output.WriteLine($"Source: {source}, Result: {result}");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Map_ListWithDifferentItemType_ReturnsListWithDefaultValues()
        {
            var source = new List<int> { 1, 2, 3 };
            var result = _mapper.Map<List<int>, List<string>>(source);

            var expected = new List<string> { string.Empty, string.Empty, string.Empty };

            _output.WriteLine($"Source: {string.Join(", ", source)}, Result: {string.Join(", ", result)}");
            Assert.NotNull(result);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Map_DictionaryWithDifferentKeyType_ReturnsEmptyDictionary()
        {
            var source = new Dictionary<string, int>
            {
                { "one", 1 },
                { "two", 2 }
            };
            var result = _mapper.Map<Dictionary<string, int>, Dictionary<int, int>>(source);

            _output.WriteLine($"Source: {string.Join(", ", source)}, Result: {string.Join(", ", result)}");
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Map_ObjectWithDifferentTypes_ReturnsDefaultDestinationType()
        {
            var source = new SourceClass
            {
                Id = 1,
                Name = "Source",
                SubClass = new SubClass { Value = "Sub" }
            };
            var result = _mapper.Map<SourceClass, AnotherClass>(source);

            _output.WriteLine($"Source: {source}, Result: {result}");
            Assert.NotNull(result);
            Assert.Equal(default(int), result.AnotherId);
            Assert.Null(result.AnotherName);
        }

        [Fact]
        public void Map_ListOfObjects_ReturnsMappedList()
        {
            var source = new List<SourceClass>
            {
                new SourceClass { Id = 1, Name = "Item1", SubClass = new SubClass { Value = "Sub1" } },
                new SourceClass { Id = 2, Name = "Item2", SubClass = new SubClass { Value = "Sub2" } }
            };
            var result = _mapper.Map<List<SourceClass>, List<DestinationClass>>(source);

            Assert.NotNull(result);
            Assert.Equal(source.Count, result.Count);

            for (int i = 0; i < source.Count; i++)
            {
                _output.WriteLine($"Source: {source[i]}, Result: {result[i]}");
                Assert.Equal(source[i].Id, result[i].Id);
                Assert.Equal(source[i].Name, result[i].Name);
                Assert.NotNull(result[i].SubClass);
                Assert.Equal(source[i].SubClass?.Value, result[i].SubClass?.Value);
            }
        }

        [Fact]
        public void Map_ObjectWithMismatchedPropertyTypes_ReturnsMappedObjectWithDefaults()
        {
            var source = new SourceClassWithMismatchedTypes
            {
                Id = 1,
                Name = 12345, // int type, different from the string type in DestinationClass
                SubClass = new SubClass { Value = "Sub" }
            };
            var result = _mapper.Map<SourceClassWithMismatchedTypes, DestinationClass>(source);

            _output.WriteLine($"Source: Id: {source.Id}, Result: Id: {result.Id}");
            Assert.NotNull(result);
            Assert.Equal(source.Id, result.Id);
            Assert.Equal(string.Empty, result.Name); // Default value for string type
            Assert.NotNull(result.SubClass);
            Assert.Equal(source.SubClass?.Value, result.SubClass?.Value);
        }

        public void Dispose()
        {
            // Clean up resources if needed
        }

        public enum SourceEnum
        {
            One,
            Two,
            Three
        }

        public enum DestinationEnum
        {
            One,
            Two
        }

        public class SourceClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public SubClass? SubClass { get; set; }
        }

        public class SourceClassWithMismatchedTypes
        {
            public int Id { get; set; }
            public int Name { get; set; } // This is intentionally different from DestinationClass.Name which is a string
            public SubClass? SubClass { get; set; }
        }

        public class DestinationClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public SubClass? SubClass { get; set; }
        }

        public class AnotherClass
        {
            public int AnotherId { get; set; }
            public string? AnotherName { get; set; }
        }

        public class SubClass
        {
            public string Value { get; set; }
        }
    }
}
