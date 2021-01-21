﻿using Xunit;

namespace WebApiClientCore.Test.BuildinExtensions
{
    public class StringExtensionsTest
    {
        [Fact]
        public void RepaceIgnoreCaseTest()
        {
            var str = "WebApiClientCore.Benchmarks.StringReplaces.WebApiClientCore";
            var newStr = str.RepaceIgnoreCase("core", "CORE", out var replaced);
            Assert.True(replaced);
            Assert.Equal("WebApiClientCORE.Benchmarks.StringReplaces.WebApiClientCORE", newStr);

            str = "AbccBAd";
            var newStr2 = str.RepaceIgnoreCase("A", "x", out replaced);
            Assert.True(replaced);
            Assert.Equal("xbccBxd", newStr2);

            str = "abc";
            var newStr3 = str.RepaceIgnoreCase("x", "x", out replaced);
            Assert.False(replaced);
            Assert.Equal(str, newStr3);

            str = "aaa";
            var newStr4 = str.RepaceIgnoreCase("A", "x", out replaced);
            Assert.True(replaced);
            Assert.Equal("xxx", newStr4);

            var newStr5 = str.RepaceIgnoreCase("a", null, out replaced);
            Assert.True(replaced);
            Assert.Equal("", newStr5);
        }
    }
}
