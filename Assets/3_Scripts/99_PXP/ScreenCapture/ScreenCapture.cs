using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
namespace PxP.Tools.ScreenCapture
{
    public class ScreenCapture : EditorWindow
    {
        [SerializeField] private ImageFormat m_imageFormat = ImageFormat.PNG;
        [SerializeField] private TextureFormat m_textureFormat = TextureFormat.RGBAFloat;

        #region Camera
        private Camera m_camera = null;
        private Vector2Int m_customResolution = Vector2Int.one;
        private Resolutions m_resolutions = Resolutions.GameView;
        [SerializeField] private string m_folderName = "Screenshot";
        [SerializeField] private RenderView m_renderView = RenderView.Camera;
        #endregion

        #region Camera Linker
        static CameraLinker m_cameraLinker = null;
        #endregion

        #region Shading
        static ShadingMode m_shadingMode = null;
        #endregion

        #region Enums
        public enum ImageFormat
        {
            EXR,
            JPG,
            PNG,
            TGA
        }
        public enum RenderView
        {
            Camera,
            Scene
        }
        public enum Resolutions
        {
            GameView,
            Custom,
            _3840x2160,
            _1920x1080,
            _1950x1030,
            _420x280,
            _160x160,
        }
        #endregion

        //static m_t = null;

        //=================== Scroll Variables ======================
        private Vector2 scrollPosition = Vector2.zero;
        private bool m_debugMode = false;

        [MenuItem("PxPTools/Screen Capture")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            ScreenCapture window = (ScreenCapture)EditorWindow.GetWindow(typeof(ScreenCapture));

            #region Camera
            window.m_camera = null;
            window.m_customResolution = Vector2Int.one;
            window.m_resolutions = Resolutions.GameView;
            //m_sceneCam
            window.m_folderName = "Screenshot";
            window.m_renderView = RenderView.Camera;
            #endregion
            #region Camera Linker
            m_cameraLinker.Init();
            #endregion
            #region Shading
            m_shadingMode.Init();
            #endregion

            //m_t.Init();

            window.Show();
        }

        #region Scene Init
        //=================== Editor Scene Change ===================
        static void SceneOpenedCallback(Scene _scene, OpenSceneMode _mode)
        {
            Init();
        }

        private void OnEnable()
        {
            //Editor Scene change
            EditorSceneManager.sceneOpened += SceneOpenedCallback;
         
            //Debug mode
            EditorInspectorMode.OnInspectorModeChanged += OnInspectorModeChangedHandler;
            m_debugMode = EditorInspectorMode.GetInspectorModeSafe() == InspectorMode.Debug;

            //Camera linker
            m_cameraLinker = CreateInstance<CameraLinker>();
            m_cameraLinker.Init();
            
            //Shading mode
            m_shadingMode = CreateInstance<ShadingMode>();
            m_shadingMode.Init();
        }

        private void OnDisable()
        {
            //Editor Scene change            
            EditorSceneManager.sceneOpened -= SceneOpenedCallback;
            
            //Debug mode
            EditorInspectorMode.OnInspectorModeChanged -= OnInspectorModeChangedHandler;
            
            //Camera linker
            DestroyImmediate(m_cameraLinker);
            
            //Shading mode
            DestroyImmediate(m_shadingMode);
        }

        /// <summary>
        /// Event handler for Inspector mode changes.
        /// </summary>
        private void OnInspectorModeChangedHandler(InspectorMode newMode)
        {
            m_debugMode = (newMode == InspectorMode.Debug);
            if (m_cameraLinker != null) m_cameraLinker.DebugMode = m_debugMode;
            if (m_shadingMode != null) m_shadingMode.DebugMode = m_debugMode;
            Repaint(); // Request a repaint of this window to reflect the change
        }
        #endregion

        void OnGUI()
        {
            //=================== Scroll Logic ===================
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);
            GUILayout.BeginVertical();

            //=============== Rendering params ===================
            #region Rendering
            GUILayout.Label("Render type", EditorStyles.boldLabel);
            
            m_imageFormat = EditorUtils.RecordEnum(
                "Image Format", 
                "Changes the extension of the screenshot \n(ex: cameraViewCapture.png)",
                m_imageFormat,
                this);

            m_textureFormat = EditorUtils.RecordEnum(
                    "Texture Format",
                    "Changes the format of the texture used for the ScreenShot Render \n(ex: RGBAFloat)",
                    m_textureFormat,
                    this);

            m_renderView = EditorUtils.RecordEnum(
                    "Render View",
                    "Defines what view is used for the ScreenShot\n\n" +
                    "[Camera]\n" +
                    "Uses the defined Camera in the scene for the render\n\n" +
                    "[Scene]\n" +
                    "Uses the Scene View for the render", 
                    m_renderView,
                    this);
            #endregion

            if (m_renderView == RenderView.Camera)
            {
                //=============== Camera Capture =====================
                #region Camera Capture
                GUILayout.Space(10);
                if (GUILayout.Button(
                    new GUIContent(
                        "Capture Camera View",
                        "Captures the Camera View and saves the Shot in the Folder\n\"Assets/Screenshot/...\"\n\n" +
                        "/!\\ Will create a folder if none already exists /!\\"
                    )))
                {
                    CaptureCameraView();
                }
                //===========DEBUG============
                if (m_debugMode)
                {
                    m_folderName = EditorUtils.RecordTextField("ScreenShot Folder","", m_folderName,this);
                }


                //=============== Camera Options ===============
                m_cameraLinker.ShowGUI();
                
                
                //=============== Resolution ===================
                #region Resolution
                GUILayout.Space(10);
                GUILayout.Label("Render Options", EditorStyles.boldLabel);

                m_resolutions = EditorUtils.RecordEnum(
                        "Resolution",
                        "Changes the resolution of the ScreenShots\n\n" +
                        "[Game View]\n" +
                        "Uses the resolution defined next to \"Display N\"\n\n" +
                        "[Custom]\n" +
                        "Enable users to input a custom resolution value for X and Y",
                        m_resolutions,
                        this);

                if (m_resolutions == Resolutions.Custom)
                {
                    m_customResolution = EditorUtils.RecordVector2IntField(
                        "Custom Resolution",
                        "Defines the Custom Resolution of the Screenshot you take",
                        m_customResolution,
                        this);
                    GUILayout.Space(10);
                }
                #endregion
                
                
                //=============== Shading ======================
                m_shadingMode.ShowGUI();
                
                #endregion
            }
            else
            {
                //=================== Scene Capture ================
                #region Scene Capture
                GUILayout.Space(10);
                if (GUILayout.Button(new GUIContent(
                        "Capture Scene View",
                        "Captures the Scene View and saves the Shot in the Folder\n\"Assets/Screenshot/...\"\n\n" +
                        "/!\\ Will create a folder if none already exists /!\\"
                    )))
                {
                    //Capture Scene View
                    CaptureSceneView();
                }
                #endregion
            }

            //=================== Scroll Logic =================
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

        }

        private void Update()
        {
            m_cameraLinker.Tick();
        }

        #region Capture Methods

        /// <summary>
        /// Captures the Game View from the defined Camera
        /// </summary>
        void CaptureCameraView()
        {
            CreateScreenShotFolderIfNoneExists();

            Camera currentCamera = null;
            if (m_cameraLinker.UseCustomCamera)
            {
                if (m_cameraLinker.CustomCamera == null) m_cameraLinker.CustomCamera = Camera.main;
                currentCamera = m_cameraLinker.CustomCamera;
            }
            else
            {
                if (m_camera == null) m_camera = Camera.main;
                currentCamera = m_camera;
            }

            uint width = 0;
            uint height = 0;
            switch (m_resolutions)
            {
                case Resolutions.GameView:
                    UnityEditor.PlayModeWindow.GetRenderingResolution(out width, out height);
                    break;
                case Resolutions.Custom:
                    width = (uint)Mathf.Abs(m_customResolution.x);
                    height = (uint)Mathf.Abs(m_customResolution.y);
                    break;
                case Resolutions._3840x2160:
                    width = 3840;
                    height = 2160;
                    break;
                case Resolutions._1950x1030:
                    width = 1950;
                    height = 1030;
                    break;
                case Resolutions._1920x1080:
                    width = 1920;
                    height = 1080;
                    break;
                case Resolutions._420x280:
                    width = 420;
                    height = 280;
                    break;
                case Resolutions._160x160:
                    width = 160;
                    height = 160;
                    break;
                default:
                    Debug.Log(
                        $"The resolution used for the screenshot \"{width}x{height}\" was not set conventionally, this is not supposed to happen");
                    break;
            }

            RenderTexture renderTexture = new RenderTexture((int)width, (int)height, 24);
            currentCamera.targetTexture = renderTexture;

            Texture2D capture = new Texture2D((int)width, (int)height, m_textureFormat, false);

            currentCamera.Render();
            RenderTexture.active = renderTexture;
            capture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            capture.Apply();

            currentCamera.targetTexture = null;
            RenderTexture.active = null;

            DestroyImmediate(renderTexture);

            string fileName = $"{SceneManager.GetActiveScene().name}_{width}x{height}";
            int screenshotCount = 0;
            string formatSuffix = "";

            byte[] bytes = null;
            switch (m_imageFormat)
            {
                case ImageFormat.EXR:
                    bytes = capture.EncodeToEXR();
                    formatSuffix = ".exr";
                    screenshotCount = GetScreenShotCountByType(fileName, formatSuffix);
                    break;
                case ImageFormat.JPG:
                    bytes = capture.EncodeToJPG();
                    formatSuffix = ".jpg";
                    screenshotCount = GetScreenShotCountByType(fileName, formatSuffix);
                    break;
                case ImageFormat.PNG:
                    bytes = capture.EncodeToPNG();
                    formatSuffix = ".png";
                    screenshotCount = GetScreenShotCountByType(fileName, formatSuffix);
                    break;
                case ImageFormat.TGA:
                    bytes = capture.EncodeToTGA();
                    formatSuffix = ".tga";
                    screenshotCount = GetScreenShotCountByType(fileName, formatSuffix);
                    break;
            }

            File.WriteAllBytes(Application.dataPath + "/" + m_folderName + "/" + fileName + "_" + screenshotCount + formatSuffix, bytes);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Captures the Scene View
        /// </summary>
        void CaptureSceneView()
        {
            CreateScreenShotFolderIfNoneExists();

            SceneView sceneView = SceneView.lastActiveSceneView;

            int width = sceneView.camera.pixelWidth;
            int height = sceneView.camera.pixelHeight;

            Texture2D capture = new Texture2D(width, height, m_textureFormat, false);

            sceneView.camera.Render();

            RenderTexture.active = sceneView.camera.targetTexture;

            capture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            capture.Apply();

            string fileName = $"(SCENE){SceneManager.GetActiveScene().name}_{width}x{height}";
            int screenshotCount = 0;
            string formatSuffix = "";


            byte[] bytes = null;
            switch (m_imageFormat)
            {
                case ImageFormat.EXR:
                    bytes = capture.EncodeToEXR();
                    formatSuffix = ".exr";
                    screenshotCount = GetScreenShotCountByType(fileName, formatSuffix);
                    break;
                case ImageFormat.JPG:
                    bytes = capture.EncodeToJPG();
                    formatSuffix = ".jpg";
                    screenshotCount = GetScreenShotCountByType(fileName, formatSuffix);
                    break;
                case ImageFormat.PNG:
                    bytes = capture.EncodeToPNG();
                    formatSuffix = ".png";
                    screenshotCount = GetScreenShotCountByType(fileName, formatSuffix);
                    break;
                case ImageFormat.TGA:
                    bytes = capture.EncodeToTGA();
                    formatSuffix = ".tga";
                    screenshotCount = GetScreenShotCountByType(fileName, formatSuffix);
                    break;
            }

            File.WriteAllBytes(Application.dataPath + "/" + m_folderName + "/" + fileName + "_" + screenshotCount + formatSuffix, bytes);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Creates a ScreenShot Folder if it doesn't already exist
        /// </summary>
        void CreateScreenShotFolderIfNoneExists()
        {
            if (!Directory.Exists(Application.dataPath + "/" + m_folderName))
                AssetDatabase.CreateFolder("Assets", m_folderName);
        }

        /// <summary>
        /// Get the number of ScreenShots by name and file format
        /// </summary>
        /// <param name="fileName">Name of the screenshot</param>
        /// <param name="formatSuffix">File format (ex: .png)</param>
        /// <returns>The number of shots with the given parameters</returns>
        int GetScreenShotCountByType(string fileName, string formatSuffix)
        {
            int screenshotCount = Directory.GetFiles(Application.dataPath + "/" + m_folderName, fileName + "*" + formatSuffix, SearchOption.AllDirectories).Length;
            return screenshotCount;
        }
        
        #endregion
    }
}
#endif