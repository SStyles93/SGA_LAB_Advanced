using System;
using UnityEditor;
using UnityEngine;

namespace PxP.Tools.ScreenCapture
{
    public class ShadingMode : EditorWindow
    {
        public ShadingStyle m_shadingMode = ShadingStyle.Shaded;
        private ShadingStyle m_currentShadingMode = ShadingStyle.Shaded;
        private Material m_wireframeMaterial = null;

        //Have to be public to be found (by string) for the debug mode
        public MeshRenderer[] m_meshRenderers = null;
        public Material[][] m_savedMaterials = null;

        private bool m_debugMode = false;
        private bool m_showMaterialsFoldout = false;

        public bool DebugMode { get => m_debugMode; set => m_debugMode = value; }

        public enum ShadingStyle
        {
            Shaded,
            Wireframe,
            ShadedWireframe,
            LitWireframe
        }

        public void Init()
        {
            m_shadingMode = ShadingStyle.Shaded;
            m_currentShadingMode = ShadingStyle.Shaded;
            m_wireframeMaterial = null;
            m_meshRenderers = null;
            m_savedMaterials = null;
        }

        public void ShowGUI()
        {
            m_shadingMode = EditorUtils.RecordEnum(
                    "Shading Mode",
                    "Changes the Shading model of the Game View\n\n" +
                    "[Shaded]\n" +
                    "Uses the original materials of the scene objects\n\n" +
                    "[Wireframe]\n" +
                    "Changes all materials to Wireframe for all scene objects\n\n" +
                    "[Shaded Wireframe]\n" +
                    "Adds a Wireframe material on top of all scene objects\n\n" +
                    "[Lit Wireframe]\n" +
                    "Changes all the materials to \"Lit\" and Adds a Wireframe material on top of all scene objects",
                    m_shadingMode, this);

            if (m_shadingMode != m_currentShadingMode)
            {
                SwitchShadingMode();
            }

            //===========DEBUG============
            if (m_debugMode)
            {
                EditorGUILayout.EnumPopup("Current Shading Mode", m_currentShadingMode);
                EditorGUILayout.ObjectField("Wireframe Material", m_wireframeMaterial, typeof(Material), true);

                ScriptableObject target = this;
                SerializedObject so = new SerializedObject(target);
                SerializedProperty property = so.FindProperty("m_meshRenderers");
                EditorGUILayout.PropertyField(property, true); // True means show children
                if (m_savedMaterials != null)
                {
                    m_showMaterialsFoldout = EditorGUILayout.Foldout(m_showMaterialsFoldout, "Saved Materials");

                    if (m_showMaterialsFoldout)
                    {
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < m_savedMaterials.Length; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label($"{m_meshRenderers[i].name}:");
                            EditorGUILayout.EndHorizontal();
                            for (int j = 0; j < m_savedMaterials[i].Length; j++)
                            {
                                EditorGUILayout.ObjectField($"Material {j}", m_savedMaterials[i][j], typeof(Material), true);
                            }
                            GUILayout.Space(5);
                        }
                    }
                }
                else
                {
                    GUILayout.Label("There are currently no saved materials");
                }
            }
            GUILayout.Space(10);
        }


        #region Shading

        /// <summary>
        /// Controle Flow for the Shading Models
        /// </summary>
        void SwitchShadingMode()
        {
            if (m_wireframeMaterial == null)
                m_wireframeMaterial = GetMaterialByName("Wireframe");
            switch (m_shadingMode)
            {
                case ShadingStyle.Shaded:
                    if (m_currentShadingMode == ShadingStyle.ShadedWireframe || m_currentShadingMode == ShadingStyle.LitWireframe)
                        RemoveLastMaterialOnAllObjects();

                    //RESOTRE Objects to the shaded version
                    ShadedMode();

                    m_currentShadingMode = ShadingStyle.Shaded;
                    break;

                case ShadingStyle.Wireframe:
                    if (m_currentShadingMode == ShadingStyle.Shaded)
                        SaveMaterials();
                    if (m_currentShadingMode == ShadingStyle.ShadedWireframe || m_currentShadingMode == ShadingStyle.LitWireframe)
                        RemoveLastMaterialOnAllObjects();

                    //SET Objects materials to WIREFRAME
                    WireframeMode();

                    m_currentShadingMode = ShadingStyle.Wireframe;
                    break;

                case ShadingStyle.ShadedWireframe:
                    if (m_currentShadingMode == ShadingStyle.Shaded)
                        SaveMaterials();
                    if (m_currentShadingMode == ShadingStyle.LitWireframe)
                        //REMOVE One element of material array
                        RemoveLastMaterialOnAllObjects();


                    //ADD WIREFRAME to Shaded Objects
                    ShadedWireframeMode();

                    m_currentShadingMode = ShadingStyle.ShadedWireframe;
                    break;

                case ShadingStyle.LitWireframe:
                    if (m_currentShadingMode == ShadingStyle.Shaded)
                        SaveMaterials();

                    if (m_currentShadingMode == ShadingStyle.ShadedWireframe)
                        //REMOVE One element of material array
                        RemoveLastMaterialOnAllObjects();

                    //ADD Wireframe to Shaded Objects
                    LitWireframeMode();

                    m_currentShadingMode = ShadingStyle.LitWireframe;
                    break;
            }
        }

        /// <summary>
        /// The Original Shading Model
        /// </summary>
        void ShadedMode()
        {
            if (m_savedMaterials == null) return;

            //FIND rendered objects and exclude plane (true)
            m_meshRenderers = FindRenderedObjects(true);

            for (int i = 0; i < m_meshRenderers.Length; i++)
            {
                //INITIALIZE tmp array for mats
                Material[] tmp = new Material[m_meshRenderers[i].sharedMaterials.Length];
                for (int j = 0; j < tmp.Length; j++)
                {
                    //COPY saved materials to tmp
                    tmp[j] = m_savedMaterials[i][j];
                }

                //REASSIGN saved materials
                m_meshRenderers[i].sharedMaterials = tmp;

                //REMOVE combined mesh if it was previously added
                if (m_currentShadingMode == ShadingStyle.LitWireframe || m_currentShadingMode == ShadingStyle.ShadedWireframe)
                {
                    RemoveCombinedMesh(m_meshRenderers[i].gameObject);
                }

            }
        }

        /// <summary>
        /// The Wireframe Shading Model
        /// </summary>
        void WireframeMode()
        {
            ShadedMode();

            for (int i = 0; i < m_meshRenderers.Length; i++)
            {
                //CREATE new Array sized to fit all MeshRenderer's materials
                Material[] tmpMats = new Material[m_meshRenderers[i].sharedMaterials.Length];
                //REPLACE all materials in array by wireframe
                for (int matIndex = 0; matIndex < m_meshRenderers[i].sharedMaterials.Length; matIndex++)
                {
                    tmpMats[matIndex] = m_wireframeMaterial;
                }
                //REASSIGN materials in array
                m_meshRenderers[i].sharedMaterials = tmpMats;

            }
        }

        /// <summary>
        /// Shaded-Wireframe Shading Model
        /// </summary>
        void ShadedWireframeMode()
        {
            ShadedMode();

            for (int i = 0; i < m_meshRenderers.Length; i++)
            {
                //Check if material already has wireframe
                foreach (Material mat in m_meshRenderers[i].sharedMaterials)
                {
                    if (mat.name.Contains("Wireframe"))
                    {
                        goto endloop;
                    }
                }

                //COPY material
                Material[] matsCopy = m_meshRenderers[i].sharedMaterials;

                //RESIZE Material array
                m_meshRenderers[i].sharedMaterials = new Material[m_meshRenderers[i].sharedMaterials.Length + 1];
                Material[] tmpMats = new Material[m_meshRenderers[i].sharedMaterials.Length];

                //PLACE COPY of mats in tmpMat array
                for (int matIndex = 0; matIndex < matsCopy.Length; matIndex++)
                {
                    tmpMats[matIndex] = matsCopy[matIndex];
                }

                //ADD WIREFRAME to tmpMat array
                tmpMats[matsCopy.Length] = m_wireframeMaterial;

                //REASSIGN mats with wireframe added
                m_meshRenderers[i].sharedMaterials = tmpMats;

                //ADD combined mesh
                AddCombinedMesh(m_meshRenderers[i].gameObject);
            endloop:;
            }
        }

        /// <summary>
        /// Lit-Wireframe Shading Model
        /// </summary>
        void LitWireframeMode()
        {
            ShadedMode();

            for (int i = 0; i < m_meshRenderers.Length; i++)
            {
                //CHECK if material already has wireframe
                foreach (Material mat in m_meshRenderers[i].sharedMaterials)
                {
                    if (mat.name.Contains("Wireframe"))
                    {
                        goto endloop;
                    }
                }

                //COPY material
                Material[] matsCopy = m_meshRenderers[i].sharedMaterials;

                //RESIZE Material array
                m_meshRenderers[i].sharedMaterials = new Material[m_meshRenderers[i].sharedMaterials.Length + 1];
                Material[] tmpMats = new Material[m_meshRenderers[i].sharedMaterials.Length];
                Material LitMaterial = GetMaterialByName();

                //COPY mats to tmpMat array
                for (int matIndex = 0; matIndex < matsCopy.Length; matIndex++)
                {
                    tmpMats[matIndex] = LitMaterial;
                }

                //ADD WIREFRAME to tmpMat array
                tmpMats[matsCopy.Length] = m_wireframeMaterial;

                //REASSIGN mats with wireframe added
                m_meshRenderers[i].sharedMaterials = tmpMats;

                //ADD Combined Mesh
                AddCombinedMesh(m_meshRenderers[i].gameObject);
            endloop:;
            }
        }

        #endregion

        private void SaveMaterials()
        {
            //FIND every object WITH MeshRenderer ORDER by InstanceID
            m_meshRenderers = FindRenderedObjects(true);

            //ALLOCATE new SavedObject Array
            m_savedMaterials = new Material[m_meshRenderers.Length][];

            //FILL array
            for (int i = 0; i < m_meshRenderers.Length; i++)
            {
                //Add SavedObject with number of mats
                m_savedMaterials[i] = new Material[m_meshRenderers[i].sharedMaterials.Length];

                //Saving Materials
                for (int j = 0; j < m_meshRenderers[i].sharedMaterials.Length; j++)
                {
                    m_savedMaterials[i][j] = m_meshRenderers[i].sharedMaterials[j];
                }

            }
        }

        /// <summary>
        /// Adds a sub mesh resulting in the combination of the previous meshes
        /// </summary>
        /// <param name="gameObject">The GameOjbect on which we want to combine meshes</param>
        private void AddCombinedMesh(GameObject gameObject)
        {
            SetMeshToReadWrite(gameObject, true);
            Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            mesh.subMeshCount = mesh.subMeshCount + 1;
            mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
        }

        /// <summary>
        /// Finds all the objects containing a MeshRenderer
        /// </summary>
        /// <param name="excludePlane">Exclude the "Plane" Primitive GameObject</param>
        /// <returns>An Array of MeshRenderers</returns>
        private MeshRenderer[] FindRenderedObjects(bool excludePlane = false)
        {
            //FIND every object WITH MeshRenderer ORDER by InstanceID
            MeshRenderer[] renderedObjects = FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            //REMOVES Objects if Plane
            for (int i = 0; i < renderedObjects.Length; i++)
            {
                //============ Exclude Plane ================
                if (renderedObjects[i].name == "Plane" && excludePlane)
                {
                    //Removes Plane object
                    RemoveItemAt(ref renderedObjects, i);
                    //Decrement since array has new size (Size-1)
                    i--;
                    continue;
                }
            }
            return renderedObjects;
        }

        /// <summary>
        /// Gets a Material by name
        /// </summary>
        /// <param name="MaterialName">The name of the material</param>
        /// <returns>The material from the folders</returns>
        private Material GetMaterialByName(string MaterialName = "")
        {
            if (MaterialName == "")
            {
                return (Material)UnityEditor.AssetDatabase.
                    LoadMainAssetAtPath("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Lit.mat");
            }

            string[] guids = UnityEditor.AssetDatabase.FindAssets(MaterialName);
            string currentAssetPath = "";
            Material tmpMat = null;
            foreach (string guid in guids)
            {
                currentAssetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (!currentAssetPath.Contains(".mat")) continue;
                tmpMat = (Material)UnityEditor.AssetDatabase.LoadMainAssetAtPath(currentAssetPath);
                break;
            }
            //Fallback on default "Lit" material if mat not found
            if (tmpMat == null)
            {
                Debug.Log("Material " + MaterialName + " was not found in folder");
                return (Material)UnityEditor.AssetDatabase.
                    LoadMainAssetAtPath("Packages/com.unity.render-pipelines.universal/Runtime/Materials/Lit.mat");
            }
            else
            {
                return tmpMat;
            }
        }

        /// <summary>
        /// Gets the ModelImporter related to a mesh
        /// </summary>
        /// <param name="sharedMesh">Shared Mesh used in the mesh filter of the targetted gameobject</param>
        /// <returns>The ModelImporter of the object</returns>
        private ModelImporter GetModelImporter(Mesh sharedMesh)
        {
            if (sharedMesh == null) return null;
            ModelImporter modelImporter = null;
            string[] guids1 = UnityEditor.AssetDatabase.FindAssets(sharedMesh.name);

            foreach (string guid in guids1)
            {
                string currentAssetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (!currentAssetPath.Contains(".fbx"))
                    continue;
                else
                    modelImporter = (ModelImporter)AssetImporter.GetAtPath(currentAssetPath);
            }
            return modelImporter;
        }

        /// <summary>
        /// Removes an Item at the given index
        /// </summary>
        /// <typeparam name="T">Array T</typeparam>
        /// <param name="arr">(ref) Array Reference</param>
        /// <param name="index">index of the item to remove</param>
        private void RemoveItemAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                // moving elements downwards, to fill the gap at [index]
                arr[a] = arr[a + 1];
            }
            // finally, let's decrement Array's size by one
            Array.Resize(ref arr, arr.Length - 1);
        }

        /// <summary>
        /// Removes a sub mesh that was a result of the combination of the previous meshes
        /// </summary>
        /// <param name="gameObject">The GameOjbect on which we want to remove the combined meshe</param>
        private void RemoveCombinedMesh(GameObject gameObject)
        {
            Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            //Ensures that the mesh has atleast one submesh before removal
            if (mesh.subMeshCount > 1)
                mesh.subMeshCount = mesh.subMeshCount - 1;
            SetMeshToReadWrite(gameObject, false);
        }

        /// <summary>
        /// Removes the Last material on all object in the Scene
        /// </summary>
        private void RemoveLastMaterialOnAllObjects()
        {
            GameObject[] gameObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            for (int i = 0; i < gameObjects.Length; i++)
            {
                //============ Exclude Plane ================
                if (gameObjects[i].name == "Plane") continue;

                MeshRenderer mr = gameObjects[i].GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    Material[] tmpMats = new Material[mr.sharedMaterials.Length - 1];
                    for (int j = 0; j < tmpMats.Length; j++)
                    {
                        tmpMats[j] = mr.sharedMaterials[j];
                    }
                    mr.sharedMaterials = tmpMats;
                }
            }
        }

        /// <summary>
        /// Sets the ModelImporter's Mesh Read/Write values
        /// </summary>
        /// <param name="gameObject">The object on which the ModelImporter should be changed</param>
        /// <param name="state">State of the Mesh R/W</param>
        private void SetMeshToReadWrite(GameObject gameObject, bool state)
        {
            //Changes the ModelImporter to support mesh changes
            ModelImporter importer = GetModelImporter(gameObject.GetComponent<MeshFilter>().sharedMesh);
            if (importer != null)
                importer.isReadable = state;
        }

    }
}
