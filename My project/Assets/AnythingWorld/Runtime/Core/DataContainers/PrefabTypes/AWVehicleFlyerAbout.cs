using System;

[Serializable]
public class AWVehicleFlyerAbout : AWThingAbout
{
    public new AWVehicleFlyerParts parts;
}

[Serializable]
public class AWVehicleFlyerParts : AWParts
{
    public new string body;
    public new string wing_left;
    public new string wing_right;
    public new string wheel_front;
    public new string wheel_right;
    public new string wheel_left;
    public new string turbine_left;
    public new string turbine_right;
    public new string tail_right;
    public new string tail_left;
    public new string tail_top;
    public new string engine_left_piece;
    public new string engine_left;
    public new string engine_right;
    public new string tailfin_right;
    public new string tailfin_left;
    public new string tailfin_middle;
}