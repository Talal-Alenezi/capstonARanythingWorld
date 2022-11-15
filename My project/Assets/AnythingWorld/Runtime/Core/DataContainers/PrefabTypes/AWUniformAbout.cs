using System;

[Serializable]
public class AWUniformAbout : AWThingAbout
{
    public new AWUniformParts parts;
}

[Serializable]
public class AWUniformParts : AWParts
{
    public new string body;
}
