using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public WebCamTexture webcam;
    public bool showCam = false;

    void Start()
    {
        webcam = new WebCamTexture();
        if (showCam){
            GetComponent<Renderer>().material.mainTexture = webcam;
        } 
        webcam.Play();
    }
}
