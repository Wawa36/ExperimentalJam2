using Tower_Management;
using UnityEngine;

public class LaunchArc : MonoBehaviour
{
    Rigidbody rigid;
    [SerializeField] PlayerMovement playerScript;
    [SerializeField] Color color1;
    [SerializeField] Color color2;
    [SerializeField] Transform squareTransform;
    [SerializeField] Transform zylinderTransform;

    [SerializeField] float maxPathTime;
    [SerializeField] LayerMask mask;
    [HideInInspector] public LineRenderer lineRenderer;
    public GameObject target;
    int rayCastResolution;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        target.SetActive(false);


    }
    public void DrawPath(Vector3 Force)
    {
        int resolution = 100;
        Vector3 previousDrawpoint = transform.position;
        Vector3 drawpoint;
        Vector3 endNormal=Vector3.zero;
        rayCastResolution = 0;
        Vector3 endVector=Vector3.zero;
       //ColorChange();
        for (int i =0; i<resolution; i++)
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
                    if (Physics.CapsuleCast(previousDrawpoint,drawpoint,0.15f,drawpoint-previousDrawpoint,out hit, Vector3.Distance(drawpoint, previousDrawpoint),mask))
                    {
                        
                        endVector = hit.point;
                        drawpoint = hit.point;
                        endNormal = hit.normal;
                    }
                }
            }
            else
            {
                drawpoint = endVector;

            }
            if (i == resolution - 1)
            {
                ChangeTarget(endVector, Tower.Calculate_Grow_Direction(playerScript.transform.forward, endNormal), endNormal, Vector3.up);
            }
            previousDrawpoint = drawpoint;
            lineRenderer.SetPosition(i, drawpoint);
        }
    }

    void ColorChange()
    {
        Gradient gradient = new Gradient();

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = Color.Lerp(color1, color2, playerScript.orbEnergy / playerScript.throwingForce).a;
        alphaKey[0].time = 0;
        alphaKey[1].alpha = Color.Lerp(color1, color2, playerScript.orbEnergy / playerScript.throwingForce).a;
        alphaKey[1].time = 1;

        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.Lerp(color1, color2, playerScript.orbEnergy / playerScript.throwingForce);
        colorKey[0].time = 0;
        colorKey[1].color = Color.Lerp(color1, color2, playerScript.orbEnergy / playerScript.throwingForce);
        colorKey[1].time = 1;

        gradient.colorKeys = colorKey;

        lineRenderer.colorGradient = gradient;
        //lineRenderer.colorGradient.colorKeys[0].color = Color.Lerp(color1, color2, playerScript.orbEnergy / playerScript.throwingForce);
        //lineRenderer.colorGradient.colorKeys[1].color = Color.Lerp(color1, color2, playerScript.orbEnergy / playerScript.throwingForce);
    }


    void ChangeTarget(Vector3 Center, Vector3 direction, Vector3 normal, Vector3 up)
    {
        if (playerScript.currentOrbIndex == 0)
        {
            direction += Vector3.up*.75f;
            direction = direction.normalized;
        }
        else if (playerScript.currentOrbIndex == 1)
        {
            direction = Vector3.up;

        }
        if (normal == Vector3.zero)
        {
            normal = Vector3.up;
            up = playerScript.transform.forward;
        }
        else if (Mathf.Approximately(normal.x, 0) && Mathf.Approximately(normal.y, 1) && Mathf.Approximately(normal.z, 0))
        {
            up = playerScript.transform.forward;
        }


        target.transform.position = Center;
        squareTransform.rotation = Quaternion.LookRotation(normal, up);
        zylinderTransform.rotation = Quaternion.LookRotation(direction);
    }


}
