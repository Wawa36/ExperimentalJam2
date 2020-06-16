using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchArc : MonoBehaviour
{
    Rigidbody rigid;
    [SerializeField] float maxPathTime;
    [HideInInspector] public LineRenderer lineRenderer;
    
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawPath(Vector3 Force)
    {
        int resolution = 30;
        Vector3 previousDrawpoint = transform.position;
        for(int i =0; i<resolution; i++)
        {
            float simulationTime = i / (float)resolution * maxPathTime;
            Vector3 displacement = Force * simulationTime + Physics.gravity * simulationTime * simulationTime / 2f;
            Vector3 drawpoint = transform.position + displacement;
            lineRenderer.SetPosition(i, drawpoint);
            previousDrawpoint = drawpoint;

        }


    }

}
