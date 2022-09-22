using DotNetNottsIot2022.ftdiDotnet;

namespace DotNetNottsIot2022
{
  class Program
  {
    private static FTDI myFtdiDevice = new FTDI();
    
    static void ShowFtdiDevicesList()
    {
      uint devCount = 0;

      var mainStatus = FT_STATUS.FT_OK;

      mainStatus = myFtdiDevice.GetNumberOfDevices(ref devCount);
      if (mainStatus != FT_STATUS.FT_OK)
      {
        Console.WriteLine($"ERROR! : Failed to call 'GetNumberOfDevices'");
        return;
      }
      
      Console.WriteLine($"{devCount} FT devices are available");

      Console.WriteLine("\nAvailable Devices:");
      var devices = new FT_DEVICE_INFO_NODE[devCount];

      var count = 0;
      mainStatus = myFtdiDevice.GetDeviceList(devices);
      if (mainStatus != FT_STATUS.FT_OK)
      {
        Console.WriteLine($"ERROR! : Failed to call 'GetDeviceList'");
        return;
      }

      foreach (var device in devices)
      {
        Console.WriteLine($"\tCount: {count++}, ID: {device.ID}, Type: {device.Type}, Description: {device.Description}");
      }
      
    }
    
    static void DisplayI2CBus(FtdiI2C i2C)
    {
      byte currentAddress = 0x03;

      Console.ForegroundColor = ConsoleColor.White;

      // Chart header
      Console.Write("    ");
      for (var xval = 0; xval < 16; xval++)
      {
        Console.Write($"{xval:X2} ");
      }
      Console.WriteLine();

      // First row (Starts at 3)
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write("00:          ");
      for (var xval = 3; xval < 16; xval++)
      {
        if(i2C.WasDeviceAddressDetected(currentAddress))
        {
          Console.ForegroundColor = ConsoleColor.Green;
          Console.Write($"{currentAddress:X2} ");
        }
        else
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.Write("-- ");
        }
        currentAddress++;
      }
      Console.WriteLine();

      // Rows 1 to 6
      for (var yval = 1; yval < 7; yval++)
      {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{yval:X1}0: ");
        for (var xval = 0; xval < 16; xval++)
        {
          if (i2C.WasDeviceAddressDetected(currentAddress))
          {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{currentAddress:X2} ");
          }
          else
          {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("-- ");
          }
          currentAddress++;
        }
        Console.WriteLine();
      }

      // Last row (Finishes at 7)
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write("70: ");
      for (var xval = 0; xval < 8; xval++)
      {
        if (i2C.WasDeviceAddressDetected(currentAddress))
        {
          Console.ForegroundColor = ConsoleColor.Green;
          Console.Write($"{currentAddress:X2} ");
        }
        else
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.Write("-- ");
        }
        currentAddress++;
      }
      Console.WriteLine();

      Console.ForegroundColor = ConsoleColor.White;
    }
    
    static void Main()
    {
      Console.WriteLine("FTDI Test 232H test 1");

      ShowFtdiDevicesList();
      
      Console.WriteLine("");
      Console.WriteLine("=======================================================");
      Console.WriteLine("I2C Operations");
      Console.WriteLine("=======================================================");

      var programStatus = myFtdiDevice.OpenByIndex(0); // We should have at least one device, and 0 will be the first
      if (programStatus != FT_STATUS.FT_OK)
      {
        throw new Exception("Could NOT open FTDI device at index 0");
      }

      FtdiI2C i2cBus = new FtdiI2C(myFtdiDevice);
      i2cBus.ScanBus();
      DisplayI2CBus(i2cBus);

      // Debugging... bloody I2C and it's wierd behaviour
      //i2cBus.SetStart();
      //Thread.Sleep(100);
      //i2cBus.SetStart();
      //Thread.Sleep(100);
      //i2cBus.SendDeviceAddrAndCheckAck(0x27);
      //Thread.Sleep(100);
      //i2cBus.SetStop();
      
      Console.WriteLine("");

      // Check I2C for an OLED display and do something with it
      if (i2cBus.WasDeviceAddressDetected(HD44780Oled.DeviceAddress))
      {
        Thread.Sleep(500); // Small delay just to sep I2C trace
        
        Console.WriteLine("Found Hitachi Compatible OLED Display (HD44780)connected to bus");
      
        var myDisplay = new HD44780Oled(i2cBus);
      
        myDisplay.Init();
        myDisplay.BacklightOn();
        
        myDisplay.PrintString("Hello there");
        myDisplay.GotoSecondLine();
        myDisplay.PrintString(".NET Notts IoT");
      
        Console.WriteLine("OLED Should be showing some text, Press return to continue.");
        var key = Console.ReadKey();
        
        myDisplay.Clear();
        myDisplay.BacklightOff();
        
      }
      
      // // Check I2C for a temperature sensor and do something with it
      if (i2cBus.WasDeviceAddressDetected(Tc74ThermalSensor.DeviceAddress))
      {
        Thread.Sleep(500); // Small delay just to sep I2C trace
        
        Console.WriteLine("Found Microchip Temperature Sensor (TC74) connected to bus");
        
        var tempSensor = new Tc74ThermalSensor(i2cBus);
        var config = tempSensor.ReadConfigRegister();
        var temp = tempSensor.ReadTemperatureRegister();
        
        Console.WriteLine($"Config register reads : {config}");
        Console.WriteLine($"Temperature register reads : {temp}");
      }
      
      
      programStatus = myFtdiDevice.Close();
      if (programStatus != FT_STATUS.FT_OK)
      {
        throw new Exception("Could NOT close FTDI device at index 0!");
      }

      //Console.WriteLine("Press enter to exit...");
      //Console.ReadKey();

    }
    
  }
}
