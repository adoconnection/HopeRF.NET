using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliableConnectionLib;
using RFMLib;

namespace ReliableReciever
{
    class Program
    {
        static void Main(string[] args)
        {
            TrancieverConnectionFactory trancieverConnectionFactory = new TrancieverConnectionFactory();
            ITransceiverSpiConnection spiConnection = trancieverConnectionFactory.CreateForDragino();

            RFM9XLoraTransceiver rfm9XLoraTransceiver = new RFM9XLoraTransceiver(spiConnection);
            rfm9XLoraTransceiver.Initialize();

            ReliableConnectionLib.Endpoint endPoint = new Endpoint(rfm9XLoraTransceiver, new byte[] { 0x01, 0x02, 0x03, 0x04 });

        }
    }
}
