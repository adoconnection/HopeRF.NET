namespace RFMLib.Configuration.FskOok
{
    public class RFM9XFskOokReciever
    {
        private readonly TransceiverRegistry fifo;
        private readonly TransceiverRegistry payloadLength;
        private readonly TransceiverRegistry nodeAddress;
        private readonly TransceiverRegistry boreadcastAddress;
        private readonly TransceiverRegistry regSeqConfig1;
        private readonly TransceiverRegistry regSeqConfig2;

        public RFM9XFskOokReciever(ITransceiverSpiConnection connection)
        {
            this.fifo = new TransceiverRegistry(connection, 0x00);
            this.payloadLength = new TransceiverRegistry(connection, 0x32);
            this.nodeAddress = new TransceiverRegistry(connection, 0x33);
            this.boreadcastAddress = new TransceiverRegistry(connection, 0x34);
            this.regSeqConfig1 = new TransceiverRegistry(connection, 0x36);
            this.regSeqConfig2 = new TransceiverRegistry(connection, 0x37);
        }

        public void Reset()
        {
            //this.fifoRxBaseAddress.Write(0x00);
        }

        public byte[] ReadPacketBuffer()
        {
            this.payloadLength.Read();

            //this.fifoPointerAddressBank.Write(this.fifoCurrentPacketAddressBank.Value);

            byte[] bytes = new byte[this.fifo.Value];

            for (int i = 0; i < this.fifo.Value; i++)
            {
                this.fifo.Read();
                bytes[i] = this.fifo.Value;
            }

            return bytes;
        }

        public int GetPacketRSSI()
        {
            //this.packetRSSSIBank.Read();
           // return this.packetRSSSIBank.Value - 137;\
            return 0;
        }
    }
}