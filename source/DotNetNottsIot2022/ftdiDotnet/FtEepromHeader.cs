namespace DotNetNottsIot2022.ftdiDotnet;

struct FT_EEPROM_HEADER
{
  public uint deviceType;
  public ushort VendorId;
  public ushort ProductId;
  public byte SerNumEnable;
  public ushort MaxPower;
  public byte SelfPowered;
  public byte RemoteWakeup;
  public byte PullDownEnable;
}
