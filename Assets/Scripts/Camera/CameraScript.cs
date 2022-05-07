using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScript : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private float minZoom = 60f;
    [SerializeField] private float maxZoom = 120f;
    [SerializeField] private float zoomScale = 10f;
    [SerializeField] [Range(0.0001f,1f)] 
    private float speed = 0.1f;

    private const float Half = 0.5f;

    void Start()
    {
        _camera = GetComponent<Camera>();
        transform.position = new Vector3(1, 5, 1);
    }
    void Update()
    {
        if ((-(Input.GetAxis("Mouse ScrollWheel")) < 0 || _camera.fieldOfView < maxZoom) &&
            (-(Input.GetAxis("Mouse ScrollWheel")) > 0 || _camera.fieldOfView > minZoom))
            _camera.fieldOfView += -(Input.GetAxis("Mouse ScrollWheel")) * zoomScale;
        transform.position += Vector3.right * (Input.GetAxis("Horizontal") * speed) + 
                              Vector3.forward * (Input.GetAxis("Vertical") * speed);
    }
}
