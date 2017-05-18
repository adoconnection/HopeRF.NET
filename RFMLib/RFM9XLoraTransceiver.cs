using System;
using System.Threading;
using System.Threading.Tasks;
using RFMLib.Configuration;

namespace RFMLib
{
    public class RFM9XLoraTransceiver : ITransceiver
    {
        private readonly ITransceiverSpiConnection connection;

        public RFM9XLoraOperation OperationConfig { get; private set; }
        public RFM9XLoraFrequencyConfig FrequencyConfig { get; private set; }
        public RFM9XIRQFalgs IRQs { get; private set; }
        public RFM9XReciever Reciever { get; private set; }
        public RFM9XTransmitter Transmitter { get; private set; }

        public RFM9XLoraTransceiver(ITransceiverSpiConnection connection)
        {
            this.connection = connection;
            this.OperationConfig = new RFM9XLoraOperation(connection);
            this.FrequencyConfig = new RFM9XLoraFrequencyConfig(connection);
            this.IRQs = new RFM9XIRQFalgs(connection);
            this.Reciever = new RFM9XReciever(connection);
            this.Transmitter = new RFM9XTransmitter(connection);
        }

        public void Initialize()
        {
            this.connection.Reset();
            this.OperationConfig.Read();
            this.FrequencyConfig.Read();
            this.IRQs.Clear();
            this.IRQs.Read();

            this.OperationConfig.Mode = LoraTransceiverMode.Sleep;
            this.OperationConfig.Write();
            Thread.Sleep(100);

            this.OperationConfig.IsLoraMode = true;
            this.OperationConfig.Write();
            Thread.Sleep(100);


            this.Reciever.Reset();
            this.Transmitter.Reset();

            this.OperationConfig.IsLowFrequencyModeOn = false;
            this.OperationConfig.Write();
            Thread.Sleep(100);

            this.FrequencyConfig.PaSelect = true;
            this.FrequencyConfig.Power = 15;
            this.FrequencyConfig.MaxPower = 7;
            this.FrequencyConfig.PaDac = 7;
            this.FrequencyConfig.Config1 = 0x72; // lazy blind-copy-pasted from cpp code
            this.FrequencyConfig.Config2 = 0x74; // lazy blind-copy-pasted from cpp code
            this.FrequencyConfig.Config3 = 0x00; // lazy blind-copy-pasted from cpp code
            this.FrequencyConfig.PreambleMsb = 8 >> 8; // lazy blind-copy-pasted from cpp code
            this.FrequencyConfig.PreambleLsb = 8 & 0xff; // lazy blind-copy-pasted from cpp code
            this.FrequencyConfig.Frequency = 434000000;
            this.FrequencyConfig.Write();

            Thread.Sleep(100);
        }

        public void StandBy()
        {
            this.OperationConfig.Mode = LoraTransceiverMode.StandBy;
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
                this.OperationConfig.Mode = LoraTransceiverMode.FrequencySynthesisReceive;
                this.OperationConfig.Write();
                Thread.Sleep(100);

                this.OperationConfig.Mode = LoraTransceiverMode.ReceiveContinuous;
                this.OperationConfig.Write();
                this.IRQs.Clear();

                while (true)
                {
                    Thread.Sleep(10);

                    if (token.IsCancellationRequested)
                    {
                        this.IRQs.Clear();
                        this.OperationConfig.Mode = LoraTransceiverMode.StandBy;
                        this.OperationConfig.Write();

                        return null;
                    }

                    this.IRQs.Read();

                    if (this.IRQs.PayloadCrcError)
                    {
                        this.IRQs.Clear();

                        this.OperationConfig.Read();

                        if (this.OperationConfig.Mode != LoraTransceiverMode.ReceiveContinuous)
                        {
                            Console.WriteLine("crc - " + this.OperationConfig.Mode);
                        }

                        return null;
                    }

                    if (this.IRQs.RxTimeout)
                    {
                        this.IRQs.Clear();

                        this.OperationConfig.Read();

                        if (this.OperationConfig.Mode != LoraTransceiverMode.ReceiveContinuous)
                        {
                            Console.WriteLine("RxTimeout - " + this.OperationConfig.Mode);
                        }

                        return null;
                    }

                    if (!this.IRQs.RxDone)
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
                this.StandBy();
                this.IRQs.Clear();

                this.Transmitter.WritePacketBuffer(buffer);

                this.OperationConfig.Mode = LoraTransceiverMode.Transmit;
                this.OperationConfig.Write();

                while (true)
                {
                    Thread.Sleep(10);

                    if (token.IsCancellationRequested)
                    {
                        this.IRQs.Clear();
                        return false;
                    }

                    this.OperationConfig.Read();

                    if (this.OperationConfig.Mode == LoraTransceiverMode.Transmit)
                    {
                        continue;
                    }
                    
                    if (this.OperationConfig.Mode == LoraTransceiverMode.StandBy)
                    {
                        this.IRQs.Read();
                        return this.IRQs.TxDone;
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

        /*
        public Task<bool> Transmit(byte[] data)
        {
            this.OperationConfig.Mode = LoraTransceiverMode.Transmit;
            this.OperationConfig.Write();
        }*/
    }
}