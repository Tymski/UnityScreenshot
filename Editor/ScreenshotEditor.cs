using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tymski
{
    public class Screenshot : EditorWindow
    {
        private const string EDITOR_PREF_KEY = "Tymski.Screenshot";
        private const string MENU_PATH = "Tools/Tymski/Screenshots/";
        [SerializeField, HideInInspector] private static string directory = "Screenshots/";
        private static string latestScreenshotPath = "";
        private bool initDone = false;
        private GUIStyle BigText;
        private GUIStyle RichText;
        private GUIStyle BigButton;

        void InitStyles()
        {
            initDone = true;

            BigText = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };

            RichText = new GUIStyle(GUI.skin.label)
            {
                richText = true
            };

            BigButton = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 35,
                fontSize = 14
            };
        }

        private void OnGUI()
        {
            if (!initDone)
            {
                InitStyles();
            }

            GUILayout.Label(new GUIContent("Game Screenshots", EditorGUIUtility.IconContent("unityeditor.gameview.png").image), BigText);
            if (GUILayout.Button("Take a screenshot", BigButton))
            {
                TakeScreenshot();
            }
            GUILayout.Label($"Resolution: <b>{GetResolution()}</b>", RichText);
            GUILayout.Label($"Directory: <b>{directory}</b>", RichText);
            if (GUILayout.Button("Reveal in Explorer"))
            {
                RevealInExplorer();
            }
            if (GUILayout.Button("Change Directory"))
            {
                ChangeDirectory();
            }
        }

        [MenuItem(MENU_PATH + "Open Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(Screenshot));
        }

        [MenuItem(MENU_PATH + "Reveal in Explorer")]
        private static void RevealInExplorer()
        {
            if (File.Exists(latestScreenshotPath))
            {
                EditorUtility.RevealInFinder(latestScreenshotPath);
                return;
            }
            Directory.CreateDirectory(directory);
            EditorUtility.RevealInFinder(directory);
        }

        [MenuItem(MENU_PATH + "Take a Screenshot")]
        private static void TakeScreenshot()
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            var currentTime = System.DateTime.Now.ToString("yyyy-MM-dd HH'-'mm'-'ss");
            var filename = currentTime + ".png";
            var path = directory + filename;
            ScreenCapture.CaptureScreenshot(path);
            latestScreenshotPath = path;
            Debug.Log($"Screenshot <b>{filename}</b> saved in <b>{directory}</b> with resolution <b>{GetResolution()}</b>");

        }

        [MenuItem(MENU_PATH + "Change save directory")]
        private static void ChangeDirectory()
        {
            directory = EditorUtility.OpenFolderPanel("Save Screenshots folder", directory, "") + "/";
            Debug.Log($"New screenshots will be saved in: {directory}");
        }

        private static string GetResolution()
        {
            Vector2 size = UnityEditor.Handles.GetMainGameViewSize();
            Vector2Int sizeInt = new Vector2Int((int)size.x, (int)size.y);
            return $"{sizeInt.x}x{sizeInt.y}";
        }

        void OnEnable()
        {
            var data = EditorPrefs.GetString(EDITOR_PREF_KEY, JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);
        }

        void OnDisable()
        {
            var data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString(EDITOR_PREF_KEY, data);
        }

    }
}