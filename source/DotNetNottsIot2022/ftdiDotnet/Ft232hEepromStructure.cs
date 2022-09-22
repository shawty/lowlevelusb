namespace DotNetNottsIot2022.ftdiDotnet;

public class FT232H_EEPROM_STRUCTURE : FT_EEPROM_DATA
{
  public bool PullDownEnable;
  public bool SerNumEnable = true;
  public bool ACSlowSlew;
  public bool ACSchmittInput;
  public byte ACDriveCurrent = 4;
  public bool ADSlowSlew;
  public bool ADSchmittInput;
  public byte ADDriveCurrent = 4;
  public byte Cbus0;
  public byte Cbus1;
  public byte Cbus2;
  public byte Cbus3;
  public byte Cbus4;
  public byte Cbus5;
  public byte Cbus6;
  public byte Cbus7;
  public byte Cbus8;
  public byte Cbus9;
  public bool IsFifo;
  public bool IsFifoTar;
  public bool IsFastSer;
  public bool IsFT1248;
  public bool FT1248Cpol;
  public bool FT1248Lsb;
  public bool FT1248FlowControl;
  public bool IsVCP = true;
  public bool PowerSaveEnable;
}
