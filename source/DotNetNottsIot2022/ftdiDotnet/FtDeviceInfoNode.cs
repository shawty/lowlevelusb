namespace DotNetNottsIot2022.ftdiDotnet;

public class FT_DEVICE_INFO_NODE
{
  public uint Flags;
  public FT_DEVICE Type;
  public uint ID;
  public uint LocId;
  public string SerialNumber;
  public string Description;
  public IntPtr ftHandle;
}
