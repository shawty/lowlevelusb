using System.Runtime.Serialization;

namespace DotNetNottsIot2022.ftdiDotnet;

[Serializable]
public class FT_EXCEPTION : Exception
{
  public FT_EXCEPTION()
  {
  }

  public FT_EXCEPTION(string message) : base(message)
  {
  }

  public FT_EXCEPTION(string message, Exception inner) : base(message, inner)
  {
  }

  protected FT_EXCEPTION(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }
}
