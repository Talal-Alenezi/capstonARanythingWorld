using System;

[Serializable]
public class AWHopperAbout : AWThingAbout
{
    public new AWHopperParts parts;
}

[Serializable]
public class AWHopperParts : AWParts
{
    public new string body;
    public new string head;
    public new string chest;
    public new string leg_left_top;
    public new string leg_left_bot;
    public new string leg_right_top;
    public new string leg_right_bot;
    public new string arm_right;
    public new string arm_left;
    public new string tail;
    public new string ear_right;
    public new string ear_left;
}
