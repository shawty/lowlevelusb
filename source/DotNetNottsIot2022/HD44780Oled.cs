namespace DotNetNottsIot2022.ftdiDotnet;

using System;
using System.Threading;

// Class for using a HD44780 compatible LCD display
// That's using an I2C piggy back module for connectivity
//
// NOTE: Because of the chips used in the backpack, the device MUST BE 
//       programmed using the 4 bit mode of HD44780 chip.
//
// Written Sep 2022 by Shawty/DS
public class HD44780Oled
{
  public const byte DeviceAddress = 0x27; // I2C Address of the LCD

  private const byte _MRegSelect = 1 << 0; // MASK 1 = Instruction/Data pin
  private const byte _MEnable = 1 << 2; // MASK 4 = Enable/Clock pin
  private const byte _MBacklight = 1 << 3; // MASK 8 = Backlight control pin

  private const byte _D4 = 4;
  private const byte _D5 = 5;
  private const byte _D6 = 6;
  private const byte _D7 = 7;

  private readonly FtdiI2C _i2CInterface;

  private bool _backlight = false; // false = off, true = on
  private bool _regSelect = false; // false = send command, true = send data

  private byte _currentDataByte = 0x00;

  public HD44780Oled(FtdiI2C i2C)
  {
    _i2CInterface = i2C;
    BacklightOff();
  }

  private void UpdateDevice()
  {
    PulseSend(Convert.ToByte((_currentDataByte & 0xF0) | (_regSelect ? _MRegSelect : 0)));
    PulseSend(Convert.ToByte((_currentDataByte & 0x0F) << 4 | (_regSelect ? _MRegSelect : 0)));
  }

  private void PulseSend(byte b)
  {
    SendByteToI2C(Convert.ToByte(b | _MEnable | (_backlight ? _MBacklight : 0)));
    SendByteToI2C(Convert.ToByte(b | (_backlight ? _MBacklight : 0)));
  }

  private void SendByteToI2C(byte b)
  {
    _i2CInterface.SetStart();
    _i2CInterface.SendDeviceAddrAndCheckAck(DeviceAddress, false);
    _i2CInterface.SendByteAndCheckAck(b);
    _i2CInterface.SetStop();
  }

  public void Init()
  {
    bool turnOnDisplay = true;
    bool turnOnCursor = true;
    bool blinkCursor = true;
    bool cursorDirection = true;
    bool textShift = false;

    //Task.Delay(100).Wait();
    Thread.Sleep(100);

    PulseSend(Convert.ToByte((1 << _D5) | (1 << _D4)));

    //Task.Delay(5).Wait();
    Thread.Sleep(100);

    PulseSend(Convert.ToByte((1 << _D5) | (1 << _D4)));

    //Task.Delay(5).Wait();
    Thread.Sleep(100);

    PulseSend(Convert.ToByte((1 << _D5) | (1 << _D4)));

    /*  Init 4-bit mode */
    PulseSend(Convert.ToByte((1 << _D5)));

    /* Init 4-bit mode + 2 line */
    PulseSend(Convert.ToByte((1 << _D5)));
    PulseSend(Convert.ToByte((1 << _D7)));

    /* Turn on display, cursor */
    PulseSend(0);
    PulseSend(Convert.ToByte((1 << _D7) | (Convert.ToByte(turnOnDisplay) << _D6) |
                             (Convert.ToByte(turnOnCursor) << _D5) | (Convert.ToByte(blinkCursor) << _D4)));

    Clear();

    PulseSend(0);
    PulseSend(
      Convert.ToByte((1 << _D6) | (Convert.ToByte(cursorDirection) << _D5) | (Convert.ToByte(textShift) << _D4)));
  }

  public void Clear()
  {
    PulseSend(0);
    PulseSend(Convert.ToByte((1 << _D4)));
    //Task.Delay(5).Wait();
    Thread.Sleep(5);
  }

  public void SendCommand(byte command)
  {
    _regSelect = false;
    _currentDataByte = command;
    UpdateDevice();
  }

  public void SendData(byte command)
  {
    _regSelect = true;
    _currentDataByte = command;
    UpdateDevice();
  }

  public void BacklightOff()
  {
    _backlight = false;

    var controlNibble = (byte) (
      (0) + // Reg select will always be 0
      (0) + // Read write we don't care as we are not enabling
      (0) + // Enable is NEVER set when not updating LCD processor
      (_backlight ? _MBacklight : 0)
    );

    SendByteToI2C((byte) (0x00 + controlNibble));
  }

  public void BacklightOn()
  {
    _backlight = true;

    var controlNibble = (byte) (
      (0) + // Reg select will always be 0
      (0) + // Read write we don't care as we are not enabling
      (0) + // Enable is NEVER set when not updating LCD processor
      (_backlight ? _MBacklight : 0)
    );

    SendByteToI2C((byte) (0x00 + controlNibble));
  }

  public void GotoSecondLine()
  {
    SendCommand(0xc0);
  }

  public void GotoXy(byte x, byte y)
  {
    // TODO
    //SendCommand(Convert.ToByte(x | _LineAddress[y] | (1 << LCD_WRITE)));
  }

  public void PrintString(string text)
  {
    foreach (var t in text)
    {
      PrintChar(t);
    }
  }

  public void PrintChar(char letter)
  {
    SendData(Convert.ToByte(letter));
  }
  
}