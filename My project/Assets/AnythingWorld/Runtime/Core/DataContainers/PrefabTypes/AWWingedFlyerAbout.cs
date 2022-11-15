using System;

[Serializable]
public class AWWingedFlyerAbout : AWThingAbout
{
    public new AWWingedFlyerParts parts;
}

[Serializable]
public class AWWingedFlyerParts : AWParts
{
    public new string body;
    public new string head;
    public new string tail;
    public new string wing_left;
    public new string wing_right;
    public new string leg_left;
    public new string leg_right;
}
