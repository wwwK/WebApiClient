﻿using WebApiClientCore.Implementations;
using Xunit;

namespace WebApiClientCore.Test.Implementations
{
    public class ApiActionDescriptorTest
    {
        [Fact]
        public void CtorTest()
        {
            var m = typeof(IDescriptorApi).GetMethod("Get1");
            var d = new DefaultApiActionDescriptor(m);

            Assert.True(d.Attributes.Count == 3);
            Assert.True(d.FilterAttributes.Count == 1);
            Assert.True(d.Parameters.Count == 2);
            Assert.True(d.Name == m.Name);
            Assert.True(d.Member == m);
            Assert.True(d.Return.Attributes.Count == 1);
            Assert.True(d.Return.ReturnType == m.ReturnType);
        }
    }
}
