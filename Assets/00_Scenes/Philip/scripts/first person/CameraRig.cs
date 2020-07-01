using Settings_Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : Singleton<CameraRig>
{



    #region first Person Parameter
    [Header("First Person Parameter")]
    [SerializeField] Transform playerTransform;
    [SerializeField] float turnSpeed;
    [SerializeField] float upDownTurnSpeed;
    float xRotation;
    public bool alreadyLanded;

    #endregion
    // Update is called once per frame
    void Update()
    {
        if (alreadyLanded)
        {
            FirstPersonRig();
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, 0, 0);
        }
        else
        {
            if (PlayerMovement.Instance.IsOnTheGround())
            {
                alreadyLanded = true;
                UI_Controller.Instance.Open_Controls();
            }
        }
    }


    /// <summary>
    /// bewegt die first Person Camera
    /// </summary>
    void FirstPersonRig()
    {
        float MouseAxisX = Input.GetAxis("Mouse X")*turnSpeed*Time.deltaTime * Game_Settings.Sensitivity_X;
        float MouseAxisY = Input.GetAxis("Mouse Y")*upDownTurnSpeed*Time.deltaTime * Game_Settings.Sensitivity_Y;

        xRotation -= MouseAxisY;
        xRotation = Mathf.Clamp(xRotation, -76, 90);

        playerTransform.Rotate(playerTransform.up*MouseAxisX);
        transform.localRotation = Quaternion.Euler(xRotation,0,0);
    }

}
