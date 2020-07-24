using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    public string screenshotName;
    public int screenshotsSinceStart = 0;
    public int sizeMult;

    public KeyCode key;

    void Start()
    {
        screenshotsSinceStart = 0;
    }

    void Update()
    {
        //Screen.SetResolution(1920, 1080, false);

        if (Input.GetKeyDown(key))
        {

            ScreenCapture.CaptureScreenshot("Screenshots/" + screenshotName + "_" + screenshotsSinceStart + "_" + System.DateTime.Now.Day + "." +
                System.DateTime.Now.Month + "." + System.DateTime.Now.Year + "."+ System.DateTime.Now.Hour + "." + System.DateTime.Now.Minute + "." + System.DateTime.Now.Second + ".png", sizeMult);
            screenshotsSinceStart++;

            Debug.Log("saved a screenshot: " + screenshotName + "_" + screenshotsSinceStart + "_" + System.DateTime.Now.Day + "." +
                System.DateTime.Now.Month + "." + System.DateTime.Now.Year + "." + System.DateTime.Now.Hour + "." + System.DateTime.Now.Minute + "." + System.DateTime.Now.Second + ".png");

        }
    
    }
}
