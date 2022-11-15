using System;

[Serializable]
public class AWWingedStandingAbout : AWThingAbout
{
    public new AWWingedStandingParts parts;
}

[Serializable]
public class AWWingedStandingParts : AWParts
{
    public new string body;
    public new string leg_left;
    public new string leg_right;
    public new string tail;
    public new string head;
    public new string leg_left_bot;
    public new string leg_left_top;
    public new string leg_right_bot;
    public new string leg_right_top;
    public new string neck;
}