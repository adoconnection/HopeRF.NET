namespace RFMLib.Configuration.FskOok
{
    public class RFM9XFskOokIRQFalgs
    {
        private readonly TransceiverRegistry irqFlagsBank1;
        private readonly TransceiverRegistry irqFlagsBank2;

        public RFM9XFskOokIRQFalgs(ITransceiverSpiConnection connection)
        {
            this.irqFlagsBank1 = new TransceiverRegistry(connection, 0x3e);
            this.irqFlagsBank2 = new TransceiverRegistry(connection, 0x3f);
        }

        /// <summary>
        /// Set when the operation mode requested in Mode, is ready 
        /// - Sleep: Entering Sleep mode
        /// - Standby: XO is running
        /// - FS: PLL is locked
        /// - Rx: RSSI sampling starts
        /// - Tx: PA ramp-up completed
        /// Cleared when changing the operating mode.
        /// </summary>
        public bool ModeReadey
        {
            get
            {
                return this.irqFlagsBank1.GetBit(7);
            }
        }

        /// <summary>
        /// Set in Rx mode, after RSSI, AGC and AFC. 
        /// Cleared when leaving Rx
        /// </summary>
        public bool RxReady
        {
            get
            {
                return this.irqFlagsBank1.GetBit(6);
            }
        }

        /// <summary>
        /// Set in Tx mode, after PA ramp-up. 
        /// Cleared when leaving Tx
        /// </summary>
        public bool TxReady
        {
            get
            {
                return this.irqFlagsBank1.GetBit(5);
            }
        }

        /// <summary>
        /// Set (in FS, Rx or Tx) when the PLL is locked. 
        /// Cleared when it is not.
        /// </summary>
        public bool PllLock
        {
            get
            {
                return this.irqFlagsBank1.GetBit(4);
            }
        }

        /// <summary>
        /// Set in Rx when the RssiValue exceeds RssiThreshold. 
        /// Cleared when leaving Rx or setting this bit to 1.
        /// </summary>
        public bool Rssi
        {
            get
            {
                return this.irqFlagsBank1.GetBit(3);
            }
        }

        /// <summary>
        /// Set when a timeout occurs 
        /// Cleared when leaving Rx or FIFO is emptied
        /// </summary>
        public bool Timeout
        {
            get
            {
                return this.irqFlagsBank1.GetBit(2);
            }
        }

        /// <summary>
        /// Set when the Preamble Detector has found valid Preamble. 
        /// bit clear when set to 1
        /// </summary>
        public bool PreambleDetect
        {
            get
            {
                return this.irqFlagsBank1.GetBit(1);
            }
        }

        /// <summary>
        /// Set when Sync and Address (if enabled) are detected. 
        /// </summary>
        public bool SyncAddressMatch
        {
            get
            {
                return this.irqFlagsBank1.GetBit(0);
            }
        }


        // page 98

        public bool FifoFull
        {
            get
            {
                return this.irqFlagsBank2.GetBit(7);
            }
        }

        public bool FifoEmpty
        {
            get
            {
                return this.irqFlagsBank2.GetBit(6);
            }
        }

        public bool FifoLevel
        {
            get
            {
                return this.irqFlagsBank2.GetBit(5);
            }
        }

        public bool FifoOverrun
        {
            get
            {
                return this.irqFlagsBank2.GetBit(4);
            }
        }

        public bool PacketSent
        {
            get
            {
                return this.irqFlagsBank2.GetBit(3);
            }
        }

        public bool PayloadReady
        {
            get
            {
                return this.irqFlagsBank2.GetBit(2);
            }
        }

        public bool CrcOk
        {
            get
            {
                return this.irqFlagsBank2.GetBit(1);
            }
        }

        public bool LowBat
        {
            get
            {
                return this.irqFlagsBank2.GetBit(0);
            }
        }


        public void Clear()
        {
            this.irqFlagsBank1.Value = 0xFF;
            this.irqFlagsBank1.Write();
            this.irqFlagsBank1.Read();

            this.irqFlagsBank2.Value = 0xFF;
            this.irqFlagsBank2.Write();
            this.irqFlagsBank2.Read();
        }

        public void Read()
        {
            this.irqFlagsBank1.Read();
            this.irqFlagsBank2.Read();
        }
    }
}