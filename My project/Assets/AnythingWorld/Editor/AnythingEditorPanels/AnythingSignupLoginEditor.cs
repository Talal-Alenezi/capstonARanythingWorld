using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using System.Text.RegularExpressions;
using AnythingWorld.Utilities;
namespace AnythingWorld.Editors
{
    /// <summary>
    /// Editor panel showing login/signup for Anything API keys, 
    /// under Anything World/Generate API
    /// </summary>
    public class AnythingSignupLoginEditor : AnythingEditor
    {
        #region Signup Params
        private string signupPass = "";
        private string signupPassCheck = "";
        private string signupEmail = "";
        private string signupName = "";
        private bool signupTerms = false;
        #endregion

        #region Login Params
        private string loginPass = "";
        private string loginEmail = "";
        #endregion


        private GUIStyle detailTextLink;
        private GUIStyle DetailTextLink
        {
            get
            {
                detailTextLink = new GUIStyle(EditorStyles.label);
                detailTextLink.fontSize = 12;
                detailTextLink.alignment = TextAnchor.UpperLeft;
                detailTextLink.font = POPPINS_REGULAR;
                detailTextLink.padding = submitButtonStyle.padding;
                detailTextLink.hover.textColor = GREEN_COLOR;
                if (detailTextLink == null)
                {

                }
                return detailTextLink;
            }
        }

        private enum APIWindowPages
        {
            Landing,
            Signup,
            Login,
            SignupLanding,
            LoginLanding,
            SignupHanging,
            LoginHanging
        }
        static APIWindowPages currentWindow;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            AnythingCreator.openAPIWindowDelegate = ShowWindow;
            currentWindow = APIWindowPages.Login;

        }
        [MenuItem("Anything World/Log In or Sign Up", false, 22)]
        static void OpenFromToolbar()
        {
            currentWindow = APIWindowPages.Login;
            ShowWindow();

        }

        public static void OpenFromScript()
        {
            currentWindow = APIWindowPages.Landing;
            ShowWindow();
        }

        public static void ShowWindow()
        {
            AnythingSignupLoginEditor window = ScriptableObject.CreateInstance(typeof(AnythingSignupLoginEditor)) as AnythingSignupLoginEditor;
            GUIContent windowContent = new GUIContent("AnythingWorld Account Login");
            window.titleContent = windowContent;
            window.minSize = new Vector2(200, 200);
            window.position = new Rect(0, 0, 420, 520);
            window.ShowUtility();
        }
        public void OnGUI()
        {
            InitializeResources();
            switch (currentWindow)
            {
                case 0:
                    LandingWindow();
                    break;
                case APIWindowPages.Signup:
                    DrawNavToolbar();
                    SignUpWindow();
                    break;
                case APIWindowPages.Login:
                    DrawNavToolbar();
                    LoginWindow();

                    break;
                case APIWindowPages.LoginLanding:
                    LoginComplete();
                    break;
                case APIWindowPages.SignupLanding:
                    SignupComplete();
                    break;
                case APIWindowPages.LoginHanging:
                    LoginHanging();
                    break;
                case APIWindowPages.SignupHanging:
                    SignupHanging();
                    break;
                default:
                    LandingWindow();
                    break;
            }
        }

        private void LandingWindow()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.FlexibleSpace();
            DrawBoldTextHeader("No API Found", 40, new RectOffset(10, 10, 10, 10));
            DrawCustomText("You need to provide an API key for the AnythingWorld platform before you can start creating models.", 12, true, TextAnchor.MiddleCenter);
            if (GUILayout.Button("Login or Signup", submitButtonStyle))
            {
#if UNITY_EDITOR
                currentWindow = APIWindowPages.Login;
#endif
            }
            if (GUILayout.Button("Manually enter API Key", submitButtonStyle))
            {
                AnythingSettingsPanelEditor.ShowWindow();
                AnythingSignupLoginEditor window = GetWindow<AnythingSignupLoginEditor>();
                window.Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            GUILayout.EndVertical();
        }
        private void DrawNavToolbar()
        {
            GUILayout.BeginHorizontal();
            try
            {
                DrawNavButton(loginButton, APIWindowPages.Login, "Login");
                DrawNavButton(signupButton, APIWindowPages.Signup, "Sign Up");
            }
            catch
            {

            }
            GUILayout.EndHorizontal();
        }
        private void DrawNavButton(Texture2D texture, APIWindowPages page, string tooltip)
        {
            guiColor = GUI.backgroundColor;
            GUIStyle buttonStyle;
            if (currentWindow == page)
            {
                GUI.backgroundColor = Color.grey;
                buttonStyle = new GUIStyle(activeButtonStyle);

            }
            else
            {
                buttonStyle = new GUIStyle(defaultButtonStyle);

            }
            buttonStyle.fixedHeight = 0;
            if (GUILayout.Button(new GUIContent("", texture, tooltip), buttonStyle, GUILayout.MinWidth(1), GUILayout.MaxHeight(40)))
            {
                currentWindow = page;
            }
            GUI.backgroundColor = guiColor;
        }


        private enum SignupError
        {
            None,
            InvalidPassword,
            PasswordLength,
            PasswordMatch,
            InvalidEmail,
            RegisteredEmail,
            EmptyField,
            EmptyPassword,
            EmptyEmail,
            ServerError,
            Terms
        }
        private enum LoginError
        {
            None,
            InvalidFormat,
            ServerError,
            EmptyField,
            UnregisteredEmail,
            //WrongPassword,
            //InvalidEmail
        }
        SignupError signupError;
        Vector2 submitWindowScrollPosition;
        private void SignUpWindow()
        {
            submitWindowScrollPosition = GUILayout.BeginScrollView(submitWindowScrollPosition);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            int spacer = 5;
            int bigspacer = 10;
            int textSize = 14;
            DrawUILine(Color.gray);
            DrawBoldTextHeader("Sign Up For API", 16);
            GUIStyle fieldStyle = new GUIStyle(EditorStyles.textField);
            fieldStyle.font = POPPINS_REGULAR;
            fieldStyle.fixedHeight = 20;

            DrawCustomText("Enter name: ", textSize);
            GUILayout.Space(spacer);
            signupName = GUILayout.TextField(signupName, fieldStyle);
            GUILayout.Space(bigspacer);

            DrawCustomText("Enter email: ", textSize);
            GUILayout.Space(spacer);
            signupEmail = GUILayout.TextField(signupEmail, fieldStyle);
            if (signupError == SignupError.InvalidEmail)
            {
                DrawCustomText("Invalid Email.", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            else if (signupError == SignupError.RegisteredEmail)
            {
                DrawCustomText("Email already registered.", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            else
            {
                DrawCustomText(".", 12, Color.clear);
            }
            GUILayout.Space(bigspacer);
            DrawCustomText("Type password: ", textSize);
            GUILayout.Space(spacer);
            signupPass = GUILayout.PasswordField(signupPass, "*"[0], 25, fieldStyle);
            GUILayout.Space(bigspacer);
            DrawCustomText("Retype password: ", textSize);
            GUILayout.Space(spacer);
            signupPassCheck = GUILayout.PasswordField(signupPassCheck, "*"[0], 25, fieldStyle);
            if (signupPass.Length > 0 && signupPassCheck.Length > 0 && signupPass != signupPassCheck)
            {
                DrawCustomText("Passwords do not match.", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            else if (signupPass.Length != signupPassCheck.Length)
            {
                DrawCustomText("Passwords do not match.", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            else if (signupError == SignupError.InvalidPassword)
            {
                DrawCustomText("Password must contain letters and numbers", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            else if (signupError == SignupError.EmptyField)
            {
                DrawCustomText("All fields must be filled.", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            else if (signupError == SignupError.Terms)
            {
                DrawCustomText("Please agree to terms and conditions.", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            else if (signupError == SignupError.ServerError)
            {
                DrawCustomText("Server connection problem, please try again.", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            else
            {
                DrawCustomText(".", 12, Color.clear);
            }
            GUILayout.Space(bigspacer);
            GUILayout.BeginHorizontal();
            GUIStyle temp = new GUIStyle(EditorStyles.label);
            temp.fontSize = 12;
            temp.alignment = TextAnchor.UpperLeft;
            temp.font = POPPINS_REGULAR;
            temp.padding.right = 0;
            GUILayout.FlexibleSpace();
            GUILayout.Label("I have read and agree with the terms in the", temp, GUILayout.ExpandWidth(false));
            temp.padding.left = 0;
            Color originalColor = temp.normal.textColor;
            temp.normal.textColor = GREEN_COLOR;
            if (GUILayout.Button(" user agreement", temp, GUILayout.ExpandWidth(false)))
            {
                Application.OpenURL("https://anything.world/terms-of-use");
            }
            temp.normal.textColor = originalColor;
            GUILayout.Label(".", temp, GUILayout.ExpandWidth(false));
            GUILayout.Space(spacer);
            signupTerms = GUILayout.Toggle(signupTerms, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(bigspacer);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = !submitting;
            if (GUILayout.Button("Submit", submitButtonStyle, GUILayout.Width(100)))
            {
#if UNITY_EDITOR
                EditorCoroutineUtility.StartCoroutine(SubmitSignup(), this);
#endif
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(bigspacer);
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

        }
        private Vector2 loginWindowScrollPosition;
        private void LoginWindow()
        {
            loginWindowScrollPosition = GUILayout.BeginScrollView(loginWindowScrollPosition);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            int spacer = 5;
            int bigspacer = 10;
            int textSize = 14;
            DrawUILine(Color.gray);
            DrawBoldTextHeader("Login", 16);

            GUIStyle fieldStyle = new GUIStyle(EditorStyles.textField);
            fieldStyle.font = POPPINS_REGULAR;
            fieldStyle.fixedHeight = 20;
            DrawCustomText("Email ", textSize);
            GUILayout.Space(spacer);
            loginEmail = GUILayout.TextField(loginEmail, fieldStyle);

            if (loginError == LoginError.UnregisteredEmail)
            {
                DrawCustomText("No account with this email found.", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            GUILayout.Space(bigspacer);
            DrawCustomText("Password ", textSize);
            GUILayout.Space(spacer);
            loginPass = GUILayout.PasswordField(loginPass, "*"[0], 25, fieldStyle);
            if (loginError == LoginError.InvalidFormat)
            {
                DrawCustomText("Invalid password or email.", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            else if (loginError == LoginError.EmptyField)
            {
                DrawCustomText("All fields must be filled.", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            else if (loginError == LoginError.ServerError)
            {
                DrawCustomText("Server connection problem, please try again.", 12, new Color(0.8f, 0.3f, 0.3f));
            }
            GUILayout.Space(bigspacer);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            //submit button and submit on enter key
            if (GUILayout.Button("Submit", submitButtonStyle, GUILayout.Width(100)))
            {
#if UNITY_EDITOR
                EditorCoroutineUtility.StartCoroutine(SubmitLogin(), this);
#endif
            }
            if (Event.current.keyCode == KeyCode.Return)
            {
#if UNITY_EDITOR
                EditorCoroutineUtility.StartCoroutine(SubmitLogin(), this);
#endif
            }

            if (GUILayout.Button("Forgot password?", DetailTextLink))
            {
                Application.OpenURL("https://get.anything.world/reset");
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(bigspacer);
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }
        private void LoginComplete()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            DrawBoldTextHeader("Login Success!", 40);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply and Close", submitButtonStyle))
            {
#if UNITY_EDITOR
                ApplyLoginResponse();
#endif
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        private void SignupComplete()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            DrawBoldTextHeader("Succesful signup!", 40);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Login and Apply API Key", submitButtonStyle))
            {
#if UNITY_EDITOR
                ApplyLoginResponse();
#endif
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private void LoginHanging()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            DrawBoldTextHeader("Logging in...", 40);
            DrawBoldTextHeader("Please wait.", 20);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        private void SignupHanging()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            DrawBoldTextHeader("Signing Up...", 40);
            DrawBoldTextHeader("Please wait.", 20);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        bool submitting = false;
        private IEnumerator SubmitSignup()
        {

            currentWindow = APIWindowPages.SignupHanging;

            signupError = SignupError.None;

            try
            {
                if (signupEmail.Length == 0 || signupPass.Length == 0 || signupPassCheck.Length == 0 || signupName.Length == 0)
                {
                    //If a field is found to empty, set error and stop submission. return to signup
                    signupError = SignupError.EmptyField;
                    currentWindow = APIWindowPages.Signup;
                    yield break;
                }
                else if (signupTerms == false)
                {
                    //If terms not accepted flag error and return to form. return to signup
                    signupError = SignupError.Terms;
                    currentWindow = APIWindowPages.Signup;
                    yield break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(new System.Exception("Error initializing signupErrors, returning", e));
                currentWindow = APIWindowPages.Signup;
                yield break;

            }

            UnityWebRequest www = null;
            try
            {
                WWWForm form = new WWWForm();
                form.AddField("email", signupEmail);
                form.AddField("password", signupPass);
                form.AddField("passwordCheck", signupPassCheck);
                form.AddField("terms", "true");
                form.AddField("tier", "individual");
                form.AddField("fullName", signupName);
                www = UnityWebRequest.Post("https://subscription-portal-backend.herokuapp.com/users/register", form);
                www.timeout = 2;
                submitting = true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(new System.Exception("Error initializing signup form, returning to signup window.", e));
                currentWindow = APIWindowPages.Signup;
                yield break;
            }

            yield return www.SendWebRequest();

            try
            {
                if (AnythingWorld.Utilities.CheckWebRequest.IsError(www))
                {
                    currentWindow = APIWindowPages.Signup;
                    //Debug.Log(www.downloadHandler.text);
                    var errorResponse = JsonUtility.FromJson<LoginErrorResponse>(www.downloadHandler.text);
                    ParseSignupError(errorResponse);
                    submitting = false;
                    yield break;
                }
                else
                {
                    submitting = false;
                    ParseSignupResponse(www.downloadHandler.text);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(new System.Exception("Error parsing signup response, returning to signup.", e));
                currentWindow = APIWindowPages.Signup;
                yield break;
            }
        }


        private IEnumerator SubmitLogin()
        {
            currentWindow = APIWindowPages.LoginHanging;




            loginError = LoginError.None;

            try
            {
                if (loginEmail.Length == 0 || loginPass.Length == 0)
                {
                    //If a field is found to empty, set error and stop submission. return to signup
                    loginError = LoginError.EmptyField;
                    currentWindow = APIWindowPages.Login;
                    yield break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(new System.Exception("Error initializing signupErrors, returning", e));
                currentWindow = APIWindowPages.Signup;
                yield break;

            }







            WWWForm form = new WWWForm();
            form.AddField("email", loginEmail);
            form.AddField("password", loginPass);

            UnityWebRequest www = UnityWebRequest.Post("https://subscription-portal-backend.herokuapp.com/users/login", form);
            www.timeout = 0;
            yield return www.SendWebRequest();

            if (CheckWebRequest.IsError(www))
            {

                var errorResponse = JsonUtility.FromJson<LoginErrorResponse>(www.downloadHandler.text);
                ParseLoginError(errorResponse);
                if (AnythingSettings.DebugEnabled) Debug.Log($"Error logging into Anything World: {www.downloadHandler.text}");
                currentWindow = APIWindowPages.Login;
            }
            else
            {
                try
                {
                    ParseLoginResponse(www.downloadHandler.text);
                }
                catch { }

                currentWindow = APIWindowPages.LoginLanding;
            }
            yield return null;
        }
        private void ParseSignupError(LoginErrorResponse error)
        {
            if (AnythingSettings.DebugEnabled) Debug.Log($"Error code {error.code}: {error.msg}");

            switch (error.code.ToLower())
            {
                case "invalid format":
                    signupError = SignupError.InvalidPassword;
                    break;
                case "password mismatch":
                    signupError = SignupError.PasswordMatch;
                    break;
                case "":
                    signupError = SignupError.PasswordLength;
                    break;
                case "unregistered email":
                    signupError = SignupError.InvalidEmail;
                    break;
                case "already registered":
                    signupError = SignupError.RegisteredEmail;
                    break;
                case "not all fields have been entered":
                    signupError = SignupError.EmptyField;
                    break;
                case "server error":
                    signupError = SignupError.ServerError;
                    break;
                case "terms of use":
                    signupError = SignupError.Terms;
                    break;
                default:
                    Debug.Log("Error message not handled");
                    Debug.Log(error);
                    break;
            }
        }

        private LoginError loginError;

        private void ParseLoginError(LoginErrorResponse error)
        {

            if (AnythingSettings.DebugEnabled) Debug.Log($"Error code {error.code}: {error.msg}");
            loginError = LoginError.None;


            switch (error.code.ToLower())
            {
                case "invalid format":
                    loginError = LoginError.InvalidFormat;
                    break;
                case "unregistered email":
                    loginError = LoginError.InvalidFormat;
                    break;
                case "server error":
                    loginError = LoginError.ServerError;
                    break;
                default:
                    Debug.Log($"Not handled - '{error.code} : {error.msg}'");
                    Debug.Log(error);
                    break;
            }
        }

        private void ParseSignupResponse(string text)
        {
            string cleanedText = Regex.Replace(text, @"[[\]]", "");
            string[] arr = cleanedText.Split(',');
            apiKey = (arr[5].ToString().Split(':'))[1].Trim('\"');
            fetchedEmail = (arr[1].ToString().Split(':')[1].Trim('\"'));
            currentWindow = APIWindowPages.SignupLanding;
        }

        private string fetchedEmail = "";
        private string apiKey = "";
        private void ParseLoginResponse(string text)
        {

            string cleanedText = Regex.Replace(text, @"[[\]]", "");
            string[] arr = cleanedText.Split(',');
            apiKey = (arr[3].ToString().Split(':'))[1].Trim('\"');
            fetchedEmail = (arr[5].ToString().Split(':')[1].Trim('\"'));
        }
        private void ApplyLoginResponse()
        {
            AnythingSettings.Instance.apiKey = apiKey;
            AnythingSettings.Instance.email = fetchedEmail;
            AnythingSignupLoginEditor window = GetWindow<AnythingSignupLoginEditor>();
            Undo.RecordObject(AnythingSettings.Instance, "Added API Key and Email to AnythingSettings");
            EditorUtility.SetDirty(AnythingSettings.Instance);
            window.Close();
        }

        protected struct LoginResponse
        {
            public string user;

        }
        public struct LoginUser
        {
            public string id;
            public string fullName;
            public string apiKey;
            public string tier;
            public string email;
            public string stripeCustomer;
        }
    }
}



