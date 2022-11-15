#if UNITY_EDITOR
using UnityEditor;
#endif
using AnythingWorld.Animation;
using AnythingWorld.DataContainers;
using AnythingWorld.Habitat;
using AnythingWorld.Utilities;
using System;
using UnityEngine;
namespace AnythingWorld.Editors
{
    /// <summary>
    /// Custom Editor window for the anything editor.
    /// </summary>
    [Serializable]
    public class AnythingEditor : EditorWindow
    {
        #region Fields
        public const int OBJECT_SPACING = 10;
        public static int OBJECT_PADDING = 1;
        public static Color GREEN_COLOR = new Color(0.53f, 1f, 0f);
        public static Color BLUE_COLOR = new Color(0.20000f, 0.68627f, 0.90588f);
        public static Color RED_COLOR = new Color(1.0f, 0.3f, 0.2f);
        public static Color guiColor;
        public enum PoppinsStyle
        {
            Regular,
            Bold,
            Medium
        }
        [SerializeField]
        public static AnythingVoiceCreator Voice
        {
            get
            {
                return AnythingVoiceCreator.Instance;
            }
        }

        [SerializeField]
        public static AnythingCreator Creator
        {
            get
            {
                return AnythingCreator.Instance;
            }
        }
        [SerializeField]
        public static SceneLedger SceneLedger
        {
            get
            {
                return Creator.SceneLedger;
            }
        }

        private static string UILookMode => EditorGUIUtility.isProSkin ? "DarkMode" : "LightMode";
        #endregion
        public void OnInspectorUpdate()
        {
            Repaint();
        }

        public void Awake()
        {
            InitializeResources();
        }
        /// <summary>
        /// Batch calls the <see cref="InitializeFonts"/> , <see cref="InitializeTextures"/> , and <see cref="InitializeCustomStyles"/> functions.
        /// </summary>
        public bool InitializeResources()
        {
            if (InitializeFonts() && InitializeTextures() && InitializeCustomStyles())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void DrawUILine(Color color, int thickness = 1, int padding = 20)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        protected void DeleteModels()
        {
            AWObj[] sceneAWObjs = FindObjectsOfType<AWObj>();
            foreach (AWObj awObj in sceneAWObjs)
            {
                awObj.StopAllCoroutines();
                AnythingSafeDestroy.SafeDestroyDelayed(awObj.gameObject);
            }
            // reset anything objects

            foreach (AnythingObject anythingObject in FindObjectsOfType<AnythingObject>())
            {
                anythingObject.DestroyRequestedModels();
            }

            foreach (AWHabitat awHab in FindObjectsOfType<AWHabitat>())
            {
                AnythingSafeDestroy.SafeDestroyDelayed(awHab.gameObject);
            }
            foreach (FlockManager flockObj in FindObjectsOfType<FlockManager>())
            {
                AnythingSafeDestroy.SafeDestroyDelayed(flockObj.gameObject);
            }
        }

        protected void DeleteHabitat()
        {
            foreach (AWHabitat awHab in FindObjectsOfType<AWHabitat>())
            {
                AnythingSafeDestroy.SafeDestroyDelayed(awHab.gameObject);
            }
        }

        protected void ResetCreator()
        {
            AnythingCreator.Instance.StopAllCoroutines();
          
            foreach (AnythingCreator anythingCreator in FindObjectsOfType<AnythingCreator>())
            {
                AnythingSafeDestroy.SafeDestroyDelayed(anythingCreator.gameObject);
            }
            ClearConsoleUtil.ClearLogConsole();
            AnythingSetup.Instance.ResetAttributionList();
        }


        #region Draw Editor Elements

        protected void DrawTransparentTex(float x, float y, Texture icon, bool space = false, float opacity = 0)
        {
            //if (opacity > 1) opacity = 1;
            //if (opacity < 0) opacity = 0;

            int width = icon.width;
            int height = icon.height;
            guiColor = GUI.color;
            GUI.color = Color.clear;

            Color background = GUI.backgroundColor;
            if (space == true) GUILayout.Button("", GUIStyle.none, GUILayout.Width(width), GUILayout.Height(height));

            EditorGUI.DrawTextureTransparent(new Rect(x, y, width, height), icon);
            //GUI.DrawTexture(new Rect(x, y, width, height), icon);
            GUI.color = guiColor;
        }
        protected void DrawTex(float x, float y, Texture icon, float opacity = 1f)
        {
            int width = icon.width;
            int height = icon.height;
            guiColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, opacity);
            Color background = GUI.backgroundColor;
            GUI.DrawTexture(new Rect(x, y, width, height), icon, ScaleMode.StretchToFill);
            GUI.color = guiColor;
        }
        protected void DrawTex(float x, float y, Texture icon, bool stretchToPanel, float panelWidth, float panelHeight, float opacity = 1f)
        {
            int width = icon.width;
            int height = icon.height;
            guiColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, opacity);
            Color background = GUI.backgroundColor;
            if (stretchToPanel)
            {
                width = (int)panelWidth;
                height = (int)panelHeight;
            }
            if (stretchToPanel) width = (int)panelWidth;
            GUI.DrawTexture(new Rect(x, y, width, height), icon, ScaleMode.StretchToFill);
            GUI.color = guiColor;
        }

        protected void DrawTex(Rect rect, Texture icon, float opacity = 1f)
        {
            guiColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, opacity);
            GUI.DrawTexture(rect, icon, ScaleMode.StretchToFill);
            GUI.color = guiColor;
        }

        protected void DrawBoldTextHeader(string text)
        {
            try
            {
                GUILayout.Label(text, titleLabelStyle);
            }
            catch
            {
                Debug.Log("error drawing label");
                stylesInitialized = false;
            }


        }


        protected void DrawBoldTextHeader(string text, int fontSize)
        {

            int oldFont = titleLabelStyle.fontSize;
            titleLabelStyle.fontSize = fontSize;
            GUILayout.Label(text, titleLabelStyle);
            titleLabelStyle.fontSize = oldFont;
        }

        protected void DrawBoldTextHeader(string text, int fontSize, RectOffset margin)
        {
            GUIStyle tempTitleLabel = titleLabelStyle;
            tempTitleLabel.fontSize = fontSize;
            tempTitleLabel.margin = margin;
            GUILayout.Label(text, tempTitleLabel);

        }
        protected GUIStyle BuildStyle(GUIStyle baseStyle, PoppinsStyle poppinsType, int fontsize, TextAnchor anchor)
        {
            GUIStyle customStyle = new GUIStyle(baseStyle);
            customStyle.font = GetPoppinsFont(poppinsType);
            customStyle.fontSize = fontsize;
            customStyle.alignment = anchor;
            return customStyle;
        }
        protected GUIStyle BuildStyle(GUIStyle baseStyle, PoppinsStyle poppinsType, int fontsize, TextAnchor anchor, RectOffset margin)
        {
            GUIStyle customStyle = new GUIStyle(baseStyle);
            customStyle.font = GetPoppinsFont(poppinsType);
            customStyle.fontSize = fontsize;
            customStyle.alignment = anchor;
            customStyle.margin = margin;
            return customStyle;
        }
        protected GUIStyle BuildStyle(GUIStyle baseStyle, PoppinsStyle poppinsType, int fontsize, TextAnchor anchor, RectOffset margin, RectOffset padding)
        {
            GUIStyle customStyle = new GUIStyle(baseStyle);
            customStyle.font = GetPoppinsFont(poppinsType);
            customStyle.fontSize = fontsize;
            customStyle.alignment = anchor;
            customStyle.padding = padding;
            customStyle.margin = margin;
            return customStyle;
        }
        protected GUIStyle BuildStyle(GUIStyle baseStyle, PoppinsStyle poppinsType, int fontsize, TextAnchor anchor, Color normalTextColor)
        {
            GUIStyle customStyle = new GUIStyle(baseStyle);
            customStyle.font = GetPoppinsFont(poppinsType);
            customStyle.fontSize = fontsize;
            customStyle.alignment = anchor;
            customStyle.normal.textColor = normalTextColor;
            return customStyle;
        }
        protected GUIStyle BuildStyle(GUIStyle baseStyle, PoppinsStyle poppinsType, int fontsize, TextAnchor anchor, Color normalTextColor, RectOffset padding, RectOffset margin)
        {
            GUIStyle customStyle = new GUIStyle(baseStyle);
            customStyle.font = GetPoppinsFont(poppinsType);
            customStyle.fontSize = fontsize;
            customStyle.alignment = anchor;
            customStyle.normal.textColor = normalTextColor;
            return customStyle;
        }

        protected void DrawBoldTextHeader(string text, int fontSize, TextAnchor anchor)
        {
            int oldFont = titleLabelStyle.fontSize;
            TextAnchor oldAnchor = titleLabelStyle.alignment;
            titleLabelStyle.alignment = anchor;
            titleLabelStyle.fontSize = fontSize;
            GUILayout.Label(text, titleLabelStyle);
            titleLabelStyle.fontSize = oldFont;
            titleLabelStyle.alignment = oldAnchor;
        }
        protected void DrawBoldTextHeader(string text, int fontSize, TextAnchor anchor, bool wrap)
        {
            int oldFont = titleLabelStyle.fontSize;
            TextAnchor oldAnchor = titleLabelStyle.alignment;
            titleLabelStyle.alignment = anchor;
            titleLabelStyle.fontSize = fontSize;
            GUILayout.Label(text, titleLabelStyle);
            titleLabelStyle.fontSize = oldFont;
            titleLabelStyle.alignment = oldAnchor;
            titleLabelStyle.wordWrap = wrap;
        }

        protected void DrawCustomText(string textString, int fontSize, bool wrap, TextAnchor anchor = TextAnchor.MiddleLeft, PoppinsStyle fontStyle = PoppinsStyle.Regular)
        {
            GUIStyle temp = new GUIStyle(EditorStyles.label);
            temp.fontSize = fontSize;
            temp.alignment = anchor;
            temp.font = GetPoppinsFont(fontStyle);
            temp.wordWrap = wrap;
            GUILayout.Label(textString, temp);
        }
        protected void DrawCustomText(string textString, int fontSize, TextAnchor anchor = TextAnchor.MiddleLeft, PoppinsStyle fontStyle = PoppinsStyle.Regular)
        {
            GUIStyle temp = new GUIStyle(EditorStyles.label);
            temp.fontSize = fontSize;
            temp.alignment = anchor;
            temp.font = GetPoppinsFont(fontStyle);

            GUILayout.Label(textString, temp);
        }
        protected void DrawCustomText(string textString, int fontSize, int fixedWidth, TextAnchor anchor = TextAnchor.MiddleLeft, PoppinsStyle fontStyle = PoppinsStyle.Regular)
        {
            GUIStyle temp = new GUIStyle(EditorStyles.label);
            temp.fontSize = fontSize;
            temp.alignment = anchor;
            temp.fixedWidth = fixedWidth;
            temp.font = GetPoppinsFont(fontStyle);

            GUILayout.Label(textString, temp);
        }
        protected void DrawCustomText(string textString, int fontSize, RectOffset margin, TextAnchor anchor = TextAnchor.MiddleLeft, PoppinsStyle fontStyle = PoppinsStyle.Regular)
        {
            GUIStyle temp = new GUIStyle(EditorStyles.label);
            temp.fontSize = fontSize;
            temp.alignment = anchor;
            temp.font = GetPoppinsFont(fontStyle);
            temp.margin = margin;
            GUILayout.Label(textString, temp);
        }
        protected void DrawCustomText(string textString, int fontSize, Color color, TextAnchor anchor = TextAnchor.MiddleLeft, PoppinsStyle fontStyle = PoppinsStyle.Regular)
        {
            GUIStyle temp = new GUIStyle(EditorStyles.label);
            temp.fontSize = fontSize;
            temp.alignment = anchor;
            temp.font = GetPoppinsFont(fontStyle);
            temp.normal.textColor = color;
            GUILayout.Label(textString, temp);
        }
        protected void DrawCustomText(string textString, int fontSize, TextAnchor anchor, PoppinsStyle fontStyle, int uniformMargin)
        {
            GUIStyle temp = new GUIStyle(EditorStyles.label);
            temp.fontSize = fontSize;
            temp.alignment = anchor;
            temp.font = GetPoppinsFont(fontStyle);
            temp.padding = UniformRectOffset(uniformMargin);
            GUILayout.Label(textString, temp);
        }
        #endregion

        #region Styles
        protected static GUIStyle customTextStyle;
        protected static GUIStyle inputFieldStyle;
        public static GUIStyle titleLabelStyle;
        protected static GUIStyle resultPicStyle;
        protected static GUIStyle resultLabelStyle;
        protected static GUIStyle resultLabelTitleStyle;
        protected static GUIStyle iconStyle;
        protected static GUIStyle centeredLabelStyle;
        protected static GUIStyle defaultButtonStyle;
        protected static GUIStyle activeButtonStyle;
        protected static GUIStyle resetButtonStyle;
        protected static GUIStyle micButtonStyle;
        protected static GUIStyle smallButtonStyle;
        protected static GUIStyle smallButtonStyleDeactivated;
        protected static GUIStyle zoomBackButton;
        protected static GUIStyle dropDownStyle;
        protected static GUIStyle toggleStyle;
        protected static GUIStyle searchButtonStyle;
        protected static GUIStyle submitButtonStyle;
        protected static GUIStyle resetSearchButtonStyle;
        protected static GUIStyle roundedThumbnailButton;
        protected static bool stylesInitialized = false;
        private static bool InitializeCustomStyles()
        {
            if (stylesInitialized == false && texturesInitialized == true && fontInitialized == true)
            {

                try
                {
                    int margin = 0;
                    int padding = 0;


                    #region Input Field Style
                    inputFieldStyle = new GUIStyle(EditorStyles.textField);
                    inputFieldStyle.font = POPPINS_MEDIUM;
                    inputFieldStyle.fontSize = 14;
                    //Margin
                    margin = 10;
                    inputFieldStyle.margin = UniformRectOffset(margin);
                    //Padding
                    padding = 0;
                    inputFieldStyle.padding.top = padding;
                    inputFieldStyle.alignment = TextAnchor.MiddleLeft;

                    #endregion

                    #region Define Label Style
                    titleLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                    titleLabelStyle.font = POPPINS_BOLD;
                    titleLabelStyle.alignment = TextAnchor.MiddleCenter;
                    titleLabelStyle.fontSize = 20;
                    margin = 10;
                    titleLabelStyle.margin = UniformRectOffset(margin);
                    #endregion

                    #region Define Result Pic Style
                    resultPicStyle = new GUIStyle();
                    resultPicStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
                    resultPicStyle.alignment = TextAnchor.MiddleCenter;
                    resultPicStyle.active.scaledBackgrounds = null;
                    resultPicStyle.normal.background = resultPicStyle.active.background;
                    float resultWidth = 200f;
                    resultPicStyle.fixedWidth = resultWidth;
                    resultPicStyle.fixedHeight = resultWidth; //* 0.77f;
                    //Set Margins
                    margin = 10; //10;

                    resultPicStyle.margin = UniformRectOffset(margin);

                    //resultPicStyle.padding = UniformRectOffset(margin);
                    #endregion

                    #region Define Result Label Style
                    resultLabelStyle = new GUIStyle(EditorStyles.label);
                    resultLabelStyle.alignment = TextAnchor.MiddleCenter;
                    resultLabelStyle.wordWrap = true;

                    margin = 1;
                    resultLabelStyle.font = POPPINS_REGULAR;
                    resultLabelStyle.fontSize = 10;

                    //resultPicStyle.margin = UniformRectOffset(margin);
                    resultLabelStyle.padding = new RectOffset(5, 5, 1, 1);
                    #endregion

                    #region Result Label Title
                    resultLabelTitleStyle = new GUIStyle(EditorStyles.label);
                    resultLabelTitleStyle.alignment = TextAnchor.MiddleCenter;
                    resultLabelTitleStyle.wordWrap = false;
                    margin = 5;
                    resultLabelTitleStyle.font = POPPINS_BOLD;
                    resultLabelTitleStyle.fontSize = 12;
                    resultLabelTitleStyle.wordWrap = true;
                    resultLabelTitleStyle.padding = new RectOffset(5, 5, 1, 1);
                    #endregion

                    #region Rounded Thumbnail Button

                    roundedThumbnailButton = new GUIStyle();
                    roundedThumbnailButton.hover.background = thumbnailBackgroundInactive;
                    roundedThumbnailButton.normal.background = thumbnailBackgroundActive;
                    roundedThumbnailButton.stretchWidth = true;
                    roundedThumbnailButton.clipping = TextClipping.Clip;

                    #endregion

                    #region CentredLabel
                    centeredLabelStyle = GUI.skin.GetStyle("Label");
                    centeredLabelStyle.alignment = TextAnchor.MiddleLeft;

                    #endregion

                    #region Nav Button

                    #region Nav Button Default
                    defaultButtonStyle = new GUIStyle(EditorStyles.miniButton);
                    defaultButtonStyle.fixedHeight = 50;
                    defaultButtonStyle.font = POPPINS_BOLD;
                    defaultButtonStyle.fontSize = 12;
                    defaultButtonStyle.hover.textColor = GREEN_COLOR;
                    margin = 10;
                    defaultButtonStyle.margin = new RectOffset(margin, margin, margin, margin);

                    #endregion




                    #region Nav Button Active
                    activeButtonStyle = new GUIStyle(EditorStyles.miniButton);
                    activeButtonStyle.fixedHeight = 50;
                    activeButtonStyle.font = POPPINS_BOLD;
                    activeButtonStyle.fontSize = 12;
                    activeButtonStyle.normal.textColor = BLUE_COLOR;
                    activeButtonStyle.hover.textColor = BLUE_COLOR;
                    margin = 10;
                    activeButtonStyle.margin = new RectOffset(margin, margin, margin, margin);
                    #endregion
                    #endregion

                    #region Search Button

                    searchButtonStyle = new GUIStyle(EditorStyles.miniButton);
                    searchButtonStyle.font = POPPINS_BOLD;
                    searchButtonStyle.fixedHeight = 30;
                    margin = 10;
                    searchButtonStyle.margin = new RectOffset(0, margin, margin, margin);
                    #endregion

                    #region Submit Button Style
                    submitButtonStyle = searchButtonStyle;
                    submitButtonStyle.margin = new RectOffset(10, 10, 10, 10);
                    #endregion

                    #region Search Button

                    resetSearchButtonStyle = new GUIStyle(EditorStyles.miniButton);
                    resetSearchButtonStyle.font = POPPINS_BOLD;
                    resetSearchButtonStyle.fixedHeight = 30;
                    resetSearchButtonStyle.alignment = TextAnchor.MiddleCenter;
                    resetSearchButtonStyle.fontSize = 25;
                    //margin = 10;
                    padding = 5;
                    resetSearchButtonStyle.padding = new RectOffset(padding, padding, padding, padding);
                    resetSearchButtonStyle.margin = new RectOffset(margin, 0, margin, margin);
                    #endregion


                    #region Reset Button Style
                    resetButtonStyle = new GUIStyle(EditorStyles.miniButton);
                    resetButtonStyle.fixedHeight = 50;
                    resetButtonStyle.font = POPPINS_REGULAR;
                    resetButtonStyle.fontSize = 12;
                    resetButtonStyle.normal.textColor = GREEN_COLOR;
                    margin = 10;
                    resetButtonStyle.margin = new RectOffset(margin, margin, margin, margin);
                    #endregion

                    #region Mic Button Style
                    micButtonStyle = new GUIStyle(GUIStyle.none);
                    micButtonStyle.alignment = TextAnchor.MiddleCenter;
                    margin = 5;
                    micButtonStyle.margin = UniformRectOffset(5);
                    #endregion

                    #region Small Button Style
                    smallButtonStyle = new GUIStyle(EditorStyles.miniButton);
                    smallButtonStyle.fixedWidth = 70;
                    smallButtonStyle.fixedHeight = 30;
                    smallButtonStyle.font = POPPINS_REGULAR;
                    smallButtonStyle.fontSize = 12;
                    smallButtonStyle.hover.textColor = BLUE_COLOR;
                    margin = 5;
                    smallButtonStyle.margin = UniformRectOffset(margin);
                    padding = 0;
                    smallButtonStyle.padding = UniformRectOffset(padding);
                    #endregion

                    #region Small Button StyleDeactivated
                    smallButtonStyleDeactivated = smallButtonStyle;
                    smallButtonStyle.hover.textColor = Color.grey;
                    smallButtonStyle.normal.textColor = Color.grey;
                    smallButtonStyle.active.textColor = Color.grey;
                    #endregion

                    #region Zoom Back Button
                    zoomBackButton = new GUIStyle(GUIStyle.none);
                    padding = 5;
                    zoomBackButton.padding = UniformRectOffset(padding);
                    zoomBackButton.alignment = TextAnchor.MiddleCenter;
                    #endregion

                    #region Drop Down Menu
                    dropDownStyle = new GUIStyle(EditorStyles.popup);
                    dropDownStyle.stretchHeight = true;
                    dropDownStyle.fixedHeight = 50;
                    dropDownStyle.font = POPPINS_BOLD;
                    dropDownStyle.fontSize = 12;
                    dropDownStyle.hover.textColor = EditorGUIUtility.isProSkin ? GREEN_COLOR : new Color(0.26f, 0.5f, 0f);
                    margin = 10;
                    dropDownStyle.margin = UniformRectOffset(margin);
                    padding = 10;
                    dropDownStyle.padding = UniformRectOffset(padding);
                    #endregion

                    #region ToggleStyle
                    toggleStyle = new GUIStyle(EditorStyles.toggle);
                    toggleStyle.margin = new RectOffset(inputFieldStyle.margin.left, 0, 0, 0);
                    toggleStyle.imagePosition = ImagePosition.ImageLeft;
                    toggleStyle.alignment = TextAnchor.MiddleLeft;
                    toggleStyle.padding = new RectOffset(20, 0, 0, 2);
                    toggleStyle.font = POPPINS_BOLD;
                    toggleStyle.fixedHeight = smallButtonStyle.fixedHeight;
                    #endregion


                }
                catch (Exception e)
                {
                    if (AnythingSettings.Instance.showDebugMessages)
                    {
                        Debug.Log("Error initializing custom styles with error: ");
                        Debug.LogException(e);
                    }
                    return false;
                }
                //Debug.Log("Styles initialized!");
                stylesInitialized = true;
            }
            return true;
        }


        #endregion

        #region Textures
        private static Texture2D darkBackground;
        protected static Texture2D DarkBackground
        {
            get
            {
                if (darkBackground == null)
                {
                    darkBackground = Resources.Load("Editor/Shared/darkBackground") as Texture2D;
                }
                return darkBackground;
            }
        }
        private static Texture2D greyAnythingGlobeLogo;
        protected static Texture2D GreyAnythingGlobeLogo
        {
            get
            {
                if (greyAnythingGlobeLogo == null)
                {

                    greyAnythingGlobeLogo = Resources.Load("Editor/Shared/greyGlobeLogo") as Texture2D;

                }
                return greyAnythingGlobeLogo;
            }
        }

        private static Texture2D whiteAnythingGlobeLogo;
        protected static Texture2D WhiteAnythingGlobeLogo
        {
            get
            {
                if (whiteAnythingGlobeLogo == null)
                {

                    whiteAnythingGlobeLogo = Resources.Load("Editor/Shared/whiteGlobeLogo") as Texture2D;

                }
                return whiteAnythingGlobeLogo;
            }
        }

        private static Texture2D filledGreenAnythingGlobeLogo;
        protected static Texture2D FilledGreenAnythingGlobeLogo
        {
            get
            {
                if (filledGreenAnythingGlobeLogo == null)
                {
                    filledGreenAnythingGlobeLogo = Resources.Load("Editor/Shared/filledGreenGlobe") as Texture2D;
                }
                return filledGreenAnythingGlobeLogo;
            }

        }

        private static Texture2D greenGradientBanner;
        protected static Texture2D GreenGradientBanner
        {
            get
            {
                if (greenGradientBanner == null)
                {
                    greenGradientBanner = Resources.Load("Editor/Shared/gradientRectangle") as Texture2D;
                }
                return greenGradientBanner;
            }
        }

        private static Texture2D blackAnythingGlobeLogo;
        protected static Texture2D BlackAnythingGlobeLogo
        {
            get
            {
                if (blackAnythingGlobeLogo == null)
                {
                    blackAnythingGlobeLogo = Resources.Load("Editor/Shared/blackGlobeLogo") as Texture2D;
                }
                return blackAnythingGlobeLogo;
            }
        }

        protected static Texture2D reset;
        protected static Texture2D creatorIcon;
        protected static Texture2D activeMicIcon;
        protected static Texture2D inactiveMicIcon;

        protected static Texture2D micButtonTex;
        protected static Texture2D inactiveWaveform;
        protected static Texture2D activeWaveform;
        protected static Texture2D gridIcon;
        protected static Texture2D speechBubble;

        protected static Texture2D objectCreatorButton;
        protected static Texture2D voiceCreatorButton;
        protected static Texture2D habitatCreatorButton;
        protected static Texture2D signupButton;
        protected static Texture2D loginButton;

        protected static Texture2D anythingWorldBanner;
        protected static Texture2D thumbnailBackgroundActive;
        protected static Texture2D thumbnailBackgroundInactive;

        protected static Texture2D defaultThumbnailImage;

        private static Texture2D roundedDarkSquare;
        protected static Texture2D RoundedDarkSquare
        {
            get
            {
                if (roundedDarkSquare == null)
                {
                    roundedDarkSquare = Resources.Load("Editor/AnythingCreatorWindow/VoicePanel/roundedDarkSquare") as Texture2D;
                }
                return roundedDarkSquare;
            }
        }

        protected static Texture2D loadingRing;
        private static Texture2D loadingCenter;
        protected static Texture2D LoadingCenter
        {
            get
            {
                if (loadingCenter == null)
                {
                    loadingCenter = Resources.Load($"Editor/AnythingCreatorWindow/CreatorPanel/Loading/{UILookMode}/loadingCenter") as Texture2D;
                }
                return loadingCenter;
            }
        }


        protected static bool texturesInitialized = false;
        private static bool InitializeTextures()
        {
            if (!texturesInitialized)
            {
                //This is not optimal but is apparently the only way to solve an issue with call order of texture loading with Unity.
                //Will look back at it at later date.
                try
                {

                    #region Creator Banner
                    anythingWorldBanner = Resources.Load("Editor/AnythingCreatorWindow/CreatorBanner/anythingWorldBanner") as Texture2D;
                    objectCreatorButton = Resources.Load($"Editor/AnythingCreatorWindow/CreatorBanner/{UILookMode}/objectCreatorButton") as Texture2D;
                    voiceCreatorButton = Resources.Load($"Editor/AnythingCreatorWindow/CreatorBanner/{UILookMode}/voiceCreatorButton") as Texture2D;
                    habitatCreatorButton = Resources.Load($"Editor/AnythingCreatorWindow/CreatorBanner/{UILookMode}/habitatCreatorButton") as Texture2D;
                    #endregion
                    #region Creator Panel
                    defaultThumbnailImage = Resources.Load("Editor/AnythingCreatorWindow/CreatorPanel/Thumbnail/defaultThumbnail") as Texture2D;
                    thumbnailBackgroundActive = Resources.Load("Editor/AnythingCreatorWindow/CreatorPanel/Thumbnail/thumbnailBackgroundActive") as Texture2D;
                    thumbnailBackgroundInactive = Resources.Load("Editor/AnythingCreatorWindow/CreatorPanel/Thumbnail/thumbnailBackgroundInactive") as Texture2D;

                    gridIcon = Resources.Load($"Editor/AnythingCreatorWindow/CreatorPanel/{UILookMode}/gridIcon") as Texture2D;
                    loadingRing = Resources.Load($"Editor/AnythingCreatorWindow/CreatorPanel/Loading/{UILookMode}/loadingRing") as Texture2D;

                    #endregion
                    #region Audio Panel
                    activeMicIcon = Resources.Load("Editor/AnythingCreatorWindow/VoicePanel/orangeMicButton") as Texture2D;
                    inactiveMicIcon = Resources.Load($"Editor/AnythingCreatorWindow/VoicePanel/{UILookMode}/greyMicButton") as Texture2D;
                    activeWaveform = Resources.Load("Editor/AnythingCreatorWindow/VoicePanel/orangeActiveSoundwave") as Texture2D;
                    inactiveWaveform = Resources.Load($"Editor/AnythingCreatorWindow/VoicePanel/{UILookMode}/greyInactiveSoundwave") as Texture2D;
                    speechBubble = Resources.Load("Editor/AnythingCreatorWindow/VoicePanel/speechBubble") as Texture2D;
                    #endregion
                    #region Signup/Login Window
                    loginButton = Resources.Load($"Editor/LoginWindow/{UILookMode}/loginButton") as Texture2D;
                    signupButton = Resources.Load($"Editor/LoginWindow/{UILookMode}/signupButton") as Texture2D;
                    #endregion
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Textures failed to initialize with error: ");
                    Debug.LogException(e);
                    return false;
                }
                texturesInitialized = true;
                return true;
            }
            else
            {
                return true;
            }

        }
        #endregion


        #region Fonts
        public static Font POPPINS_REGULAR;
        public static Font POPPINS_MEDIUM;
        public static Font POPPINS_BOLD;
        public static bool fontInitialized = false;
        private static bool InitializeFonts()
        {
            if (!fontInitialized)
            {
                try
                {
                    POPPINS_REGULAR = (Font)Resources.Load("Fonts/Poppins/Poppins-Regular", typeof(Font));
                    POPPINS_MEDIUM = (Font)Resources.Load("Fonts/Poppins/Poppins-Medium", typeof(Font));
                    POPPINS_BOLD = (Font)Resources.Load("Fonts/Poppins/Poppins-Bold", typeof(Font));
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Fonts failed to initialize with error: ");
                    Debug.Log(e);
                    return false;
                }
                fontInitialized = true;

            }
            return true;
        }
        #endregion

        #region Utility Functions

        Font GetPoppinsFont(PoppinsStyle style)
        {
            Font chosenFont = null;
            switch (style)
            {
                case PoppinsStyle.Regular:
                    chosenFont = POPPINS_REGULAR;
                    break;
                case PoppinsStyle.Medium:
                    chosenFont = POPPINS_MEDIUM;
                    break;
                case PoppinsStyle.Bold:
                    chosenFont = POPPINS_BOLD;
                    break;
                default:
                    chosenFont = POPPINS_REGULAR;
                    break;
            }
            return chosenFont;
        }
        protected static RectOffset UniformRectOffset(int offset)
        {
            return new RectOffset(offset, offset, offset, offset);
        }
        protected void ToggleGUIClear()
        {
            if (GUI.color != Color.clear)
            {
                guiColor = GUI.color;
                GUI.color = Color.clear;
            }
            else
            {
                GUI.color = guiColor;
            }
        }



        protected void ToggleGUIColor(Color tempColor)
        {
            if (GUI.color != tempColor)
            {
                guiColor = GUI.color;
                GUI.color = tempColor;
            }
            else
            {
                GUI.color = guiColor;
            }
        }
        protected void ToggleGUIBackgroundColor(Color tempColor)
        {
            if (GUI.backgroundColor != tempColor)
            {
                guiColor = GUI.backgroundColor;
                GUI.backgroundColor = tempColor;
            }
            else
            {
                GUI.backgroundColor = guiColor;
            }
        }


        protected void ToggleGUIBackgroundClear()
        {
            if (GUI.backgroundColor != Color.clear)
            {
                guiColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.clear;
            }
            else
            {
                GUI.backgroundColor = guiColor;
            }
        }

        public static void SpaceWrapped(float pixels)
        {
            try
            {
                GUILayout.Space(pixels);
            }
            catch
            {
                GUIUtility.ExitGUI();
            }
        }
        public static void FlexibleSpaceWrapped()
        {
            try
            {
                GUILayout.FlexibleSpace();
            }
            catch
            {
                GUIUtility.ExitGUI();
            }
        }
        #endregion


    }
}

