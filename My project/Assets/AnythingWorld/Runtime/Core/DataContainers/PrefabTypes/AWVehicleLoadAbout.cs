using System;

[Serializable]
public class AWVehicleLoadAbout : AWThingAbout
{
    public new AWVehicleLoadParts parts;
}

[Serializable]
public class AWVehicleLoadParts : AWParts
{
    public new string body;
    public new string wheel;
}