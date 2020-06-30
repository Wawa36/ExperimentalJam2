using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    [SerializeField] PlayerMovement player;
    [SerializeField] Animator anim;
    [SerializeField] float speed;

    // Update is called once per frame
    void Update()
    {
        var vel = player.velocity;
        vel.y = 0;
        anim.SetFloat("Speed", vel.magnitude * speed);    
    }
}
