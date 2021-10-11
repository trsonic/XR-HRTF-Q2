using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalLightPositioning : MonoBehaviour
{
    public GameObject _mainCamera;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position = _mainCamera.transform.position;
        transform.rotation = _mainCamera.transform.rotation;
        transform.Translate(0.0f, 0.3f, 0.3f);
        transform.Rotate(5.0f, 0.0f, 0.0f);
    }
}
