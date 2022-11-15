using System;

[Serializable]
public class AWVehicleFourWheelAbout : AWThingAbout
{
    public new AWVehicleFourWheelParts parts;
}

[Serializable]
public class AWVehicleFourWheelParts : AWParts
{
    public new string wheel_front_left;
    public new string wheel_front_right;
    public new string wheel_hind_left;
    public new string wheel_hind_right;
    public new string body;
}