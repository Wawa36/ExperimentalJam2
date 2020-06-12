using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
   
    enum CameraMode { FirstPerson,ThirdPerson};
    [Header("general Settings")]
    [SerializeField] CameraMode cameraMode;
    [SerializeField] GameObject Player;
    [SerializeField] Camera cameraOne;
    [SerializeField] Camera cameraTwo;
   

    #region first Person Parameter
    [Header ("First Person Parameter")]
    
    bool firstPersonLoaded;

    #endregion

    #region third Person Parameter
    [Header("Third Person Parameter")]
    [SerializeField] Vector3 CameraDistance;
    bool thirdPersonLoaded;
    #endregion

    // Update is called once per frame
    void Update()
    {
        switch (cameraMode)
        {
            case CameraMode.FirstPerson:
                if (!firstPersonLoaded)
                {
                    FirstPersonLoad();
                }
                FirstPersonRig();
                break;
            case CameraMode.ThirdPerson:
                if (!thirdPersonLoaded)
                {
                    ThirdPersonLoad();
                }
                ThirdPersonRig();
                break;
        }
    }

    /// <summary>
    /// etabliert alle wichtigen Einstellungen für eine first Person Camera
    /// </summary>
    void FirstPersonLoad()
    {


        firstPersonLoaded = true;
        thirdPersonLoaded = false;
    }

    /// <summary>
    /// etabliert alle wichtigen Einstellungen für eine third Person Camera
    /// </summary>
    void ThirdPersonLoad()
    {



        firstPersonLoaded = false;
        thirdPersonLoaded = true;
    }

    /// <summary>
    /// bewegt die first Person Camera
    /// </summary>
    void FirstPersonRig()
    {

    }

    /// <summary>
    /// bewegt die third PErson Camera
    /// </summary>
    void ThirdPersonRig()
    {

    }
    
}
