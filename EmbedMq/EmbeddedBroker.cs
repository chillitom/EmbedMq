using System;
using System.IO;
using org.apache.activemq.broker;

namespace EmbedMq
{
    public class EmbeddedBroker : IDisposable
    {
        private readonly BrokerService _bs;
        private readonly string _dataDir;
        private const string BindAddress = "tcp://localhost:0";

        public EmbeddedBroker()
        {
            _bs = new BrokerService();

            _dataDir = GetTemporaryDirectory();

            _bs.setDataDirectory(_dataDir);

            var connector = _bs.addConnector(BindAddress);

            Uri = "failover:" + connector.getUri().ToString();

            _bs.start();
        }

        public string Uri { get; private set; }

        private string GetTemporaryDirectory()
        {
            string tempFolder = Path.GetTempFileName();           
            File.Delete(tempFolder);            
            Directory.CreateDirectory(tempFolder);
            return tempFolder;
        }

        public void Dispose()
        {
            try
            {
                _bs.stop();

                Directory.Delete(_dataDir, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
