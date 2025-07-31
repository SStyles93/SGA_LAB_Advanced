using UnityEngine;

[ExecuteAlways]
public class BottleEffect : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    [SerializeField] private Transform planeTranform;

    private void Awake()
    {
        rend ??= GetComponent<Renderer>();
        if(planeTranform == null && this.transform.GetChild(0).TryGetComponent<Transform>(out var transform))
        {
            planeTranform = transform;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rend.sharedMaterial.SetVector("_PlanePosition", planeTranform.position);
        rend.sharedMaterial.SetVector("_PlaneNormal", Vector3.up);
    }
}
