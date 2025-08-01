using System.Collections.Generic;
using UnityEngine;

public class SeeThroughController : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    [SerializeField] private List<GameObject> m_walls = new List<GameObject>();


    private Camera m_camera;

    private void Awake()
    {
        m_camera = Camera.main;
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
