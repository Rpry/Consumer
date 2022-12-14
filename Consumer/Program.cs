﻿using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace Consumer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                Configure(cfg);
                RegisterEndPoints(cfg, int.Parse(args[0]));
            });

            await busControl.StartAsync();
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

        /// <summary>
        /// КОнфигурирование
        /// </summary>
        /// <param name="configurator"></param>
        private static void Configure(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Host("cow.rmq2.cloudamqp.com",
                "xvvcjzoi",
                h =>
                {
                    h.Username("xvvcjzoi");
                    h.Password("3zzqgto8t6iqz6EMWhrx3fj8ubnToHJ6");
                });
        }

        /// <summary>
        /// регистрация эндпоинтов
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="index"></param>
        private static void RegisterEndPoints(IRabbitMqBusFactoryConfigurator configurator, int index)
        {
            configurator.ReceiveEndpoint($"masstransit_event_queue_{index}", e =>
            {
                e.Consumer<EventConsumer>();
                e.UseMessageRetry(r =>
                {
                    //r.Ignore<ArithmeticException>();
                    r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                });
            });
            configurator.ReceiveEndpoint($"masstransit_request_queue", e =>
            {
                e.Consumer<RequestConsumer>();
            });
        }
    }
}