using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubCallerDecorator : IStubCaller
    {
        private readonly IStubCaller Caller;

        public StubCallerDecorator(IStubCaller caller)
        {
            Caller = caller;
        }

        public EventWaitHandle WaitHandle { get; set; }

        public void EmptyCall()
        {
            Caller.EmptyCall();
            WaitHandle.Set();
        }

        public void Call(ReadOnlyMemory<byte> message)
        {
            Caller.Call(message);
        }

        public void Call(string message)
        {
            Caller.Call(message);
        }

        public Task CallAsync(string message)
        {
            return Caller.CallAsync(message);
        }
    }
}