namespace DotNetNottsIot2022.ftdiDotnet;

public class FT2232H_EEPROM_STRUCTURE : FT_EEPROM_DATA
{
  public bool PullDownEnable;
  public bool SerNumEnable = true;
  public bool ALSlowSlew;
  public bool ALSchmittInput;
  public byte ALDriveCurrent = 4;
  public bool AHSlowSlew;
  public bool AHSchmittInput;
  public byte AHDriveCurrent = 4;
  public bool BLSlowSlew;
  public bool BLSchmittInput;
  public byte BLDriveCurrent = 4;
  public bool BHSlowSlew;
  public bool BHSchmittInput;
  public byte BHDriveCurrent = 4;
  public bool IFAIsFifo;
  public bool IFAIsFifoTar;
  public bool IFAIsFastSer;
  public bool AIsVCP = true;
  public bool IFBIsFifo;
  public bool IFBIsFifoTar;
  public bool IFBIsFastSer;
  public bool BIsVCP = true;
  public bool PowerSaveEnable;
}
