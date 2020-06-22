using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;
    Rigidbody orbRigid;
    SphereArtifact orbScript;
    public Vector3 velocity;
    [HideInInspector] public Vector3 lookDirection;

    public bool carryingTheOrb;
    
    [HideInInspector] public bool notAiming;
    int currentOrbIndex;
    
    [SerializeField] List<GameObject> orbs;
    GameObject activeOrb;
    [SerializeField] Transform cameraRigTransform;
    [SerializeField] LaunchArc launchArc;

    [SerializeField] float gravity;
    [SerializeField] float movespeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float throwForceIncrease;
    [SerializeField] float maxThrowingForce;
    [SerializeField] LayerMask mask;

    [HideInInspector] public float throwForce=0;
    

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        activeOrb = orbs[0];
        orbRigid = activeOrb.GetComponent<Rigidbody>();
        orbScript = activeOrb.GetComponent<SphereArtifact>();
        carryingTheOrb = true;
    }
    private void Update()
    {
        Gravity();
       // Throw();
        
        Jump();

        Move();
        Teleport();
        if (carryingTheOrb)
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
            {
                SwapOrbs();
            }
        }
    }
    private void LateUpdate()
    {
        Throw ();
    }
    /// <summary>
    /// wandelt den input in die bewegung des players um
    /// </summary>
    void Move()
    {
        float XAxis = Input.GetAxis("Horizontal")*movespeed*Time.deltaTime;
        float YAxis = Input.GetAxis("Vertical")*movespeed*Time.deltaTime;

        velocity.x = XAxis;
        velocity.z = YAxis;
        
        controller.Move(transform.right * velocity.x + transform.forward * velocity.z + Vector3.up* velocity.y* Time.deltaTime);
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
            lookDirection = transform.forward;
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
        

        if (Input.GetMouseButtonDown(1) && !carryingTheOrb && controller.isGrounded)
        {
            
            controller.Move(activeOrb.transform.position + Vector3.up-transform.position);
            orbScript.GetCollected();
        }
    }
    bool IsOnTheGround()
    {
        if (Physics.CheckBox(transform.position -transform.up,new Vector3(.5f,.2f,.5f),transform.rotation, mask,0))
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
        if (Input.GetKeyDown(KeyCode.Space)&&controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }

    void Gravity()
    {
       
            velocity.y += gravity * Time.deltaTime;
        
        if(IsOnTheGround()&& velocity.y<0)
        {
            velocity.y = -2f;
           
        }

    }
}
