namespace RFMLib.Configuration.FskOok
{
    public class RFM9XFskOokOperation
    {
        private readonly TransceiverRegistry modeBank;
        private readonly TransceiverRegistry packetConfig1;
        private readonly TransceiverRegistry packetConfig2;
        private readonly TransceiverRegistry seqConfig1;
        private readonly TransceiverRegistry seqConfig2;

        public RFM9XFskOokOperation(ITransceiverSpiConnection connection)
        {
            this.modeBank = new TransceiverRegistry(connection, 0x01);
            this.packetConfig1 = new TransceiverRegistry(connection, 0x30);
            this.packetConfig2 = new TransceiverRegistry(connection, 0x31);

            this.seqConfig1 = new TransceiverRegistry(connection, 0x36);
            this.seqConfig2 = new TransceiverRegistry(connection, 0x37);
        }


        public string Value
        {
            get
            {
                return this.seqConfig1.Value + " / " + this.seqConfig2.Value;
            }
        }

        public FskOokTranscieverMode Mode
        {
            get
            {
                return (FskOokTranscieverMode)(this.modeBank.Value & 0x07); // 00000111
            }
            set
            {
                this.modeBank.Value = (byte)((this.modeBank.Value & 0xF8) | (byte)value); // 11111000
            }
        }

        public bool PacketFixedLength
        {
            get
            {
                return !this.packetConfig1.GetBit(7);
            }
            set
            {
                this.packetConfig1.SetBit(7, !value);
            }
        }

        public bool IsPacketMode
        {
            get
            {
                return this.packetConfig2.GetBit(6);
            }
            set
            {
                this.packetConfig2.SetBit(6, value);
            }
        }

        public bool IsFskOokMode
        {
            get
            {
                return !this.modeBank.GetBit(7);
            }
            set
            {
                this.modeBank.SetBit(7, !value);
            }
        }

        public bool IsLowFrequencyModeOn
        {
            get
            {
                return this.modeBank.GetBit(3);
            }
            set
            {
                this.modeBank.SetBit(3, value);
            }
        }

        public void Read()
        {
            this.modeBank.Read();

            this.packetConfig1.Read();
            this.packetConfig2.Read();

            this.seqConfig1.Read();
            this.seqConfig2.Read();
        }

        public void Write()
        {
            this.modeBank.Write();

            this.packetConfig1.Write();
            this.packetConfig2.Write();

            this.seqConfig1.Write();
            this.seqConfig2.Write();
        }
    }
}