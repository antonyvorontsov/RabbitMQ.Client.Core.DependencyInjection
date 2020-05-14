using System;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public interface IStubCaller
    {
        void EmptyCall();

        void Call(ReadOnlyMemory<byte> message);

        void Call(string message);
        
        Task CallAsync(string message);
    }
}