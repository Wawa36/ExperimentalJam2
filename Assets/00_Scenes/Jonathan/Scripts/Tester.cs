using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public LayerMask mask;

    // Update is called once per frame
    void Update()
    {
        var hit = new RaycastHit();
        var ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, 10f, mask);

        if (hit.collider)
        {
            Debug.DrawRay(transform.position, (hit.point - transform.position), Color.green);
            Debug.DrawRay(hit.point + Vector3.up, (hit.normal * (hit.point - transform.position).magnitude), Color.red);
        }
    }
}
