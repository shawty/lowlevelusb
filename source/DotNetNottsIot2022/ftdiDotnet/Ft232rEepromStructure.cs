namespace DotNetNottsIot2022.ftdiDotnet;

public class FT232R_EEPROM_STRUCTURE : FT_EEPROM_DATA
{
  public bool UseExtOsc;
  public bool HighDriveIOs;
  public byte EndpointSize = 64;
  public bool PullDownEnable;
  public bool SerNumEnable = true;
  public bool InvertTXD;
  public bool InvertRXD;
  public bool InvertRTS;
  public bool InvertCTS;
  public bool InvertDTR;
  public bool InvertDSR;
  public bool InvertDCD;
  public bool InvertRI;
  public byte Cbus0 = 5;
  public byte Cbus1 = 5;
  public byte Cbus2 = 5;
  public byte Cbus3 = 5;
  public byte Cbus4 = 5;
  public bool RIsD2XX;
}
