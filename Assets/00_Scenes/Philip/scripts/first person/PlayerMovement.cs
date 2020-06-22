using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rigid;
    Rigidbody orbRigid;
    SphereArtifact orbScript;
    public bool carryingTheOrb;
    [HideInInspector] public bool notAiming;
    int currentOrbIndex;
    
    [SerializeField] List<GameObject> orbs;
    GameObject activeOrb;
    [SerializeField] Transform cameraRigTransform;
    [SerializeField] LaunchArc launchArc;

    [SerializeField] float movespeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float throwForceIncrease;
    [SerializeField] float maxThrowingForce;
    
    [HideInInspector] public float throwForce=0;
    

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        activeOrb = orbs[0];
        orbRigid = activeOrb.GetComponent<Rigidbody>();
        orbScript = activeOrb.GetComponent<SphereArtifact>();
        carryingTheOrb = true;
    }
    private void Update()
    {
        Throw();
        Teleport();
        Jump();
        if (carryingTheOrb)
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
            {
                SwapOrbs();
            }
        }
       
          
        
    }
    private void FixedUpdate()
    {
        Move();
    }
    /// <summary>
    /// wandelt den input in die bewegung des players um
    /// </summary>
    void Move()
    {
        float XAxis = Input.GetAxis("Horizontal");
        float YAxis = Input.GetAxis("Vertical");

        rigid.velocity = transform.TransformDirection( new Vector3(XAxis * Time.fixedDeltaTime * movespeed, rigid.velocity.y, YAxis * Time.fixedDeltaTime * movespeed));
        
    }
    /// <summary>
    /// wirft den orb in richtung vector3.forward des Spielers
    /// </summary>
    void Throw()
    {
        if (notAiming && Input.GetMouseButtonDown(0))
        {
            notAiming = false;
        }
        if (!notAiming && carryingTheOrb && Input.GetMouseButton(0))
        {
            if (throwForce < maxThrowingForce)
            {
                throwForce += Time.deltaTime * throwForceIncrease;
            }
            launchArc.lineRenderer.enabled = true;
            launchArc.DrawPath(cameraRigTransform.forward * throwForce + cameraRigTransform.up * throwForce/4);
        }
        if (!notAiming && carryingTheOrb && Input.GetMouseButtonUp(0))
        {
            launchArc.targetSphere.enabled = false;
            launchArc.lineRenderer.enabled = false;
            carryingTheOrb = false;
            orbRigid.isKinematic = false;
            orbRigid.useGravity = true;
            StartCoroutine(orbScript.FlyTime());
            orbRigid.velocity= cameraRigTransform.forward  * throwForce + cameraRigTransform.up * throwForce / 4;
            activeOrb.transform.parent = null;
        }
        
        if (!carryingTheOrb&& Input.GetMouseButtonDown(0))
        {

            notAiming = true;
            orbScript.collider.enabled = false;
            orbRigid.useGravity = false;
            orbScript.StartCoroutine(orbScript.FlyToPlayer());
        }

    }
    /// <summary>
    /// teleportiert den Spieler zum Orb
    /// </summary>
    void Teleport()
    {
        if (Input.GetMouseButtonDown(1) && !carryingTheOrb && IsOnTheGround())
        {
            transform.position =activeOrb.transform.position + Vector3.up;
            rigid.velocity = Vector3.zero;
            
            orbScript.GetCollected();
        }
    }
    bool IsOnTheGround()
    {
        if (Physics.Raycast(transform.position+Vector3.right*0.5f, Vector3.down,1.2f)|| Physics.Raycast(transform.position + Vector3.right * -0.5f, Vector3.down, 1.2f)|| Physics.Raycast(transform.position + Vector3.forward * 0.5f, Vector3.down, 1.2f)|| Physics.Raycast(transform.position + Vector3.forward * -0.5f, Vector3.down, 1.2f))
        {
            
            return true;
        }
        else
        return false;

    }

    void SwapOrbs()
    {
        orbs[currentOrbIndex].SetActive(false);
        launchArc.lineRenderer.enabled = false;
        if (Input.GetKeyDown(KeyCode.Q))
        {
           
            currentOrbIndex += 1;
            if (currentOrbIndex >= orbs.Count)
            {
                currentOrbIndex = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentOrbIndex -= 1;
            if (currentOrbIndex < 0)
            {
                currentOrbIndex = orbs.Count - 1;
            }
        }
        orbs[currentOrbIndex].SetActive(true);
        activeOrb = orbs[currentOrbIndex];
        orbRigid = activeOrb.GetComponent<Rigidbody>();
        orbScript = activeOrb.GetComponent<SphereArtifact>();
        launchArc = activeOrb.GetComponent<LaunchArc>();

    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsOnTheGround())
        {
            rigid.velocity+= Vector3.up * jumpForce;
        }
    }
}
