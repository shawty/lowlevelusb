namespace DotNetNottsIot2022;

// Class for using an MCP-TC74 I2C Thermal Sensor
// Written Sep 2022 by Shawty/DS
public class Tc74ThermalSensor
{
  public const byte DeviceAddress = 0x48; 
  
  private readonly FtdiI2C _i2CBus;
  private const byte _TemperatureRegister = 0x00;
  private const byte _ConfigRegister = 0x01;

  private const byte _DataReadyMask = 0b01000000;

  private byte IsReady(byte config) =>
    (byte)((config & _DataReadyMask) >> 6);

  public Tc74ThermalSensor(FtdiI2C i2CBus)
  {
    _i2CBus = i2CBus;
  }
  
  private void SendByteToI2C(byte b)
  {
    _i2CBus.SetStart();
    _i2CBus.SendDeviceAddrAndCheckAck(DeviceAddress, false);
    _i2CBus.SendByteAndCheckAck(b);
    _i2CBus.SetStop();
  }
  
  private byte ReadRegister(byte registerAddress)
  {
    _i2CBus.SetStart();
    _i2CBus.SendDeviceAddrAndCheckAck(DeviceAddress, false);
    _i2CBus.SendByteAndCheckAck(registerAddress);
    _i2CBus.SetStart();
    _i2CBus.SendDeviceAddrAndCheckAck(DeviceAddress);
    var result = _i2CBus.ReadByte(false);
    _i2CBus.SetStop();

    return result;
  }

  public byte ReadTemperatureRegister()
  {
    return ReadRegister(_TemperatureRegister);
  }

  public byte ReadConfigRegister()
  {
    return ReadRegister(_ConfigRegister);
  }

}