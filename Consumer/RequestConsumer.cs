using System;
using System.Threading.Tasks;
using MassTransit;
using CommonNamespace;

namespace Consumer
{
    class RequestConsumer: IConsumer<Request>
    {
        public async Task Consume(ConsumeContext<Request> context)
        {
            Console.WriteLine("Value: {0}", context.Message.Message.Content);
            await context.RespondAsync<CommonNamespace.Response>(new CommonNamespace.Response
            {
                IsSuccess = true
            });
        }
    }
}