using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RFMLib;

namespace SimpleTransmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting fsk");

            TrancieverConnectionFactory connectionFactory = new TrancieverConnectionFactory();
            ITransceiverSpiConnection spiConnection = connectionFactory.CreateForDragino();

            ITransceiver transceiver = new RFM9XFskOokTransciever(spiConnection);
            transceiver.Initialize();

            while (true)
            {
                Console.Write("Sending..");

                Task<bool> transmitTask = transceiver.Transmit(Encoding.ASCII.GetBytes("FSK/OOK/" + DateTime.Now.Second));
                transmitTask.Wait();

                Console.WriteLine("OK");

                //Thread.Sleep(1000);
            }
        }
    }
}
