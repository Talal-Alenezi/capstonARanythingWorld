using System;

[Serializable]
public class AWQuadAbout : AWThingAbout
{
    public new AWQuadParts parts;
}

[Serializable]
public class AWQuadParts : AWParts
{
    public new string leg_front_right_top;
    public new string leg_front_right_bot;
    public new string leg_front_right;
    public new string leg_front_left;
    public new string leg_front_left_top;
    public new string leg_front_left_bot;
    public new string leg_hind_right_bot;
    public new string leg_hind_right_top;
    public new string leg_hind_left_bot;
    public new string leg_hind_left_top;
    public new string foot_front_right;
    public new string foot_front_left;
    public new string foot_hind_right;
    public new string foot_hind_left;

    public new string head;
    public new string tail;
    public new string body;
}








