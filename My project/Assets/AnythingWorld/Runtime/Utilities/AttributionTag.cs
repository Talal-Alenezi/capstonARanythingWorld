using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Add this tag to any game object with a "Text" UI component for it to be updated with the attribution data.
/// </summary>
[ExecuteInEditMode]
[Serializable]
public class AttributionTag : MonoBehaviour
{
    [SerializeField]
    private static List<AttributionTag> attributionTags;
    public static List<AttributionTag> AttributionTags
    {
        get
        {
            if (attributionTags == null)
            {
                attributionTags = new List<AttributionTag>();
            }
            return attributionTags;
        }
        set
        {
            attributionTags = value;
        }
    }

    [SerializeField]
    private Text textElement;

    private static void AddAttributionTag(AttributionTag instance)
    {
        if (AttributionTags == null)
        {
            AttributionTags = new List<AttributionTag>();
        }

        if (!AttributionTags.Contains(instance))
        {
            AttributionTags.Add(instance);
        }

    }

    public void OnEnable()
    {
        Initialize();
    }
    private void Initialize()
    {
        AddThisAttributionTag();
        FindTextComponent();
    }
    private bool FindTextComponent()
    {
        if (TryGetComponent(out Text textComponent))
        {
            textElement = textComponent;
            return true;
        }
        else
        {
            Debug.Log("No text element found on game object, removing AttributeTag");
            return false;
        }
    }
    private void AddThisAttributionTag()
    {
        AddAttributionTag(this);
    }

    public void ClearTextTag()
    {
        var attributionTextBlock = "";
        //Debug.Log("UpdateTextTag");
        if (textElement == null)
        {
            if (FindTextComponent())
            {
                Debug.Log("updating text tags");
                textElement.text = attributionTextBlock;
#if UNITY_EDITOR
                EditorUtility.SetDirty(textElement);
#endif
            }
            else
            {
                Debug.LogWarning("Could not update attribution on tag, no text component");
                return;
            }
        }
        else
        {
            textElement.text = attributionTextBlock;
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(textElement);
#endif
    }

    public void UpdateTextTag(string attributionTextBlock)
    {
        if (textElement == null)
        {
            if (FindTextComponent())
            {
                Debug.Log("updating text tags");
                textElement.text = attributionTextBlock;
#if UNITY_EDITOR
                EditorUtility.SetDirty(textElement);
#endif
            }
            else
            {
                Debug.LogWarning("Could not update attribution on tag, no text component");
                return;
            }
        }
        else
        {
            textElement.text = attributionTextBlock;
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(textElement);
#endif
    }



}
