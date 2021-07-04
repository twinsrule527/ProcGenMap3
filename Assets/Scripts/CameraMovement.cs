using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Moves the camera with arrow keys and scrolling
public class CameraMovement : MonoBehaviour
{
    private float camSpeed;
    private Camera myCam;
    [SerializeField] private float baseZoom;//How much the camera starts zoomed out
    private float curZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    [SerializeField] private float zoomSpeed;
    void Start()
    {
        myCam = GetComponent<Camera>();
        camSpeed = 5;
        myCam.orthographicSize = baseZoom;
        curZoom = baseZoom;
    }

    void Update()
    {
        //Move the camera around
        if(Input.GetKey(KeyCode.LeftArrow)) {
            transform.position -= transform.right * camSpeed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.RightArrow)) {
            transform.position += transform.right * camSpeed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.UpArrow)) {
            transform.position += transform.up * camSpeed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.DownArrow)) {
            transform.position -= transform.up * camSpeed * Time.deltaTime;
        }
        //Zoom in/out
        if(Input.GetKey(KeyCode.Equals)) {
            curZoom -= zoomSpeed * Time.deltaTime;
            curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);
            myCam.orthographicSize = curZoom;
        }
        if(Input.GetKey(KeyCode.Minus)) {
            curZoom += zoomSpeed * Time.deltaTime;
            curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);
            myCam.orthographicSize = curZoom;
        }

        //TODO: Zooming and moving should happen relative to current position
    }
}
