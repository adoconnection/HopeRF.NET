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
            var FSK_FDEV                    =             25000  ;  // Hz
            var FSK_DATARATE                =             19200  ;  // bps
            var FSK_BANDWIDTH               =             50000  ;  // Hz
            var FSK_AFC_BANDWIDTH           =             83333  ;  // Hz
            var FSK_PREAMBLE_LENGTH         =             5      ;  // Same for Tx and Rx
            var FSK_FIX_LENGTH_PAYLOAD_ON   =             false  ;
            var FSK_CRC_ENABLED = true;

            /*
             *  /*
             *  Radio.SetTxConfig( MODEM_FSK, TX_OUTPUT_POWER, FSK_FDEV, 0,
                         FSK_DATARATE, 0,
                         FSK_PREAMBLE_LENGTH, FSK_FIX_LENGTH_PAYLOAD_ON,
                         FSK_CRC_ENABLED, 0, 0, 0, 2000000 );
             * 
            void SX1276::SetTxConfig( ModemType modem, int8_t power, uint32_t fdev, 
                        uint32_t bandwidth, uint32_t datarate,
                        uint8_t coderate, uint16_t preambleLen,
                        bool fixLen, bool crcOn, bool freqHopOn, 
                        uint8_t hopPeriod, bool iqInverted, uint32_t timeout )
{
    uint8_t paConfig = 0;
    uint8_t paDac = 0;

    SetModem( modem );
    
    paConfig = Read( REG_PACONFIG );
    paDac = Read( REG_PADAC );

    paConfig = ( paConfig & RF_PACONFIG_PASELECT_MASK ) | GetPaSelect( this->settings.Channel );
    paConfig = ( paConfig & RF_PACONFIG_MAX_POWER_MASK ) | 0x70;

    if( ( paConfig & RF_PACONFIG_PASELECT_PABOOST ) == RF_PACONFIG_PASELECT_PABOOST )
    {
        if( power > 17 )
        {
            paDac = ( paDac & RF_PADAC_20DBM_MASK ) | RF_PADAC_20DBM_ON;
        }
        else
        {
            paDac = ( paDac & RF_PADAC_20DBM_MASK ) | RF_PADAC_20DBM_OFF;
        }
        if( ( paDac & RF_PADAC_20DBM_ON ) == RF_PADAC_20DBM_ON )
        {
            if( power < 5 )
            {
                power = 5;
            }
            if( power > 20 )
            {
                power = 20;
            }
            paConfig = ( paConfig & RF_PACONFIG_OUTPUTPOWER_MASK ) | ( uint8_t )( ( uint16_t )( power - 5 ) & 0x0F );
        }
        else
        {
            if( power < 2 )
            {
                power = 2;
            }
            if( power > 17 )
            {
                power = 17;
            }
            paConfig = ( paConfig & RF_PACONFIG_OUTPUTPOWER_MASK ) | ( uint8_t )( ( uint16_t )( power - 2 ) & 0x0F );
        }
    }
    else
    {
        if( power < -1 )
        {
            power = -1;
        }
        if( power > 14 )
        {
            power = 14;
        }
        paConfig = ( paConfig & RF_PACONFIG_OUTPUTPOWER_MASK ) | ( uint8_t )( ( uint16_t )( power + 1 ) & 0x0F );
    }
    Write( REG_PACONFIG, paConfig );
    Write( REG_PADAC, paDac );

    switch( modem )
    {
    case MODEM_FSK:
        {
            this->settings.Fsk.Power = power;
            this->settings.Fsk.Fdev = fdev;
            this->settings.Fsk.Bandwidth = bandwidth;
            this->settings.Fsk.Datarate = datarate;
            this->settings.Fsk.PreambleLen = preambleLen;
            this->settings.Fsk.FixLen = fixLen;
            this->settings.Fsk.CrcOn = crcOn;
            this->settings.Fsk.IqInverted = iqInverted;
            this->settings.Fsk.TxTimeout = timeout;
            
            fdev = ( uint16_t )( ( double )fdev / ( double )FREQ_STEP );
            Write( REG_FDEVMSB, ( uint8_t )( fdev >> 8 ) );
            Write( REG_FDEVLSB, ( uint8_t )( fdev & 0xFF ) );

            datarate = ( uint16_t )( ( double )XTAL_FREQ / ( double )datarate );
            Write( REG_BITRATEMSB, ( uint8_t )( datarate >> 8 ) );
            Write( REG_BITRATELSB, ( uint8_t )( datarate & 0xFF ) );

            Write( REG_PREAMBLEMSB, ( preambleLen >> 8 ) & 0x00FF );
            Write( REG_PREAMBLELSB, preambleLen & 0xFF );

            Write( REG_PACKETCONFIG1,
                         ( Read( REG_PACKETCONFIG1 ) & 
                           RF_PACKETCONFIG1_CRC_MASK &
                           RF_PACKETCONFIG1_PACKETFORMAT_MASK ) |
                           ( ( fixLen == 1 ) ? RF_PACKETCONFIG1_PACKETFORMAT_FIXED : RF_PACKETCONFIG1_PACKETFORMAT_VARIABLE ) |
                           ( crcOn << 4 ) );
        }
        break;
*/

            // this.fifoTxBaseAddress.Write(0x00);
        }
    }
}