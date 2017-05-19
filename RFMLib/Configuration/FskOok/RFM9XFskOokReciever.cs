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
            /*
             *    Radio.SetRxConfig( MODEM_FSK, FSK_BANDWIDTH, FSK_DATARATE,
                         0, FSK_AFC_BANDWIDTH, FSK_PREAMBLE_LENGTH,
                         0, FSK_FIX_LENGTH_PAYLOAD_ON, 0, FSK_CRC_ENABLED,
                         0, 0, false, true );

            void SX1276::SetRxConfig( ModemType modem, uint32_t bandwidth,
                         uint32_t datarate, uint8_t coderate,
                         uint32_t bandwidthAfc, uint16_t preambleLen,
                         uint16_t symbTimeout, bool fixLen,
                         uint8_t payloadLen,
                         bool crcOn, bool freqHopOn, uint8_t hopPeriod,
                         bool iqInverted, bool rxContinuous )
{
    SetModem( modem );

    switch( modem )
    {
    case MODEM_FSK:
        {
            this->settings.Fsk.Bandwidth = bandwidth;
            this->settings.Fsk.Datarate = datarate;
            this->settings.Fsk.BandwidthAfc = bandwidthAfc;
            this->settings.Fsk.FixLen = fixLen;
            this->settings.Fsk.PayloadLen = payloadLen;
            this->settings.Fsk.CrcOn = crcOn;
            this->settings.Fsk.IqInverted = iqInverted;
            this->settings.Fsk.RxContinuous = rxContinuous;
            this->settings.Fsk.PreambleLen = preambleLen;
            
            datarate = ( uint16_t )( ( double )XTAL_FREQ / ( double )datarate );
            Write( REG_BITRATEMSB, ( uint8_t )( datarate >> 8 ) );
            Write( REG_BITRATELSB, ( uint8_t )( datarate & 0xFF ) );

            Write( REG_RXBW, GetFskBandwidthRegValue( bandwidth ) );
            Write( REG_AFCBW, GetFskBandwidthRegValue( bandwidthAfc ) );

            Write( REG_PREAMBLEMSB, ( uint8_t )( ( preambleLen >> 8 ) & 0xFF ) );
            Write( REG_PREAMBLELSB, ( uint8_t )( preambleLen & 0xFF ) );

            Write( REG_PACKETCONFIG1,
                         ( Read( REG_PACKETCONFIG1 ) & 
                           RF_PACKETCONFIG1_CRC_MASK &
                           RF_PACKETCONFIG1_PACKETFORMAT_MASK ) |
                           ( ( fixLen == 1 ) ? RF_PACKETCONFIG1_PACKETFORMAT_FIXED : RF_PACKETCONFIG1_PACKETFORMAT_VARIABLE ) |
                           ( crcOn << 4 ) );
            if( fixLen == 1 )
            {
                Write( REG_PAYLOADLENGTH, payloadLen );
            }
        }
        break;
*/

            //this.fifoRxBaseAddress.Write(0x00);
        }

        public byte[] ReadPacketBuffer()
        {
            /*   
            // DIO0=PayloadReady
            // DIO1=FifoLevel
            // DIO2=SyncAddr
            // DIO3=FifoEmpty
            // DIO4=Preamble
            // DIO5=ModeReady
            Write( REG_DIOMAPPING1, ( Read( REG_DIOMAPPING1 ) & RF_DIOMAPPING1_DIO0_MASK & RF_DIOMAPPING1_DIO1_MASK &
                                                                            RF_DIOMAPPING1_DIO2_MASK ) |
                                                                            RF_DIOMAPPING1_DIO0_00 |
                                                                            RF_DIOMAPPING1_DIO2_11 );
            
            Write( REG_DIOMAPPING2, ( Read( REG_DIOMAPPING2 ) & RF_DIOMAPPING2_DIO4_MASK &
                                                                            RF_DIOMAPPING2_MAP_MASK ) | 
                                                                            RF_DIOMAPPING2_DIO4_11 |
                                                                            RF_DIOMAPPING2_MAP_PREAMBLEDETECT );
            
            this->settings.FskPacketHandler.FifoThresh = Read( REG_FIFOTHRESH ) & 0x3F;*/



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