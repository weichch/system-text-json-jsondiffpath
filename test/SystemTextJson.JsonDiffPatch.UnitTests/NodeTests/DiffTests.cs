using System;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;
using Xunit;

namespace SystemTextJson.JsonDiffPatch.UnitTests.NodeTests
{
    public class DiffTests
    {
        [Fact]
        public void Diff_Added()
        {
            var left = JsonNode.Parse("{}");
            var right = JsonNode.Parse("{\"a\":1}");

            var diff = left.Diff(right);

            Assert.Equal("{\"a\":[1]}", diff!.ToJsonString());
        }

        [Fact]
        public void Diff_Modified()
        {
            var left = JsonNode.Parse("1");
            var right = JsonNode.Parse("2");

            var diff = left.Diff(right);

            Assert.Equal("[1,2]", diff!.ToJsonString());
        }

        [Fact]
        public void Diff_Deleted()
        {
            var left = JsonNode.Parse("{\"a\":1}");
            var right = JsonNode.Parse("{}");

            var diff = left.Diff(right);

            Assert.Equal("{\"a\":[1,0,0]}", diff!.ToJsonString());
        }

        [Fact]
        public void Diff_ArrayMove()
        {
            var left = JsonNode.Parse("[1,2,3]");
            var right = JsonNode.Parse("[2,1,3]");

            var diff = left.Diff(right);

            Assert.Equal("{\"_t\":\"a\",\"_0\":[\"\",1,3]}", diff!.ToJsonString());
        }

        [Fact]
        public void Diff_Array_WithArrayObjectItemkeyFinder()
        {
            var source = "{ \"id\": \"1\", \"myArray\": [ { \"id\": \"2\", \"comment\": \"bogus\" }, { \"id\": \"3\", \"comment\": \"willberemoved\" }, { \"id\": \"4\", \"comment\": \"foobar\" }, { \"id\": \"5\", \"comment\": \"example\" }, { \"id\": \"6\", \"comment\": \"ok\" } ] }";
            var modified = "{ \"id\": \"1\", \"myArray\": [ { \"id\": \"2\", \"comment\": \"bogus\" }, { \"id\": \"4\", \"comment\": \"foobar\" }, { \"id\": \"5\", \"comment\": \"example adapted\" }, { \"id\": \"6\", \"comment\": \"ok\" }, { \"id\": \"myid\", \"comment\": \"isadded\" }, { \"id\": \"myid2\", \"comment\": \"isadded2\" }]}";
            
            var left = JsonNode.Parse(source);
            var right = JsonNode.Parse(modified);

            var options = new JsonDiffOptions
            {
                ArrayObjectItemKeyFinder = (node, index) =>
                {
                    if (node is JsonObject obj)
                    {
                        if (obj.TryGetPropertyValue("id", out var value))
                        {
                            try
                            {
                                return value?.GetValue<string>() ?? "";
                            }
                            catch (InvalidOperationException)
                            {
                                return value?.GetValue<int>() ?? 0;
                            }

                        }
                        else if (obj.TryGetPropertyValue("name", out value))
                        {
                            return value?.GetValue<string>() ?? "";
                        }
                    }
                    return index; //fallback
                }
            };

            var diff = left.Diff(right, options);

            Assert.Equal("{\"myArray\":{\"_t\":\"a\",\"_1\":[{\"id\":\"3\",\"comment\":\"willberemoved\"},0,0],\"2\":{\"comment\":[\"example\",\"example adapted\"]},\"4\":[{\"id\":\"myid\",\"comment\":\"isadded\"}],\"5\":[{\"id\":\"myid2\",\"comment\":\"isadded2\"}]}}", diff!.ToJsonString());
        }

        [Fact]
        public void Diff_NullProperty()
        {
            var left = JsonNode.Parse("{\"a\":1}");
            var right = JsonNode.Parse("{\"a\":null}");

            var diff = left.Diff(right);

            Assert.Equal("{\"a\":[1,null]}", diff!.ToJsonString());
        }

        [Fact]
        public void Diff_NullArrayItem()
        {
            var left = JsonNode.Parse("[1]");
            var right = JsonNode.Parse("[null]");

            var diff = left.Diff(right);

            Assert.Equal("{\"_t\":\"a\",\"_0\":[1,0,0],\"0\":[null]}", diff!.ToJsonString());
        }

        [Fact]
        public void PropertyFilter_LeftProperty()
        {
            var left = JsonNode.Parse("{\"a\":1}");
            var right = JsonNode.Parse("{}");

            var diff = left.Diff(right, new JsonDiffOptions
            {
                PropertyFilter = (prop, _) => !string.Equals(prop, "a")
            });

            Assert.Null(diff);
        }

        [Fact]
        public void PropertyFilter_RightProperty()
        {
            var left = JsonNode.Parse("{}");
            var right = JsonNode.Parse("{\"a\":1}");

            var diff = left.Diff(right, new JsonDiffOptions
            {
                PropertyFilter = (prop, _) => !string.Equals(prop, "a")
            });

            Assert.Null(diff);
        }

        [Fact]
        public void PropertyFilter_NestedProperty()
        {
            var left = JsonNode.Parse("{\"foo\":{\"bar\":{\"a\":1}}}");
            var right = JsonNode.Parse("{\"foo\":{\"bar\":{\"a\":2}}}");

            var diff = left.Diff(right, new JsonDiffOptions
            {
                PropertyFilter = (prop, _) => !string.Equals(prop, "a")
            });

            Assert.Null(diff);
        }

        [Fact]
        public void PropertyFilter_ArrayItem()
        {
            var left = JsonNode.Parse("[{\"a\":1}]");
            var right = JsonNode.Parse("[{\"a\":2}]");

            var diff = left.Diff(right, new JsonDiffOptions
            {
                PropertyFilter = (prop, _) => !string.Equals(prop, "a")
            });

            Assert.Null(diff);
        }

        [Fact]
        public void PropertyFilter_IgnoreByPath()
        {
            var left = JsonNode.Parse("{\"a\":{\"b\":{\"c\":1}}}");
            var right = JsonNode.Parse("{\"a\":{\"b\":{\"c\":2}}}");

            var diff = left.Diff(right, new JsonDiffOptions
            {
                PropertyFilter = (_, ctx) =>
                    ctx.Left<JsonNode>().GetPath() != "$.a.b" && ctx.Right<JsonNode>().GetPath() != "$.a.b"
            });

            Assert.Null(diff);
        }

        [Fact]
        public void PropertyFilter_ArrayItem_ExplicitFallbackMatch()
        {
            var left = JsonNode.Parse("[{\"a\":1}]");
            var right = JsonNode.Parse("[{\"a\":2}]");

            var diff = left.Diff(right, new JsonDiffOptions
            {
                ArrayObjectItemMatchByPosition = true,
                PropertyFilter = (prop, _) => !string.Equals(prop, "a")
            });

            Assert.Null(diff);
        }
    }
}
