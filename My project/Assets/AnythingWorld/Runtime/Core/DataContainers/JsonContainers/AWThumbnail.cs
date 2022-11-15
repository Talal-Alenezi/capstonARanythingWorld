using System;

[Serializable]
public class AWThumbnail
{
    public string original_reference;
    public string aw_reference;
    public string aw_reference_transparent;
    /// <summary>
    /// 225 square px thumbnail.
    /// </summary>
    public string aw_thumbnail;
    /// <summary>
    /// 225p2 thumbnail (transparent background)
    /// </summary>
    public string aw_thumbnail_transparent;
}
