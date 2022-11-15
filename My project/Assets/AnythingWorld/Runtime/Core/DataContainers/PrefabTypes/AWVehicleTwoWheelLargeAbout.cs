using System;

[Serializable]
public class AWVehicleTwoWheelLargeAbout : AWThingAbout
{
    public new AWVehicleTwoWheelLargeParts parts;
}

[Serializable]
public class AWVehicleTwoWheelLargeParts : AWParts
{
    public new string body;
    public new string head;
    public new string wheel_hind;
    public new string wheel_front;
    public new string wheel_right;
    public new string wheel_left;
}