namespace DotNetNottsIot2022.ftdiDotnet;

public class FT4232H_EEPROM_STRUCTURE : FT_EEPROM_DATA
{
  public bool PullDownEnable;
  public bool SerNumEnable = true;
  public bool ASlowSlew;
  public bool ASchmittInput;
  public byte ADriveCurrent = 4;
  public bool BSlowSlew;
  public bool BSchmittInput;
  public byte BDriveCurrent = 4;
  public bool CSlowSlew;
  public bool CSchmittInput;
  public byte CDriveCurrent = 4;
  public bool DSlowSlew;
  public bool DSchmittInput;
  public byte DDriveCurrent = 4;
  public bool ARIIsTXDEN;
  public bool BRIIsTXDEN;
  public bool CRIIsTXDEN;
  public bool DRIIsTXDEN;
  public bool AIsVCP = true;
  public bool BIsVCP = true;
  public bool CIsVCP = true;
  public bool DIsVCP = true;
}
