using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class LaunchArc : MonoBehaviour
{
    Rigidbody rigid;
    [SerializeField] float maxPathTime;
    [SerializeField] LayerMask mask;
    public MeshRenderer targetSphere;
    [HideInInspector] public LineRenderer lineRenderer;
    int rayCastResolution;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawPath(Vector3 Force)
    {
        targetSphere.enabled = true;
        int resolution = 100;
        Vector3 previousDrawpoint = transform.position;
        Vector3 drawpoint;
        rayCastResolution = 0;
        Vector3 endVector=Vector3.zero;
        lineRenderer.SetPosition(0,transform.position);
        for (int i =1; i<resolution; i++)
        {
            

            if (endVector== Vector3.zero )
            {
                float simulationTime = i / (float)resolution * maxPathTime;
                Vector3 displacement = Force * simulationTime + Physics.gravity * simulationTime * simulationTime / 2f;
                drawpoint = transform.position + displacement;
                RaycastHit hit;

                if (rayCastResolution <= i)
                {
                    rayCastResolution += 1;
                    if (Physics.Raycast(previousDrawpoint, drawpoint-previousDrawpoint, out hit, Vector3.Distance(drawpoint, previousDrawpoint),mask))
                    {
                        endVector = hit.point;
                        drawpoint = hit.point;
                    }
                }
            }
            else
            {
                drawpoint = endVector;

            }
            if (i == resolution - 1)
            {
                targetSphere.transform.position = drawpoint;
            }
            previousDrawpoint = drawpoint;
            lineRenderer.SetPosition(i, drawpoint);
        }
    }

}
