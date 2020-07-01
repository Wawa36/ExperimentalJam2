using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Start_Teleporter : MonoBehaviour
{
    [SerializeField] Vector3 target_position;
    [SerializeField] float speed;
    Animator anim;
   
    // Start is called before the first frame update
    void Start()
    {
        PlayerMovement.Instance.StartCoroutine(PlayerMovement.Instance.Teleporting(PlayerMovement.Instance.transform.position, target_position, speed));
        Destroy(this);
    }
}
