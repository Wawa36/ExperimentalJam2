using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Movement : Singleton<Camera_Movement>
{
    [SerializeField] Transform rig;
    [SerializeField] float rot_speed;
    [SerializeField] float move_speed;

    float current_rot_speed;
    float current_move_speed;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;

        current_rot_speed = rot_speed;
        current_move_speed = move_speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, rig.position, current_move_speed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, rig.rotation, current_rot_speed * Time.deltaTime);
    }

    public void Set_Rot_Speed(float to_speed) 
    {
        current_rot_speed = to_speed;
    }

    public void Set_Move_Speed(float to_speed)
    {
        current_move_speed = to_speed;
    }

    public void Reset_Rot_Speed()
    {
        current_rot_speed = rot_speed;
    }

    public void Reset_Move_Speed()
    {
        current_move_speed = move_speed;
    }
}
