using System;
using System.Reactive.Linq;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using EmbedMq;

namespace LibConsumer
{
    class Program
    {
        private static readonly ActiveMQTopic Topic = new ActiveMQTopic("TestTopic");

        static void Main()
        {
            using (var broker = new EmbeddedBroker())
            {
                Console.WriteLine("{0} Started", DateTime.Now);

                Console.WriteLine("Client connecting to: {0}", broker.FailoverUri);

                ConnectionFactory connectionFactory = new ConnectionFactory(broker.FailoverUri);

                var connection = connectionFactory.CreateConnection();

                connection.Start();

                var session = connection.CreateSession();

                var consumer = session.CreateConsumer(Topic);

                consumer.Listener += message =>
                {
                    var msg = (ActiveMQTextMessage)message;
                    Console.WriteLine("{0} Received: {1}", DateTime.Now, msg.Text);
                };

                var producer = session.CreateProducer(Topic);

                Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(i =>
                {
                    var msg = producer.CreateTextMessage("Message #" + i);
                    producer.Send(msg);
                });

                Console.ReadKey();
            }
        }
    }
}
