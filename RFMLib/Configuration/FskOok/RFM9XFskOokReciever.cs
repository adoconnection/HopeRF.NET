namespace RFMLib.Configuration.FskOok
{
    public class RFM9XFskOokReciever
    {
        private readonly TransceiverRegistry fifoBank;
        private readonly TransceiverRegistry payloadLengthBank;
        private readonly TransceiverRegistry nodeAddressBank;
        private readonly TransceiverRegistry boreadcastAddressBank;
        private readonly TransceiverRegistry regSeqConfig1;
        private readonly TransceiverRegistry regSeqConfig2;

        public RFM9XFskOokReciever(ITransceiverSpiConnection connection)
        {
            this.fifoBank = new TransceiverRegistry(connection, 0x00);
            this.payloadLengthBank = new TransceiverRegistry(connection, 0x32);
            this.nodeAddressBank = new TransceiverRegistry(connection, 0x33);
            this.boreadcastAddressBank = new TransceiverRegistry(connection, 0x34);
            this.regSeqConfig1 = new TransceiverRegistry(connection, 0x36);
            this.regSeqConfig2 = new TransceiverRegistry(connection, 0x37);
        }
    }
}