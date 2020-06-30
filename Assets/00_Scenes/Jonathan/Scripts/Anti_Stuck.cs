using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anti_Stuck : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float length;
    [SerializeField] LayerMask mask;

    GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public void Set_Free() 
    {
        var ray = new Ray(player.transform.position + player.transform.forward * length, -player.transform.forward);
        var hit = new RaycastHit();

        Physics.Raycast(ray, out hit , length, mask, QueryTriggerInteraction.Ignore);

        if (hit.collider)
            Teleport(hit.point);
    }

    void Teleport(Vector3 to_point) 
    {
        player.GetComponent<PlayerMovement>().StartCoroutine("Teleporting");
    }
}
