namespace RFMLib.Configuration.FskOok
{
    public class RFM9XFskOokTransmitter
    {
        private readonly TransceiverRegistry fifoBank;

        public RFM9XFskOokTransmitter(ITransceiverSpiConnection connection)
        {
            this.fifoBank = new TransceiverRegistry(connection, 0x00);
        }
    }
}