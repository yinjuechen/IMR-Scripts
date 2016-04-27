using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{

    public float turningSpeed = 50;
    public float horizontalSpeed = 3.0F;
    public float verticalSpeed = 3.0F;
    private float fov;
    public float zoomSpeed = 5;
    private float camAngle = 0;



    // Use this for initialization
    void Start()
    {
        fov = Camera.main.fieldOfView;

    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("sphere screen").GetComponent<FrameViewer>().reviewMode || GameObject.Find("sphere screen").GetComponent<FrameViewer>().playNavigationMode)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.up, -Time.deltaTime * turningSpeed, Space.World);// turn left
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(Vector3.up, Time.deltaTime * turningSpeed, Space.World);// turn right
            }
        }
        Turning();
        Zooming();
    }


    // Rotate camare to face mouse
    void Turning()
    {
        if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            if (Input.GetMouseButton(0))
            {
                float rotationHor = Input.GetAxis("Mouse X") * horizontalSpeed;
                float rotatonVer = Input.GetAxis("Mouse Y") * verticalSpeed;
                transform.Rotate(0, rotationHor, 0);// Horizontal turning

            }
        }
    }

    void Zooming()
    {
        if (Camera.main.fieldOfView >= 40 && Camera.main.fieldOfView <= fov)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)// zoom out
            {
                Camera.main.fieldOfView = Camera.main.fieldOfView + zoomSpeed;
                Debug.Log("zoom out");
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)// zoom in
            {
                Camera.main.fieldOfView = Camera.main.fieldOfView - zoomSpeed;
                Debug.Log("zoom in");
            }

        }
        else if (Camera.main.fieldOfView < 40)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)// zoom out
            {
                Camera.main.fieldOfView = Camera.main.fieldOfView + zoomSpeed;
            }
        }
        else if (Camera.main.fieldOfView > fov)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)// zoom in
            {
                Camera.main.fieldOfView = Camera.main.fieldOfView - zoomSpeed;
            }

        }
        Debug.Log(Camera.main.fieldOfView);
    }

    public IEnumerator Rotate360()
    {
        camAngle = 0;
        while (camAngle < 180)
        {
            transform.Rotate(new Vector3(0,0.5f,0));
            camAngle += transform.rotation.eulerAngles.y;
            Debug.LogWarning("Camera Angle: " + camAngle);
            yield return new WaitForSeconds(0.0001f);
        }
        
    }

}
