using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RFMLib;

namespace TestReciever
{
    class Program
    {
        static void Main(string[] args)
        {
            TrancieverConnectionFactory trancieverConnectionFactory = new TrancieverConnectionFactory();
            ITransceiverSpiConnection spiConnection = trancieverConnectionFactory.CreateForDragino();

            RFM9XLoraTransceiver rfm9XLoraTransceiver = new RFM9XLoraTransceiver(spiConnection);
            rfm9XLoraTransceiver.Initialize();



            Console.WriteLine("Listening");

            while (true)
            {
                Task<RawData> recieveTask = rfm9XLoraTransceiver.Recieve();

                recieveTask.Wait();

                if (recieveTask.Result != null)
                {
                    string message = Encoding.ASCII.GetString(recieveTask.Result.Buffer);
                    Console.WriteLine(message + " / " + recieveTask.Result.RSSI);

                    Notify(message, recieveTask);
                }
            }
        }

        private static void Notify(string message, Task<RawData> recieveTask)
        {
            try
            {
                WebClient client = new WebClient();

                if (string.IsNullOrWhiteSpace(message))
                {
                    return;
                }

                string[] strings = message.Split('/');

                if (strings.Length < 2)
                {
                    return;
                }

                client.UploadValues("http://radio.personal.ado.me.uk/Home/Post", new NameValueCollection()
                {
                    {"name", strings[0]},
                    {"data", strings[1]},
                    {"rssi", recieveTask.Result.RSSI.ToString()}
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
