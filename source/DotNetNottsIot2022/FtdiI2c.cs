using DotNetNottsIot2022.ftdiDotnet;

namespace DotNetNottsIot2022;

  // Class to set up and use the I2C interface on an FTDI
  // FT232H serial processor chip.
  //
  // NOTE: This code WILL also work with the FT232RL 
  //       however due to a fault in the RLs design there
  //       are a LOT of stability and timing issues, so
  //       for things like I2C/SPI and other timing sensitive
  //       protocols, the 232H is recommended.  For plain old
  //       UART serial however both devices are very capable
  //
  // Written Sep 2022 by Shawty/DS
  public class FtdiI2C
  {

    // ###### I2C Library defines ######
    private const byte _I2CDirSdaOutSclOut = 0x03;
    private const byte _I2CDataSdAhiScLhi = 0x03;
    private const byte _I2CDataSdAloScLhi = 0x01;
    private const byte _I2CDataSdAloScLlo = 0x00;
    private const byte _I2CDataSdAhiScLlo = 0x02;

    // MPSSE clocking commands
    private const byte _MsbRisingEdgeClockByteIn = 0x20;
    private const byte _MsbFallingEdgeClockByteOut = 0x11;
    private const byte _MsbRisingEdgeClockBitIn = 0x22;
    private const byte _MsbFallingEdgeClockBitOut = 0x13;

    // I2C Clock speed
    // Set to 199 for slow 100khz I2C
    // Set to 49 for fast 400khz I2C
    private const uint _ClockDivisor = 199;

    // GPIO
    private const byte _GpioLowDat = 0;
    private const byte _GpioLowDir = 0;
    
    private readonly FTDI _myFtdiDevice;

    private readonly List<byte> _foundDevices = new List<byte>();

    public FtdiI2C(FTDI ftdiDevice)
    {
      _myFtdiDevice = ftdiDevice;

      ResetMpsse();
      ConfigureMpsse();
    }

    private bool Send_Data(byte[] buffer, out int bytesActuallySent)
    {

      var numBytesToSend = buffer.Length;
      var numBytesSent = 0;

      // Send data. This will return once all sent or if times out
      var localFtStatus = _myFtdiDevice.Write(buffer, numBytesToSend, ref numBytesSent);

      bytesActuallySent = numBytesSent;

      // Ensure that call completed OK and that all bytes sent as requested
      return (numBytesSent == numBytesToSend) && (localFtStatus == FT_STATUS.FT_OK);
    }

    private byte[] Receive_Data(uint bytesToRead)
    {
      uint numBytesInQueue = 0;
      uint queueTimeOut = 0;
      var totalBytesRead = 0;
      var queueTimeoutFlag = false;
      uint numBytesRxd = 0;
      var inputBuffer = new byte[500];

      var receivedBytes = new List<byte>();

      // Keep looping until all requested bytes are received or we've tried 5000 times (value can be chosen as required)
      while ((totalBytesRead < bytesToRead) && (queueTimeoutFlag == false))
      {
        var operationStatus = _myFtdiDevice.GetRxBytesAvailable(ref numBytesInQueue);

        if ((numBytesInQueue <= 0) || (operationStatus != FT_STATUS.FT_OK)) continue;
        operationStatus = _myFtdiDevice.Read(inputBuffer, numBytesInQueue, ref numBytesRxd);  // if any available read them

        if ((numBytesInQueue == numBytesRxd) && (operationStatus == FT_STATUS.FT_OK))
        {
          uint bufferIndex = 0;

          while (bufferIndex < numBytesRxd)
          {
            receivedBytes.Add(inputBuffer[bufferIndex]);     // copy into main overall application buffer
            bufferIndex++;
          }
          totalBytesRead = totalBytesRead + (int)numBytesRxd;                  // Keep track of total
        }
        else
          return Array.Empty<byte>();

        queueTimeOut++;

        if (queueTimeOut == 5000)
          queueTimeoutFlag = true;
      }

      return receivedBytes.ToArray();

    }

    public byte ReadByte(bool ack)
    {
      byte aDbusVal = 0;
      byte aDbusDir = 0;

      var mpsseBuffer = new List<byte>();

      // Clock in one data byte
      mpsseBuffer.Add(_MsbRisingEdgeClockByteIn);       // Clock data byte in
      mpsseBuffer.Add(0x00);
      mpsseBuffer.Add(0x00);                                // Data length of 0x0000 means 1 byte data to clock in

      // clock out one bit as ack/nak bit
      mpsseBuffer.Add(_MsbFallingEdgeClockBitOut);      // Clock data bit out
      mpsseBuffer.Add(0x00);                                // Length of 0 means 1 bit
      
      mpsseBuffer.Add(ack ? (byte) 0x00 : (byte) 0xFF); // Data bit to send is a '0' or '1'
      
      // I2C lines back to idle state 
      aDbusVal = 0x00 | _I2CDataSdAhiScLlo | (_GpioLowDat & 0xF8);
      aDbusDir = 0x00 | _I2CDirSdaOutSclOut | (_GpioLowDir & 0xF8);

      mpsseBuffer.Add(0x80);                                // Command - set low byte
      mpsseBuffer.Add(aDbusVal);                            // Set the values
      mpsseBuffer.Add(aDbusDir);                            // Set the directions

      // This command then tells the MPSSE to send any results gathered back immediately
      mpsseBuffer.Add(0x87);                                // Send answer back immediate command

      // send commands to chip
      var sendStatus = Send_Data(mpsseBuffer.ToArray(), out _);
      if (!sendStatus)
      {
        throw new Exception($"I2C Read Byte: Failed to program FTDI device to receive byte");
      }

      // get the byte which has been read from the driver's receive buffer
      var rxData = Receive_Data(1);

      //if (I2C_Status != 0)
      //{
      //  return 1;
      //}

      // InputBuffer2[0] now contains the results

      return rxData[0];
    }

    public bool SendByteAndCheckAck(byte dataByteToSend)
    {
      byte aDbusVal = 0;
      byte aDbusDir = 0;

      var mpsseBuffer = new List<byte>();

      mpsseBuffer.Add(_MsbFallingEdgeClockByteOut);        // clock data byte out
      mpsseBuffer.Add(0x00);                                   // 
      mpsseBuffer.Add(0x00);                                   //  Data length of 0x0000 means 1 byte data to clock in
      mpsseBuffer.Add(dataByteToSend);// DataSend[0];          //  Byte to send

      // Put line back to idle (data released, clock pulled low)
      aDbusVal = (byte)(0x00 | _I2CDataSdAhiScLlo | (_GpioLowDat & 0xF8));
      aDbusDir = (byte)(0x00 | _I2CDirSdaOutSclOut | (_GpioLowDir & 0xF8));// make data input

      mpsseBuffer.Add(0x80);                                   // Command - set low byte
      mpsseBuffer.Add(aDbusVal);                               // Set the values
      mpsseBuffer.Add(aDbusDir);                               // Set the directions

      // CLOCK IN ACK
      mpsseBuffer.Add(_MsbRisingEdgeClockBitIn);           // clock data bits in
      mpsseBuffer.Add(0x00);                                   // Length of 0 means 1 bit

      // This command then tells the MPSSE to send any results gathered (in this case the ack bit) back immediately
      mpsseBuffer.Add(0x87);                                //  ' Send answer back immediate command

      // send commands to chip
      var sendStatus = Send_Data(mpsseBuffer.ToArray(), out _);
      if (!sendStatus)
      {
        return false; // NOt really correct, this should probbably throw
      }

      // read back byte containing ack
      var rxData = Receive_Data(1);
      //if (I2C_Status != 0)
      //{
      //  return 1;            // can also check NumBytesRead
      //}
      // if ack bit is 0 then sensor acked the transfer, otherwise it nak'd the transfer
      return (rxData[0] & 0x01) == 0;

    }

    // Read is the default, set to false for a write
    public bool SendDeviceAddrAndCheckAck(byte address, bool read = true)
    {
      byte aDbusVal = 0;
      byte aDbusDir = 0;

      address <<= 1;
      if (read == true)
        address |= 0x01;

      var mpsseBuffer = new List<byte>();

      mpsseBuffer.Add(_MsbFallingEdgeClockByteOut);         // clock data byte out
      mpsseBuffer.Add(0x00);                                    // 
      mpsseBuffer.Add(0x00);                                    //  Data length of 0x0000 means 1 byte data to clock in
      mpsseBuffer.Add(address);                                 //  Byte to send

      // Put line back to idle (data released, clock pulled low)
      aDbusVal = (byte)(0x00 | _I2CDataSdAhiScLlo | (_GpioLowDat & 0xF8));
      aDbusDir = (byte)(0x00 | _I2CDirSdaOutSclOut | (_GpioLowDir & 0xF8));// make data input

      mpsseBuffer.Add(0x80);                                    // Command - set low byte
      mpsseBuffer.Add(aDbusVal);                                // Set the values
      mpsseBuffer.Add(aDbusDir);                                // Set the directions

      // CLOCK IN ACK
      mpsseBuffer.Add(_MsbRisingEdgeClockBitIn);            // clock data bits in
      mpsseBuffer.Add(0x00);                                    // Length of 0 means 1 bit

      // This command then tells the MPSSE to send any results gathered (in this case the ack bit) back immediately
      mpsseBuffer.Add(0x87);                                    // Send answer back immediate command

      // send commands to chip
      var sendStatus = Send_Data(mpsseBuffer.ToArray(), out _);
      if (!sendStatus)
      {
        return false; // Not really correct, this should probably throw
      }

      // read back byte containing ack
      var rxData = Receive_Data(1);
      //if (I2C_Status != 0)
      //{
      //  return 1;            // can also check NumBytesRead
      //}
      // if ack bit is 0 then sensor acked the transfer, otherwise it nak'd the transfer
      return (rxData[0] & 0x01) == 0;

    }

    public bool SetStart()
    {
      //NOTE: This code is configured ONLY for the FT232H, see the MPSSE Sensor demo on FTDIs
      //Web site to configure for OTHER FTDI devices
      
      // NUmber of repeats each I2C condition is repeated for, if this is too low, The signalling will
      // be garbled, if it's 2 high, the bus will take too long to setup. You may need to change this
      // on a PC by PC basis
      var repCount = 6;

      // SDA high, SCL high
      byte aDbusVal = (byte)(0x00 | _I2CDataSdAhiScLhi | (_GpioLowDat & 0xF8));
      byte aDbusDir = (byte)(0x00 | _I2CDirSdaOutSclOut | (_GpioLowDir & 0xF8));    // on FT232H lines always output

      var mpsseBuffer = new List<byte>();

      for (var count = 0; count < repCount; count++)
      {
        mpsseBuffer.Add(0x80);  // ADbus GPIO command
        mpsseBuffer.Add(aDbusVal);  // Set data value
        mpsseBuffer.Add(aDbusDir);  // Set direction
      }

      // SDA lo, SCL high
      aDbusVal = (byte)(0x00 | _I2CDataSdAloScLhi | (_GpioLowDat & 0xF8));

      for (var count = 0; count < repCount; count++) // Repeat commands to ensure the minimum period of the start setup time ie 600ns is achieved
      {
        mpsseBuffer.Add(0x80);      // ADbus GPIO command
        mpsseBuffer.Add(aDbusVal);  // Set data value
        mpsseBuffer.Add(aDbusDir);  // Set direction
      }

      // SDA lo, SCL lo
      aDbusVal = (byte)(0x00 | _I2CDataSdAloScLlo | (_GpioLowDat & 0xF8));

      for (var count = 0; count < repCount; count++) // Repeat commands to ensure the minimum period of the start setup time ie 600ns is achieved
      {
        mpsseBuffer.Add(0x80);      // ADbus GPIO command
        mpsseBuffer.Add(aDbusVal);  // Set data value
        mpsseBuffer.Add(aDbusDir);  // Set direction
      }

      // Release SDA
      aDbusVal = (byte)(0x00 | _I2CDataSdAhiScLlo | (_GpioLowDat & 0xF8));

      mpsseBuffer.Add(0x80);        // ADbus GPIO command
      mpsseBuffer.Add(aDbusVal);    // Set data value
      mpsseBuffer.Add(aDbusDir);	  // Set direction

      var sendStatus = Send_Data(mpsseBuffer.ToArray(), out _);
      return sendStatus;
    }

    public bool SetStop()
    {
      // Number of repeats each I2C condition is repeated for, if this is too low, The signalling will
      // be garbled, if it's too high, the bus will take too long to setup. You may need to change this
      // on a PC by PC basis
      var repCount = 6;

      // SDA low, SCL low
      byte aDbusVal = 0x00 | _I2CDataSdAloScLlo | (_GpioLowDat & 0xF8);
      byte aDbusDir = 0x00 | _I2CDirSdaOutSclOut | (_GpioLowDir & 0xF8);    // on FT232H lines always output

      var mpsseBuffer = new List<byte>();

      for (var count = 0; count < repCount; count++)
      {
        mpsseBuffer.Add(0x80);        // ADbus GPIO command
        mpsseBuffer.Add(aDbusVal);    // Set data value
        mpsseBuffer.Add(aDbusDir);    // Set direction
      }

      // SDA low, SCL high
      aDbusVal = 0x00 | _I2CDataSdAloScLhi | (_GpioLowDat & 0xF8);
      aDbusDir = 0x00 | _I2CDirSdaOutSclOut | (_GpioLowDir & 0xF8);    // on FT232H lines always output

      for (var count = 0; count < repCount; count++)
      {
        mpsseBuffer.Add(0x80);        // ADbus GPIO command
        mpsseBuffer.Add(aDbusVal);    // Set data value
        mpsseBuffer.Add(aDbusDir);    // Set direction
      }

      // SDA high, SCL high
      aDbusVal = 0x00 | _I2CDataSdAhiScLhi | (_GpioLowDat & 0xF8);
      aDbusDir = 0x00 | _I2CDirSdaOutSclOut | (_GpioLowDir & 0xF8);        // on FT232H lines always output

      for (var count = 0; count < repCount; count++)
      {
        mpsseBuffer.Add(0x80);        // ADbus GPIO command
        mpsseBuffer.Add(aDbusVal);    // Set data value
        mpsseBuffer.Add(aDbusDir);    // Set direction
      }

      // send the buffer of commands to the chip 
      var sendStatus = Send_Data(mpsseBuffer.ToArray(), out _);
      return sendStatus;
    }

    public void ResetMpsse()
    {
      var localFtStatus = FT_STATUS.FT_OK;
      bool I2C_Status = false;

      //localFtStatus |= _myFtdiDevice.ResetDevice();
      //localFtStatus |= _myFtdiDevice.SetCharacters(0, false, 0, false);
      
      localFtStatus |= _myFtdiDevice.SetTimeouts(5000, 5000);
      localFtStatus |= _myFtdiDevice.SetLatency(16);
      localFtStatus |= _myFtdiDevice.SetFlowControl(FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x00, 0x00);
      localFtStatus |= _myFtdiDevice.SetBitMode(0x00, 0x00); // General Device Reset
      localFtStatus |= _myFtdiDevice.SetBitMode(0x00, 0x02); // MPSSE mode 

      if (localFtStatus != FT_STATUS.FT_OK)
      {
        throw new Exception($"ERROR: Failed to reset device status: {localFtStatus}");
      }

      // Small delay to allow the FT232H to stabilise
      Thread.Sleep(6);

      I2C_Status = FlushBuffer();
      if (!I2C_Status)
      {
        throw new Exception($"ERROR: failed to flush FTDI device receive buffer");
      }
      
      // Synchronize the MPSSE interface by sending bad command 0xAA
      var i2Cbytes = new byte[] { 0xAA };

      I2C_Status = Send_Data(i2Cbytes, out var actualByteCount);
      if (!I2C_Status)
      {
        throw new Exception($"ERROR: Bad command 0xAA test send failed (Bytes sent: {actualByteCount})");
      }

      var receivedData = Receive_Data(2);

      if ((receivedData[0] != 0xFA) && (receivedData[1] != 0xAA))
      {
        throw new Exception($"ERROR: Bad command 0xAA test send unexpected response.");
      }

      i2Cbytes = new byte[] { 0xAB };

      I2C_Status = Send_Data(i2Cbytes, out actualByteCount);
      if (!I2C_Status)
      {
        throw new Exception($"ERROR: Bad command 0xAB test send failed (Bytes sent: {actualByteCount})");
      }

      receivedData = Receive_Data(2);

      if ((receivedData[0] != 0xFA) && (receivedData[1] != 0xAB))
      {
        throw new Exception($"ERROR: Bad command 0xAB test send unexpected response.");
      }
      
      // Synchronize the MPSSE interface by sending bad command 0xAB
      i2Cbytes = new byte[] { 0xAB };

      I2C_Status = Send_Data(i2Cbytes, out actualByteCount);
      if (!I2C_Status)
      {
        throw new Exception($"ERROR: Bad command 0xAB test send failed (Bytes sent: {actualByteCount})");
      }

      receivedData = Receive_Data(2);

      if ((receivedData[0] != 0xFA) && (receivedData[1] != 0xAB))
      {
        throw new Exception($"ERROR: Bad command 0xAB test send unexpected response.");
      }
      
    }

    public void ConfigureMpsse()
    {
      var mpssEbuffer = new List<byte>();

      mpssEbuffer.Add(0x85);                                  // loopback off



      mpssEbuffer.Add(0x8A);                                  // Disable clock divide by 5 for 60Mhz master clock (FT232H only)
      mpssEbuffer.Add(0x97);                                  // Turn off adaptive clocking (FT232H only)
      mpssEbuffer.Add(0x8C);                                  // Enable 3 phase data clock, used by I2C to allow data on both clock edges (FT232H only)

      // The SK clock frequency can be worked out by below algorithm with divide by 5 set as off
      // SK frequency  = 60MHz /((1 +  [(1 +0xValueH*256) OR 0xValueL])*2)
      mpssEbuffer.Add(0x86);                                  // Command to set clock divisor
      mpssEbuffer.Add((byte)(_ClockDivisor & 0x00FF));        // Set 0xValueL of clock divisor
      mpssEbuffer.Add(0);                                     // Set 0xValueH of clock divisor
      
      mpssEbuffer.Add(0x85);                                  // Loopback off

      mpssEbuffer.Add(0x9E);                                  // Enable the FT232H's drive-zero mode with the following enable mask (FT232H only)
      mpssEbuffer.Add(0x07);                                  // ... Low byte (ADx) enables - bits 0, 1 and 2 and ... 
      mpssEbuffer.Add(0x00);                                  // ... High byte (ACx) enables - all off

      var aDbusVal = (byte)(0x00 | _I2CDataSdAhiScLhi | (_GpioLowDat & 0xF8)); // SDA and SCL both output high (open drain)
      var aDbusDir = (byte)(0x00 | _I2CDirSdaOutSclOut | (_GpioLowDir & 0xF8));

      mpssEbuffer.Add(0x80);                                  // Command to set directions of lower 8 pins and force value on bits set as output 
      mpssEbuffer.Add(aDbusVal);
      mpssEbuffer.Add(aDbusDir);

      var sendStatus = Send_Data(mpssEbuffer.ToArray(), out var actualByteCountSent);
      if (!sendStatus)
      {
        throw new Exception($"ERROR: send mpsse configuration data failed (Byte count sent {actualByteCountSent})");
      }

    }

    public void ScanBus()
    {
      _foundDevices.Clear();
      for (byte address = 0x08; address < 0x78; address++)
      {
        // Pull the I2C bus to start condition
        SetStart();
Thread.Sleep(50);
        // Send the current address loop value as an address and see
        // if anything acknowledges it
        if (SendDeviceAddrAndCheckAck(address))
        {
          _foundDevices.Add(address);
        }

        // Put the bus back into a stop condition
        SetStop();
        
        // Small pause to let the bus settle
        Thread.Sleep(50);
      }
    }

    public bool WasDeviceAddressDetected(byte deviceAddress)
    {
      return _foundDevices.Contains(deviceAddress);
    }

    private bool FlushBuffer()
    {
      var ftStatus = FT_STATUS.FT_OK;
      uint BytesAvailable = 0;
      uint NumBytesRead = 0;
      byte[] InputBuffer = new byte[1024]; // 1k temp buffer
      
      ftStatus = _myFtdiDevice.GetRxBytesAvailable(ref BytesAvailable);	 // Get the number of bytes in the receive buffer
      if (ftStatus != FT_STATUS.FT_OK)
        return false;
            
      if(BytesAvailable > 0)
      {
        ftStatus = _myFtdiDevice.Read(InputBuffer, BytesAvailable, ref NumBytesRead);  	//Read out the data from receive buffer
        return ftStatus == FT_STATUS.FT_OK;
      }
      else
      {
        return true;           // there were no bytes to read
      }
    }

  }
