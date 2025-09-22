using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public WebCamTexture webcam;

    void Start()
    {
        webcam = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = webcam;
        webcam.Play();
    }
}
