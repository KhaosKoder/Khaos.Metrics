using System;
using System.Collections.Generic;
using System.Linq;
using Khaos.Metrics;
using Khaos.Metrics.AspNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;

namespace Khaos.Metrics.AspNet.Tests;

public class MonitoringEndpointRouteBuilderExtensionsTests
{
    [Fact]
    public void MapMonitoringEndpoints_Throws_WhenBuilderIsNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => MonitoringEndpointRouteBuilderExtensions.MapMonitoringEndpoints(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void MapMonitoringEndpoints_Throws_WhenPatternInvalid(string pattern)
    {
        var builder = FakeEndpointRouteBuilder.Create();

        Assert.Throws<ArgumentException>(() => builder.MapMonitoringEndpoints(pattern));
    }

    [Fact]
    public void MapMonitoringEndpoints_RegistersSnapshotRoute()
    {
        var builder = FakeEndpointRouteBuilder.Create();

        builder.MapMonitoringEndpoints("/ops");

        var endpoint = Assert.Single(builder.Endpoints);
        Assert.Equal("/ops/snapshot", endpoint.RoutePattern.RawText);
    }

    private sealed class FakeEndpointRouteBuilder : IEndpointRouteBuilder
    {
        private FakeEndpointRouteBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public ICollection<EndpointDataSource> DataSources { get; } = new List<EndpointDataSource>();

        public IEnumerable<RouteEndpoint> Endpoints => DataSources
            .SelectMany(ds => ds.Endpoints)
            .OfType<RouteEndpoint>();

        public IApplicationBuilder CreateApplicationBuilder()
            => new ApplicationBuilder(ServiceProvider);

        public static FakeEndpointRouteBuilder Create()
        {
            var services = new ServiceCollection();
            services.AddRouting();
            services.AddSingleton<IMonitoringSnapshotProvider>(new StubSnapshotProvider());

            var provider = services.BuildServiceProvider();
            return new FakeEndpointRouteBuilder(provider);
        }

        private sealed class StubSnapshotProvider : IMonitoringSnapshotProvider
        {
            public MonitoringSnapshot GetSnapshot() => new(DateTimeOffset.UtcNow, Array.Empty<OperationSnapshot>());

            public OperationSnapshot? GetOperationSnapshot(string name) => null;
        }
    }
}
