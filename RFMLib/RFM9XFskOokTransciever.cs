using System.Threading;
using System.Threading.Tasks;

namespace RFMLib
{
    public class RFM9XFskOokTransciever : ITransceiver
    {
        private readonly ITransceiverSpiConnection connection;

        public RFM9XFskOokTransciever(ITransceiverSpiConnection connection)
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

            this.OperationConfig.Mode = TransceiverMode.Sleep;
            this.OperationConfig.Write();
            Thread.Sleep(100);

            this.OperationConfig.IsLongRange = true;
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