using System;
using System.Threading;
using System.Threading.Tasks;
using RFMLib.Configuration.FskOok;

namespace RFMLib
{
    public class RFM9XFskOokTransciever : ITransceiver
    {
        private readonly ITransceiverSpiConnection connection;
        public RFM9XFskOokOperation OperationConfig { get; private set; }
        public RFM9XFskOokFrequencyConfig FrequencyConfig { get; private set; }
        public RFM9XFskOokIRQFalgs IRQs { get; private set; }
        public RFM9XFskOokReciever Reciever { get; private set; }
        public RFM9XFskOokTransmitter Transmitter { get; private set; }

        public RFM9XFskOokTransciever(ITransceiverSpiConnection connection)
        {
            this.connection = connection;
            this.OperationConfig = new RFM9XFskOokOperation(connection);
            this.FrequencyConfig = new RFM9XFskOokFrequencyConfig(connection);
            this.IRQs = new RFM9XFskOokIRQFalgs(connection);
            this.Reciever = new RFM9XFskOokReciever(connection);
            this.Transmitter = new RFM9XFskOokTransmitter(connection);
        }

        //  https://developer.mbed.org/questions/60994/Continuous-setup-works-with-LoRa-not-wit/

        public void Initialize()
        {


            this.connection.Reset();
            this.OperationConfig.Read();
            this.FrequencyConfig.Read();

            this.IRQs.Clear();
            this.IRQs.Read();

            this.OperationConfig.Mode = FskOokTranscieverMode.Sleep;
            this.OperationConfig.Write();
            Thread.Sleep(100);

            this.OperationConfig.IsFskOokMode = true;
            this.OperationConfig.Write();
            Thread.Sleep(100);


            this.Reciever.Reset();
            this.Transmitter.Reset();

            this.OperationConfig.IsLowFrequencyModeOn = false;
            this.OperationConfig.IsPacketMode = true;
            this.OperationConfig.PacketFixedLength = true;
            this.OperationConfig.Write();
            Thread.Sleep(100);

            this.FrequencyConfig.Frequency = 434000000;

            this.FrequencyConfig.PaSelect = true;
             this.FrequencyConfig.Power = 15;
            //  this.FrequencyConfig.MaxPower = 7;
            // this.FrequencyConfig.PaDac = 7;
            //   this.FrequencyConfig.Config1 = 0x72; // lazy blind-copy-pasted from cpp code
            // this.FrequencyConfig.Config2 = 0x74; // lazy blind-copy-pasted from cpp code
            // this.FrequencyConfig.Config3 = 0x00; // lazy blind-copy-pasted from cpp code
            //  this.FrequencyConfig.PreambleMsb = 8 >> 8; // lazy blind-copy-pasted from cpp code
            //  this.FrequencyConfig.PreambleLsb = 8 & 0xff; // lazy blind-copy-pasted from cpp code

            this.FrequencyConfig.Write();

            Thread.Sleep(100);
        }

        public void StandBy()
        {
            this.OperationConfig.Mode = FskOokTranscieverMode.StandBy;
            this.OperationConfig.Write();
            Thread.Sleep(100);
        }
            
        public Task<RawData> Recieve()
        {
            return this.Recieve(CancellationToken.None);
        }

        public Task<RawData> Recieve(CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                this.IRQDebug("1");

                this.OperationConfig.Mode = FskOokTranscieverMode.FrequencySynthesisReceive;
                this.OperationConfig.Write();
                Thread.Sleep(100);

                this.IRQDebug("2");

                this.OperationConfig.Mode = FskOokTranscieverMode.Receiver;
                this.OperationConfig.Write();
                this.IRQs.Clear();

                while (true)
                {
                    Thread.Sleep(10);

                    this.IRQDebug("3");

                    if (token.IsCancellationRequested)
                    {
                        this.IRQs.Clear();
                        this.OperationConfig.Mode = FskOokTranscieverMode.StandBy;
                        this.OperationConfig.Write();

                        return null;
                    }

                    this.IRQs.Read();

                    if (this.IRQs.CrcError)
                    {
                        this.IRQs.Clear();

                        this.OperationConfig.Read();

                        if (this.OperationConfig.Mode != FskOokTranscieverMode.Receiver)
                        {
                            Console.WriteLine("crc - " + this.OperationConfig.Mode);
                        }

                        return null;
                    }

                    if (this.IRQs.Timeout)
                    {
                        this.IRQs.Clear();

                        this.OperationConfig.Read();

                        if (this.OperationConfig.Mode != FskOokTranscieverMode.Receiver)
                        {
                            Console.WriteLine("RxTimeout - " + this.OperationConfig.Mode);
                        }

                        return null;
                    }

                    if (!this.IRQs.RxReady)
                    {
                        continue;
                    }

                    this.IRQs.Clear();

                    return new RawData()
                    {
                        Buffer = this.Reciever.ReadPacketBuffer(),
                        RSSI = this.Reciever.GetPacketRSSI()
                    };
                }
            }, token).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    return task.Result;
                }

                this.StandBy();
                this.IRQs.Clear();

                return null;
            }, token);
        }

        public Task<bool> Transmit(byte[] buffer)
        {
            return this.Transmit(buffer, CancellationToken.None);
        }

        public Task<bool> Transmit(byte[] buffer, CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                IRQDebug("1");

                this.StandBy();
                this.IRQs.Clear();

                IRQDebug("2");

                this.Transmitter.WritePacketBuffer(buffer);

                IRQDebug("3");

                this.OperationConfig.Mode = FskOokTranscieverMode.Transmitter;
                this.OperationConfig.Write();

                Thread.Sleep(100);

                this.Transmitter.WritePacketBuffer(buffer);

                IRQDebug("4");

                while (true)
                {
                    Thread.Sleep(10);

                    IRQDebug("5");

                 //   this.Transmitter.WritePacketBuffer(buffer);

                    if (token.IsCancellationRequested)
                    {
                        this.IRQs.Clear();
                        return false;
                    }

                    this.OperationConfig.Read();

                    if (this.OperationConfig.Mode == FskOokTranscieverMode.Transmitter)
                    {
                        continue;
                    }

                    if (this.OperationConfig.Mode == FskOokTranscieverMode.FrequencySynthesisReceive)
                    {
                        this.IRQs.Read();
                        return this.IRQs.PacketSent;
                    }

                    Console.WriteLine(this.OperationConfig.Mode + " - " + this.IRQs.Value);
                }
            }).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    return true;
                }

                this.StandBy();
                this.IRQs.Clear();

                return false;
            });
        }

        void IRQDebug(string data)
        {
            this.OperationConfig.Read();

            this.IRQs.Read();
            Console.WriteLine(data + " - " + this.IRQs.Value + " - " + this.OperationConfig.Value);
        }
    }
}