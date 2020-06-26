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
    public LineRenderer pyramidRenderer;
    int rayCastResolution;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();


    }
    public void DrawPath(Vector3 Force)
    {
        pyramidRenderer.enabled = true;
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
                pyramidRenderer.transform.position = drawpoint;
                CreatePyramid(pyramidRenderer, endVector, endNormal,endNormal, .1f, .5f);
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
        pyramidRenderer.material.color = Color.Lerp(color1, color2, playerScript.orbEnergy / playerScript.throwingForce);
    }


    void ChangeTargetDirection(Vector3 Center, Vector3 direction, Vector3 normal, Vector3 up)
    {
        squareTransform.position = Center;
        squareTransform.rotation = Quaternion.LookRotation(normal, up);
        zylinderTransform.position = Center;
        zylinderTransform.rotation = Quaternion.LookRotation(direction,up);
    }

    void CreatePyramid(LineRenderer line,Vector3 scareCenter,Vector3 direction,Vector3 normal,float radius,float height)
    {
        if (normal == Vector3.zero)
        {
            normal = transform.forward;
        }
        line.transform.position = scareCenter;
        
        line.transform.rotation = Quaternion.LookRotation(normal);
        
        Vector3 corner1 = scareCenter  + line.transform.right * -radius + line.transform.up * -radius;
        Vector3 corner2 = scareCenter  + line.transform.right * -radius + line.transform.up *  radius;
        Vector3 corner3 = scareCenter  + line.transform.right *  radius + line.transform.up *  radius;
        Vector3 corner4= scareCenter   + line.transform.right *  radius + line.transform.up * -radius;
        Vector3 cornerTop =scareCenter + direction*height;
         
        Vector3[] cornerOrder = new Vector3[10] { corner1, corner2, corner3, corner4, cornerTop, corner1, corner4, corner3, cornerTop, corner2 };
        line.positionCount = 10;
        line.SetPositions(cornerOrder);
        
    }

}
