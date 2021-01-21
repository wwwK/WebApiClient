﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using WebApiClientCore.Implementations;
using Xunit;

namespace WebApiClientCore.Test.Attributes.ActionAttributes
{
    public class HttpPostAttributeTest
    {
        [Fact]
        public async Task OnRequestAsyncTest()
        {
            var apiAction = new DefaultApiActionDescriptor(typeof(ITestApi).GetMethod("PostAsync"));
            var context = new TestRequestContext(apiAction, string.Empty);

            context.HttpContext.RequestMessage.RequestUri = new Uri("http://www.webapi.com/");
            context.HttpContext.RequestMessage.Method = HttpMethod.Post;


            var attr = new HttpPostAttribute();
            await attr.OnRequestAsync(context);
            Assert.True(context.HttpContext.RequestMessage.Method == HttpMethod.Post);

            var attr2 = new HttpPostAttribute("/login");
            await attr2.OnRequestAsync(context);
            Assert.True(context.HttpContext.RequestMessage.Method == HttpMethod.Post);
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.webapi.com/login"));

            var attr3 = new HttpPostAttribute("http://www.baidu.com");
            await attr3.OnRequestAsync(context);
            Assert.True(context.HttpContext.RequestMessage.Method == HttpMethod.Post);
            Assert.True(context.HttpContext.RequestMessage.RequestUri == new Uri("http://www.baidu.com"));
        }
    }
}
