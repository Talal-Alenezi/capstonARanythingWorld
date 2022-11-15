using System;

[Serializable]
public class AWVehicleTwoWheelAbout : AWThingAbout
{
    public new AWVehicleTwoWheelParts parts;
}

[Serializable]
public class AWVehicleTwoWheelParts : AWParts
{
    public new string body;
    public new string head;
    public new string wheel_hind;
    public new string wheel_front;
    public new string wheel_right;
    public new string wheel_left;
}