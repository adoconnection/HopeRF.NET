namespace RFMLib.Configuration.FskOok
{
    public class RFM9XFskOokTransmitter
    {
        private readonly TransceiverRegistry fifo;
        private readonly TransceiverRegistry payloadLength;
        private readonly TransceiverRegistry sequencer;

        public RFM9XFskOokTransmitter(ITransceiverSpiConnection connection)
        {
            this.fifo = new TransceiverRegistry(connection, 0x00);
            this.payloadLength = new TransceiverRegistry(connection, 0x32);
            this.sequencer = new TransceiverRegistry(connection, 0x36);
        }

        public void WritePacketBuffer(byte[] buffer)
        {
            this.payloadLength.Write((byte)buffer.Length);

            foreach (byte b in buffer)
            {
                this.fifo.Write(b);
            }

          //  this.sequencer.SetBit(7, true);
          //  this.sequencer.Write();
        }

        public void Reset()
        {
           // this.fifoTxBaseAddress.Write(0x00);
        }
    }
}