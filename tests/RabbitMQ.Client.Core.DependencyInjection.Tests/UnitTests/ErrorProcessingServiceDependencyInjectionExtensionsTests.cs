using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class ErrorProcessingServiceDependencyInjectionExtensionsTests
    {
        [Fact]
        public void ShouldProperlyRegisterCustomErrorProcessingService()
        {
            var collection = new ServiceCollection();
            collection.TryAddSingleton<IErrorProcessingService, ErrorProcessingService>();
            collection.AddCustomMessageHandlingErrorProcessingService<StubErrorProcessingService>();

            var errorProcessingServices = collection.Where(x => x.ServiceType == typeof(IErrorProcessingService)).ToList();
            Assert.Single(errorProcessingServices);
            var errorProcessingServiceDescriptor = errorProcessingServices.FirstOrDefault();
            Assert.NotNull(errorProcessingServiceDescriptor);
            Assert.Equal(typeof(StubErrorProcessingService), errorProcessingServiceDescriptor.ImplementationType);
        }
    }
}