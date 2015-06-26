using System;
using System.IO;
using org.apache.activemq.broker;

namespace EmbedMq
{
    public class EmbeddedBroker : IDisposable
    {
        private readonly BrokerService _bs;
        private readonly string _dataDir;

        public EmbeddedBroker(ushort port = 0, bool persistent = false)
        {
            _bs = new BrokerService();
            
            _bs.setPersistent(persistent);
            _bs.setUseJmx(false);
            _bs.setEnableStatistics(false);
            _bs.setStartAsync(false);

            _dataDir = GetTemporaryDirectory();

            _bs.setDataDirectory(_dataDir);

            var connector = _bs.addConnector("tcp://localhost:" + port);

            Uri = connector.getUri().ToString();

            _bs.start();
        }

        public string Uri { get; private set; }
        public string FailoverUri { get { return "failover:" + Uri; } }

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
