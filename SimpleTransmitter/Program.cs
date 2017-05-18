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
            TrancieverConnectionFactory connectionFactory = new TrancieverConnectionFactory();
            ITransceiverSpiConnection spiConnection = connectionFactory.CreateForDragino();

            ITransceiver transceiver = new RFM9XLoraTransceiver(spiConnection);
            transceiver.Initialize();

            while (true)
            {
                Console.Write("Sending..");

                Task<bool> transmitTask = transceiver.Transmit(Encoding.ASCII.GetBytes("Lora1/" + DateTime.Now.Second));
                transmitTask.Wait();

                Console.WriteLine("OK");

                Thread.Sleep(1000);
            }
        }
    }
}
