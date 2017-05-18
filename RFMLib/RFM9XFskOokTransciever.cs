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

        public RFM9XFskOokTransciever(ITransceiverSpiConnection connection)
        {
            this.connection = connection;
            this.OperationConfig = new RFM9XFskOokOperation(connection);
            this.FrequencyConfig = new RFM9XFskOokFrequencyConfig(connection);
            this.IRQs = new RFM9XFskOokIRQFalgs(connection);
            //this.Reciever = new RFM9XLoraReciever(connection);
           // this.Transmitter = new RFM9XTransmitter(connection);
        }

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
            this.OperationConfig.Write();
            Thread.Sleep(100);

            this.FrequencyConfig.PaSelect = true;
            this.FrequencyConfig.Power = 15;
            this.FrequencyConfig.MaxPower = 7;
            this.FrequencyConfig.PaDac = 7;
         //   this.FrequencyConfig.Config1 = 0x72; // lazy blind-copy-pasted from cpp code
            // this.FrequencyConfig.Config2 = 0x74; // lazy blind-copy-pasted from cpp code
            // this.FrequencyConfig.Config3 = 0x00; // lazy blind-copy-pasted from cpp code
            this.FrequencyConfig.PreambleMsb = 8 >> 8; // lazy blind-copy-pasted from cpp code
            this.FrequencyConfig.PreambleLsb = 8 & 0xff; // lazy blind-copy-pasted from cpp code
            this.FrequencyConfig.Frequency = 434000000;
            this.FrequencyConfig.Write();

            Thread.Sleep(100);
        }

        public Task<RawData> Recieve()
        {
            throw new System.NotImplementedException();
        }

        public Task<RawData> Recieve(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Transmit(byte[] buffer)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Transmit(byte[] buffer, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}