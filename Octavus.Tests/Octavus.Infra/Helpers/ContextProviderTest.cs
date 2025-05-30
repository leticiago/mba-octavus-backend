using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Octavus.Infra.Persistence;
using System;

namespace Octavus.Tests.Helpers
{
    public static class TestContextProvider
    {
        public static Context CreateContext()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            var context = new Context(options);
            context.Database.EnsureCreated();

            return context;
        }
    }
}
