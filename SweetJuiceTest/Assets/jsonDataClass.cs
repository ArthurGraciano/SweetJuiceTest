using System;
using System.Collections.Generic;

[Serializable]
public class jsonDataClass
{ 
  public bool success;
  public string status;
  public DataClass data;

}


[Serializable]
public class DataClass
{
    public string message;
}

