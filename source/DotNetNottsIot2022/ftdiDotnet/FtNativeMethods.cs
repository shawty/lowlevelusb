namespace DotNetNottsIot2022.ftdiDotnet;

using System.Runtime.InteropServices;

public class FtNativeMethods
{
  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_Close(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_ClrDtr(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_ClrRts(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_CreateDeviceInfoList(ref uint numdevs);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_CyclePort(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_EE_Program(IntPtr ftHandle, FT_PROGRAM_DATA pData);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_EE_Read(IntPtr ftHandle, FT_PROGRAM_DATA pData);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_EE_UARead(IntPtr ftHandle, byte[] pucData, int dwDataLen, ref uint lpdwDataRead);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_EE_UASize(IntPtr ftHandle, ref uint dwSize);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_EE_UAWrite(IntPtr ftHandle, byte[] pucData, int dwDataLen);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_EEPROM_Program(IntPtr ftHandle, IntPtr eepromData, int eepromDataSize, byte[] manufacturer, byte[] manufacturerID, byte[] description, byte[] serialnumber);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_EEPROM_Read(IntPtr ftHandle, IntPtr eepromData, int eepromDataSize, byte[] manufacturer, byte[] manufacturerID, byte[] description, byte[] serialnumber);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_EraseEE(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_GetBitMode(IntPtr ftHandle, ref byte ucMode);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_GetComPortNumber(IntPtr ftHandle, ref int dwComPortNumber);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_GetDeviceInfo(IntPtr ftHandle, ref FT_DEVICE pftType, ref uint lpdwID, byte[] pcSerialNumber, byte[] pcDescription, IntPtr pvDummy);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_GetDeviceInfoDetail(uint index, ref uint flags, ref FT_DEVICE chiptype, ref uint id, ref uint locid, byte[] serialnumber, byte[] description, ref IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_GetDriverVersion(IntPtr ftHandle, ref uint lpdwDriverVersion);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_GetLatencyTimer(IntPtr ftHandle, ref byte ucLatency);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_GetLibraryVersion(ref uint lpdwLibraryVersion);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_GetModemStatus(IntPtr ftHandle, ref uint lpdwModemStatus);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_GetQueueStatus(IntPtr ftHandle, ref uint lpdwAmountInRxQueue);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_GetStatus(IntPtr ftHandle, ref uint lpdwAmountInRxQueue, ref uint lpdwAmountInTxQueue, ref uint lpdwEventStatus);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_Open(uint index, ref IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_OpenEx(string devstring, uint dwFlags, ref IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_OpenExLoc(uint devloc, uint dwFlags, ref IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_Purge(IntPtr ftHandle, uint dwMask);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_Read(IntPtr ftHandle, byte[] lpBuffer, uint dwBytesToRead, ref uint lpdwBytesReturned);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_ReadEE(IntPtr ftHandle, uint dwWordOffset, ref ushort lpwValue);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_Reload(ushort wVID, ushort wPID);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_Rescan();

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_ResetDevice(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_ResetPort(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_RestartInTask(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetBaudRate(IntPtr ftHandle, uint dwBaudRate);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetBitMode(IntPtr ftHandle, byte ucMask, byte ucMode);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetBreakOff(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetBreakOn(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetChars(IntPtr ftHandle, byte uEventCh, byte uEventChEn, byte uErrorCh, byte uErrorChEn);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetDataCharacteristics(IntPtr ftHandle, byte uWordLength, byte uStopBits, byte uParity);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetDeadmanTimeout(IntPtr ftHandle, uint dwDeadmanTimeout);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetDtr(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetEventNotification(IntPtr ftHandle, uint dwEventMask, SafeHandle hEvent);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetFlowControl(IntPtr ftHandle, ushort usFlowControl, byte uXon, byte uXoff);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetLatencyTimer(IntPtr ftHandle, byte ucLatency);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetResetPipeRetryCount(IntPtr ftHandle, uint dwCount);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetRts(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetTimeouts(IntPtr ftHandle, uint dwReadTimeout, uint dwWriteTimeout);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_SetUSBParameters(IntPtr ftHandle, uint dwInTransferSize, uint dwOutTransferSize);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_StopInTask(IntPtr ftHandle);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_Write(IntPtr ftHandle, byte[] lpBuffer, int dwBytesToWrite, ref int lpdwBytesWritten);

  [UnmanagedFunctionPointer(CallingConvention.StdCall)]
  public delegate FT_STATUS tFT_WriteEE(IntPtr ftHandle, uint dwWordOffset, ushort wValue);

  [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false)]
  public static extern IntPtr LoadLibrary(string dllToLoad);

  [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false)]
  public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
  
  [DllImport("kernel32.dll", CharSet = CharSet.None, ExactSpelling = false)]
  public static extern bool FreeLibrary(IntPtr hModule);

}