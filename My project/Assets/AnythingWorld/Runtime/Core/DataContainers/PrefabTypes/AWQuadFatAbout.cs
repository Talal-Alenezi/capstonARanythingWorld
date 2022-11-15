using System;

[Serializable]
public class AWQuadFatAbout : AWThingAbout
{
    public new AWQuadFatParts parts;
}

[Serializable]
public class AWQuadFatParts : AWParts
{
    public new string leg_front_right;
    public new string leg_front_left;
    public new string leg_hind_right;
    public new string leg_hind_left;
    public new string ear_right;
    public new string ear_left;
    public new string jaw_bot;
    public new string head;
    public new string tail;
    public new string body;
}








