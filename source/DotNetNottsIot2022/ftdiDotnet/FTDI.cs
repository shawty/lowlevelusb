using System.Runtime.InteropServices;
using System.Text;

namespace DotNetNottsIot2022.ftdiDotnet;

public class FTDI
{
  private const uint _FtOpenBySerialNumber = 1;
  private const uint _FtOpenByDescription = 2;
  private const uint _FtOpenByLocation = 4;
  private const uint _FtDefaultBaudRate = 9600;
  private const uint _FtDefaultDeadmanTimeout = 5000;
  private const int _FtComPortNotAssigned = -1;
  private const uint _FtDefaultInTransferSize = 4096;
  private const uint _FtDefaultOutTransferSize = 4096;
  private const byte _FtDefaultLatency = 16;
  private const uint _FtDefaultDeviceId = 67330049;

  private IntPtr _ftHandle = IntPtr.Zero;
  private IntPtr _ftd2Xxdll = IntPtr.Zero;
  private IntPtr _ftCreateDeviceInfoList = IntPtr.Zero;
  private IntPtr _ftGetDeviceInfoDetail = IntPtr.Zero;
  private IntPtr _ftOpen = IntPtr.Zero;
  private IntPtr _ftOpenEx = IntPtr.Zero;
  private IntPtr _ftClose = IntPtr.Zero;
  private IntPtr _ftRead = IntPtr.Zero;
  private IntPtr _ftWrite = IntPtr.Zero;
  private IntPtr _ftGetQueueStatus = IntPtr.Zero;
  private IntPtr _ftGetModemStatus = IntPtr.Zero;
  private IntPtr _ftGetStatus = IntPtr.Zero;
  private IntPtr _ftSetBaudRate = IntPtr.Zero;
  private IntPtr _ftSetDataCharacteristics = IntPtr.Zero;
  private IntPtr _ftSetFlowControl = IntPtr.Zero;
  private IntPtr _ftSetDtr = IntPtr.Zero;
  private IntPtr _ftClrDtr = IntPtr.Zero;
  private IntPtr _ftSetRts = IntPtr.Zero;
  private IntPtr _ftClrRts = IntPtr.Zero;
  private IntPtr _ftResetDevice = IntPtr.Zero;
  private IntPtr _ftResetPort = IntPtr.Zero;
  private IntPtr _ftCyclePort = IntPtr.Zero;
  private IntPtr _ftRescan = IntPtr.Zero;
  private IntPtr _ftReload = IntPtr.Zero;
  private IntPtr _ftPurge = IntPtr.Zero;
  private IntPtr _ftSetTimeouts = IntPtr.Zero;
  private IntPtr _ftSetBreakOn = IntPtr.Zero;
  private IntPtr _ftSetBreakOff = IntPtr.Zero;
  private IntPtr _ftGetDeviceInfo = IntPtr.Zero;
  private IntPtr _ftSetResetPipeRetryCount = IntPtr.Zero;
  private IntPtr _ftStopInTask = IntPtr.Zero;
  private IntPtr _ftRestartInTask = IntPtr.Zero;
  private IntPtr _ftGetDriverVersion = IntPtr.Zero;
  private IntPtr _ftGetLibraryVersion = IntPtr.Zero;
  private IntPtr _ftSetDeadmanTimeout = IntPtr.Zero;
  private IntPtr _ftSetChars = IntPtr.Zero;
  private IntPtr _ftSetEventNotification = IntPtr.Zero;
  private IntPtr _ftGetComPortNumber = IntPtr.Zero;
  private IntPtr _ftSetLatencyTimer = IntPtr.Zero;
  private IntPtr _ftGetLatencyTimer = IntPtr.Zero;
  private IntPtr _ftSetBitMode = IntPtr.Zero;
  private IntPtr _ftGetBitMode = IntPtr.Zero;
  private IntPtr _ftSetUsbParameters = IntPtr.Zero;
  private IntPtr _ftReadEe = IntPtr.Zero;
  private IntPtr _ftWriteEe = IntPtr.Zero;
  private IntPtr _ftEraseEe = IntPtr.Zero;
  private IntPtr _ftEeUaSize = IntPtr.Zero;
  private IntPtr _ftEeUaRead = IntPtr.Zero;
  private IntPtr _ftEeUaWrite = IntPtr.Zero;
  private IntPtr _ftEeRead = IntPtr.Zero;
  private IntPtr _ftEeProgram = IntPtr.Zero;
  private IntPtr _ftEepromRead = IntPtr.Zero;
  private IntPtr _ftEepromProgram = IntPtr.Zero;

  private string InterfaceIdentifier
  {
    get
    {
      string str;
      var empty = string.Empty;
      if (!IsOpen) return empty;
      var fTDEVICE = FT_DEVICE.FT_DEVICE_BM;
      GetDeviceType(ref fTDEVICE);
      if (!(fTDEVICE == FT_DEVICE.FT_DEVICE_2232 | fTDEVICE == FT_DEVICE.FT_DEVICE_2232H |
            fTDEVICE == FT_DEVICE.FT_DEVICE_4232H)) return empty;
      GetDescription(out str);
      empty = str.Substring(str.Length - 1);
      return empty;
    }
  }

  public bool IsOpen =>
    _ftHandle != IntPtr.Zero;

  public FTDI()
  {
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      _ftd2Xxdll = FtNativeMethods.LoadLibrary("FTD2XX.DLL");
      if (_ftd2Xxdll == IntPtr.Zero)
      {
        //TODO: Make this a debug write
        Console.WriteLine(
          $"Attempting to load FTD2XX.DLL from: {Path.GetDirectoryName(GetType().Assembly.Location)}\n");
        _ftd2Xxdll =
          FtNativeMethods.LoadLibrary(string.Concat(Path.GetDirectoryName(GetType().Assembly.Location),
            "\\FTD2XX.DLL"));
      }
    }

    if (_ftd2Xxdll == IntPtr.Zero)
    {
      //TODO: Make this a custom exception
      Console.WriteLine("Failed to load FTD2XX.DLL.  Are the FTDI drivers installed?");
      return;
    }

    _ftCreateDeviceInfoList = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_CreateDeviceInfoList");
    _ftGetDeviceInfoDetail = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_GetDeviceInfoDetail");
    _ftOpen = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_Open");
    _ftOpenEx = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_OpenEx");
    _ftClose = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_Close");
    _ftRead = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_Read");
    _ftWrite = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_Write");
    _ftGetQueueStatus = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_GetQueueStatus");
    _ftGetModemStatus = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_GetModemStatus");
    _ftGetStatus = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_GetStatus");
    _ftSetBaudRate = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetBaudRate");
    _ftSetDataCharacteristics = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetDataCharacteristics");
    _ftSetFlowControl = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetFlowControl");
    _ftSetDtr = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetDtr");
    _ftClrDtr = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_ClrDtr");
    _ftSetRts = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetRts");
    _ftClrRts = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_ClrRts");
    _ftResetDevice = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_ResetDevice");
    _ftResetPort = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_ResetPort");
    _ftCyclePort = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_CyclePort");
    _ftRescan = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_Rescan");
    _ftReload = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_Reload");
    _ftPurge = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_Purge");
    _ftSetTimeouts = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetTimeouts");
    _ftSetBreakOn = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetBreakOn");
    _ftSetBreakOff = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetBreakOff");
    _ftGetDeviceInfo = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_GetDeviceInfo");
    _ftSetResetPipeRetryCount = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetResetPipeRetryCount");
    _ftStopInTask = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_StopInTask");
    _ftRestartInTask = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_RestartInTask");
    _ftGetDriverVersion = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_GetDriverVersion");
    _ftGetLibraryVersion = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_GetLibraryVersion");
    _ftSetDeadmanTimeout = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetDeadmanTimeout");
    _ftSetChars = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetChars");
    _ftSetEventNotification = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetEventNotification");
    _ftGetComPortNumber = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_GetComPortNumber");
    _ftSetLatencyTimer = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetLatencyTimer");
    _ftGetLatencyTimer = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_GetLatencyTimer");
    _ftSetBitMode = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetBitMode");
    _ftGetBitMode = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_GetBitMode");
    _ftSetUsbParameters = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_SetUSBParameters");
    _ftReadEe = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_ReadEE");
    _ftWriteEe = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_WriteEE");
    _ftEraseEe = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_EraseEE");
    _ftEeUaSize = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_EE_UASize");
    _ftEeUaRead = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_EE_UARead");
    _ftEeUaWrite = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_EE_UAWrite");
    _ftEeRead = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_EE_Read");
    _ftEeProgram = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_EE_Program");
    _ftEepromRead = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_EEPROM_Read");
    _ftEepromProgram = FtNativeMethods.GetProcAddress(_ftd2Xxdll, "FT_EEPROM_Program");
  }

  ~FTDI()
  {
    FtNativeMethods.FreeLibrary(_ftd2Xxdll);
    _ftd2Xxdll = IntPtr.Zero;
  }

  public FT_STATUS Close()
  {
    var closeStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return closeStatus;
    }

    if (_ftClose != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_Close) Marshal.GetDelegateForFunctionPointer(_ftClose, typeof(FtNativeMethods.tFT_Close));
      closeStatus = delegateForFunctionPointer(_ftHandle);
      if (closeStatus == FT_STATUS.FT_OK)
      {
        _ftHandle = IntPtr.Zero;
      }
    }
    else if (_ftClose == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_Close.");
    }

    return closeStatus;
  }

  public FT_STATUS CyclePort()
  {
    var cycleStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return cycleStatus;
    }

    if (!(_ftCyclePort != IntPtr.Zero & _ftClose != IntPtr.Zero))
    {
      if (_ftCyclePort == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_CyclePort.");
      }

      if (_ftClose == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_Close.");
      }
    }
    else
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_CyclePort) Marshal.GetDelegateForFunctionPointer(_ftCyclePort,
          typeof(FtNativeMethods.tFT_CyclePort));
      var tFtClose =
        (FtNativeMethods.tFT_Close) Marshal.GetDelegateForFunctionPointer(_ftClose, typeof(FtNativeMethods.tFT_Close));
      if (_ftHandle != IntPtr.Zero)
      {
        cycleStatus = delegateForFunctionPointer(_ftHandle);
        if (cycleStatus == FT_STATUS.FT_OK)
        {
          cycleStatus = tFtClose(_ftHandle);
          if (cycleStatus == FT_STATUS.FT_OK)
          {
            _ftHandle = IntPtr.Zero;
          }
        }
      }
    }

    return cycleStatus;
  }

  public FT_STATUS EEReadUserArea(byte[] UserAreaDataBuffer, ref uint numBytesRead)
  {
    var readUserAreaStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return readUserAreaStatus;
    }

    if (!(_ftEeUaSize != IntPtr.Zero & _ftEeUaRead != IntPtr.Zero))
    {
      if (_ftEeUaSize == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_EE_UASize.");
      }

      if (_ftEeUaRead == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_EE_UARead.");
      }
    }
    else
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_UASize) Marshal.GetDelegateForFunctionPointer(_ftEeUaSize,
          typeof(FtNativeMethods.tFT_EE_UASize));
      var tFTEEUARead =
        (FtNativeMethods.tFT_EE_UARead) Marshal.GetDelegateForFunctionPointer(_ftEeUaRead,
          typeof(FtNativeMethods.tFT_EE_UARead));
      if (_ftHandle != IntPtr.Zero)
      {
        uint num = 0;
        readUserAreaStatus = delegateForFunctionPointer(_ftHandle, ref num);
        if (UserAreaDataBuffer.Length >= num)
        {
          readUserAreaStatus = tFTEEUARead(_ftHandle, UserAreaDataBuffer, UserAreaDataBuffer.Length, ref numBytesRead);
        }
      }
    }

    return readUserAreaStatus;
  }

  public FT_STATUS EEUserAreaSize(ref uint UASize)
  {
    var userAreaSizeStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return userAreaSizeStatus;
    }

    if (_ftEeUaSize != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_UASize) Marshal.GetDelegateForFunctionPointer(_ftEeUaSize,
          typeof(FtNativeMethods.tFT_EE_UASize));
      if (_ftHandle != IntPtr.Zero)
      {
        userAreaSizeStatus = delegateForFunctionPointer(_ftHandle, ref UASize);
      }
    }
    else if (_ftEeUaSize == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_UASize.");
    }

    return userAreaSizeStatus;
  }

  public FT_STATUS EEWriteUserArea(byte[] UserAreaDataBuffer)
  {
    var EeWriteUserAreaStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return EeWriteUserAreaStatus;
    }

    if (!(_ftEeUaSize != IntPtr.Zero & _ftEeUaWrite != IntPtr.Zero))
    {
      if (_ftEeUaSize == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_EE_UASize.");
      }

      if (_ftEeUaWrite == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_EE_UAWrite.");
      }
    }
    else
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_UASize) Marshal.GetDelegateForFunctionPointer(_ftEeUaSize,
          typeof(FtNativeMethods.tFT_EE_UASize));
      var tFTEEUAWrite =
        (FtNativeMethods.tFT_EE_UAWrite) Marshal.GetDelegateForFunctionPointer(_ftEeUaWrite,
          typeof(FtNativeMethods.tFT_EE_UAWrite));
      if (_ftHandle != IntPtr.Zero)
      {
        uint num = 0;
        EeWriteUserAreaStatus = delegateForFunctionPointer(_ftHandle, ref num);
        if (UserAreaDataBuffer.Length <= num)
        {
          EeWriteUserAreaStatus = tFTEEUAWrite(_ftHandle, UserAreaDataBuffer, UserAreaDataBuffer.Length);
        }
      }
    }

    return EeWriteUserAreaStatus;
  }

  public FT_STATUS EraseEEPROM()
  {
    var eraseEpromStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return eraseEpromStatus;
    }

    if (_ftEraseEe != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EraseEE) Marshal.GetDelegateForFunctionPointer(_ftEraseEe,
          typeof(FtNativeMethods.tFT_EraseEE));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE == FT_DEVICE.FT_DEVICE_232R)
        {
          ErrorHandler(eraseEpromStatus, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        eraseEpromStatus = delegateForFunctionPointer(_ftHandle);
      }
    }
    else if (_ftEraseEe == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EraseEE.");
    }

    return eraseEpromStatus;
  }

  public FT_STATUS GetCOMPort(out string ComPortName)
  {
    var getComPortStatus = FT_STATUS.FT_OTHER_ERROR;
    ComPortName = string.Empty;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getComPortStatus;
    }

    if (_ftGetComPortNumber != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetComPortNumber) Marshal.GetDelegateForFunctionPointer(_ftGetComPortNumber,
          typeof(FtNativeMethods.tFT_GetComPortNumber));
      var num = -1;
      if (_ftHandle != IntPtr.Zero)
      {
        getComPortStatus = delegateForFunctionPointer(_ftHandle, ref num);
      }

      if (num != -1)
      {
        ComPortName = string.Concat("COM", num.ToString());
      }
      else
      {
        ComPortName = string.Empty;
      }
    }
    else if (_ftGetComPortNumber == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetComPortNumber.");
    }

    return getComPortStatus;
  }

  public FT_STATUS GetDescription(out string Description)
  {
    var getDescriptionStatus = FT_STATUS.FT_OTHER_ERROR;
    Description = string.Empty;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getDescriptionStatus;
    }

    if (_ftGetDeviceInfo != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetDeviceInfo) Marshal.GetDelegateForFunctionPointer(_ftGetDeviceInfo,
          typeof(FtNativeMethods.tFT_GetDeviceInfo));
      uint num = 0;
      var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
      var numArray = new byte[16];
      var numArray1 = new byte[64];
      if (_ftHandle != IntPtr.Zero)
      {
        getDescriptionStatus =
          delegateForFunctionPointer(_ftHandle, ref fTDEVICE, ref num, numArray, numArray1, IntPtr.Zero);
        Description = Encoding.ASCII.GetString(numArray1);
        Description = Description.Substring(0, Description.IndexOf("\0"));
      }
    }
    else if (_ftGetDeviceInfo == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetDeviceInfo.");
    }

    return getDescriptionStatus;
  }

  public FT_STATUS GetDeviceID(ref uint DeviceID)
  {
    var getDeviceIdStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getDeviceIdStatus;
    }

    if (_ftGetDeviceInfo != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetDeviceInfo) Marshal.GetDelegateForFunctionPointer(_ftGetDeviceInfo,
          typeof(FtNativeMethods.tFT_GetDeviceInfo));
      var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
      var numArray = new byte[16];
      var numArray1 = new byte[64];
      if (_ftHandle != IntPtr.Zero)
      {
        getDeviceIdStatus =
          delegateForFunctionPointer(_ftHandle, ref fTDEVICE, ref DeviceID, numArray, numArray1, IntPtr.Zero);
      }
    }
    else if (_ftGetDeviceInfo == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetDeviceInfo.");
    }

    return getDeviceIdStatus;
  }

  public FT_STATUS GetDeviceList(FT_DEVICE_INFO_NODE[] devicelist)
  {
    unsafe
    {
      var getDeviceListStatus = FT_STATUS.FT_OTHER_ERROR;
      if (_ftd2Xxdll == IntPtr.Zero)
      {
        return getDeviceListStatus;
      }

      if (!(_ftCreateDeviceInfoList != IntPtr.Zero & _ftGetDeviceInfoDetail != IntPtr.Zero))
      {
        if (_ftCreateDeviceInfoList == IntPtr.Zero)
        {
          Console.WriteLine("Failed to load function FT_CreateDeviceInfoList.");
        }

        if (_ftGetDeviceInfoDetail == IntPtr.Zero)
        {
          Console.WriteLine("Failed to load function FT_GetDeviceInfoListDetail.");
        }
      }
      else
      {
        uint num = 0;
        var delegateForFunctionPointer =
          (FtNativeMethods.tFT_CreateDeviceInfoList) Marshal.GetDelegateForFunctionPointer(_ftCreateDeviceInfoList,
            typeof(FtNativeMethods.tFT_CreateDeviceInfoList));
        var tFTGetDeviceInfoDetail =
          (FtNativeMethods.tFT_GetDeviceInfoDetail) Marshal.GetDelegateForFunctionPointer(_ftGetDeviceInfoDetail,
            typeof(FtNativeMethods.tFT_GetDeviceInfoDetail));
        getDeviceListStatus = delegateForFunctionPointer(ref num);
        var numArray = new byte[16];
        var numArray1 = new byte[64];
        if (num > 0)
        {
          if (devicelist.Length < num)
          {
            ErrorHandler(getDeviceListStatus, FT_ERROR.FT_BUFFER_SIZE);
          }

          for (uint i = 0; i < num; i++)
          {
            devicelist[i] = new FT_DEVICE_INFO_NODE();
            getDeviceListStatus = tFTGetDeviceInfoDetail(i, ref devicelist[i].Flags, ref devicelist[i].Type,
              ref devicelist[i].ID, ref devicelist[i].LocId, numArray, numArray1, ref devicelist[i].ftHandle);
            devicelist[i].SerialNumber = Encoding.ASCII.GetString(numArray);
            devicelist[i].Description = Encoding.ASCII.GetString(numArray1);
            devicelist[i].SerialNumber =
              devicelist[i].SerialNumber.Substring(0, devicelist[i].SerialNumber.IndexOf("\0"));
            devicelist[i].Description = devicelist[i].Description.Substring(0, devicelist[i].Description.IndexOf("\0"));
          }
        }
      }

      return getDeviceListStatus;
    }
  }

  public FT_STATUS GetDeviceType(ref FT_DEVICE DeviceType)
  {
    var getDeviceTypeStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getDeviceTypeStatus;
    }

    if (_ftGetDeviceInfo != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetDeviceInfo) Marshal.GetDelegateForFunctionPointer(_ftGetDeviceInfo,
          typeof(FtNativeMethods.tFT_GetDeviceInfo));
      uint num = 0;
      var numArray = new byte[16];
      var numArray1 = new byte[64];
      DeviceType = FT_DEVICE.FT_DEVICE_UNKNOWN;
      if (_ftHandle != IntPtr.Zero)
      {
        getDeviceTypeStatus =
          delegateForFunctionPointer(_ftHandle, ref DeviceType, ref num, numArray, numArray1, IntPtr.Zero);
      }
    }
    else if (_ftGetDeviceInfo == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetDeviceInfo.");
    }

    return getDeviceTypeStatus;
  }

  public FT_STATUS GetDriverVersion(ref uint DriverVersion)
  {
    var getDriverVersionStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getDriverVersionStatus;
    }

    if (_ftGetDriverVersion != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetDriverVersion) Marshal.GetDelegateForFunctionPointer(_ftGetDriverVersion,
          typeof(FtNativeMethods.tFT_GetDriverVersion));
      if (_ftHandle != IntPtr.Zero)
      {
        getDriverVersionStatus = delegateForFunctionPointer(_ftHandle, ref DriverVersion);
      }
    }
    else if (_ftGetDriverVersion == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetDriverVersion.");
    }

    return getDriverVersionStatus;
  }

  public FT_STATUS GetEventType(ref uint EventType)
  {
    var getEventTypeStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getEventTypeStatus;
    }

    if (_ftGetStatus != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetStatus) Marshal.GetDelegateForFunctionPointer(_ftGetStatus,
          typeof(FtNativeMethods.tFT_GetStatus));
      uint num = 0;
      uint num1 = 0;
      if (_ftHandle != IntPtr.Zero)
      {
        getEventTypeStatus = delegateForFunctionPointer(_ftHandle, ref num, ref num1, ref EventType);
      }
    }
    else if (_ftGetStatus == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetStatus.");
    }

    return getEventTypeStatus;
  }

  public FT_STATUS GetLatency(ref byte Latency)
  {
    var getLatencyStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getLatencyStatus;
    }

    if (_ftGetLatencyTimer != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetLatencyTimer) Marshal.GetDelegateForFunctionPointer(_ftGetLatencyTimer,
          typeof(FtNativeMethods.tFT_GetLatencyTimer));
      if (_ftHandle != IntPtr.Zero)
      {
        getLatencyStatus = delegateForFunctionPointer(_ftHandle, ref Latency);
      }
    }
    else if (_ftGetLatencyTimer == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetLatencyTimer.");
    }

    return getLatencyStatus;
  }

  public FT_STATUS GetLibraryVersion(ref uint LibraryVersion)
  {
    var getLibraryVersionStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getLibraryVersionStatus;
    }

    if (_ftGetLibraryVersion != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetLibraryVersion) Marshal.GetDelegateForFunctionPointer(_ftGetLibraryVersion,
          typeof(FtNativeMethods.tFT_GetLibraryVersion));
      getLibraryVersionStatus = delegateForFunctionPointer(ref LibraryVersion);
    }
    else if (_ftGetLibraryVersion == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetLibraryVersion.");
    }

    return getLibraryVersionStatus;
  }

  public FT_STATUS GetLineStatus(ref byte LineStatus)
  {
    var getLineStatusStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getLineStatusStatus;
    }

    if (_ftGetModemStatus != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetModemStatus) Marshal.GetDelegateForFunctionPointer(_ftGetModemStatus,
          typeof(FtNativeMethods.tFT_GetModemStatus));
      uint num = 0;
      if (_ftHandle != IntPtr.Zero)
      {
        getLineStatusStatus = delegateForFunctionPointer(_ftHandle, ref num);
      }

      LineStatus = Convert.ToByte(num >> 8 & 255);
    }
    else if (_ftGetModemStatus == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetModemStatus.");
    }

    return getLineStatusStatus;
  }

  public FT_STATUS GetModemStatus(ref byte ModemStatus)
  {
    var getModemStatusStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getModemStatusStatus;
    }

    if (_ftGetModemStatus != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetModemStatus) Marshal.GetDelegateForFunctionPointer(_ftGetModemStatus,
          typeof(FtNativeMethods.tFT_GetModemStatus));
      uint num = 0;
      if (_ftHandle != IntPtr.Zero)
      {
        getModemStatusStatus = delegateForFunctionPointer(_ftHandle, ref num);
      }

      ModemStatus = Convert.ToByte(num & 255);
    }
    else if (_ftGetModemStatus == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetModemStatus.");
    }

    return getModemStatusStatus;
  }

  public FT_STATUS GetNumberOfDevices(ref uint devcount)
  {
    var getNumberOfDevicesStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getNumberOfDevicesStatus;
    }

    if (_ftCreateDeviceInfoList == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_CreateDeviceInfoList.");
    }
    else
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_CreateDeviceInfoList) Marshal.GetDelegateForFunctionPointer(_ftCreateDeviceInfoList,
          typeof(FtNativeMethods.tFT_CreateDeviceInfoList));
      getNumberOfDevicesStatus = delegateForFunctionPointer(ref devcount);
    }

    return getNumberOfDevicesStatus;
  }

  public FT_STATUS GetPinStates(ref byte BitMode)
  {
    var getPinStatesStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getPinStatesStatus;
    }

    if (_ftGetBitMode != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetBitMode) Marshal.GetDelegateForFunctionPointer(_ftGetBitMode,
          typeof(FtNativeMethods.tFT_GetBitMode));
      if (_ftHandle != IntPtr.Zero)
      {
        getPinStatesStatus = delegateForFunctionPointer(_ftHandle, ref BitMode);
      }
    }
    else if (_ftGetBitMode == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetBitMode.");
    }

    return getPinStatesStatus;
  }

  public FT_STATUS GetRxBytesAvailable(ref uint RxQueue)
  {
    var GetRxBytesAvailableStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return GetRxBytesAvailableStatus;
    }

    if (_ftGetQueueStatus != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetQueueStatus) Marshal.GetDelegateForFunctionPointer(_ftGetQueueStatus,
          typeof(FtNativeMethods.tFT_GetQueueStatus));
      if (_ftHandle != IntPtr.Zero)
      {
        GetRxBytesAvailableStatus = delegateForFunctionPointer(_ftHandle, ref RxQueue);
      }
    }
    else if (_ftGetQueueStatus == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetQueueStatus.");
    }

    return GetRxBytesAvailableStatus;
  }

  public FT_STATUS GetSerialNumber(out string SerialNumber)
  {
    var GetSerialNumberStatus = FT_STATUS.FT_OTHER_ERROR;
    SerialNumber = string.Empty;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return GetSerialNumberStatus;
    }

    if (_ftGetDeviceInfo != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetDeviceInfo) Marshal.GetDelegateForFunctionPointer(_ftGetDeviceInfo,
          typeof(FtNativeMethods.tFT_GetDeviceInfo));
      uint num = 0;
      var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
      var numArray = new byte[16];
      var numArray1 = new byte[64];
      if (_ftHandle != IntPtr.Zero)
      {
        GetSerialNumberStatus =
          delegateForFunctionPointer(_ftHandle, ref fTDEVICE, ref num, numArray, numArray1, IntPtr.Zero);
        SerialNumber = Encoding.ASCII.GetString(numArray);
        SerialNumber = SerialNumber.Substring(0, SerialNumber.IndexOf("\0"));
      }
    }
    else if (_ftGetDeviceInfo == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetDeviceInfo.");
    }

    return GetSerialNumberStatus;
  }

  public FT_STATUS GetTxBytesWaiting(ref uint TxQueue)
  {
    var getTxBytesWaitingStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return getTxBytesWaitingStatus;
    }

    if (_ftGetStatus != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_GetStatus) Marshal.GetDelegateForFunctionPointer(_ftGetStatus,
          typeof(FtNativeMethods.tFT_GetStatus));
      uint num = 0;
      uint num1 = 0;
      if (_ftHandle != IntPtr.Zero)
      {
        getTxBytesWaitingStatus = delegateForFunctionPointer(_ftHandle, ref num, ref TxQueue, ref num1);
      }
    }
    else if (_ftGetStatus == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_GetStatus.");
    }

    return getTxBytesWaitingStatus;
  }

  public FT_STATUS InTransferSize(uint InTransferSize)
  {
    var inTransferSizeStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return inTransferSizeStatus;
    }

    if (_ftSetUsbParameters != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetUSBParameters) Marshal.GetDelegateForFunctionPointer(_ftSetUsbParameters,
          typeof(FtNativeMethods.tFT_SetUSBParameters));
      var num = InTransferSize;
      if (_ftHandle != IntPtr.Zero)
      {
        inTransferSizeStatus = delegateForFunctionPointer(_ftHandle, InTransferSize, num);
      }
    }
    else if (_ftSetUsbParameters == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_SetUSBParameters.");
    }

    return inTransferSizeStatus;
  }

  public FT_STATUS OpenByDescription(string description)
  {
    var openByDescriptionStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return openByDescriptionStatus;
    }

    if (!(_ftOpenEx != IntPtr.Zero & _ftSetDataCharacteristics != IntPtr.Zero & _ftSetFlowControl != IntPtr.Zero &
          _ftSetBaudRate != IntPtr.Zero))
    {
      if (_ftOpenEx == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_OpenEx.");
      }

      if (_ftSetDataCharacteristics == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetDataCharacteristics.");
      }

      if (_ftSetFlowControl == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetFlowControl.");
      }

      if (_ftSetBaudRate == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetBaudRate.");
      }
    }
    else
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_OpenEx) Marshal.GetDelegateForFunctionPointer(_ftOpenEx,
          typeof(FtNativeMethods.tFT_OpenEx));
      var tFTSetDataCharacteristic =
        (FtNativeMethods.tFT_SetDataCharacteristics) Marshal.GetDelegateForFunctionPointer(_ftSetDataCharacteristics,
          typeof(FtNativeMethods.tFT_SetDataCharacteristics));
      var tFTSetFlowControl =
        (FtNativeMethods.tFT_SetFlowControl) Marshal.GetDelegateForFunctionPointer(_ftSetFlowControl,
          typeof(FtNativeMethods.tFT_SetFlowControl));
      var tFTSetBaudRate =
        (FtNativeMethods.tFT_SetBaudRate) Marshal.GetDelegateForFunctionPointer(_ftSetBaudRate,
          typeof(FtNativeMethods.tFT_SetBaudRate));
      openByDescriptionStatus = delegateForFunctionPointer(description, 2, ref _ftHandle);
      if (openByDescriptionStatus != FT_STATUS.FT_OK)
      {
        _ftHandle = IntPtr.Zero;
      }

      if (_ftHandle != IntPtr.Zero)
      {
        openByDescriptionStatus = tFTSetDataCharacteristic(_ftHandle, 8, 0, 0);
        openByDescriptionStatus = tFTSetFlowControl(_ftHandle, 0, 17, 19);
        openByDescriptionStatus = tFTSetBaudRate(_ftHandle, 9600);
      }
    }

    return openByDescriptionStatus;
  }

  public FT_STATUS OpenByIndex(uint index)
  {
    var openByIndexStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return openByIndexStatus;
    }

    if (!(_ftOpen != IntPtr.Zero & _ftSetDataCharacteristics != IntPtr.Zero & _ftSetFlowControl != IntPtr.Zero &
          _ftSetBaudRate != IntPtr.Zero))
    {
      if (_ftOpen == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_Open.");
      }

      if (_ftSetDataCharacteristics == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetDataCharacteristics.");
      }

      if (_ftSetFlowControl == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetFlowControl.");
      }

      if (_ftSetBaudRate == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetBaudRate.");
      }
    }
    else
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_Open) Marshal.GetDelegateForFunctionPointer(_ftOpen, typeof(FtNativeMethods.tFT_Open));
      var tFTSetDataCharacteristic =
        (FtNativeMethods.tFT_SetDataCharacteristics) Marshal.GetDelegateForFunctionPointer(_ftSetDataCharacteristics,
          typeof(FtNativeMethods.tFT_SetDataCharacteristics));
      var tFTSetFlowControl =
        (FtNativeMethods.tFT_SetFlowControl) Marshal.GetDelegateForFunctionPointer(_ftSetFlowControl,
          typeof(FtNativeMethods.tFT_SetFlowControl));
      var tFTSetBaudRate =
        (FtNativeMethods.tFT_SetBaudRate) Marshal.GetDelegateForFunctionPointer(_ftSetBaudRate,
          typeof(FtNativeMethods.tFT_SetBaudRate));
      openByIndexStatus = delegateForFunctionPointer(index, ref _ftHandle);
      if (openByIndexStatus != FT_STATUS.FT_OK)
      {
        _ftHandle = IntPtr.Zero;
      }

      if (_ftHandle != IntPtr.Zero)
      {
        openByIndexStatus = tFTSetDataCharacteristic(_ftHandle, 8, 0, 0);
        openByIndexStatus = tFTSetFlowControl(_ftHandle, 0, 17, 19);
        openByIndexStatus = tFTSetBaudRate(_ftHandle, 9600);
      }
    }

    return openByIndexStatus;
  }

  public FT_STATUS OpenByLocation(uint location)
  {
    var openByLocationStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return openByLocationStatus;
    }

    if (!(_ftOpenEx != IntPtr.Zero & _ftSetDataCharacteristics != IntPtr.Zero & _ftSetFlowControl != IntPtr.Zero &
          _ftSetBaudRate != IntPtr.Zero))
    {
      if (_ftOpenEx == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_OpenEx.");
      }

      if (_ftSetDataCharacteristics == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetDataCharacteristics.");
      }

      if (_ftSetFlowControl == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetFlowControl.");
      }

      if (_ftSetBaudRate == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetBaudRate.");
      }
    }
    else
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_OpenExLoc) Marshal.GetDelegateForFunctionPointer(_ftOpenEx,
          typeof(FtNativeMethods.tFT_OpenExLoc));
      var tFTSetDataCharacteristic =
        (FtNativeMethods.tFT_SetDataCharacteristics) Marshal.GetDelegateForFunctionPointer(_ftSetDataCharacteristics,
          typeof(FtNativeMethods.tFT_SetDataCharacteristics));
      var tFTSetFlowControl =
        (FtNativeMethods.tFT_SetFlowControl) Marshal.GetDelegateForFunctionPointer(_ftSetFlowControl,
          typeof(FtNativeMethods.tFT_SetFlowControl));
      var tFTSetBaudRate =
        (FtNativeMethods.tFT_SetBaudRate) Marshal.GetDelegateForFunctionPointer(_ftSetBaudRate,
          typeof(FtNativeMethods.tFT_SetBaudRate));
      openByLocationStatus = delegateForFunctionPointer(location, 4, ref _ftHandle);
      if (openByLocationStatus != FT_STATUS.FT_OK)
      {
        _ftHandle = IntPtr.Zero;
      }

      if (_ftHandle != IntPtr.Zero)
      {
        openByLocationStatus = tFTSetDataCharacteristic(_ftHandle, 8, 0, 0);
        openByLocationStatus = tFTSetFlowControl(_ftHandle, 0, 17, 19);
        openByLocationStatus = tFTSetBaudRate(_ftHandle, 9600);
      }
    }

    return openByLocationStatus;
  }

  public FT_STATUS OpenBySerialNumber(string serialnumber)
  {
    var openBySerialNumberStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return openBySerialNumberStatus;
    }

    if (!(_ftOpenEx != IntPtr.Zero & _ftSetDataCharacteristics != IntPtr.Zero & _ftSetFlowControl != IntPtr.Zero &
          _ftSetBaudRate != IntPtr.Zero))
    {
      if (_ftOpenEx == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_OpenEx.");
      }

      if (_ftSetDataCharacteristics == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetDataCharacteristics.");
      }

      if (_ftSetFlowControl == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetFlowControl.");
      }

      if (_ftSetBaudRate == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetBaudRate.");
      }
    }
    else
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_OpenEx) Marshal.GetDelegateForFunctionPointer(_ftOpenEx,
          typeof(FtNativeMethods.tFT_OpenEx));
      var tFTSetDataCharacteristic =
        (FtNativeMethods.tFT_SetDataCharacteristics) Marshal.GetDelegateForFunctionPointer(_ftSetDataCharacteristics,
          typeof(FtNativeMethods.tFT_SetDataCharacteristics));
      var tFTSetFlowControl =
        (FtNativeMethods.tFT_SetFlowControl) Marshal.GetDelegateForFunctionPointer(_ftSetFlowControl,
          typeof(FtNativeMethods.tFT_SetFlowControl));
      var tFTSetBaudRate =
        (FtNativeMethods.tFT_SetBaudRate) Marshal.GetDelegateForFunctionPointer(_ftSetBaudRate,
          typeof(FtNativeMethods.tFT_SetBaudRate));
      openBySerialNumberStatus = delegateForFunctionPointer(serialnumber, 1, ref _ftHandle);
      if (openBySerialNumberStatus != FT_STATUS.FT_OK)
      {
        _ftHandle = IntPtr.Zero;
      }

      if (_ftHandle != IntPtr.Zero)
      {
        openBySerialNumberStatus = tFTSetDataCharacteristic(_ftHandle, 8, 0, 0);
        openBySerialNumberStatus = tFTSetFlowControl(_ftHandle, 0, 17, 19);
        openBySerialNumberStatus = tFTSetBaudRate(_ftHandle, 9600);
      }
    }

    return openBySerialNumberStatus;
  }

  public FT_STATUS Purge(uint purgemask)
  {
    var purgeStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return purgeStatus;
    }

    if (_ftPurge != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_Purge) Marshal.GetDelegateForFunctionPointer(_ftPurge, typeof(FtNativeMethods.tFT_Purge));
      if (_ftHandle != IntPtr.Zero)
      {
        purgeStatus = delegateForFunctionPointer(_ftHandle, purgemask);
      }
    }
    else if (_ftPurge == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_Purge.");
    }

    return purgeStatus;
  }

  public FT_STATUS Read(byte[] dataBuffer, uint numBytesToRead, ref uint numBytesRead)
  {
    var readStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return readStatus;
    }

    if (_ftRead != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_Read) Marshal.GetDelegateForFunctionPointer(_ftRead, typeof(FtNativeMethods.tFT_Read));
      if (dataBuffer.Length < numBytesToRead)
      {
        numBytesToRead = (uint) dataBuffer.Length;
      }

      if (_ftHandle != IntPtr.Zero)
      {
        readStatus = delegateForFunctionPointer(_ftHandle, dataBuffer, numBytesToRead, ref numBytesRead);
      }
    }
    else if (_ftRead == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_Read.");
    }

    return readStatus;
  }

  public FT_STATUS Read(out string dataBuffer, uint numBytesToRead, ref uint numBytesRead)
  {
    unsafe
    {
      var readStatus = FT_STATUS.FT_OTHER_ERROR;
      dataBuffer = string.Empty;
      if (_ftd2Xxdll == IntPtr.Zero)
      {
        return readStatus;
      }

      if (_ftRead != IntPtr.Zero)
      {
        var delegateForFunctionPointer =
          (FtNativeMethods.tFT_Read) Marshal.GetDelegateForFunctionPointer(_ftRead, typeof(FtNativeMethods.tFT_Read));
        var numArray = new byte[numBytesToRead];
        if (_ftHandle != IntPtr.Zero)
        {
          readStatus = delegateForFunctionPointer(_ftHandle, numArray, numBytesToRead, ref numBytesRead);
          dataBuffer = Encoding.ASCII.GetString(numArray);
          dataBuffer = dataBuffer.Substring(0, (int) numBytesRead);
        }
      }
      else if (_ftRead == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_Read.");
      }

      return readStatus;
    }
  }

  public FT_STATUS ReadEEPROMLocation(uint Address, ref ushort EEValue)
  {
    var readEepromStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return readEepromStatus;
    }

    if (_ftReadEe != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_ReadEE) Marshal.GetDelegateForFunctionPointer(_ftReadEe,
          typeof(FtNativeMethods.tFT_ReadEE));
      if (_ftHandle != IntPtr.Zero)
      {
        readEepromStatus = delegateForFunctionPointer(_ftHandle, Address, ref EEValue);
      }
    }
    else if (_ftReadEe == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_ReadEE.");
    }

    return readEepromStatus;
  }

  public FT_STATUS ReadFT2232EEPROM(FT2232_EEPROM_STRUCTURE ee2232)
  {
    var readEepromStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return readEepromStatus;
    }

    if (_ftEeRead != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Read) Marshal.GetDelegateForFunctionPointer(_ftEeRead,
          typeof(FtNativeMethods.tFT_EE_Read));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_2232)
        {
          ErrorHandler(readEepromStatus, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 2,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        readEepromStatus = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        ee2232.Manufacturer = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Manufacturer);
        ee2232.ManufacturerID = Marshal.PtrToStringAnsi(fTPROGRAMDATum.ManufacturerID);
        ee2232.Description = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Description);
        ee2232.SerialNumber = Marshal.PtrToStringAnsi(fTPROGRAMDATum.SerialNumber);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
        ee2232.VendorID = fTPROGRAMDATum.VendorID;
        ee2232.ProductID = fTPROGRAMDATum.ProductID;
        ee2232.MaxPower = fTPROGRAMDATum.MaxPower;
        ee2232.SelfPowered = Convert.ToBoolean(fTPROGRAMDATum.SelfPowered);
        ee2232.RemoteWakeup = Convert.ToBoolean(fTPROGRAMDATum.RemoteWakeup);
        ee2232.PullDownEnable = Convert.ToBoolean(fTPROGRAMDATum.PullDownEnable5);
        ee2232.SerNumEnable = Convert.ToBoolean(fTPROGRAMDATum.SerNumEnable5);
        ee2232.USBVersionEnable = Convert.ToBoolean(fTPROGRAMDATum.USBVersionEnable5);
        ee2232.USBVersion = fTPROGRAMDATum.USBVersion5;
        ee2232.AIsHighCurrent = Convert.ToBoolean(fTPROGRAMDATum.AIsHighCurrent);
        ee2232.BIsHighCurrent = Convert.ToBoolean(fTPROGRAMDATum.BIsHighCurrent);
        ee2232.IFAIsFifo = Convert.ToBoolean(fTPROGRAMDATum.IFAIsFifo);
        ee2232.IFAIsFifoTar = Convert.ToBoolean(fTPROGRAMDATum.IFAIsFifoTar);
        ee2232.IFAIsFastSer = Convert.ToBoolean(fTPROGRAMDATum.IFAIsFastSer);
        ee2232.AIsVCP = Convert.ToBoolean(fTPROGRAMDATum.AIsVCP);
        ee2232.IFBIsFifo = Convert.ToBoolean(fTPROGRAMDATum.IFBIsFifo);
        ee2232.IFBIsFifoTar = Convert.ToBoolean(fTPROGRAMDATum.IFBIsFifoTar);
        ee2232.IFBIsFastSer = Convert.ToBoolean(fTPROGRAMDATum.IFBIsFastSer);
        ee2232.BIsVCP = Convert.ToBoolean(fTPROGRAMDATum.BIsVCP);
      }
    }
    else if (_ftEeRead == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Read.");
    }

    return readEepromStatus;
  }

  public FT_STATUS ReadFT2232HEEPROM(FT2232H_EEPROM_STRUCTURE ee2232h)
  {
    var readEepromStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return readEepromStatus;
    }

    if (_ftEeRead != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Read) Marshal.GetDelegateForFunctionPointer(_ftEeRead,
          typeof(FtNativeMethods.tFT_EE_Read));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_2232H)
        {
          ErrorHandler(readEepromStatus, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 3,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        readEepromStatus = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        ee2232h.Manufacturer = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Manufacturer);
        ee2232h.ManufacturerID = Marshal.PtrToStringAnsi(fTPROGRAMDATum.ManufacturerID);
        ee2232h.Description = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Description);
        ee2232h.SerialNumber = Marshal.PtrToStringAnsi(fTPROGRAMDATum.SerialNumber);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
        ee2232h.VendorID = fTPROGRAMDATum.VendorID;
        ee2232h.ProductID = fTPROGRAMDATum.ProductID;
        ee2232h.MaxPower = fTPROGRAMDATum.MaxPower;
        ee2232h.SelfPowered = Convert.ToBoolean(fTPROGRAMDATum.SelfPowered);
        ee2232h.RemoteWakeup = Convert.ToBoolean(fTPROGRAMDATum.RemoteWakeup);
        ee2232h.PullDownEnable = Convert.ToBoolean(fTPROGRAMDATum.PullDownEnable7);
        ee2232h.SerNumEnable = Convert.ToBoolean(fTPROGRAMDATum.SerNumEnable7);
        ee2232h.ALSlowSlew = Convert.ToBoolean(fTPROGRAMDATum.ALSlowSlew);
        ee2232h.ALSchmittInput = Convert.ToBoolean(fTPROGRAMDATum.ALSchmittInput);
        ee2232h.ALDriveCurrent = fTPROGRAMDATum.ALDriveCurrent;
        ee2232h.AHSlowSlew = Convert.ToBoolean(fTPROGRAMDATum.AHSlowSlew);
        ee2232h.AHSchmittInput = Convert.ToBoolean(fTPROGRAMDATum.AHSchmittInput);
        ee2232h.AHDriveCurrent = fTPROGRAMDATum.AHDriveCurrent;
        ee2232h.BLSlowSlew = Convert.ToBoolean(fTPROGRAMDATum.BLSlowSlew);
        ee2232h.BLSchmittInput = Convert.ToBoolean(fTPROGRAMDATum.BLSchmittInput);
        ee2232h.BLDriveCurrent = fTPROGRAMDATum.BLDriveCurrent;
        ee2232h.BHSlowSlew = Convert.ToBoolean(fTPROGRAMDATum.BHSlowSlew);
        ee2232h.BHSchmittInput = Convert.ToBoolean(fTPROGRAMDATum.BHSchmittInput);
        ee2232h.BHDriveCurrent = fTPROGRAMDATum.BHDriveCurrent;
        ee2232h.IFAIsFifo = Convert.ToBoolean(fTPROGRAMDATum.IFAIsFifo7);
        ee2232h.IFAIsFifoTar = Convert.ToBoolean(fTPROGRAMDATum.IFAIsFifoTar7);
        ee2232h.IFAIsFastSer = Convert.ToBoolean(fTPROGRAMDATum.IFAIsFastSer7);
        ee2232h.AIsVCP = Convert.ToBoolean(fTPROGRAMDATum.AIsVCP7);
        ee2232h.IFBIsFifo = Convert.ToBoolean(fTPROGRAMDATum.IFBIsFifo7);
        ee2232h.IFBIsFifoTar = Convert.ToBoolean(fTPROGRAMDATum.IFBIsFifoTar7);
        ee2232h.IFBIsFastSer = Convert.ToBoolean(fTPROGRAMDATum.IFBIsFastSer7);
        ee2232h.BIsVCP = Convert.ToBoolean(fTPROGRAMDATum.BIsVCP7);
        ee2232h.PowerSaveEnable = Convert.ToBoolean(fTPROGRAMDATum.PowerSaveEnable);
      }
    }
    else if (_ftEeRead == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Read.");
    }

    return readEepromStatus;
  }

  public FT_STATUS ReadFT232BEEPROM(FT232B_EEPROM_STRUCTURE ee232b)
  {
    var readEepromStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return readEepromStatus;
    }

    if (_ftEeRead != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Read) Marshal.GetDelegateForFunctionPointer(_ftEeRead,
          typeof(FtNativeMethods.tFT_EE_Read));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_BM)
        {
          ErrorHandler(readEepromStatus, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 2,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        readEepromStatus = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        ee232b.Manufacturer = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Manufacturer);
        ee232b.ManufacturerID = Marshal.PtrToStringAnsi(fTPROGRAMDATum.ManufacturerID);
        ee232b.Description = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Description);
        ee232b.SerialNumber = Marshal.PtrToStringAnsi(fTPROGRAMDATum.SerialNumber);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
        ee232b.VendorID = fTPROGRAMDATum.VendorID;
        ee232b.ProductID = fTPROGRAMDATum.ProductID;
        ee232b.MaxPower = fTPROGRAMDATum.MaxPower;
        ee232b.SelfPowered = Convert.ToBoolean(fTPROGRAMDATum.SelfPowered);
        ee232b.RemoteWakeup = Convert.ToBoolean(fTPROGRAMDATum.RemoteWakeup);
        ee232b.PullDownEnable = Convert.ToBoolean(fTPROGRAMDATum.PullDownEnable);
        ee232b.SerNumEnable = Convert.ToBoolean(fTPROGRAMDATum.SerNumEnable);
        ee232b.USBVersionEnable = Convert.ToBoolean(fTPROGRAMDATum.USBVersionEnable);
        ee232b.USBVersion = fTPROGRAMDATum.USBVersion;
      }
    }
    else if (_ftEeRead == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Read.");
    }

    return readEepromStatus;
  }

  public FT_STATUS ReadFT232HEEPROM(FT232H_EEPROM_STRUCTURE ee232h)
  {
    var readEepromStatus = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return readEepromStatus;
    }

    if (_ftEeRead != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Read) Marshal.GetDelegateForFunctionPointer(_ftEeRead,
          typeof(FtNativeMethods.tFT_EE_Read));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_232H)
        {
          ErrorHandler(readEepromStatus, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 5,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        readEepromStatus = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        ee232h.Manufacturer = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Manufacturer);
        ee232h.ManufacturerID = Marshal.PtrToStringAnsi(fTPROGRAMDATum.ManufacturerID);
        ee232h.Description = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Description);
        ee232h.SerialNumber = Marshal.PtrToStringAnsi(fTPROGRAMDATum.SerialNumber);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
        ee232h.VendorID = fTPROGRAMDATum.VendorID;
        ee232h.ProductID = fTPROGRAMDATum.ProductID;
        ee232h.MaxPower = fTPROGRAMDATum.MaxPower;
        ee232h.SelfPowered = Convert.ToBoolean(fTPROGRAMDATum.SelfPowered);
        ee232h.RemoteWakeup = Convert.ToBoolean(fTPROGRAMDATum.RemoteWakeup);
        ee232h.PullDownEnable = Convert.ToBoolean(fTPROGRAMDATum.PullDownEnableH);
        ee232h.SerNumEnable = Convert.ToBoolean(fTPROGRAMDATum.SerNumEnableH);
        ee232h.ACSlowSlew = Convert.ToBoolean(fTPROGRAMDATum.ACSlowSlewH);
        ee232h.ACSchmittInput = Convert.ToBoolean(fTPROGRAMDATum.ACSchmittInputH);
        ee232h.ACDriveCurrent = fTPROGRAMDATum.ACDriveCurrentH;
        ee232h.ADSlowSlew = Convert.ToBoolean(fTPROGRAMDATum.ADSlowSlewH);
        ee232h.ADSchmittInput = Convert.ToBoolean(fTPROGRAMDATum.ADSchmittInputH);
        ee232h.ADDriveCurrent = fTPROGRAMDATum.ADDriveCurrentH;
        ee232h.Cbus0 = fTPROGRAMDATum.Cbus0H;
        ee232h.Cbus1 = fTPROGRAMDATum.Cbus1H;
        ee232h.Cbus2 = fTPROGRAMDATum.Cbus2H;
        ee232h.Cbus3 = fTPROGRAMDATum.Cbus3H;
        ee232h.Cbus4 = fTPROGRAMDATum.Cbus4H;
        ee232h.Cbus5 = fTPROGRAMDATum.Cbus5H;
        ee232h.Cbus6 = fTPROGRAMDATum.Cbus6H;
        ee232h.Cbus7 = fTPROGRAMDATum.Cbus7H;
        ee232h.Cbus8 = fTPROGRAMDATum.Cbus8H;
        ee232h.Cbus9 = fTPROGRAMDATum.Cbus9H;
        ee232h.IsFifo = Convert.ToBoolean(fTPROGRAMDATum.IsFifoH);
        ee232h.IsFifoTar = Convert.ToBoolean(fTPROGRAMDATum.IsFifoTarH);
        ee232h.IsFastSer = Convert.ToBoolean(fTPROGRAMDATum.IsFastSerH);
        ee232h.IsFT1248 = Convert.ToBoolean(fTPROGRAMDATum.IsFT1248H);
        ee232h.FT1248Cpol = Convert.ToBoolean(fTPROGRAMDATum.FT1248CpolH);
        ee232h.FT1248Lsb = Convert.ToBoolean(fTPROGRAMDATum.FT1248LsbH);
        ee232h.FT1248FlowControl = Convert.ToBoolean(fTPROGRAMDATum.FT1248FlowControlH);
        ee232h.IsVCP = Convert.ToBoolean(fTPROGRAMDATum.IsVCPH);
        ee232h.PowerSaveEnable = Convert.ToBoolean(fTPROGRAMDATum.PowerSaveEnableH);
      }
    }
    else if (_ftEeRead == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Read.");
    }

    return readEepromStatus;
  }

  public FT_STATUS ReadFT232REEPROM(FT232R_EEPROM_STRUCTURE ee232r)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftEeRead != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Read) Marshal.GetDelegateForFunctionPointer(_ftEeRead,
          typeof(FtNativeMethods.tFT_EE_Read));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_232R)
        {
          ErrorHandler(fTSTATU, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 2,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        fTSTATU = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        ee232r.Manufacturer = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Manufacturer);
        ee232r.ManufacturerID = Marshal.PtrToStringAnsi(fTPROGRAMDATum.ManufacturerID);
        ee232r.Description = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Description);
        ee232r.SerialNumber = Marshal.PtrToStringAnsi(fTPROGRAMDATum.SerialNumber);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
        ee232r.VendorID = fTPROGRAMDATum.VendorID;
        ee232r.ProductID = fTPROGRAMDATum.ProductID;
        ee232r.MaxPower = fTPROGRAMDATum.MaxPower;
        ee232r.SelfPowered = Convert.ToBoolean(fTPROGRAMDATum.SelfPowered);
        ee232r.RemoteWakeup = Convert.ToBoolean(fTPROGRAMDATum.RemoteWakeup);
        ee232r.UseExtOsc = Convert.ToBoolean(fTPROGRAMDATum.UseExtOsc);
        ee232r.HighDriveIOs = Convert.ToBoolean(fTPROGRAMDATum.HighDriveIOs);
        ee232r.EndpointSize = fTPROGRAMDATum.EndpointSize;
        ee232r.PullDownEnable = Convert.ToBoolean(fTPROGRAMDATum.PullDownEnableR);
        ee232r.SerNumEnable = Convert.ToBoolean(fTPROGRAMDATum.SerNumEnableR);
        ee232r.InvertTXD = Convert.ToBoolean(fTPROGRAMDATum.InvertTXD);
        ee232r.InvertRXD = Convert.ToBoolean(fTPROGRAMDATum.InvertRXD);
        ee232r.InvertRTS = Convert.ToBoolean(fTPROGRAMDATum.InvertRTS);
        ee232r.InvertCTS = Convert.ToBoolean(fTPROGRAMDATum.InvertCTS);
        ee232r.InvertDTR = Convert.ToBoolean(fTPROGRAMDATum.InvertDTR);
        ee232r.InvertDSR = Convert.ToBoolean(fTPROGRAMDATum.InvertDSR);
        ee232r.InvertDCD = Convert.ToBoolean(fTPROGRAMDATum.InvertDCD);
        ee232r.InvertRI = Convert.ToBoolean(fTPROGRAMDATum.InvertRI);
        ee232r.Cbus0 = fTPROGRAMDATum.Cbus0;
        ee232r.Cbus1 = fTPROGRAMDATum.Cbus1;
        ee232r.Cbus2 = fTPROGRAMDATum.Cbus2;
        ee232r.Cbus3 = fTPROGRAMDATum.Cbus3;
        ee232r.Cbus4 = fTPROGRAMDATum.Cbus4;
        ee232r.RIsD2XX = Convert.ToBoolean(fTPROGRAMDATum.RIsD2XX);
      }
    }
    else if (_ftEeRead == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Read.");
    }

    return fTSTATU;
  }

  public FT_STATUS ReadFT4232HEEPROM(FT4232H_EEPROM_STRUCTURE ee4232h)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftEeRead != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Read) Marshal.GetDelegateForFunctionPointer(_ftEeRead,
          typeof(FtNativeMethods.tFT_EE_Read));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_4232H)
        {
          ErrorHandler(fTSTATU, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 4,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        fTSTATU = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        ee4232h.Manufacturer = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Manufacturer);
        ee4232h.ManufacturerID = Marshal.PtrToStringAnsi(fTPROGRAMDATum.ManufacturerID);
        ee4232h.Description = Marshal.PtrToStringAnsi(fTPROGRAMDATum.Description);
        ee4232h.SerialNumber = Marshal.PtrToStringAnsi(fTPROGRAMDATum.SerialNumber);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
        ee4232h.VendorID = fTPROGRAMDATum.VendorID;
        ee4232h.ProductID = fTPROGRAMDATum.ProductID;
        ee4232h.MaxPower = fTPROGRAMDATum.MaxPower;
        ee4232h.SelfPowered = Convert.ToBoolean(fTPROGRAMDATum.SelfPowered);
        ee4232h.RemoteWakeup = Convert.ToBoolean(fTPROGRAMDATum.RemoteWakeup);
        ee4232h.PullDownEnable = Convert.ToBoolean(fTPROGRAMDATum.PullDownEnable8);
        ee4232h.SerNumEnable = Convert.ToBoolean(fTPROGRAMDATum.SerNumEnable8);
        ee4232h.ASlowSlew = Convert.ToBoolean(fTPROGRAMDATum.ASlowSlew);
        ee4232h.ASchmittInput = Convert.ToBoolean(fTPROGRAMDATum.ASchmittInput);
        ee4232h.ADriveCurrent = fTPROGRAMDATum.ADriveCurrent;
        ee4232h.BSlowSlew = Convert.ToBoolean(fTPROGRAMDATum.BSlowSlew);
        ee4232h.BSchmittInput = Convert.ToBoolean(fTPROGRAMDATum.BSchmittInput);
        ee4232h.BDriveCurrent = fTPROGRAMDATum.BDriveCurrent;
        ee4232h.CSlowSlew = Convert.ToBoolean(fTPROGRAMDATum.CSlowSlew);
        ee4232h.CSchmittInput = Convert.ToBoolean(fTPROGRAMDATum.CSchmittInput);
        ee4232h.CDriveCurrent = fTPROGRAMDATum.CDriveCurrent;
        ee4232h.DSlowSlew = Convert.ToBoolean(fTPROGRAMDATum.DSlowSlew);
        ee4232h.DSchmittInput = Convert.ToBoolean(fTPROGRAMDATum.DSchmittInput);
        ee4232h.DDriveCurrent = fTPROGRAMDATum.DDriveCurrent;
        ee4232h.ARIIsTXDEN = Convert.ToBoolean(fTPROGRAMDATum.ARIIsTXDEN);
        ee4232h.BRIIsTXDEN = Convert.ToBoolean(fTPROGRAMDATum.BRIIsTXDEN);
        ee4232h.CRIIsTXDEN = Convert.ToBoolean(fTPROGRAMDATum.CRIIsTXDEN);
        ee4232h.DRIIsTXDEN = Convert.ToBoolean(fTPROGRAMDATum.DRIIsTXDEN);
        ee4232h.AIsVCP = Convert.ToBoolean(fTPROGRAMDATum.AIsVCP8);
        ee4232h.BIsVCP = Convert.ToBoolean(fTPROGRAMDATum.BIsVCP8);
        ee4232h.CIsVCP = Convert.ToBoolean(fTPROGRAMDATum.CIsVCP8);
        ee4232h.DIsVCP = Convert.ToBoolean(fTPROGRAMDATum.DIsVCP8);
      }
    }
    else if (_ftEeRead == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Read.");
    }

    return fTSTATU;
  }

  public FT_STATUS ReadXSeriesEEPROM(FT_XSERIES_EEPROM_STRUCTURE eeX)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftEepromRead != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EEPROM_Read) Marshal.GetDelegateForFunctionPointer(_ftEepromRead,
          typeof(FtNativeMethods.tFT_EEPROM_Read));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_X_SERIES)
        {
          ErrorHandler(fTSTATU, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        var structure = new FT_XSERIES_DATA();
        var fTEEPROMHEADER = new FT_EEPROM_HEADER();
        var numArray = new byte[32];
        var numArray1 = new byte[16];
        var numArray2 = new byte[64];
        var numArray3 = new byte[16];
        fTEEPROMHEADER.deviceType = 9;
        structure.common = fTEEPROMHEADER;
        var num = Marshal.SizeOf(structure);
        var intPtr = Marshal.AllocHGlobal(num);
        Marshal.StructureToPtr(structure, intPtr, false);
        fTSTATU = delegateForFunctionPointer(_ftHandle, intPtr, num, numArray, numArray1, numArray2, numArray3);
        if (fTSTATU == FT_STATUS.FT_OK)
        {
          structure = (FT_XSERIES_DATA) Marshal.PtrToStructure(intPtr, typeof(FT_XSERIES_DATA));
          var uTF8Encoding = new UTF8Encoding();
          eeX.Manufacturer = uTF8Encoding.GetString(numArray);
          eeX.ManufacturerID = uTF8Encoding.GetString(numArray1);
          eeX.Description = uTF8Encoding.GetString(numArray2);
          eeX.SerialNumber = uTF8Encoding.GetString(numArray3);
          eeX.VendorID = structure.common.VendorId;
          eeX.ProductID = structure.common.ProductId;
          eeX.MaxPower = structure.common.MaxPower;
          eeX.SelfPowered = Convert.ToBoolean(structure.common.SelfPowered);
          eeX.RemoteWakeup = Convert.ToBoolean(structure.common.RemoteWakeup);
          eeX.SerNumEnable = Convert.ToBoolean(structure.common.SerNumEnable);
          eeX.PullDownEnable = Convert.ToBoolean(structure.common.PullDownEnable);
          eeX.Cbus0 = structure.Cbus0;
          eeX.Cbus1 = structure.Cbus1;
          eeX.Cbus2 = structure.Cbus2;
          eeX.Cbus3 = structure.Cbus3;
          eeX.Cbus4 = structure.Cbus4;
          eeX.Cbus5 = structure.Cbus5;
          eeX.Cbus6 = structure.Cbus6;
          eeX.ACDriveCurrent = structure.ACDriveCurrent;
          eeX.ACSchmittInput = structure.ACSchmittInput;
          eeX.ACSlowSlew = structure.ACSlowSlew;
          eeX.ADDriveCurrent = structure.ADDriveCurrent;
          eeX.ADSchmittInput = structure.ADSchmittInput;
          eeX.ADSlowSlew = structure.ADSlowSlew;
          eeX.BCDDisableSleep = structure.BCDDisableSleep;
          eeX.BCDEnable = structure.BCDEnable;
          eeX.BCDForceCbusPWREN = structure.BCDForceCbusPWREN;
          eeX.FT1248Cpol = structure.FT1248Cpol;
          eeX.FT1248FlowControl = structure.FT1248FlowControl;
          eeX.FT1248Lsb = structure.FT1248Lsb;
          eeX.I2CDeviceId = structure.I2CDeviceId;
          eeX.I2CDisableSchmitt = structure.I2CDisableSchmitt;
          eeX.I2CSlaveAddress = structure.I2CSlaveAddress;
          eeX.InvertCTS = structure.InvertCTS;
          eeX.InvertDCD = structure.InvertDCD;
          eeX.InvertDSR = structure.InvertDSR;
          eeX.InvertDTR = structure.InvertDTR;
          eeX.InvertRI = structure.InvertRI;
          eeX.InvertRTS = structure.InvertRTS;
          eeX.InvertRXD = structure.InvertRXD;
          eeX.InvertTXD = structure.InvertTXD;
          eeX.PowerSaveEnable = structure.PowerSaveEnable;
          eeX.RS485EchoSuppress = structure.RS485EchoSuppress;
          eeX.IsVCP = structure.DriverType;
        }
      }
    }
    else if (_ftEeRead == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Read.");
    }

    return fTSTATU;
  }

  public FT_STATUS Reload(ushort VendorID, ushort ProductID)
  {
    var vendorID = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return vendorID;
    }

    if (_ftReload != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_Reload) Marshal.GetDelegateForFunctionPointer(_ftReload,
          typeof(FtNativeMethods.tFT_Reload));
      vendorID = delegateForFunctionPointer(VendorID, ProductID);
    }
    else if (_ftReload == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_Reload.");
    }

    return vendorID;
  }

  public FT_STATUS Rescan()
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftRescan != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_Rescan) Marshal.GetDelegateForFunctionPointer(_ftRescan,
          typeof(FtNativeMethods.tFT_Rescan));
      fTSTATU = delegateForFunctionPointer();
    }
    else if (_ftRescan == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_Rescan.");
    }

    return fTSTATU;
  }

  public FT_STATUS ResetDevice()
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftResetDevice != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_ResetDevice) Marshal.GetDelegateForFunctionPointer(_ftResetDevice,
          typeof(FtNativeMethods.tFT_ResetDevice));
      if (_ftHandle != IntPtr.Zero)
      {
        fTSTATU = delegateForFunctionPointer(_ftHandle);
      }
    }
    else if (_ftResetDevice == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_ResetDevice.");
    }

    return fTSTATU;
  }

  public FT_STATUS ResetPort()
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftResetPort != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_ResetPort) Marshal.GetDelegateForFunctionPointer(_ftResetPort,
          typeof(FtNativeMethods.tFT_ResetPort));
      if (_ftHandle != IntPtr.Zero)
      {
        fTSTATU = delegateForFunctionPointer(_ftHandle);
      }
    }
    else if (_ftResetPort == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_ResetPort.");
    }

    return fTSTATU;
  }

  public FT_STATUS RestartInTask()
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftRestartInTask != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_RestartInTask) Marshal.GetDelegateForFunctionPointer(_ftRestartInTask,
          typeof(FtNativeMethods.tFT_RestartInTask));
      if (_ftHandle != IntPtr.Zero)
      {
        fTSTATU = delegateForFunctionPointer(_ftHandle);
      }
    }
    else if (_ftRestartInTask == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_RestartInTask.");
    }

    return fTSTATU;
  }

  public FT_STATUS SetBaudRate(uint BaudRate)
  {
    var baudRate = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return baudRate;
    }

    if (_ftSetBaudRate != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetBaudRate) Marshal.GetDelegateForFunctionPointer(_ftSetBaudRate,
          typeof(FtNativeMethods.tFT_SetBaudRate));
      if (_ftHandle != IntPtr.Zero)
      {
        baudRate = delegateForFunctionPointer(_ftHandle, BaudRate);
      }
    }
    else if (_ftSetBaudRate == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_SetBaudRate.");
    }

    return baudRate;
  }

  public FT_STATUS SetBitMode(byte Mask, byte BitMode)
  {
    var mask = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return mask;
    }

    if (_ftSetBitMode != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetBitMode) Marshal.GetDelegateForFunctionPointer(_ftSetBitMode,
          typeof(FtNativeMethods.tFT_SetBitMode));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE == FT_DEVICE.FT_DEVICE_AM)
        {
          ErrorHandler(mask, FT_ERROR.FT_INVALID_BITMODE);
        }
        else if (fTDEVICE == FT_DEVICE.FT_DEVICE_100AX)
        {
          ErrorHandler(mask, FT_ERROR.FT_INVALID_BITMODE);
        }
        else if (fTDEVICE == FT_DEVICE.FT_DEVICE_BM && BitMode != 0)
        {
          if ((BitMode & 1) == 0)
          {
            ErrorHandler(mask, FT_ERROR.FT_INVALID_BITMODE);
          }
        }
        else if (fTDEVICE == FT_DEVICE.FT_DEVICE_2232 && BitMode != 0)
        {
          if ((BitMode & 31) == 0)
          {
            ErrorHandler(mask, FT_ERROR.FT_INVALID_BITMODE);
          }

          if (BitMode == 2 & InterfaceIdentifier != "A")
          {
            ErrorHandler(mask, FT_ERROR.FT_INVALID_BITMODE);
          }
        }
        else if (fTDEVICE == FT_DEVICE.FT_DEVICE_232R && BitMode != 0)
        {
          if ((BitMode & 37) == 0)
          {
            ErrorHandler(mask, FT_ERROR.FT_INVALID_BITMODE);
          }
        }
        else if (fTDEVICE == FT_DEVICE.FT_DEVICE_2232H && BitMode != 0)
        {
          if ((BitMode & 95) == 0)
          {
            ErrorHandler(mask, FT_ERROR.FT_INVALID_BITMODE);
          }

          if ((BitMode == 8 | BitMode == 64) & InterfaceIdentifier != "A")
          {
            ErrorHandler(mask, FT_ERROR.FT_INVALID_BITMODE);
          }
        }
        else if (fTDEVICE == FT_DEVICE.FT_DEVICE_4232H && BitMode != 0)
        {
          if ((BitMode & 7) == 0)
          {
            ErrorHandler(mask, FT_ERROR.FT_INVALID_BITMODE);
          }

          if (BitMode == 2 & InterfaceIdentifier != "A" & InterfaceIdentifier != "B")
          {
            ErrorHandler(mask, FT_ERROR.FT_INVALID_BITMODE);
          }
        }
        else if (fTDEVICE == FT_DEVICE.FT_DEVICE_232H && BitMode != 0 && BitMode > 64)
        {
          ErrorHandler(mask, FT_ERROR.FT_INVALID_BITMODE);
        }

        mask = delegateForFunctionPointer(_ftHandle, Mask, BitMode);
      }
    }
    else if (_ftSetBitMode == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_SetBitMode.");
    }

    return mask;
  }

  public FT_STATUS SetBreak(bool Enable)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (!(_ftSetBreakOn != IntPtr.Zero & _ftSetBreakOff != IntPtr.Zero))
    {
      if (_ftSetBreakOn == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetBreakOn.");
      }

      if (_ftSetBreakOff == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetBreakOff.");
      }
    }
    else
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetBreakOn) Marshal.GetDelegateForFunctionPointer(_ftSetBreakOn,
          typeof(FtNativeMethods.tFT_SetBreakOn));
      var tFTSetBreakOff =
        (FtNativeMethods.tFT_SetBreakOff) Marshal.GetDelegateForFunctionPointer(_ftSetBreakOff,
          typeof(FtNativeMethods.tFT_SetBreakOff));
      if (_ftHandle != IntPtr.Zero)
      {
        fTSTATU = !Enable ? tFTSetBreakOff(_ftHandle) : delegateForFunctionPointer(_ftHandle);
      }
    }

    return fTSTATU;
  }

  public FT_STATUS SetCharacters(byte EventChar, bool EventCharEnable, byte ErrorChar, bool ErrorCharEnable)
  {
    var eventChar = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return eventChar;
    }

    if (_ftSetChars != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetChars) Marshal.GetDelegateForFunctionPointer(_ftSetChars,
          typeof(FtNativeMethods.tFT_SetChars));
      if (_ftHandle != IntPtr.Zero)
      {
        eventChar = delegateForFunctionPointer(_ftHandle, EventChar, Convert.ToByte(EventCharEnable), ErrorChar,
          Convert.ToByte(ErrorCharEnable));
      }
    }
    else if (_ftSetChars == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_SetChars.");
    }

    return eventChar;
  }

  public FT_STATUS SetDataCharacteristics(byte DataBits, byte StopBits, byte Parity)
  {
    var dataBits = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return dataBits;
    }

    if (_ftSetDataCharacteristics != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetDataCharacteristics) Marshal.GetDelegateForFunctionPointer(_ftSetDataCharacteristics,
          typeof(FtNativeMethods.tFT_SetDataCharacteristics));
      if (_ftHandle != IntPtr.Zero)
      {
        dataBits = delegateForFunctionPointer(_ftHandle, DataBits, StopBits, Parity);
      }
    }
    else if (_ftSetDataCharacteristics == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_SetDataCharacteristics.");
    }

    return dataBits;
  }

  public FT_STATUS SetDeadmanTimeout(uint DeadmanTimeout)
  {
    var deadmanTimeout = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return deadmanTimeout;
    }

    if (_ftSetDeadmanTimeout != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetDeadmanTimeout) Marshal.GetDelegateForFunctionPointer(_ftSetDeadmanTimeout,
          typeof(FtNativeMethods.tFT_SetDeadmanTimeout));
      if (_ftHandle != IntPtr.Zero)
      {
        deadmanTimeout = delegateForFunctionPointer(_ftHandle, DeadmanTimeout);
      }
    }
    else if (_ftSetDeadmanTimeout == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_SetDeadmanTimeout.");
    }

    return deadmanTimeout;
  }

  public FT_STATUS SetDTR(bool Enable)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (!(_ftSetDtr != IntPtr.Zero & _ftClrDtr != IntPtr.Zero))
    {
      if (_ftSetDtr == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetDtr.");
      }

      if (_ftClrDtr == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_ClrDtr.");
      }
    }
    else
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetDtr) Marshal.GetDelegateForFunctionPointer(_ftSetDtr,
          typeof(FtNativeMethods.tFT_SetDtr));
      var tFTClrDtr =
        (FtNativeMethods.tFT_ClrDtr) Marshal.GetDelegateForFunctionPointer(_ftClrDtr,
          typeof(FtNativeMethods.tFT_ClrDtr));
      if (_ftHandle != IntPtr.Zero)
      {
        fTSTATU = !Enable ? tFTClrDtr(_ftHandle) : delegateForFunctionPointer(_ftHandle);
      }
    }

    return fTSTATU;
  }

  public FT_STATUS SetEventNotification(uint eventmask, EventWaitHandle eventhandle)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftSetEventNotification != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetEventNotification) Marshal.GetDelegateForFunctionPointer(_ftSetEventNotification,
          typeof(FtNativeMethods.tFT_SetEventNotification));
      if (_ftHandle != IntPtr.Zero)
      {
        fTSTATU = delegateForFunctionPointer(_ftHandle, eventmask, eventhandle.SafeWaitHandle);
      }
    }
    else if (_ftSetEventNotification == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_SetEventNotification.");
    }

    return fTSTATU;
  }

  public FT_STATUS SetFlowControl(ushort FlowControl, byte Xon, byte Xoff)
  {
    var flowControl = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return flowControl;
    }

    if (_ftSetFlowControl != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetFlowControl) Marshal.GetDelegateForFunctionPointer(_ftSetFlowControl,
          typeof(FtNativeMethods.tFT_SetFlowControl));
      if (_ftHandle != IntPtr.Zero)
      {
        flowControl = delegateForFunctionPointer(_ftHandle, FlowControl, Xon, Xoff);
      }
    }
    else if (_ftSetFlowControl == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_SetFlowControl.");
    }

    return flowControl;
  }

  public FT_STATUS SetLatency(byte Latency)
  {
    var latency = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return latency;
    }

    if (_ftSetLatencyTimer != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetLatencyTimer) Marshal.GetDelegateForFunctionPointer(_ftSetLatencyTimer,
          typeof(FtNativeMethods.tFT_SetLatencyTimer));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if ((fTDEVICE == FT_DEVICE.FT_DEVICE_BM || fTDEVICE == FT_DEVICE.FT_DEVICE_2232) && Latency < 2)
        {
          Latency = 2;
        }

        latency = delegateForFunctionPointer(_ftHandle, Latency);
      }
    }
    else if (_ftSetLatencyTimer == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_SetLatencyTimer.");
    }

    return latency;
  }

  public FT_STATUS SetResetPipeRetryCount(uint ResetPipeRetryCount)
  {
    var resetPipeRetryCount = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return resetPipeRetryCount;
    }

    if (_ftSetResetPipeRetryCount != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetResetPipeRetryCount) Marshal.GetDelegateForFunctionPointer(_ftSetResetPipeRetryCount,
          typeof(FtNativeMethods.tFT_SetResetPipeRetryCount));
      if (_ftHandle != IntPtr.Zero)
      {
        resetPipeRetryCount = delegateForFunctionPointer(_ftHandle, ResetPipeRetryCount);
      }
    }
    else if (_ftSetResetPipeRetryCount == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_SetResetPipeRetryCount.");
    }

    return resetPipeRetryCount;
  }

  public FT_STATUS SetRTS(bool Enable)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (!(_ftSetRts != IntPtr.Zero & _ftClrRts != IntPtr.Zero))
    {
      if (_ftSetRts == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_SetRts.");
      }

      if (_ftClrRts == IntPtr.Zero)
      {
        Console.WriteLine("Failed to load function FT_ClrRts.");
      }
    }
    else
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetRts) Marshal.GetDelegateForFunctionPointer(_ftSetRts,
          typeof(FtNativeMethods.tFT_SetRts));
      var tFTClrRt =
        (FtNativeMethods.tFT_ClrRts) Marshal.GetDelegateForFunctionPointer(_ftClrRts,
          typeof(FtNativeMethods.tFT_ClrRts));
      if (_ftHandle != IntPtr.Zero)
      {
        fTSTATU = !Enable ? tFTClrRt(_ftHandle) : delegateForFunctionPointer(_ftHandle);
      }
    }

    return fTSTATU;
  }

  public FT_STATUS SetTimeouts(uint ReadTimeout, uint WriteTimeout)
  {
    var readTimeout = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return readTimeout;
    }

    if (_ftSetTimeouts != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_SetTimeouts) Marshal.GetDelegateForFunctionPointer(_ftSetTimeouts,
          typeof(FtNativeMethods.tFT_SetTimeouts));
      if (_ftHandle != IntPtr.Zero)
      {
        readTimeout = delegateForFunctionPointer(_ftHandle, ReadTimeout, WriteTimeout);
      }
    }
    else if (_ftSetTimeouts == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_SetTimeouts.");
    }

    return readTimeout;
  }

  public FT_STATUS StopInTask()
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftStopInTask != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_StopInTask) Marshal.GetDelegateForFunctionPointer(_ftStopInTask,
          typeof(FtNativeMethods.tFT_StopInTask));
      if (_ftHandle != IntPtr.Zero)
      {
        fTSTATU = delegateForFunctionPointer(_ftHandle);
      }
    }
    else if (_ftStopInTask == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_StopInTask.");
    }

    return fTSTATU;
  }

  public FT_STATUS Write(byte[] dataBuffer, int numBytesToWrite, ref int numBytesWritten)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftWrite != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_Write) Marshal.GetDelegateForFunctionPointer(_ftWrite, typeof(FtNativeMethods.tFT_Write));
      if (_ftHandle != IntPtr.Zero)
      {
        fTSTATU = delegateForFunctionPointer(_ftHandle, dataBuffer, numBytesToWrite, ref numBytesWritten);
      }
    }
    else if (_ftWrite == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_Write.");
    }

    return fTSTATU;
  }

  public FT_STATUS Write(string dataBuffer, int numBytesToWrite, ref int numBytesWritten)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftWrite != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_Write) Marshal.GetDelegateForFunctionPointer(_ftWrite, typeof(FtNativeMethods.tFT_Write));
      var bytes = Encoding.ASCII.GetBytes(dataBuffer);
      if (_ftHandle != IntPtr.Zero)
      {
        fTSTATU = delegateForFunctionPointer(_ftHandle, bytes, numBytesToWrite, ref numBytesWritten);
      }
    }
    else if (_ftWrite == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_Write.");
    }

    return fTSTATU;
  }

  public FT_STATUS WriteEEPROMLocation(uint Address, ushort EEValue)
  {
    var address = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return address;
    }

    if (_ftWriteEe != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_WriteEE) Marshal.GetDelegateForFunctionPointer(_ftWriteEe,
          typeof(FtNativeMethods.tFT_WriteEE));
      if (_ftHandle != IntPtr.Zero)
      {
        address = delegateForFunctionPointer(_ftHandle, Address, EEValue);
      }
    }
    else if (_ftWriteEe == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_WriteEE.");
    }

    return address;
  }

  public FT_STATUS WriteFT2232EEPROM(FT2232_EEPROM_STRUCTURE ee2232)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftEeProgram != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Program) Marshal.GetDelegateForFunctionPointer(_ftEeProgram,
          typeof(FtNativeMethods.tFT_EE_Program));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_2232)
        {
          ErrorHandler(fTSTATU, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        if (ee2232.VendorID == 0 | ee2232.ProductID == 0)
        {
          return FT_STATUS.FT_INVALID_PARAMETER;
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 2,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        if (ee2232.Manufacturer.Length > 32)
        {
          ee2232.Manufacturer = ee2232.Manufacturer.Substring(0, 32);
        }

        if (ee2232.ManufacturerID.Length > 16)
        {
          ee2232.ManufacturerID = ee2232.ManufacturerID.Substring(0, 16);
        }

        if (ee2232.Description.Length > 64)
        {
          ee2232.Description = ee2232.Description.Substring(0, 64);
        }

        if (ee2232.SerialNumber.Length > 16)
        {
          ee2232.SerialNumber = ee2232.SerialNumber.Substring(0, 16);
        }

        fTPROGRAMDATum.Manufacturer = Marshal.StringToHGlobalAnsi(ee2232.Manufacturer);
        fTPROGRAMDATum.ManufacturerID = Marshal.StringToHGlobalAnsi(ee2232.ManufacturerID);
        fTPROGRAMDATum.Description = Marshal.StringToHGlobalAnsi(ee2232.Description);
        fTPROGRAMDATum.SerialNumber = Marshal.StringToHGlobalAnsi(ee2232.SerialNumber);
        fTPROGRAMDATum.VendorID = ee2232.VendorID;
        fTPROGRAMDATum.ProductID = ee2232.ProductID;
        fTPROGRAMDATum.MaxPower = ee2232.MaxPower;
        fTPROGRAMDATum.SelfPowered = Convert.ToUInt16(ee2232.SelfPowered);
        fTPROGRAMDATum.RemoteWakeup = Convert.ToUInt16(ee2232.RemoteWakeup);
        fTPROGRAMDATum.Rev5 = Convert.ToByte(true);
        fTPROGRAMDATum.PullDownEnable5 = Convert.ToByte(ee2232.PullDownEnable);
        fTPROGRAMDATum.SerNumEnable5 = Convert.ToByte(ee2232.SerNumEnable);
        fTPROGRAMDATum.USBVersionEnable5 = Convert.ToByte(ee2232.USBVersionEnable);
        fTPROGRAMDATum.USBVersion5 = ee2232.USBVersion;
        fTPROGRAMDATum.AIsHighCurrent = Convert.ToByte(ee2232.AIsHighCurrent);
        fTPROGRAMDATum.BIsHighCurrent = Convert.ToByte(ee2232.BIsHighCurrent);
        fTPROGRAMDATum.IFAIsFifo = Convert.ToByte(ee2232.IFAIsFifo);
        fTPROGRAMDATum.IFAIsFifoTar = Convert.ToByte(ee2232.IFAIsFifoTar);
        fTPROGRAMDATum.IFAIsFastSer = Convert.ToByte(ee2232.IFAIsFastSer);
        fTPROGRAMDATum.AIsVCP = Convert.ToByte(ee2232.AIsVCP);
        fTPROGRAMDATum.IFBIsFifo = Convert.ToByte(ee2232.IFBIsFifo);
        fTPROGRAMDATum.IFBIsFifoTar = Convert.ToByte(ee2232.IFBIsFifoTar);
        fTPROGRAMDATum.IFBIsFastSer = Convert.ToByte(ee2232.IFBIsFastSer);
        fTPROGRAMDATum.BIsVCP = Convert.ToByte(ee2232.BIsVCP);
        fTSTATU = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
      }
    }
    else if (_ftEeProgram == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Program.");
    }

    return fTSTATU;
  }

  public FT_STATUS WriteFT2232HEEPROM(FT2232H_EEPROM_STRUCTURE ee2232h)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftEeProgram != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Program) Marshal.GetDelegateForFunctionPointer(_ftEeProgram,
          typeof(FtNativeMethods.tFT_EE_Program));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_2232H)
        {
          ErrorHandler(fTSTATU, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        if (ee2232h.VendorID == 0 | ee2232h.ProductID == 0)
        {
          return FT_STATUS.FT_INVALID_PARAMETER;
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 3,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        if (ee2232h.Manufacturer.Length > 32)
        {
          ee2232h.Manufacturer = ee2232h.Manufacturer.Substring(0, 32);
        }

        if (ee2232h.ManufacturerID.Length > 16)
        {
          ee2232h.ManufacturerID = ee2232h.ManufacturerID.Substring(0, 16);
        }

        if (ee2232h.Description.Length > 64)
        {
          ee2232h.Description = ee2232h.Description.Substring(0, 64);
        }

        if (ee2232h.SerialNumber.Length > 16)
        {
          ee2232h.SerialNumber = ee2232h.SerialNumber.Substring(0, 16);
        }

        fTPROGRAMDATum.Manufacturer = Marshal.StringToHGlobalAnsi(ee2232h.Manufacturer);
        fTPROGRAMDATum.ManufacturerID = Marshal.StringToHGlobalAnsi(ee2232h.ManufacturerID);
        fTPROGRAMDATum.Description = Marshal.StringToHGlobalAnsi(ee2232h.Description);
        fTPROGRAMDATum.SerialNumber = Marshal.StringToHGlobalAnsi(ee2232h.SerialNumber);
        fTPROGRAMDATum.VendorID = ee2232h.VendorID;
        fTPROGRAMDATum.ProductID = ee2232h.ProductID;
        fTPROGRAMDATum.MaxPower = ee2232h.MaxPower;
        fTPROGRAMDATum.SelfPowered = Convert.ToUInt16(ee2232h.SelfPowered);
        fTPROGRAMDATum.RemoteWakeup = Convert.ToUInt16(ee2232h.RemoteWakeup);
        fTPROGRAMDATum.PullDownEnable7 = Convert.ToByte(ee2232h.PullDownEnable);
        fTPROGRAMDATum.SerNumEnable7 = Convert.ToByte(ee2232h.SerNumEnable);
        fTPROGRAMDATum.ALSlowSlew = Convert.ToByte(ee2232h.ALSlowSlew);
        fTPROGRAMDATum.ALSchmittInput = Convert.ToByte(ee2232h.ALSchmittInput);
        fTPROGRAMDATum.ALDriveCurrent = ee2232h.ALDriveCurrent;
        fTPROGRAMDATum.AHSlowSlew = Convert.ToByte(ee2232h.AHSlowSlew);
        fTPROGRAMDATum.AHSchmittInput = Convert.ToByte(ee2232h.AHSchmittInput);
        fTPROGRAMDATum.AHDriveCurrent = ee2232h.AHDriveCurrent;
        fTPROGRAMDATum.BLSlowSlew = Convert.ToByte(ee2232h.BLSlowSlew);
        fTPROGRAMDATum.BLSchmittInput = Convert.ToByte(ee2232h.BLSchmittInput);
        fTPROGRAMDATum.BLDriveCurrent = ee2232h.BLDriveCurrent;
        fTPROGRAMDATum.BHSlowSlew = Convert.ToByte(ee2232h.BHSlowSlew);
        fTPROGRAMDATum.BHSchmittInput = Convert.ToByte(ee2232h.BHSchmittInput);
        fTPROGRAMDATum.BHDriveCurrent = ee2232h.BHDriveCurrent;
        fTPROGRAMDATum.IFAIsFifo7 = Convert.ToByte(ee2232h.IFAIsFifo);
        fTPROGRAMDATum.IFAIsFifoTar7 = Convert.ToByte(ee2232h.IFAIsFifoTar);
        fTPROGRAMDATum.IFAIsFastSer7 = Convert.ToByte(ee2232h.IFAIsFastSer);
        fTPROGRAMDATum.AIsVCP7 = Convert.ToByte(ee2232h.AIsVCP);
        fTPROGRAMDATum.IFBIsFifo7 = Convert.ToByte(ee2232h.IFBIsFifo);
        fTPROGRAMDATum.IFBIsFifoTar7 = Convert.ToByte(ee2232h.IFBIsFifoTar);
        fTPROGRAMDATum.IFBIsFastSer7 = Convert.ToByte(ee2232h.IFBIsFastSer);
        fTPROGRAMDATum.BIsVCP7 = Convert.ToByte(ee2232h.BIsVCP);
        fTPROGRAMDATum.PowerSaveEnable = Convert.ToByte(ee2232h.PowerSaveEnable);
        fTSTATU = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
      }
    }
    else if (_ftEeProgram == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Program.");
    }

    return fTSTATU;
  }

  public FT_STATUS WriteFT232BEEPROM(FT232B_EEPROM_STRUCTURE ee232b)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftEeProgram != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Program) Marshal.GetDelegateForFunctionPointer(_ftEeProgram,
          typeof(FtNativeMethods.tFT_EE_Program));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_BM)
        {
          ErrorHandler(fTSTATU, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        if (ee232b.VendorID == 0 | ee232b.ProductID == 0)
        {
          return FT_STATUS.FT_INVALID_PARAMETER;
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 2,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        if (ee232b.Manufacturer.Length > 32)
        {
          ee232b.Manufacturer = ee232b.Manufacturer.Substring(0, 32);
        }

        if (ee232b.ManufacturerID.Length > 16)
        {
          ee232b.ManufacturerID = ee232b.ManufacturerID.Substring(0, 16);
        }

        if (ee232b.Description.Length > 64)
        {
          ee232b.Description = ee232b.Description.Substring(0, 64);
        }

        if (ee232b.SerialNumber.Length > 16)
        {
          ee232b.SerialNumber = ee232b.SerialNumber.Substring(0, 16);
        }

        fTPROGRAMDATum.Manufacturer = Marshal.StringToHGlobalAnsi(ee232b.Manufacturer);
        fTPROGRAMDATum.ManufacturerID = Marshal.StringToHGlobalAnsi(ee232b.ManufacturerID);
        fTPROGRAMDATum.Description = Marshal.StringToHGlobalAnsi(ee232b.Description);
        fTPROGRAMDATum.SerialNumber = Marshal.StringToHGlobalAnsi(ee232b.SerialNumber);
        fTPROGRAMDATum.VendorID = ee232b.VendorID;
        fTPROGRAMDATum.ProductID = ee232b.ProductID;
        fTPROGRAMDATum.MaxPower = ee232b.MaxPower;
        fTPROGRAMDATum.SelfPowered = Convert.ToUInt16(ee232b.SelfPowered);
        fTPROGRAMDATum.RemoteWakeup = Convert.ToUInt16(ee232b.RemoteWakeup);
        fTPROGRAMDATum.Rev4 = Convert.ToByte(true);
        fTPROGRAMDATum.PullDownEnable = Convert.ToByte(ee232b.PullDownEnable);
        fTPROGRAMDATum.SerNumEnable = Convert.ToByte(ee232b.SerNumEnable);
        fTPROGRAMDATum.USBVersionEnable = Convert.ToByte(ee232b.USBVersionEnable);
        fTPROGRAMDATum.USBVersion = ee232b.USBVersion;
        fTSTATU = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
      }
    }
    else if (_ftEeProgram == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Program.");
    }

    return fTSTATU;
  }

  public FT_STATUS WriteFT232HEEPROM(FT232H_EEPROM_STRUCTURE ee232h)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftEeProgram != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Program) Marshal.GetDelegateForFunctionPointer(_ftEeProgram,
          typeof(FtNativeMethods.tFT_EE_Program));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_232H)
        {
          ErrorHandler(fTSTATU, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        if (ee232h.VendorID == 0 | ee232h.ProductID == 0)
        {
          return FT_STATUS.FT_INVALID_PARAMETER;
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 5,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        if (ee232h.Manufacturer.Length > 32)
        {
          ee232h.Manufacturer = ee232h.Manufacturer.Substring(0, 32);
        }

        if (ee232h.ManufacturerID.Length > 16)
        {
          ee232h.ManufacturerID = ee232h.ManufacturerID.Substring(0, 16);
        }

        if (ee232h.Description.Length > 64)
        {
          ee232h.Description = ee232h.Description.Substring(0, 64);
        }

        if (ee232h.SerialNumber.Length > 16)
        {
          ee232h.SerialNumber = ee232h.SerialNumber.Substring(0, 16);
        }

        fTPROGRAMDATum.Manufacturer = Marshal.StringToHGlobalAnsi(ee232h.Manufacturer);
        fTPROGRAMDATum.ManufacturerID = Marshal.StringToHGlobalAnsi(ee232h.ManufacturerID);
        fTPROGRAMDATum.Description = Marshal.StringToHGlobalAnsi(ee232h.Description);
        fTPROGRAMDATum.SerialNumber = Marshal.StringToHGlobalAnsi(ee232h.SerialNumber);
        fTPROGRAMDATum.VendorID = ee232h.VendorID;
        fTPROGRAMDATum.ProductID = ee232h.ProductID;
        fTPROGRAMDATum.MaxPower = ee232h.MaxPower;
        fTPROGRAMDATum.SelfPowered = Convert.ToUInt16(ee232h.SelfPowered);
        fTPROGRAMDATum.RemoteWakeup = Convert.ToUInt16(ee232h.RemoteWakeup);
        fTPROGRAMDATum.PullDownEnableH = Convert.ToByte(ee232h.PullDownEnable);
        fTPROGRAMDATum.SerNumEnableH = Convert.ToByte(ee232h.SerNumEnable);
        fTPROGRAMDATum.ACSlowSlewH = Convert.ToByte(ee232h.ACSlowSlew);
        fTPROGRAMDATum.ACSchmittInputH = Convert.ToByte(ee232h.ACSchmittInput);
        fTPROGRAMDATum.ACDriveCurrentH = Convert.ToByte(ee232h.ACDriveCurrent);
        fTPROGRAMDATum.ADSlowSlewH = Convert.ToByte(ee232h.ADSlowSlew);
        fTPROGRAMDATum.ADSchmittInputH = Convert.ToByte(ee232h.ADSchmittInput);
        fTPROGRAMDATum.ADDriveCurrentH = Convert.ToByte(ee232h.ADDriveCurrent);
        fTPROGRAMDATum.Cbus0H = Convert.ToByte(ee232h.Cbus0);
        fTPROGRAMDATum.Cbus1H = Convert.ToByte(ee232h.Cbus1);
        fTPROGRAMDATum.Cbus2H = Convert.ToByte(ee232h.Cbus2);
        fTPROGRAMDATum.Cbus3H = Convert.ToByte(ee232h.Cbus3);
        fTPROGRAMDATum.Cbus4H = Convert.ToByte(ee232h.Cbus4);
        fTPROGRAMDATum.Cbus5H = Convert.ToByte(ee232h.Cbus5);
        fTPROGRAMDATum.Cbus6H = Convert.ToByte(ee232h.Cbus6);
        fTPROGRAMDATum.Cbus7H = Convert.ToByte(ee232h.Cbus7);
        fTPROGRAMDATum.Cbus8H = Convert.ToByte(ee232h.Cbus8);
        fTPROGRAMDATum.Cbus9H = Convert.ToByte(ee232h.Cbus9);
        fTPROGRAMDATum.IsFifoH = Convert.ToByte(ee232h.IsFifo);
        fTPROGRAMDATum.IsFifoTarH = Convert.ToByte(ee232h.IsFifoTar);
        fTPROGRAMDATum.IsFastSerH = Convert.ToByte(ee232h.IsFastSer);
        fTPROGRAMDATum.IsFT1248H = Convert.ToByte(ee232h.IsFT1248);
        fTPROGRAMDATum.FT1248CpolH = Convert.ToByte(ee232h.FT1248Cpol);
        fTPROGRAMDATum.FT1248LsbH = Convert.ToByte(ee232h.FT1248Lsb);
        fTPROGRAMDATum.FT1248FlowControlH = Convert.ToByte(ee232h.FT1248FlowControl);
        fTPROGRAMDATum.IsVCPH = Convert.ToByte(ee232h.IsVCP);
        fTPROGRAMDATum.PowerSaveEnableH = Convert.ToByte(ee232h.PowerSaveEnable);
        fTSTATU = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
      }
    }
    else if (_ftEeProgram == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Program.");
    }

    return fTSTATU;
  }

  public FT_STATUS WriteFT232REEPROM(FT232R_EEPROM_STRUCTURE ee232r)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftEeProgram != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Program) Marshal.GetDelegateForFunctionPointer(_ftEeProgram,
          typeof(FtNativeMethods.tFT_EE_Program));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_232R)
        {
          ErrorHandler(fTSTATU, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        if (ee232r.VendorID == 0 | ee232r.ProductID == 0)
        {
          return FT_STATUS.FT_INVALID_PARAMETER;
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 2,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        if (ee232r.Manufacturer.Length > 32)
        {
          ee232r.Manufacturer = ee232r.Manufacturer.Substring(0, 32);
        }

        if (ee232r.ManufacturerID.Length > 16)
        {
          ee232r.ManufacturerID = ee232r.ManufacturerID.Substring(0, 16);
        }

        if (ee232r.Description.Length > 64)
        {
          ee232r.Description = ee232r.Description.Substring(0, 64);
        }

        if (ee232r.SerialNumber.Length > 16)
        {
          ee232r.SerialNumber = ee232r.SerialNumber.Substring(0, 16);
        }

        fTPROGRAMDATum.Manufacturer = Marshal.StringToHGlobalAnsi(ee232r.Manufacturer);
        fTPROGRAMDATum.ManufacturerID = Marshal.StringToHGlobalAnsi(ee232r.ManufacturerID);
        fTPROGRAMDATum.Description = Marshal.StringToHGlobalAnsi(ee232r.Description);
        fTPROGRAMDATum.SerialNumber = Marshal.StringToHGlobalAnsi(ee232r.SerialNumber);
        fTPROGRAMDATum.VendorID = ee232r.VendorID;
        fTPROGRAMDATum.ProductID = ee232r.ProductID;
        fTPROGRAMDATum.MaxPower = ee232r.MaxPower;
        fTPROGRAMDATum.SelfPowered = Convert.ToUInt16(ee232r.SelfPowered);
        fTPROGRAMDATum.RemoteWakeup = Convert.ToUInt16(ee232r.RemoteWakeup);
        fTPROGRAMDATum.PullDownEnableR = Convert.ToByte(ee232r.PullDownEnable);
        fTPROGRAMDATum.SerNumEnableR = Convert.ToByte(ee232r.SerNumEnable);
        fTPROGRAMDATum.UseExtOsc = Convert.ToByte(ee232r.UseExtOsc);
        fTPROGRAMDATum.HighDriveIOs = Convert.ToByte(ee232r.HighDriveIOs);
        fTPROGRAMDATum.EndpointSize = 64;
        fTPROGRAMDATum.PullDownEnableR = Convert.ToByte(ee232r.PullDownEnable);
        fTPROGRAMDATum.SerNumEnableR = Convert.ToByte(ee232r.SerNumEnable);
        fTPROGRAMDATum.InvertTXD = Convert.ToByte(ee232r.InvertTXD);
        fTPROGRAMDATum.InvertRXD = Convert.ToByte(ee232r.InvertRXD);
        fTPROGRAMDATum.InvertRTS = Convert.ToByte(ee232r.InvertRTS);
        fTPROGRAMDATum.InvertCTS = Convert.ToByte(ee232r.InvertCTS);
        fTPROGRAMDATum.InvertDTR = Convert.ToByte(ee232r.InvertDTR);
        fTPROGRAMDATum.InvertDSR = Convert.ToByte(ee232r.InvertDSR);
        fTPROGRAMDATum.InvertDCD = Convert.ToByte(ee232r.InvertDCD);
        fTPROGRAMDATum.InvertRI = Convert.ToByte(ee232r.InvertRI);
        fTPROGRAMDATum.Cbus0 = ee232r.Cbus0;
        fTPROGRAMDATum.Cbus1 = ee232r.Cbus1;
        fTPROGRAMDATum.Cbus2 = ee232r.Cbus2;
        fTPROGRAMDATum.Cbus3 = ee232r.Cbus3;
        fTPROGRAMDATum.Cbus4 = ee232r.Cbus4;
        fTPROGRAMDATum.RIsD2XX = Convert.ToByte(ee232r.RIsD2XX);
        fTSTATU = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
      }
    }
    else if (_ftEeProgram == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Program.");
    }

    return fTSTATU;
  }

  public FT_STATUS WriteFT4232HEEPROM(FT4232H_EEPROM_STRUCTURE ee4232h)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftEeProgram != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EE_Program) Marshal.GetDelegateForFunctionPointer(_ftEeProgram,
          typeof(FtNativeMethods.tFT_EE_Program));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_4232H)
        {
          ErrorHandler(fTSTATU, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        if (ee4232h.VendorID == 0 | ee4232h.ProductID == 0)
        {
          return FT_STATUS.FT_INVALID_PARAMETER;
        }

        var fTPROGRAMDATum = new FT_PROGRAM_DATA()
        {
          Signature1 = 0,
          Signature2 = -1,
          Version = 4,
          Manufacturer = Marshal.AllocHGlobal(32),
          ManufacturerID = Marshal.AllocHGlobal(16),
          Description = Marshal.AllocHGlobal(64),
          SerialNumber = Marshal.AllocHGlobal(16)
        };
        if (ee4232h.Manufacturer.Length > 32)
        {
          ee4232h.Manufacturer = ee4232h.Manufacturer.Substring(0, 32);
        }

        if (ee4232h.ManufacturerID.Length > 16)
        {
          ee4232h.ManufacturerID = ee4232h.ManufacturerID.Substring(0, 16);
        }

        if (ee4232h.Description.Length > 64)
        {
          ee4232h.Description = ee4232h.Description.Substring(0, 64);
        }

        if (ee4232h.SerialNumber.Length > 16)
        {
          ee4232h.SerialNumber = ee4232h.SerialNumber.Substring(0, 16);
        }

        fTPROGRAMDATum.Manufacturer = Marshal.StringToHGlobalAnsi(ee4232h.Manufacturer);
        fTPROGRAMDATum.ManufacturerID = Marshal.StringToHGlobalAnsi(ee4232h.ManufacturerID);
        fTPROGRAMDATum.Description = Marshal.StringToHGlobalAnsi(ee4232h.Description);
        fTPROGRAMDATum.SerialNumber = Marshal.StringToHGlobalAnsi(ee4232h.SerialNumber);
        fTPROGRAMDATum.VendorID = ee4232h.VendorID;
        fTPROGRAMDATum.ProductID = ee4232h.ProductID;
        fTPROGRAMDATum.MaxPower = ee4232h.MaxPower;
        fTPROGRAMDATum.SelfPowered = Convert.ToUInt16(ee4232h.SelfPowered);
        fTPROGRAMDATum.RemoteWakeup = Convert.ToUInt16(ee4232h.RemoteWakeup);
        fTPROGRAMDATum.PullDownEnable8 = Convert.ToByte(ee4232h.PullDownEnable);
        fTPROGRAMDATum.SerNumEnable8 = Convert.ToByte(ee4232h.SerNumEnable);
        fTPROGRAMDATum.ASlowSlew = Convert.ToByte(ee4232h.ASlowSlew);
        fTPROGRAMDATum.ASchmittInput = Convert.ToByte(ee4232h.ASchmittInput);
        fTPROGRAMDATum.ADriveCurrent = ee4232h.ADriveCurrent;
        fTPROGRAMDATum.BSlowSlew = Convert.ToByte(ee4232h.BSlowSlew);
        fTPROGRAMDATum.BSchmittInput = Convert.ToByte(ee4232h.BSchmittInput);
        fTPROGRAMDATum.BDriveCurrent = ee4232h.BDriveCurrent;
        fTPROGRAMDATum.CSlowSlew = Convert.ToByte(ee4232h.CSlowSlew);
        fTPROGRAMDATum.CSchmittInput = Convert.ToByte(ee4232h.CSchmittInput);
        fTPROGRAMDATum.CDriveCurrent = ee4232h.CDriveCurrent;
        fTPROGRAMDATum.DSlowSlew = Convert.ToByte(ee4232h.DSlowSlew);
        fTPROGRAMDATum.DSchmittInput = Convert.ToByte(ee4232h.DSchmittInput);
        fTPROGRAMDATum.DDriveCurrent = ee4232h.DDriveCurrent;
        fTPROGRAMDATum.ARIIsTXDEN = Convert.ToByte(ee4232h.ARIIsTXDEN);
        fTPROGRAMDATum.BRIIsTXDEN = Convert.ToByte(ee4232h.BRIIsTXDEN);
        fTPROGRAMDATum.CRIIsTXDEN = Convert.ToByte(ee4232h.CRIIsTXDEN);
        fTPROGRAMDATum.DRIIsTXDEN = Convert.ToByte(ee4232h.DRIIsTXDEN);
        fTPROGRAMDATum.AIsVCP8 = Convert.ToByte(ee4232h.AIsVCP);
        fTPROGRAMDATum.BIsVCP8 = Convert.ToByte(ee4232h.BIsVCP);
        fTPROGRAMDATum.CIsVCP8 = Convert.ToByte(ee4232h.CIsVCP);
        fTPROGRAMDATum.DIsVCP8 = Convert.ToByte(ee4232h.DIsVCP);
        fTSTATU = delegateForFunctionPointer(_ftHandle, fTPROGRAMDATum);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Manufacturer);
        Marshal.FreeHGlobal(fTPROGRAMDATum.ManufacturerID);
        Marshal.FreeHGlobal(fTPROGRAMDATum.Description);
        Marshal.FreeHGlobal(fTPROGRAMDATum.SerialNumber);
      }
    }
    else if (_ftEeProgram == IntPtr.Zero)
    {
      Console.WriteLine("Failed to load function FT_EE_Program.");
    }

    return fTSTATU;
  }

  public FT_STATUS WriteXSeriesEEPROM(FT_XSERIES_EEPROM_STRUCTURE eeX)
  {
    var fTSTATU = FT_STATUS.FT_OTHER_ERROR;
    if (_ftd2Xxdll == IntPtr.Zero)
    {
      return fTSTATU;
    }

    if (_ftEepromProgram != IntPtr.Zero)
    {
      var delegateForFunctionPointer =
        (FtNativeMethods.tFT_EEPROM_Program) Marshal.GetDelegateForFunctionPointer(_ftEepromProgram,
          typeof(FtNativeMethods.tFT_EEPROM_Program));
      if (_ftHandle != IntPtr.Zero)
      {
        var fTDEVICE = FT_DEVICE.FT_DEVICE_UNKNOWN;
        GetDeviceType(ref fTDEVICE);
        if (fTDEVICE != FT_DEVICE.FT_DEVICE_X_SERIES)
        {
          ErrorHandler(fTSTATU, FT_ERROR.FT_INCORRECT_DEVICE);
        }

        if (eeX.VendorID == 0 | eeX.ProductID == 0)
        {
          return FT_STATUS.FT_INVALID_PARAMETER;
        }

        var vendorID = new FT_XSERIES_DATA();
        var bytes = new byte[32];
        var numArray = new byte[16];
        var bytes1 = new byte[64];
        var numArray1 = new byte[16];
        if (eeX.Manufacturer.Length > 32)
        {
          eeX.Manufacturer = eeX.Manufacturer.Substring(0, 32);
        }

        if (eeX.ManufacturerID.Length > 16)
        {
          eeX.ManufacturerID = eeX.ManufacturerID.Substring(0, 16);
        }

        if (eeX.Description.Length > 64)
        {
          eeX.Description = eeX.Description.Substring(0, 64);
        }

        if (eeX.SerialNumber.Length > 16)
        {
          eeX.SerialNumber = eeX.SerialNumber.Substring(0, 16);
        }

        var uTF8Encoding = new UTF8Encoding();
        bytes = uTF8Encoding.GetBytes(eeX.Manufacturer);
        numArray = uTF8Encoding.GetBytes(eeX.ManufacturerID);
        bytes1 = uTF8Encoding.GetBytes(eeX.Description);
        numArray1 = uTF8Encoding.GetBytes(eeX.SerialNumber);
        vendorID.common.deviceType = 9;
        vendorID.common.VendorId = eeX.VendorID;
        vendorID.common.ProductId = eeX.ProductID;
        vendorID.common.MaxPower = eeX.MaxPower;
        vendorID.common.SelfPowered = Convert.ToByte(eeX.SelfPowered);
        vendorID.common.RemoteWakeup = Convert.ToByte(eeX.RemoteWakeup);
        vendorID.common.SerNumEnable = Convert.ToByte(eeX.SerNumEnable);
        vendorID.common.PullDownEnable = Convert.ToByte(eeX.PullDownEnable);
        vendorID.Cbus0 = eeX.Cbus0;
        vendorID.Cbus1 = eeX.Cbus1;
        vendorID.Cbus2 = eeX.Cbus2;
        vendorID.Cbus3 = eeX.Cbus3;
        vendorID.Cbus4 = eeX.Cbus4;
        vendorID.Cbus5 = eeX.Cbus5;
        vendorID.Cbus6 = eeX.Cbus6;
        vendorID.ACDriveCurrent = eeX.ACDriveCurrent;
        vendorID.ACSchmittInput = eeX.ACSchmittInput;
        vendorID.ACSlowSlew = eeX.ACSlowSlew;
        vendorID.ADDriveCurrent = eeX.ADDriveCurrent;
        vendorID.ADSchmittInput = eeX.ADSchmittInput;
        vendorID.ADSlowSlew = eeX.ADSlowSlew;
        vendorID.BCDDisableSleep = eeX.BCDDisableSleep;
        vendorID.BCDEnable = eeX.BCDEnable;
        vendorID.BCDForceCbusPWREN = eeX.BCDForceCbusPWREN;
        vendorID.FT1248Cpol = eeX.FT1248Cpol;
        vendorID.FT1248FlowControl = eeX.FT1248FlowControl;
        vendorID.FT1248Lsb = eeX.FT1248Lsb;
        vendorID.I2CDeviceId = eeX.I2CDeviceId;
        vendorID.I2CDisableSchmitt = eeX.I2CDisableSchmitt;
        vendorID.I2CSlaveAddress = eeX.I2CSlaveAddress;
        vendorID.InvertCTS = eeX.InvertCTS;
        vendorID.InvertDCD = eeX.InvertDCD;
        vendorID.InvertDSR = eeX.InvertDSR;
        vendorID.InvertDTR = eeX.InvertDTR;
        vendorID.InvertRI = eeX.InvertRI;
        vendorID.InvertRTS = eeX.InvertRTS;
        vendorID.InvertRXD = eeX.InvertRXD;
        vendorID.InvertTXD = eeX.InvertTXD;
        vendorID.PowerSaveEnable = eeX.PowerSaveEnable;
        vendorID.RS485EchoSuppress = eeX.RS485EchoSuppress;
        vendorID.DriverType = eeX.IsVCP;
        var num = Marshal.SizeOf(vendorID);
        var intPtr = Marshal.AllocHGlobal(num);
        Marshal.StructureToPtr(vendorID, intPtr, false);
        fTSTATU = delegateForFunctionPointer(_ftHandle, intPtr, num, bytes, numArray, bytes1, numArray1);
      }
    }

    return FT_STATUS.FT_DEVICE_NOT_FOUND;
  }

  private void ErrorHandler(FT_STATUS ftStatus, FT_ERROR ftErrorCondition)
  {
    if (ftStatus != FT_STATUS.FT_OK)
    {
      switch (ftStatus)
      {
        case FT_STATUS.FT_INVALID_HANDLE:
        {
          throw new FT_EXCEPTION("Invalid handle for FTDI device.");
        }
        case FT_STATUS.FT_DEVICE_NOT_FOUND:
        {
          throw new FT_EXCEPTION("FTDI device not found.");
        }
        case FT_STATUS.FT_DEVICE_NOT_OPENED:
        {
          throw new FT_EXCEPTION("FTDI device not opened.");
        }
        case FT_STATUS.FT_IO_ERROR:
        {
          throw new FT_EXCEPTION("FTDI device IO error.");
        }
        case FT_STATUS.FT_INSUFFICIENT_RESOURCES:
        {
          throw new FT_EXCEPTION("Insufficient resources.");
        }
        case FT_STATUS.FT_INVALID_PARAMETER:
        {
          throw new FT_EXCEPTION("Invalid parameter for FTD2XX function call.");
        }
        case FT_STATUS.FT_INVALID_BAUD_RATE:
        {
          throw new FT_EXCEPTION("Invalid Baud rate for FTDI device.");
        }
        case FT_STATUS.FT_DEVICE_NOT_OPENED_FOR_ERASE:
        {
          throw new FT_EXCEPTION("FTDI device not opened for erase.");
        }
        case FT_STATUS.FT_DEVICE_NOT_OPENED_FOR_WRITE:
        {
          throw new FT_EXCEPTION("FTDI device not opened for write.");
        }
        case FT_STATUS.FT_FAILED_TO_WRITE_DEVICE:
        {
          throw new FT_EXCEPTION("Failed to write to FTDI device.");
        }
        case FT_STATUS.FT_EEPROM_READ_FAILED:
        {
          throw new FT_EXCEPTION("Failed to read FTDI device EEPROM.");
        }
        case FT_STATUS.FT_EEPROM_WRITE_FAILED:
        {
          throw new FT_EXCEPTION("Failed to write FTDI device EEPROM.");
        }
        case FT_STATUS.FT_EEPROM_ERASE_FAILED:
        {
          throw new FT_EXCEPTION("Failed to erase FTDI device EEPROM.");
        }
        case FT_STATUS.FT_EEPROM_NOT_PRESENT:
        {
          throw new FT_EXCEPTION("No EEPROM fitted to FTDI device.");
        }
        case FT_STATUS.FT_EEPROM_NOT_PROGRAMMED:
        {
          throw new FT_EXCEPTION("FTDI device EEPROM not programmed.");
        }
        case FT_STATUS.FT_INVALID_ARGS:
        {
          throw new FT_EXCEPTION("Invalid arguments for FTD2XX function call.");
        }
        case FT_STATUS.FT_OTHER_ERROR:
        {
          throw new FT_EXCEPTION("An unexpected error has occurred when trying to communicate with the FTDI device.");
        }
      }
    }

    if (ftErrorCondition == FT_ERROR.FT_NO_ERROR)
    {
      return;
    }

    switch (ftErrorCondition)
    {
      case FT_ERROR.FT_INCORRECT_DEVICE:
      {
        throw new FT_EXCEPTION("The current device type does not match the EEPROM structure.");
      }
      case FT_ERROR.FT_INVALID_BITMODE:
      {
        throw new FT_EXCEPTION("The requested bit mode is not valid for the current device.");
      }
      case FT_ERROR.FT_BUFFER_SIZE:
      {
        throw new FT_EXCEPTION("The supplied buffer is not big enough.");
      }
      default:
      {
        return;
      }
    }
  }
}