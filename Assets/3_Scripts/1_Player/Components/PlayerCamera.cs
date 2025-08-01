using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCamera : MonoBehaviour
{

    [SerializeField] private Vector3 cameraOffset = Vector3.zero;
    
    private Camera m_camera;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (m_camera == null)
        {
            m_camera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_camera.transform.position =
            cameraOffset + new Vector3(transform.position.x, transform.position.y, transform.position.z);
        m_camera.transform.LookAt(transform);
    }
}
