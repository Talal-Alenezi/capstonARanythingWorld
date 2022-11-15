using System;

[Serializable]
public class AWVehicleOneWheelAbout : AWThingAbout
{
    public new AWVehicleOneWheelParts parts;
}

[Serializable]
public class AWVehicleOneWheelParts : AWParts
{
    public new string body;
    public new string head;
    public new string wheel;
    public new string pedal_right;
    public new string pedal_left;
}