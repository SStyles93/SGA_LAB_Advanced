using UnityEngine;
using TMPro;

public class WorldItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;

    private void OnEnable()
    {
        WorldItem.OnMouseOverObject += ShowName;
    }

    private void OnDisable()
    {
        WorldItem.OnMouseOverObject -= ShowName;        
    }

    private void Awake()
    {
        if(textMesh == null)
        if(TryGetComponent<TextMeshPro>(out var textMeshPro))
        {
            this.textMesh = textMeshPro;
        }
    }

    private void ShowName(string name, bool state)
    {
        textMesh.text = name;
        textMesh.enabled = state;
    }
}
