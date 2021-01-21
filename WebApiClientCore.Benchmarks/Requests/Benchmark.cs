﻿using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    [MemoryDiagnoser]
    public abstract class Benchmark : IBenchmark
    {
        protected IServiceProvider ServiceProvider { get; set; }


        [GlobalSetup]
        public async Task SetupAsync()
        {
            var services = new ServiceCollection();

            services
                .AddHttpClient(typeof(HttpClient).FullName)
                .AddHttpMessageHandler(() => new MockResponseHandler());

            services
                .AddHttpApi<IWebApiClientCoreApi>(o =>
                {
                    o.UseParameterPropertyValidate = false;
                    o.UseReturnValuePropertyValidate = false;
                })
                .AddHttpMessageHandler(() => new MockResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            services
                .AddRefitClient<IRefitApi>(new RefitSettings
                {
                    Buffered = true,
                })
                .AddHttpMessageHandler(() => new MockResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            this.ServiceProvider = services.BuildServiceProvider();

            using var scope = this.ServiceProvider.CreateScope();

            var core = scope.ServiceProvider.GetService<IWebApiClientCoreApi>();
            var refit = scope.ServiceProvider.GetService<IRefitApi>();

            await core.GetAsyc("id");
            await core.PostJsonAsync(new Model { });

            await refit.GetAsyc("id");
            await refit.PostJsonAsync(new Model { });
        }
    }
}
