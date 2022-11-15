using System;

[Serializable]
public class AWVehicleThreeWheelAbout : AWThingAbout
{
    public new AWVehicleThreeWheelParts parts;
}

[Serializable]
public class AWVehicleThreeWheelParts : AWParts
{
    public new string body;
    public new string head;
    public new string wheel_hind;
    public new string wheel_hind_right;
    public new string wheel_hind_left;
    public new string wheel_front;
    public new string pedal_right;
    public new string pedal_left;
}