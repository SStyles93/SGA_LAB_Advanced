using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace PxP.Tools.ScreenCapture
{
    public class CameraLinker : EditorWindow
    {
        private Camera m_camera = null;
        private bool m_linkCameraToSceneView = false;
        private SceneView m_sceneCam = null;
        private Camera m_customCamera = null;
        private bool m_useCustomCamera = false;

        private bool m_debugMode = false;

        public Camera CustomCamera { get => m_customCamera; set => m_customCamera = value; }
        public bool UseCustomCamera { get => m_useCustomCamera; }
        public bool DebugMode { get => m_debugMode; set => m_debugMode = value; }

        public void Init()
        {
            m_linkCameraToSceneView = false;
            m_sceneCam = null;
            m_customCamera = null;
            m_useCustomCamera = false;
        }

        public void ShowGUI()
        {
            #region Camera Linker
            GUILayout.Space(10);
            GUILayout.Label("Camera Options", EditorStyles.boldLabel);
            if (m_linkCameraToSceneView)
                GUI.color = Color.yellow;
            if (!m_useCustomCamera)
            {
                m_linkCameraToSceneView = EditorUtils.RecordToggle(
                     "Link Camera",
                     "Links the Camera (Main Camera) to the movements done in Scene",
                     m_linkCameraToSceneView,
                     this);

                m_useCustomCamera = EditorUtils.RecordToggle(
                    "Custom Camera",
                    "Bool used to define if a custom camera is used or not\n\n" +
                    "[Disabled]\n" +
                    "Uses the Camera in the scene defined as \"Main Camera\"\n\n" +
                    "[Enabled]\n" +
                    "Gives the user the possibility define a Camera used for the ScreenShots",
                    m_useCustomCamera,
                    this);
            }
            if (m_useCustomCamera)
            {
                m_linkCameraToSceneView = EditorUtils.RecordToggle(
                     "Link Camera",
                     "Links the Camera defined in \"Camera\" parameter to the movements done in Scene",
                     m_linkCameraToSceneView,
                     this);

                m_useCustomCamera = EditorUtils.RecordToggle(
                    "Custom Camera",
                    "Bool used to define if a custom camera is used or not",
                    m_useCustomCamera,
                    this);

                m_customCamera = EditorUtils.RecordObjectField<Camera>(
                    "Camera",
                    "The camera the user wants to use for the ScreenShot\n\n" +
                    "If \"None (Camera)\" is selected the script will fallback to the Main Camera",
                    m_customCamera, true, this);
            }
            GUI.color = Color.white;
            //===========DEBUG============
            if (m_debugMode)
            {
                EditorGUILayout.ObjectField("Main Camera", m_camera, typeof(Camera), true);
                EditorGUILayout.ObjectField("Custom Camera", m_customCamera, typeof(Camera), true);
                EditorGUILayout.ObjectField("Scene View", m_sceneCam, typeof(SceneView), true);
            }
            #endregion
        }

        public void Tick()
        {
            //=============== Camera Linker Logic ===================
            if (m_linkCameraToSceneView)
            {
                if (m_camera == null)
                    m_camera = Camera.main;

                if (focusedWindow.titleContent.text == "Inspector")
                {
                    if (m_useCustomCamera)
                        if (m_customCamera != null)
                            SceneView.lastActiveSceneView.AlignViewToObject(m_customCamera.transform);
                        else
                            SceneView.lastActiveSceneView.AlignViewToObject(m_camera.transform);
                }
                else
                {
                    if (m_useCustomCamera)
                        LinkCameraToSceneView(m_customCamera);
                    else
                        LinkCameraToSceneView();
                }
            }
        }

        /// <summary>
        /// Links the movements of the Scene View to the Game View Camera
        /// </summary>
        void LinkCameraToSceneView(Camera camera = null)
        {
            m_sceneCam = SceneView.lastActiveSceneView;
            if (camera == null)
            {
                m_camera.transform.position = m_sceneCam.camera.transform.position;
                m_camera.transform.rotation = m_sceneCam.camera.transform.rotation;
            }
            else
            {
                camera.transform.position = m_sceneCam.camera.transform.position;
                camera.transform.rotation = m_sceneCam.camera.transform.rotation;
            }

        }

    }
}
