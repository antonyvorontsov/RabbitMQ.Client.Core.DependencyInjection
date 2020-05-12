using System;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests.Stubs
{
    public interface IStubCaller
    {
        void EmptyCall();

        void Call(ReadOnlyMemory<byte> message);

        void Call(string message);
    }
}