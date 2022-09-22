﻿namespace DotNetNottsIot2022.ftdiDotnet;

public class FT_XSERIES_EEPROM_STRUCTURE : FT_EEPROM_DATA
{
  public bool PullDownEnable;
  public bool SerNumEnable = true;
  public bool USBVersionEnable = true;
  public ushort USBVersion = 512;
  public byte ACSlowSlew;
  public byte ACSchmittInput;
  public byte ACDriveCurrent;
  public byte ADSlowSlew;
  public byte ADSchmittInput;
  public byte ADDriveCurrent;
  public byte Cbus0;
  public byte Cbus1;
  public byte Cbus2;
  public byte Cbus3;
  public byte Cbus4;
  public byte Cbus5;
  public byte Cbus6;
  public byte InvertTXD;
  public byte InvertRXD;
  public byte InvertRTS;
  public byte InvertCTS;
  public byte InvertDTR;
  public byte InvertDSR;
  public byte InvertDCD;
  public byte InvertRI;
  public byte BCDEnable;
  public byte BCDForceCbusPWREN;
  public byte BCDDisableSleep;
  public ushort I2CSlaveAddress;
  public uint I2CDeviceId;
  public byte I2CDisableSchmitt;
  public byte FT1248Cpol;
  public byte FT1248Lsb;
  public byte FT1248FlowControl;
  public byte RS485EchoSuppress;
  public byte PowerSaveEnable;
  public byte IsVCP;
}
