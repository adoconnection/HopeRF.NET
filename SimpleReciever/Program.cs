using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RFMLib;

namespace SimpleReciever
{
    class Program
    {
        static void Main(string[] args)
        {
            TrancieverConnectionFactory connectionFactory = new TrancieverConnectionFactory();
            ITransceiverSpiConnection spiConnection = connectionFactory.CreateForDragino();

            ITransceiver transceiver = new RFM9XLoraTransceiver(spiConnection);
            transceiver.Initialize();

            Console.WriteLine("Listening");

            while (true)
            {
                Task<RawData> recieveTask = transceiver.Recieve();

                recieveTask.Wait();

                if (recieveTask.Result != null)
                {
                    Console.WriteLine(Encoding.ASCII.GetString(recieveTask.Result.Buffer) + " / " + recieveTask.Result.RSSI);
                }
            }
        }
    }
}
