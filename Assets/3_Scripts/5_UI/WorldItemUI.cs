using UnityEngine;
using TMPro;

public class WorldItemUI : MonoBehaviour
{
    [SerializeField] private WorldItem worldItem;
    [SerializeField] private TextMeshPro textMesh;

    private void Awake()
    {
        if (textMesh == null && TryGetComponent<TextMeshPro>(out var textMeshPro))
            this.textMesh = textMeshPro;
        // null-coalescing assignment operator (??)
        // It only executes the right side if textMesh is null.
        worldItem ??= GetComponentInParent<WorldItem>();
    }

    private void Start()
    {
        textMesh.enabled = true;
        textMesh.text = worldItem.name;
    }
}
