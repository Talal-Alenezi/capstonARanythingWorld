using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace AnythingWorld.DataContainers
{
    /// <summary>
    /// Class holding information about search results for the Object Creator panel.
    /// Holds object names and thumbnails, and handles serialization of of thumbnail images across runtime/editor.
    /// </summary>
    [Serializable]
    public class SearchResult
    {

        public SearchResult(AWThing jsonData)
        {
            //Set AWThing json data
            data = jsonData;
            //Set name of object
            name = data.name;

            //Set source of object
            if (data.original_source == "POLY API") isPolyResult = true;
            else isPolyResult = false;

            //Set if animated or static
            if (data.behaviour == "static") isAnimated = false;
            else isAnimated = true;
        }
        public string DisplayName
        {
            get
            {
                return GetDisplayName();
            }
        }


        [SerializeField]
        public bool ResultHasThumbnail = true;
        [SerializeField]
        public string name;
        [SerializeField]
        public Texture2D thumbnail;
        [SerializeField]
        public bool isPolyResult;
        [SerializeField]
        public bool isAnimated;
        [SerializeField]
        public AWThing data;

        public Texture2D Thumbnail
        {
            get
            {
                if (ResultHasThumbnail)
                {
                    if (thumbnail != null)
                    {
                        return thumbnail;
                    }
                    else
                    {

                        if (texStream != null)
                        {
                            var chain = false;
                            if (mipMapCount > 1) chain = true;
                            try
                            {
                                var texCopy = new Texture2D(texWidth, texHeight, format, chain);
                                texCopy.LoadRawTextureData(texStream);
                                texCopy.Apply();

                                if (texCopy != null)
                                {
                                    thumbnail = texCopy;
                                    return thumbnail;
                                }
                                else
                                {
                                    ResultHasThumbnail = false;
                                    return null;
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogError($"Error serializing thumbnail for {this.name}: {e}");
                                texStream = null;
                                ResultHasThumbnail = false;
                                return null;
                            }
                        }
                        else
                        {
                            ResultHasThumbnail = false;
                            return null;
                        }
                    }
                }
                else
                {
                    return null;
                }



            }

            set
            {
                try
                {
                    if (value == null)
                    {
                        ResultHasThumbnail = false;
                        return;
                    }
                    else
                    {
                        texStream = value.GetRawTextureData();
                        texWidth = value.width;
                        texHeight = value.height;
                        format = value.format;
                        mipMapCount = value.mipmapCount;
                        thumbnail = value;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error setting thumbnail for {this.name}: {e}");
                    ResultHasThumbnail = false;
                    return;
                }

            }
        }
        [SerializeField]
        public byte[] texStream;
        [SerializeField]
        int texHeight;
        [SerializeField]
        int texWidth;
        [SerializeField]
        TextureFormat format;
        [SerializeField]
        int mipMapCount;
        private string GetDisplayName()
        {
            if (name != null)
            {
                var disp = Regex.Replace(name, @"\d", "");
                disp = disp.Replace("_", " ");
                disp = Regex.Replace(disp, @"\#.*", "");

                var dispArr = disp.ToCharArray();
                var displayName = new string(dispArr);

                var textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;

                return textInfo.ToTitleCase(displayName);
            }
            else
            {
                return null;
            }
        }


        public void CreateResultFromAWThing(AWThing awThing)
        {
            //Set AWThing json data
            data = awThing;
            //Set name of object
            name = awThing.name;

            //Set source of object
            if (data.original_source == "POLY API") isPolyResult = true;
            else isPolyResult = false;

            //Set if animated or static
            if (data.behaviour == "static") isAnimated = false;
            else isAnimated = true;

        }

    }
}
