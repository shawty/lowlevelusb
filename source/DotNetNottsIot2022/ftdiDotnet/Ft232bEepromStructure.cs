namespace DotNetNottsIot2022.ftdiDotnet;

public class FT232B_EEPROM_STRUCTURE : FT_EEPROM_DATA
{
  public bool PullDownEnable;
  public bool SerNumEnable = true;
  public bool USBVersionEnable = true;
  public ushort USBVersion = 512;
}
