namespace DotNetNottsIot2022.ftdiDotnet;

public class FT_BIT_MODES
{
  public const byte FT_BIT_MODE_RESET = 0;
  public const byte FT_BIT_MODE_ASYNC_BITBANG = 1;
  public const byte FT_BIT_MODE_MPSSE = 2;
  public const byte FT_BIT_MODE_SYNC_BITBANG = 4;
  public const byte FT_BIT_MODE_MCU_HOST = 8;
  public const byte FT_BIT_MODE_FAST_SERIAL = 16;
  public const byte FT_BIT_MODE_CBUS_BITBANG = 32;
  public const byte FT_BIT_MODE_SYNC_FIFO = 64;
}
