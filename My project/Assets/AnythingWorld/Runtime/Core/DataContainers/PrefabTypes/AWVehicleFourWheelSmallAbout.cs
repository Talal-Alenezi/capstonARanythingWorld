using System;

[Serializable]
public class AWVehicleFourWheelSmallAbout : AWThingAbout
{
    public new AWVehicleFourWheelSmallParts parts;
}

[Serializable]
public class AWVehicleFourWheelSmallParts : AWParts
{
    public new string wheel_front_left;
    public new string wheel_front_right;
    public new string wheel_hind_left;
    public new string wheel_hind_right;
    public new string body;
}