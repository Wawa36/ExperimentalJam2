using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{



    #region first Person Parameter
    [Header("First Person Parameter")]
    [SerializeField] Transform playerTransform;
    [SerializeField] float turnSpeed;
    [SerializeField] float upDownTurnSpeed;
    

    #endregion


    // Update is called once per frame
    void Update()
    {
        FirstPersonRig();
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, 0, 0);
    }


    /// <summary>
    /// bewegt die first Person Camera
    /// </summary>
    void FirstPersonRig()
    {
        float MouseAxisX = Input.GetAxis("Mouse X");
        float MouseAxisY = Input.GetAxis("Mouse Y");

        Quaternion targetRotationHorizontal = Quaternion.AngleAxis(MouseAxisX * turnSpeed * Time.deltaTime, playerTransform.up);
        Quaternion targetRotationVertical = Quaternion.AngleAxis(MouseAxisY * -upDownTurnSpeed * Time.deltaTime, transform.right);
        playerTransform.rotation = targetRotationHorizontal * playerTransform.rotation;
        transform.rotation = targetRotationVertical * transform.rotation;
    }

}
