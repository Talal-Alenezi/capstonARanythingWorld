using System;

[Serializable]
public class AWFloaterAbout : AWThingAbout
{
    public new AWFloaterParts parts;
}

[Serializable]
public class AWFloaterParts : AWParts
{
    public new string body;
}
