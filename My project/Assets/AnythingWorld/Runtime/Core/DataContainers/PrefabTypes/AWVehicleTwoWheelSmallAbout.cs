using System;

[Serializable]
public class AWVehicleTwoWheelSmallAbout : AWThingAbout
{
    public new AWVehicleTwoWheelSmallParts parts;
}

[Serializable]
public class AWVehicleTwoWheelSmallParts : AWParts
{
    public new string body;
    public new string head;
    public new string wheel_hind;
    public new string wheel_front;
    public new string wheel_right;
    public new string wheel_left;
}