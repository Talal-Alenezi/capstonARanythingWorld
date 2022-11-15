 using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AnythingWorld.Habitat;
#endif
#if UNITY_EDITOR_WIN
#elif UNITY_EDITOR_OSX
using System.Runtime.InteropServices;
#endif
using AnythingWorld.DataContainers;
namespace AnythingWorld.Editors
{
    [ExecuteAlways]

    /// <summary>
    /// Custom editor for the AnythingCreator.
    /// </summary>
    public class AnythingCreatorEditor : AnythingEditor
    {
        //Object Creator variables
        List<SearchResult> searchResults = null;
        List<SearchResult> animatedResults = null;
        private string searchTerm;
        private string searchTermFieldString = "";
        private int searchResultsCount = 0;
        private int selectedNavButtonIndex = 0;
        private bool searchReturned = false;
        private bool searchStarted = false;
        private bool searchLoading = false;
        private bool errorWhileSearching = false;
        private bool showAnimatedOnly = false;
        private bool createOnGrid = false;
        private bool resetSearchField = false;

        #region Loading Animation Fields
        private float searchRingAngle = 0;
        private double lastEditorTime = 0;
        ///Holds last rect for manually calculating element placement.
        Rect lastRect = Rect.zero;
        ///Show full logo animation if space in window large enough, otherwise placeholder.
        ///This is because the matrix rotation method for animation doesn't work at smaller scale.
        bool showFullLogo = true;
        #endregion

        #region Result Drawing
        private Vector2 newScrollPosition;
        private bool copiedToKeyboard = false;
        private Rect copiedRect;
        private int copiedResult = 0;
        #endregion

        //Voice Creator variables
        public bool autoGenerateFromVoice = true;
        private int selectedMicIndex = 0;
        private string voiceInputString = "...";
        //Habitat Creator variables
        private bool populateHabitat = false;
        private int selectedHabitatDropdownIndex = 0;




        private new void Awake()
        {
            if (searchResults == null) searchResults = new List<SearchResult>();
        }

        public void OnEnable()
        {
            StopSearch();
        }

        /// <summary>
        /// Opens API login window.
        /// </summary>
        public static void ShowAPIWindow()
        {
            AnythingSignupLoginEditor.OpenFromScript();
        }

        /// <summary>
        /// Checks is AnythingSettings has API key present.
        /// </summary>
        /// <returns>Returns false if API key not present, true if present.</returns>
        private bool CheckAPI()
        {
            if (AnythingSettings.Instance.apiKey == "")
            {
                if (AnythingSettings.Instance == null)
                {
                    AnythingSettings.CreateInstance<AnythingSettings>();
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        // ReSharper restore Unity.ExpensiveCode
        // ReSharper disable Unity.PerformanceAnalysis
        public void OnGUI()
        {
            #region Initialize GUI
            Voice.SubscribeOutputText(UpdateRecognisedString);
            Voice.SubscribeToListeningStatus(ToggleMicTex);
            Voice.SubscribeToListeningEnded(AutoCreateOnFinish);
            InitializeResources();
            #endregion

            #region Top Bar
            DrawWindowBanner();
            DrawNavToolbar();

            #endregion

            #region Window Content

            switch (selectedNavButtonIndex)
            {
                case 0:
                    ObjectCreatorPanel();
                    break;
                case 1:
                    VoiceCreatorPanel();
                    break;
                case 2:
                    HabitatCreatorPanel();
                    break;
                default:
                    ObjectCreatorPanel();
                    break;
            }
            #endregion

            Repaint();
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }

        /// <summary>
        /// GUI manager for Object Creator panel.
        /// </summary>
        private void ObjectCreatorPanel()
        {
            if (!InitializeResources())
            {
                return;
            }
            EditorGUILayout.BeginVertical();
            DrawBoldTextHeader("Create 3D Models");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            ResetGridButton();
            ResetCreatorButton();
            ResetAllButton();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            ShowAnimatedOnlyToggle();
            GUILayout.FlexibleSpace();
            CreateOnGridToggle();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            DrawSearchBar();
            EditorGUILayout.EndHorizontal();
            DrawResultsGrid();
            EditorGUILayout.EndVertical();
        }

        private void ResetAllButton()
        {
            if (GUILayout.Button("Reset All", searchButtonStyle))
            {
                if (EditorUtility.DisplayDialog("Reset Creations",
                        "Are you sure you want reset scene and remove all creations?", "Reset", "Cancel"))
                {
                    ResetCreator();
                    DeleteHabitat();
                    DeleteModels();
                    AnythingCreator.Instance.LayoutGrid.InitNewGrid(10, 10);
                }
            }
        }
        private void ResetCreatorButton()
        {
            if (GUILayout.Button("Reset Creator", searchButtonStyle))
            {
                if (EditorUtility.DisplayDialog("Reset Creations",
                        "Reset creator processes?", "Reset", "Cancel"))
                {
                    ResetCreator();
                    AnythingCreator.Instance.LayoutGrid.InitNewGrid(10, 10);
                }
            }
        }

        private static void ResetGridButton()
        {
            if (GUILayout.Button("Reset Grid", searchButtonStyle))
            {
                if (EditorUtility.DisplayDialog("Reset Grid", "Are you sure you want to reset grid?", "Reset", "Cancel"))
                {
                    AnythingCreator.Instance.LayoutGrid.InitNewGrid(10, 10);
                }
            }
        }

        private void CreateOnGridToggle()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            createOnGrid = GUILayout.Toggle(createOnGrid, "ENABLE GRID PLACEMENT", toggleStyle);
            GUILayout.EndVertical();
        }

        private void ShowAnimatedOnlyToggle()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            //Toggle for showing animated models only
            showAnimatedOnly = GUILayout.Toggle(showAnimatedOnly, "ANIMATED ONLY", toggleStyle);
            GUILayout.EndVertical();
        }

        

        /// <summary>
        /// GUI manager for Voice Creator panel.
        /// </summary>
        private void VoiceCreatorPanel()
        {
            EditorGUILayout.BeginVertical();
            DrawBoldTextHeader("Create Models With Voice");
            EditorGUILayout.BeginHorizontal();
            var micActive = MicButtonPanel();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            DrawSoundWave(micActive);
            EditorGUILayout.EndHorizontal();
            DrawSpeechPreview(voiceInputString);
            MicSelectionDropDown();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// GUI manager for Habitat Creator panel.
        /// </summary>
        private void HabitatCreatorPanel()
        {
            EditorGUILayout.BeginVertical();
            DrawBoldTextHeader("Create Habitat");
            HabitatDropDownMenu();
            EditorGUILayout.EndVertical();
        }


        #region Initialization
        /// <summary>
        /// Initializes and shows window, called from Anything World top bar menu.
        /// </summary>
        [MenuItem("Anything World/Anything Creator", false, 1)]
        private static void Init()
        {
            if (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline) { }
            else
            {
                Debug.LogWarning("Warning: Standard RP detected, HDRP or URP must be installed to use Anything World.");
            }
            var window = GetWindow(typeof(AnythingCreatorEditor), false, "Anything Creator");
            window.position = new Rect(10, 10, 500, 500);
            window.Show();
        }

        /// <summary>
        /// Refresh window size variables (position)
        /// </summary>
        public void GetWindowSize()
        {
            GetWindow<AnythingCreatorEditor>("position", false);
        }

        public void UpdateSearchResults(SearchResult[] thumbnailResults, AWThing[] jsonResults, bool failed)
        {
            errorWhileSearching = false;
            searchLoading = false;
            searchStarted = true;
            searchResults = new List<SearchResult>();
            animatedResults = new List<SearchResult>();

            if (thumbnailResults == null)
            {
                errorWhileSearching = true;
                searchReturned = false;
                return;
            }

            if (thumbnailResults.Length > 0)
            {
                searchReturned = true;
                searchResults = thumbnailResults.ToList<SearchResult>();
            }
            else
            {
                searchReturned = false;
            }
            foreach (var res in searchResults)
            {
                if (res.isAnimated)
                {
                    animatedResults.Add(res);
                }
            }
        }
        public void UpdateRecognisedString(string updateText)
        {
            voiceInputString = updateText;
        }
        #endregion

        #region Draw Elements

        /// <summary>
        /// Draws Anything world logo and Reset All button for the Anything Creator window.
        /// </summary>
        /// <param name="bannerIcon"> Anything world banner texture to display in top left.</param>
        /// <param name="horizontalPadding"></param>
        /// <param name="verticalPadding"></param>
        private void DrawWindowBanner()
        {
            var globeRect = new Rect(10, 10, 64, 64);
            var bannerRect = new Rect(0, 0, position.width, globeRect.yMax + 10);
            GUI.DrawTexture(bannerRect, GreenGradientBanner);
            GUI.DrawTexture(globeRect, BlackAnythingGlobeLogo);
            var titleRect = new Rect(globeRect.xMax + 10, 21, position.width - globeRect.xMax, 40);
            var title = new GUIContent("ANYTHING CREATOR");
            GUI.Label(titleRect, title, BuildStyle(EditorStyles.label, PoppinsStyle.Bold, 22, TextAnchor.UpperLeft, Color.black));
            var version = new GUIContent(AnythingSettings.PackageVersion);
            GUI.Label(titleRect, version, BuildStyle(EditorStyles.label, PoppinsStyle.Regular, 12, TextAnchor.LowerLeft, Color.black));
            //Mask banner in layouting
            GUILayoutUtility.GetRect(position.width, bannerRect.yMax, GUILayout.MinWidth(500));
        }

        /// <summary>
        /// Handles search input from user and calls request to API in anything creator.
        /// </summary>
        /// <param name="searchBarActive">Is search bar active and can accept input</param>
        /// <returns></returns>
        private string DrawSearchBar(bool searchBarActive = true)
        {
            if (searchBarActive)
            {
                var previousSearch = searchTerm;

                if (Event.current.Equals(Event.KeyboardEvent("return")))
                {

                    if (CheckAPI() == false)
                    {
                        ShowAPIWindow();
                        searchTermFieldString = null;
                        return null;
                    }
                    else
                    {
                        searchTerm = searchTermFieldString;
                        DoSearch(searchTermFieldString);
                    }

                }

               
                if (resetSearchField)
                {
                    EditorGUI.FocusTextInControl(null);
                    searchTermFieldString = "";
                    resetSearchField = false;
                }


                GUI.SetNextControlName("SearchBar");
                searchTermFieldString = EditorGUILayout.TextField(searchTermFieldString, inputFieldStyle, GUILayout.Height(30));

                if (searchLoading) GUI.enabled = false;
                if (GUILayout.Button("Search", searchButtonStyle, GUILayout.MaxWidth(60)))
                {
                    if (CheckAPI() == false)
                    {
                        ShowAPIWindow();
                        searchTermFieldString = null;
                        return null;
                    }
                    else
                    {
                        searchTerm = searchTermFieldString;
                        DoSearch(searchTermFieldString);
                    }
                }
                GUI.enabled = true;
                if (GUILayout.Button("×", resetSearchButtonStyle, GUILayout.MaxWidth(30)))
                {
                    StopSearch();
                    ResetPanel();
                    
                };
                GUILayout.Space(5);

             
                return searchTerm;
            }
            else
            {
                GUILayout.Label(searchTerm, inputFieldStyle, GUILayout.Height(30));
                return searchTerm;
            }
        }
        private void StopSearch()
        {
            searchLoading = false;
            AnythingCreator.Instance.CancelCategorySearchCoroutine();
        }
        private void ResetPanel()
        {

            searchReturned = false;
            searchStarted = false;
            resetSearchField = true;
            searchTermFieldString = "";
            searchTerm = "";

            Repaint();

        }
        private void DoSearch(string termToSearch)
        {
            ResetSearchWindow();
            if (termToSearch == "")
            {
                searchStarted = false;
                searchReturned = false;
            }
            else
            {
                searchLoading = true;
                errorWhileSearching = false;
                //Call request category search, delegate UpdateSearchResult called once search finished.
                AnythingCreator.Instance.RequestCategorySearchResults(termToSearch, UpdateSearchResults);
            }
        }

        private void ResetSearchWindow()
        {
            searchRingAngle = 0;
            searchResults = new List<SearchResult>();
            newScrollPosition = Vector2.zero;
        }

        private void DrawResults(List<SearchResult> customSearchResults, float scaleMultiplier = 1)
        {
            var internalMultiplier = 1.5f;
            var buttonWidth = 100f * internalMultiplier * scaleMultiplier;
            var buttonHeight = 140f * internalMultiplier * scaleMultiplier;
            var verticalMargin = 5 * internalMultiplier;
            var horizontalMargin = 5 * internalMultiplier;
            float scrollBarAllowance = 6;
            //Scale font size
            resultLabelTitleStyle.fontSize = Mathf.RoundToInt(12 * scaleMultiplier * internalMultiplier);
            resultLabelStyle.fontSize = Mathf.RoundToInt(8 * scaleMultiplier * internalMultiplier);
            //Calculate results per line and rows.
            var buttonWidthWithMargin = buttonWidth + horizontalMargin;
            searchResultsCount = customSearchResults.Count;
            if (customSearchResults == null) { customSearchResults = new List<SearchResult>(); }
            var resultsPerLine = Mathf.Floor((position.width - horizontalMargin) / buttonWidthWithMargin);
            if (resultsPerLine == 0)
            {
                return;
            }
            var rows = (int)Math.Ceiling(searchResultsCount / resultsPerLine);
            //Caculate width of results block including horizontalMargin on either side
            var actualBlockWidth = (resultsPerLine * buttonWidthWithMargin) + horizontalMargin;
            //Free space that isn't block but too small for another result to be displayed
            var outerRemainder = position.width - (actualBlockWidth);
            //Margin either side
            var remainderMargin = outerRemainder / 2;


            var searchResultNumber = 0;
            //Setup scroll view
            var lastRect = GUILayoutUtility.GetLastRect();
            var gridArea = new Rect(0, lastRect.yMax, position.width + scrollBarAllowance, (buttonHeight * rows) + (verticalMargin * rows));
            var view = new Rect(0, lastRect.yMax, position.width, position.height - lastRect.yMax);
            newScrollPosition = GUI.BeginScrollView(view, newScrollPosition, gridArea);

            //Reset copiedToKeyboard string once mouse leaves panel
            var labelText = "";
            if (copiedToKeyboard)
            {
                if (!copiedRect.Contains(Event.current.mousePosition))
                {
                    copiedToKeyboard = false;
                }
                labelText = "copied to keyboard";

            }

            //Iterate through elements in the grid and draw the thumbnail and data.
            for (var yPos = 0; yPos < rows; yPos++)
            {
                var rowCoord = lastRect.yMax + (yPos * buttonHeight) + (verticalMargin * yPos);
                for (var xPos = 0; xPos < resultsPerLine; xPos++)
                {
                    if (searchResultNumber < searchResultsCount)
                    {
                        var columnCoord = ((xPos * buttonWidthWithMargin) + horizontalMargin + (remainderMargin - scrollBarAllowance));
                        var thisResult = customSearchResults[searchResultNumber];

                        //Assign thumbnail
                        var displayThumbnail = thisResult.Thumbnail;
                        if (displayThumbnail == null)
                        {
                            displayThumbnail = defaultThumbnailImage;
                        }
                        //Define button position and content
                        var buttonContent = new GUIContent("");
                        var buttonRect = new Rect(columnCoord, rowCoord, buttonWidth, buttonHeight);
                        //Render button
                        var thumbnailBackgroundRect = new Rect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.width);
                        GUI.DrawTexture(thumbnailBackgroundRect, thumbnailBackgroundActive);

                        GUI.DrawTexture(thumbnailBackgroundRect, thumbnailBackgroundActive);
                        if (GUI.Button(buttonRect, buttonContent, roundedThumbnailButton))
                        {
                            //If left click, make object.
                            if (Event.current.button == 0)
                            {

                                if (copiedToKeyboard)
                                {
                                    copiedToKeyboard = false;
                                }
                                else
                                {
                                    AnythingCreator.Instance.MakeObject(thisResult.name, createOnGrid, true, hasCollider: true);
                                }


                            }// If right click, copy GUID value to clipboard.
                            else if (Event.current.button == 1)
                            {
                                var te = new TextEditor();
                                te.text = thisResult.name;
                                te.SelectAll();
                                te.Copy();
                                copiedResult = searchResultNumber;
                                copiedRect = buttonRect;
                                copiedToKeyboard = true;
                            }
                        }
                        //Draw thumbnail
                        GUI.DrawTexture(thumbnailBackgroundRect, displayThumbnail);
                        //Define label and sublabel positions
                        var labelRect = new Rect(buttonRect.x, thumbnailBackgroundRect.yMax, buttonRect.width, ((buttonRect.height - thumbnailBackgroundRect.height) / 2));
                        var sublabelRect = new Rect(labelRect.x, labelRect.y + labelRect.height, labelRect.width, labelRect.height);
                        //If has been copied draw "Copied to keyboard" label.
                        if (copiedToKeyboard && copiedResult == searchResultNumber)
                        {
                            var guidRect = new Rect(labelRect.x, labelRect.y + (0.5f * labelRect.height), labelRect.width, labelRect.height);
                            //If this entry is the one copied to keyboard 
                            labelText = "GUID copied to keyboard";
                            var label = new GUIContent(labelText);
                            GUI.Label(guidRect, label, resultLabelStyle);
                        }
                        else
                        {
                            //If mouseover display both Title & GUID
                            if (buttonRect.Contains(Event.current.mousePosition))
                            {

                                var label = new GUIContent(thisResult.name, thisResult.name);
                                var subLabelContent = new GUIContent(thisResult.data.author);

                                //Draw big label
                                var sizeOfLabel = resultLabelTitleStyle.CalcSize(label);
                                if (labelRect.width < sizeOfLabel.x)
                                {
                                    var ratio = labelRect.width / sizeOfLabel.x;
                                    var oldFontSize = resultLabelTitleStyle.fontSize;
                                    resultLabelTitleStyle.fontSize = Mathf.RoundToInt((oldFontSize) * ratio) - 1;
                                    GUI.Label(labelRect, label, resultLabelTitleStyle);
                                    resultLabelTitleStyle.fontSize = oldFontSize;
                                }
                                else
                                {
                                    GUI.Label(labelRect, label, resultLabelTitleStyle);
                                }



                                //Draw small label
                                var sizeOfRenderedSublabelText = resultLabelStyle.CalcSize(subLabelContent);
                                if (sublabelRect.width < sizeOfRenderedSublabelText.x)
                                {
                                    var ratio = labelRect.width / sizeOfRenderedSublabelText.x;

                                    var oldFontSize = resultLabelStyle.fontSize;
                                    resultLabelStyle.fontSize = Mathf.RoundToInt((oldFontSize * ratio)) - 2;
                                    GUI.Label(sublabelRect, subLabelContent, resultLabelStyle);
                                    resultLabelStyle.fontSize = oldFontSize;

                                }
                                else
                                {
                                    GUI.Label(sublabelRect, subLabelContent, resultLabelStyle);
                                }
                            }
                            else
                            {

                                var centeredLabelRect = new Rect(labelRect.x, labelRect.y + (0.5f * labelRect.height), labelRect.width, labelRect.height);
                                //Else just display display name, eg. guid: cat#0000, display name: Cat
                                var label = new GUIContent(thisResult.DisplayName, thisResult.name);
                                var sizeOfLabel = resultLabelTitleStyle.CalcSize(label);

                                labelText = thisResult.DisplayName;
                                if (labelRect.width < sizeOfLabel.x)
                                {
                                    var ratio = labelRect.width / sizeOfLabel.x;
                                    var oldFontSize = resultLabelTitleStyle.fontSize;
                                    resultLabelTitleStyle.fontSize = Mathf.RoundToInt((oldFontSize - 1) * ratio);
                                    GUI.Label(centeredLabelRect, label, resultLabelTitleStyle);
                                    resultLabelTitleStyle.fontSize = oldFontSize;
                                }
                                else
                                {
                                    GUI.Label(centeredLabelRect, label, resultLabelTitleStyle);
                                }


                            }



                        }
                    }
                    else
                    {
                        GUI.EndScrollView();
                        return;
                    }
                    searchResultNumber++;
                }
            }
            GUI.EndScrollView();
        }

        private float resultThumbnailMultiplier = 1;
        private void DrawResultsGrid()
        {
            if (searchLoading)
            {
                DrawResultsLoading();
                return;
            }
            else if (errorWhileSearching)
            {
                DrawErrorWhileSearching();
                return;
            }
            else if (searchReturned)
            {
                SearchResultsOptionsBar();
                DrawResults(showAnimatedOnly ? animatedResults : searchResults, resultThumbnailMultiplier);
            }
            else if (searchStarted)
            {
                DrawResultsNotFound();
            }
            else
            {
                SearchResultLandingPage();
            }
        }

        private void SearchResultLandingPage()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(EditorGUIUtility.isProSkin ? GreyAnythingGlobeLogo : BlackAnythingGlobeLogo, new GUIStyle(), GUILayout.MaxWidth(100), GUILayout.MaxHeight(100));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            DrawCustomText("Try searching for a model... eg. \"cat\", \"flower\", \"car\"  ", 14, TextAnchor.MiddleCenter);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private void SearchResultsOptionsBar()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(inputFieldStyle.margin.left);
            if (showAnimatedOnly)
            {
                DrawCustomText($"{animatedResults.Count} animated results found for \"{searchTerm}\"", 12,
                    TextAnchor.MiddleLeft);
            }
            else
            {
                DrawCustomText($"{searchResults.Count} results found for \"{searchTerm}\"", 12, TextAnchor.MiddleLeft);
            }

            //Scale slider
            var empty = new GUIStyle();
            if (GUILayout.Button(gridIcon, empty, GUILayout.ExpandWidth(false), GUILayout.Height(20)))
            {
                resultThumbnailMultiplier = 1f;
            }

            resultThumbnailMultiplier =
                GUILayout.HorizontalSlider(resultThumbnailMultiplier, 0.5f, 2.5f, GUILayout.MaxWidth(100));
            var roundedScale = (Mathf.RoundToInt(resultThumbnailMultiplier * 10)) / 10f;
            GUILayout.Label($"{roundedScale.ToString()}x", GUILayout.ExpandWidth(false), GUILayout.Width(30));
            GUILayout.Space(inputFieldStyle.margin.left);
            GUILayout.EndHorizontal();
            GUILayout.Space(inputFieldStyle.margin.bottom);
        }

        /// <summary>
        /// GUI for 
        /// </summary>
        private void DrawResultsLoading()
        {
            if (Event.current.type == EventType.Repaint)
            {
                lastRect = GUILayoutUtility.GetLastRect();
            }
            if (Event.current.type == EventType.Layout)
            {
                showFullLogo = position.height - lastRect.yMin < 300;
            }

            if (showFullLogo)
            {
                DrawStaticLogo();
            }
            else
            {
                DrawRotatingLogo();
            }
        }

        private void DrawRotatingLogo()
        {
            var thisTime = EditorApplication.timeSinceStartup;
            var spinningRect = new Rect((position.width / 2) - (255 / 2), (position.height / 2) - (255 / 2) + 120, 255, 255);
            var dt = EditorApplication.timeSinceStartup - lastEditorTime;
            var matrixBack = GUI.matrix;
            searchRingAngle += 50f * (float)dt;
            GUIUtility.RotateAroundPivot(searchRingAngle, spinningRect.center);
            GUI.DrawTexture(spinningRect, loadingRing);
            GUI.matrix = matrixBack;
            GUI.DrawTexture(spinningRect, LoadingCenter);
            lastEditorTime = thisTime;
        }

        private void DrawStaticLogo()
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var temp = new GUIStyle();
            temp.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(new GUIContent("", LoadingCenter), temp, GUILayout.MinHeight(1), GUILayout.MaxHeight(300));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label("Searching...", BuildStyle(EditorStyles.label, PoppinsStyle.Bold, 20, TextAnchor.MiddleCenter));
            GUILayout.FlexibleSpace();
            GUILayout.Space(10);
        }

        /// <summary>
        /// GUI for error while searching in Object Creator panel.
        /// </summary>
        private void DrawErrorWhileSearching()
        {
            GUILayout.FlexibleSpace();
            DrawBoldTextHeader($"Error searching for that term, please try again.", 20, TextAnchor.MiddleCenter, true);
            GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// GUI for no results after search in Object Creator panel.
        /// </summary>
        private void DrawResultsNotFound()
        {
            GUILayout.FlexibleSpace();
            DrawBoldTextHeader($"No results found for \"{searchTerm}\".", 20, TextAnchor.MiddleCenter, true);
            GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// Draw all elements in navigation toolbar.
        /// </summary>
        private void DrawNavToolbar()
        {
            GUILayout.BeginHorizontal();
            try
            {
                DrawNavButton(objectCreatorButton, 0, "Object creation window");
                DrawNavButton(voiceCreatorButton, 1, "Voice creation window");
                DrawNavButton(habitatCreatorButton, 2, "Habitat creation window");
            }
            catch { }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Custom function to draw individual navigation bar buttons. 
        /// [Object Creator][Voice Creator][Habitat Creator]
        /// </summary>
        /// <param name="texture">Texture to draw on button.</param>
        /// <param name="buttonOption">Index of button.</param>
        /// <param name="tooltip">Tooltip to display on mouseover.</param>
        private void DrawNavButton(Texture2D texture, int buttonOption, string tooltip)
        {
            guiColor = GUI.backgroundColor;
            GUIStyle buttonStyle;
            if (selectedNavButtonIndex == buttonOption)
            {
                GUI.backgroundColor = Color.grey;
                buttonStyle = activeButtonStyle;
            }
            else
            {
                buttonStyle = defaultButtonStyle;
            }
            if (GUILayout.Button(new GUIContent("", texture, tooltip), buttonStyle, GUILayout.MinWidth(1)))
            {
                HabitatMap.RefreshHabitatDictionary();
                selectedNavButtonIndex = buttonOption;
            }
            GUI.backgroundColor = guiColor;
        }

        /// <summary>
        /// GUI display for microphone button.
        /// </summary>
        /// <returns></returns>
        private bool MicButtonPanel()
        {
            var activeMicTex = activeMicIcon;
            var restingMicTex = inactiveMicIcon;
            var micActive = false;
            ToggleGUIBackgroundClear();
            GUILayout.FlexibleSpace();
            if (micButtonTex == null) micButtonTex = restingMicTex;
            DrawMicButton();
            GUILayout.FlexibleSpace();
            ToggleGUIBackgroundClear();
            if (micButtonTex == activeMicTex) micActive = true;
            return micActive;
        }

        private void DrawMicButton()
        {
            if (!GUILayout.Button(micButtonTex, micButtonStyle, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64))) return;
            //Checks if API key is present and shows login prompt if not.
            if (!CheckAPI())
            {
                ShowAPIWindow();
            }

            AnythingVoiceCreator.Instance.StartVoiceInput();
            Repaint();
        }

        /// <summary>
        /// GUI for sound-wave element in Voice Creator panel.
        /// </summary>
        /// <param name="active"></param>
        private static void DrawSoundWave(bool active)
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label(active ? activeWaveform : inactiveWaveform, centeredLabelStyle, GUILayout.MaxWidth(64));
            GUILayout.FlexibleSpace();
            Voice.GetMaxLevel();
        }

        /// <summary>
        /// GUI for speech preview element in Voice creator panel.
        /// </summary>
        /// <param name="inputString"></param>
        private void DrawSpeechPreview(string inputString)
        {
            GUILayout.Label("Output:", BuildStyle(centeredLabelStyle, PoppinsStyle.Regular, 14, TextAnchor.MiddleCenter));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            SpeechPreviewTextField(inputString);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        private void SpeechPreviewTextField(string inputString)
        {
            var fieldStyle = BuildStyle(EditorStyles.textArea, PoppinsStyle.Regular, 12, TextAnchor.UpperCenter);
            fieldStyle.wordWrap = true;
            fieldStyle.padding = new RectOffset(10, 10, 10, 10);
            fieldStyle.font = POPPINS_REGULAR;
            fieldStyle.fontSize = 16;
            GUILayout.BeginVertical();
            var calculatedSize = fieldStyle.CalcSize(new GUIContent(inputString));
            var size = Math.Min(calculatedSize.x, 300);
            GUILayout.Label(inputString, fieldStyle, GUILayout.MinWidth(10), GUILayout.MaxWidth(size));
            GUILayout.EndVertical();
        }

        /// <summary>
        /// GUI for microphone selection dropdown in VoiceCreator panel.
        /// </summary>
        private void MicSelectionDropDown()
        {
            if (TryGetMicArrayFailed(out var options)) return;

            if (options == null) return;

            if (options.Length == 0)
            {
                NoMicDetectedDropdown();
            }
            else
            {
                SelectMicDropdown(options);
                GUILayout.Space(10);
                Voice.SelectedInputMic = selectedMicIndex < options.Length ? options[selectedMicIndex] : options[0];
            }
       
            Repaint();
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }

        private static bool TryGetMicArrayFailed(out string[] options)
        {
            try
            {
                options = Voice.GetMicrophoneArray();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error getting microphone list: {e}");
                options = null;
                return true;
            }

            return false;
        }

        private void NoMicDetectedDropdown()
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label("Choose audio source:", BuildStyle(centeredLabelStyle, PoppinsStyle.Regular, 14, TextAnchor.MiddleCenter));
            GUI.enabled = false;
            selectedMicIndex = EditorGUILayout.Popup(0, new string[] { "No microphone detected." }, EditorStyles.popup, GUILayout.MinWidth(400), GUILayout.Height(40), GUILayout.ExpandWidth(false));
            GUI.enabled = true;
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        private void SelectMicDropdown(string[] options)
        {
            var micDropdownStyle = new GUIStyle(EditorStyles.popup);
            micDropdownStyle.font = POPPINS_REGULAR;
            micDropdownStyle.fontSize = 12;
            micDropdownStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label("Choose audio source:",
                BuildStyle(centeredLabelStyle, PoppinsStyle.Regular, 14, TextAnchor.MiddleCenter));
            selectedMicIndex = EditorGUILayout.Popup(selectedMicIndex, options, micDropdownStyle, GUILayout.MinWidth(400),
                GUILayout.Height(40), GUILayout.ExpandWidth(false));
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// GUI for habitat selection dropdown element in Habitat creator panel.
        /// </summary>
        private void HabitatDropDownMenu()
        {
            if (GetHabitatOptionsFailed(out var options)) return;
            selectedHabitatDropdownIndex = EditorGUILayout.Popup(selectedHabitatDropdownIndex, options, dropDownStyle, GUILayout.Height(50));
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            CreateHabitatButton();

            if (GUILayout.Button("Clear", smallButtonStyle)) selectedHabitatDropdownIndex = 0;
            populateHabitat = GUILayout.Toggle(populateHabitat, "Populate Habitat", toggleStyle);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private bool GetHabitatOptionsFailed(out string[] options)
        {
            options = new string[] { };
            try
            {
                options = AppendHabitatOptionsToDefault();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error getting habitat list: {e}");
                return true;
            }

            return false;
        }

        private void CreateHabitatButton()
        {
            if (EditorGUIUtility.isProSkin)
            {
                smallButtonStyle.normal.textColor = GREEN_COLOR;
            }
            else
            {
                smallButtonStyle.normal.textColor = Color.black;
                smallButtonStyle.hover.textColor = Color.black;
                smallButtonStyle.normal.background = ChangeBackgroundColour(GREEN_COLOR);
                smallButtonStyle.hover.background = ChangeBackgroundColour(Color.grey);
            }

            if (GUILayout.Button("Create", smallButtonStyle))
            {
                var chosenIndex = selectedHabitatDropdownIndex;
                if (!CheckAPI())
                {
                    ShowAPIWindow();
                }

                if (selectedHabitatDropdownIndex != 0)
                {
                    var habitatSelected = HabitatMap.GetHabitatAtIndex(chosenIndex - 1);
                    AnythingHabitat.Instance.MakeHabitat(habitatSelected, populateHabitat);
                }
            }

            if (EditorGUIUtility.isProSkin)
            {
                smallButtonStyle.normal.textColor = RED_COLOR;
            }
            else
            {
                smallButtonStyle.normal.textColor = Color.black;
                smallButtonStyle.hover.textColor = Color.black;
                smallButtonStyle.normal.background = ChangeBackgroundColour(RED_COLOR);
                smallButtonStyle.hover.background = ChangeBackgroundColour(Color.grey);
            }
        }
        #endregion

        #region Utility Functions
        public void AutoCreateOnFinish()
        {
            if (autoGenerateFromVoice == true)
            {
                Voice.TryCreate();
            }
        }

        private Texture2D ChangeBackgroundColour(Color colour)
        {
            Color[] texturePixels = new Color[1];

            for (int i = 0; i < texturePixels.Length; i++)
            {
                texturePixels[i] = colour;
            }
            
            Texture2D newBackgroundTexture = new Texture2D(1,1);

            newBackgroundTexture.SetPixels(texturePixels);
            newBackgroundTexture.Apply();

            return newBackgroundTexture;
        }
        #endregion

        private readonly string[] defaultOption = new string[] { "Select Habitat" };
        public string[] AppendHabitatOptionsToDefault()
        {
            var habitatList = HabitatMap.HabitatNameArray;
            var options = new string[defaultOption.Length + habitatList.Length];
            defaultOption.CopyTo(options, 0);
            habitatList.CopyTo(options, defaultOption.Length);
            return options;
        }
        public static void ToggleMicTex(bool listening)
        {
            micButtonTex = Voice.ListenForVoice ? activeMicIcon : inactiveMicIcon;
        }
    }
}


