namespace DotNetNottsIot2022.ftdiDotnet;

public class FT2232_EEPROM_STRUCTURE : FT_EEPROM_DATA
{
  public bool PullDownEnable;
  public bool SerNumEnable = true;
  public bool USBVersionEnable = true;
  public ushort USBVersion = 512;
  public bool AIsHighCurrent;
  public bool BIsHighCurrent;
  public bool IFAIsFifo;
  public bool IFAIsFifoTar;
  public bool IFAIsFastSer;
  public bool AIsVCP = true;
  public bool IFBIsFifo;
  public bool IFBIsFifoTar;
  public bool IFBIsFastSer;
  public bool BIsVCP = true;
}
