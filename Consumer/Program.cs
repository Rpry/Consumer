using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;

namespace Consumer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("hawk.rmq.cloudamqp.com",
                    "ykziztbb",
                    h =>
                    {
                        h.Username("ykziztbb");
                        h.Password("oZaUpy2Sru1P0b04K9ghjx3MSFpXTMIU");
                    });
                
                cfg.ReceiveEndpoint($"masstransit_event_queue_{args[0]}", e =>
                {
                    e.Consumer<EventConsumer>();
                    e.UseMessageRetry(r =>
                    {
                        //r.Ignore<ArithmeticException>();
                        r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                    });
                });
                cfg.ReceiveEndpoint($"masstransit_request_queue", e =>
                {
                    e.Consumer<RequestConsumer>();
                });
            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                Console.WriteLine("Press enter to exit");

                await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}