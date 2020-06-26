using UnityEngine;

public class LaunchArc : MonoBehaviour
{
    Rigidbody rigid;
    [SerializeField] PlayerMovement playerScript;
    [SerializeField] Color color1;
    [SerializeField] Color color2;

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
       //ColorChange();
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
                    if (Physics.CapsuleCast(previousDrawpoint,drawpoint,0.15f,drawpoint-previousDrawpoint,out hit, Vector3.Distance(drawpoint, previousDrawpoint),mask))
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
        targetSphere.material.color = Color.Lerp(color1, color2, playerScript.orbEnergy / playerScript.throwingForce);
    }

}
