using UnityEngine;

public class ExampleUIHandler : MonoBehaviour
{
    #region Fields
    public GameObject helperWindow = null;
    #endregion
    void Start()
    {
        CloseHelperWindow();
    }
    /// <summary>
    /// Display the helper panel defined in variables.
    /// </summary>
    public void DisplayHelperWindow()
    {
        if (helperWindow != null)
        {
            helperWindow.SetActive(true);
        }
    }
    public void CloseHelperWindow()
    {
        if (helperWindow != null)
        {
            helperWindow.SetActive(false);
        }
    }
}
