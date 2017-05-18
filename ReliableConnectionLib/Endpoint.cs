using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RFMLib;

namespace ReliableConnectionLib
{
    public class Endpoint
    {
        private readonly ITransceiver transceiver;
        public byte[] Address { get;  private set; }

        private Task recieveTask;
        private int packetsIndex;
        private CancellationTokenSource recieveCancellationTokenSource = null;
        Queue<Packet> incomingPackets = new Queue<Packet>();
        Queue<Packet> outgoingPackets = new Queue<Packet>();

        public Endpoint(ITransceiver transceiver, byte[] address)
        {
            this.transceiver = transceiver;
            this.Address = address;
        }

        public void QueueTransmit(byte[] addressTo, byte[] data)
        {
            this.outgoingPackets.Enqueue(new Packet()
            {
                FromAddress = this.Address,
                ToAddress = addressTo,
                Data = data,
                Index = packetsIndex++
            });
        }

        public Task Start(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (recieveCancellationTokenSource == null || recieveCancellationTokenSource.IsCancellationRequested)
                    {
                        this.recieveCancellationTokenSource = new CancellationTokenSource();
                        this.recieveTask = this.transceiver.Recieve(recieveCancellationTokenSource.Token).ContinueWith(task =>
                        {
                            if (task.IsCompleted && task.Result != null)
                            {
                                Console.WriteLine("New data..1");

                                Packet packet = Packet.Parse(task.Result.Buffer);

                                Console.WriteLine("New data..2");

                                if (packet != null)
                                {
                                    Console.WriteLine("New data..3");
                                    this.incomingPackets.Enqueue(packet);
                                }
                            }

                            recieveCancellationTokenSource = null;
                        });
                    }


                    this.recieveTask.Wait(100);

                    if (this.outgoingPackets.Count > 0)
                    {
                        Console.WriteLine("Sensing.1 - ");

                        recieveCancellationTokenSource?.Cancel();

                        CancellationTokenSource transmitCancellationTokenSource = new CancellationTokenSource();
                        byte[] buffer = this.outgoingPackets.Dequeue().ToBytes();
                        Console.WriteLine("Sensing.2 - " + DebugByteArrayToString(buffer));
                        Task<bool> transmit = this.transceiver.Transmit(buffer, transmitCancellationTokenSource.Token);
                        Console.WriteLine("Sensing.3");
                        transmit.Wait(1000);
                        Console.WriteLine("Sensing.4");
                    }
                }
            });
        }


        public static string DebugByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}