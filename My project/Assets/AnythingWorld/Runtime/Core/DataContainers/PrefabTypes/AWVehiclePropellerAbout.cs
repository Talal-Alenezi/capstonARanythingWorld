using System;

[Serializable]
public class AWVehiclePropellerAbout : AWThingAbout
{
    public new AWVehiclePropellerParts parts;
}

[Serializable]
public class AWVehiclePropellerParts : AWParts
{
    public new string body;
    public new string wing1;
    public new string wing2;
    public new string wing3;
    public new string landing_right;
    public new string landing_left;
    public new string leg_left;
    public new string leg_right;
    public new string tail;
    public new string tail_bot;
    public new string tail_top;
    public new string tailfin;
}