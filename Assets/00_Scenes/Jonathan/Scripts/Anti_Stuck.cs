using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anti_Stuck : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float length;
    [SerializeField] LayerMask mask;

    public void Set_Free() 
    {
        var ray = new Ray(PlayerMovement.Instance.transform.position + PlayerMovement.Instance.transform.forward * length, -PlayerMovement.Instance.transform.forward);
        var hit = new RaycastHit();

        Physics.Raycast(ray, out hit, length, mask, QueryTriggerInteraction.Ignore);

        Debug.DrawRay(ray.origin, ray.direction, Color.green, length);

        if (hit.collider)
        {
            Teleport(hit.point);
        }
    }

    void Teleport(Vector3 to_point) 
    {
        PlayerMovement.Instance.StartCoroutine(PlayerMovement.Instance.Teleporting(PlayerMovement.Instance.transform.position, to_point));
    }
}
