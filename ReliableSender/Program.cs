using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReliableConnectionLib;
using RFMLib;

namespace ReliableSender
{
    class Program
    {
        static void Main(string[] args)
        {
            TrancieverConnectionFactory trancieverConnectionFactory = new TrancieverConnectionFactory();
            ITransceiverSpiConnection spiConnection = trancieverConnectionFactory.CreateForDragino();

            RFM9XLoraTransceiver rfm9XLoraTransceiver = new RFM9XLoraTransceiver(spiConnection);
            rfm9XLoraTransceiver.Initialize();

            ReliableConnectionLib.Endpoint endPoint = new Endpoint(rfm9XLoraTransceiver, new byte[] {0x11, 0x12, 0x13, 0x14});

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Task start = endPoint.Start(cancellationTokenSource.Token);

            endPoint.QueueTransmit(new byte[] { 0x01, 0x02, 0x03, 0x04 }, Encoding.ASCII.GetBytes("Hi all"));

            Console.WriteLine("END");
            Console.ReadLine();

            cancellationTokenSource.Cancel();
            Console.WriteLine("Stopped");
        }
    }
}
