namespace DotNetNottsIot2022.ftdiDotnet;

public class FT_EEPROM_DATA
{
  public ushort VendorID = 1027;
  public ushort ProductID = 24577;
  public string Manufacturer = "FTDI";
  public string ManufacturerID = "FT";
  public string Description = "USB-Serial Converter";
  public string SerialNumber = "";
  public ushort MaxPower = 144;
  public bool SelfPowered;
  public bool RemoteWakeup;
}
