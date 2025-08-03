using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ReplacementScript : MonoBehaviour
{
    [SerializeField] private GameObject[] m_placedObjects = new GameObject[0];
    public GameObject[] PlacedObjects { get => m_placedObjects; }

    public void GetPlacedObjects()
    {
        int childCount = transform.childCount;
        m_placedObjects = new GameObject[childCount];
        for (int i = 0; i < childCount; i++)
        {
            m_placedObjects[i] = transform.GetChild(i).gameObject;
        }
    }

    public void ReplaceObjectsByPrefab()
    {
        foreach (GameObject go in m_placedObjects)
        {
            if (UnityEditor.PrefabUtility.IsPartOfNonAssetPrefabInstance(go))
            {
                UnityEditor.PrefabUtility.UnpackPrefabInstance(go, UnityEditor.PrefabUnpackMode.OutermostRoot, UnityEditor.InteractionMode.AutomatedAction);
            }

            string gameObjectName = "";
            int lastCharIndex = go.name.LastIndexOf("_");
            if(lastCharIndex != -1)
            {
                gameObjectName = go.name[..lastCharIndex];
            }

            if (gameObjectName == "") return;

            string[] guids1 = UnityEditor.AssetDatabase.FindAssets(gameObjectName);

            foreach (string guid in guids1)
            {
                string currentAssetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (currentAssetPath.Contains(".fbx")) continue;

                var asset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(currentAssetPath);
                UnityEditor.ConvertToPrefabInstanceSettings settings = new UnityEditor.ConvertToPrefabInstanceSettings();
                UnityEditor.PrefabUtility.ConvertToPrefabInstance(go, (GameObject)asset, settings, UnityEditor.InteractionMode.UserAction);
                break;
            }
        }
    }

    public void LinkToMesh()
    {
        GetCurrentScene();

        foreach (GameObject go in m_placedObjects)
        {
            string gameObjectName = "";
            int lastCharIndex = go.name.LastIndexOf("_");
            if (lastCharIndex != -1)
            {
                gameObjectName = go.name[..lastCharIndex];
            }

            if (gameObjectName == "") return;

            string[] guids1 = UnityEditor.AssetDatabase.FindAssets(gameObjectName);

            Scene currentScene = GetCurrentScene();
            string[] projectPath = currentScene.path.Split('/');

            foreach (string guid in guids1)
            {
                //Check for FBX 
                string currentAssetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (!currentAssetPath.Contains(".fbx")) continue;
                
                //Only FBX -> Check for asset-project path correspondance
                string[] assetPath = currentAssetPath.Split('/');
                string assetFirstPart = assetPath[0] + '/' + assetPath[1];
                string projectFirstPart = projectPath[0] + '/' + projectPath[1];
                if (assetFirstPart != projectFirstPart) continue;

                GameObject fbxObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(currentAssetPath);
                MeshFilter[] meshesFilters = fbxObject.gameObject.GetComponentsInChildren<MeshFilter>();

                for (int i = 0; i < go.GetComponentsInChildren<MeshFilter>().Length; i++)
                {
                    if (go.transform.childCount == 0) go.transform.GetComponent<MeshFilter>().sharedMesh = meshesFilters[i].sharedMesh;
                    else go.transform.GetChild(i).GetComponent<MeshFilter>().sharedMesh = meshesFilters[i].sharedMesh;
                }
            }
        }
    }

    public void ReplaceObjectByMesh()
    {
        GetCurrentScene();

        foreach (GameObject go in m_placedObjects)
        {
            string gameObjectName = "";
            int lastCharIndex = go.name.LastIndexOf("_");
            if (lastCharIndex != -1)
            {
                gameObjectName = go.name[..lastCharIndex];
            }

            if (gameObjectName == "") return;

            string[] guids1 = UnityEditor.AssetDatabase.FindAssets(gameObjectName);

            Scene currentScene = GetCurrentScene();
            string[] projectPath = currentScene.path.Split('/');

            foreach (string guid in guids1)
            {
                //Check for FBX 
                string currentAssetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (!currentAssetPath.Contains(".fbx")) continue;

                //Only FBX -> Check for asset-project path correspondance
                string[] assetPath = currentAssetPath.Split('/');
                string assetFirstPart = assetPath[0] + '/' + assetPath[1];
                string projectFirstPart = projectPath[0] + '/' + projectPath[1];
                if (assetFirstPart != projectFirstPart) continue;
                GameObject fbxObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(currentAssetPath);
                
                //Data of object to be replaced
                Transform objTransform = go.transform;
                string objName = go.name;
                MeshRenderer[] objRenderers = new MeshRenderer[1];
                Collider[] objCollider = new Collider[1];
                int childCount = go.transform.childCount;
                switch (childCount)
                {
                    case 0:
                        objRenderers[0] = go.GetComponent<MeshRenderer>();
                        objCollider[0] = go.GetComponent<Collider>();
                        break;
                    case > 0:
                        objRenderers = new MeshRenderer[childCount];
                        objCollider = new Collider[childCount];
                        for (int i = 0; i < childCount; i++)
                        {
                            objRenderers[i] = go.transform.GetChild(i).GetComponent<MeshRenderer>();
                            objCollider[i] = go.transform.GetChild(i).GetComponent<Collider>();
                        }
                        break;
                }
                        
                //New object to replace the previous one
                var newObj = Instantiate(fbxObject, this.transform);
                newObj.transform.SetLocalPositionAndRotation(go.transform.position, go.transform.rotation);
                newObj.name = objName;
                switch (go.transform.childCount)
                {
                    case 0:
                        if (objCollider != null)
                        {
                            newObj.gameObject.GetComponent<MeshRenderer>().sharedMaterial = objRenderers[0].sharedMaterial;
                            Component newObjCollider = newObj.AddComponent(objCollider[0].GetType());
                            newObjCollider = objCollider[0];
                        }
                        break;
                    case > 0:
                        for (int i = 0; i < go.transform.childCount; i++)
                        {
                            newObj.transform.GetChild(i).GetComponent<MeshRenderer>().sharedMaterial = objRenderers[i].sharedMaterial;
                            Component newObjCollider = newObj.transform.GetChild(i).gameObject.AddComponent(objCollider[i].GetType());
                            newObjCollider = objCollider[i];
                        }
                        break;
                }
                DestroyImmediate(go);
            }
            m_placedObjects = new GameObject[0];
        }
    }
    private Scene GetCurrentScene()
    {
       return SceneManager.GetActiveScene();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(ReplacementScript))]
public class ReplacementScriptCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ReplacementScript _target = (ReplacementScript)target;

        GUILayout.Space(20);

        if (GUILayout.Button("Get Objects"))
        {
            _target.GetPlacedObjects();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Replace [" + _target.PlacedObjects.Length + "] objects with Prefabs"))
        {
            if (EditorUtility.DisplayDialog("REPLACE OBJECTS ?",
                "Click on \"Replace Objects\" if you wish to replace the [" + _target.PlacedObjects.Length + "] objects contained in the \"placedObjects\" list by thier prefabs",
                "Replace Objects", "Cancel"))
            {
                _target.ReplaceObjectsByPrefab();
            }
        }

        if (GUILayout.Button("Relink [" + _target.PlacedObjects.Length + "] Objects Meshes"))
        {
            if (EditorUtility.DisplayDialog("RELINK OBJECTS' MESHES ?",
                "Click on \"Relink Objects\" if you wish to relink the [" + _target.PlacedObjects.Length + "] objects' meshed to the ones contained in the current project",
                "Relink Objects", "Cancel"))
            {
                _target.LinkToMesh();
            }
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Replace [" + _target.PlacedObjects.Length + "] Objects by Meshes"))
        {
            if (EditorUtility.DisplayDialog("REPLACE OBJECTS BY MESHES ?",
                "Click on \"Replace\" if you wish to replace the [" + _target.PlacedObjects.Length + "] objects by their mesh vesion",
                "Replace", "Cancel"))
            {
                _target.ReplaceObjectByMesh();
            }
        }
    }
}
#endif
