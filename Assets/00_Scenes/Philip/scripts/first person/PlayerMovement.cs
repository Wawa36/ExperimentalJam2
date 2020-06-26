using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public CharacterController controller;
    Rigidbody orbRigid;
    SphereArtifact orbScript;
    [HideInInspector] public Vector3 velocity;
    [SerializeField] float velocityMagnitude;
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
    [SerializeField] float orbEnergyIncrease;
    public float throwingForce;
    [SerializeField] LayerMask mask;

    [HideInInspector] public float orbEnergy=0;
    

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
        SwapOrbs();
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
        Vector3 previousPosition = transform.position;
        controller.Move(transform.right * velocity.x + transform.forward * velocity.z + Vector3.up* velocity.y* Time.deltaTime);
        if( Mathf.Approximately(transform.position.y, previousPosition.y))
        {
            velocity.y = (transform.position.y - previousPosition.y) / Time.deltaTime;
        }
        
        

    }
    /// <summary>
    /// wirft den orb in richtung vector3.forward des Spielers
    /// </summary>
    void Throw()
    {
        if (notAiming && Input.GetButtonDown("Fire1"))
        {
            notAiming = false;
            

            orbEnergy = 0;
        }
        if (!notAiming && carryingTheOrb && Input.GetButton("Fire1"))
        {
            if (Input.GetButton("Fire2"))
            {
                notAiming = true;
                orbScript.circle.gameObject.SetActive(false);
                orbScript.GetCollected();
                launchArc.lineRenderer.enabled = false;
                launchArc.targetSphere.enabled = false;
                orbEnergy = 0;
            }

            else
            {
                if (orbEnergy < throwingForce)
                {
                    orbEnergy += Time.deltaTime * orbEnergyIncrease;
                    //Orb -lädt auf sound 

                }
                else
                {
                    orbScript.circle.gameObject.SetActive(true);
                    //Orb voll aufgeladen sound
                }

                launchArc.lineRenderer.enabled = true;
                launchArc.DrawPath(cameraRigTransform.forward * throwingForce + cameraRigTransform.up * throwingForce / 4);
            }
        }
        if (!notAiming && carryingTheOrb && Input.GetButtonUp("Fire1"))
        {

            orbScript.colided = false;
            orbScript.circle.gameObject.SetActive(false);
            lookDirection = transform.forward;
            orbScript.trail.time = 4;
            launchArc.targetSphere.enabled = false;
            launchArc.lineRenderer.enabled = false;
            carryingTheOrb = false;
            orbRigid.isKinematic = false;
            orbRigid.useGravity = true;
            orbScript.timer = 0;
            orbScript.StartCoroutine(orbScript.FlyTime());
            orbRigid.velocity= cameraRigTransform.forward  * throwingForce + cameraRigTransform.up * throwingForce / 4;
            activeOrb.transform.parent = null;
        }
        
        if (!carryingTheOrb&& Input.GetButtonDown("Fire1"))
        {
            orbScript.trail.time = 1;
            notAiming = true;
            orbScript.collider.enabled = false;
            orbRigid.velocity = Vector3.zero;
            orbRigid.useGravity = false;
            orbScript.StartCoroutine(orbScript.FlyToPlayer());
        }

    }
    /// <summary>
    /// teleportiert den Spieler zum Orb
    /// </summary>
    void Teleport()
    {
        if (Input.GetButtonDown("Fire2") && !carryingTheOrb && controller.isGrounded)
        {
            orbEnergy = 0;
            controller.enabled = false;
            transform.position= activeOrb.transform.position + Vector3.up*3;
            controller.enabled = true;
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
        if (carryingTheOrb)
        {
            if (Input.GetButtonDown("Fire3"))
            {
                orbs[currentOrbIndex].SetActive(false);
                launchArc.lineRenderer.enabled = false;

                currentOrbIndex += 1;
                if (currentOrbIndex >= orbs.Count)
                {
                    currentOrbIndex = 0;
                }
                orbs[currentOrbIndex].SetActive(true);
                activeOrb = orbs[currentOrbIndex];
                orbRigid = activeOrb.GetComponent<Rigidbody>();
                orbScript = activeOrb.GetComponent<SphereArtifact>();
                launchArc = activeOrb.GetComponent<LaunchArc>();
            }
            if (Input.GetButtonDown("Fire4"))
            {
                orbs[currentOrbIndex].SetActive(false);
                launchArc.lineRenderer.enabled = false;
                currentOrbIndex -= 1;
                if (currentOrbIndex < 0)
                {
                    currentOrbIndex = orbs.Count - 1;
                }
                orbs[currentOrbIndex].SetActive(true);
                activeOrb = orbs[currentOrbIndex];
                orbRigid = activeOrb.GetComponent<Rigidbody>();
                orbScript = activeOrb.GetComponent<SphereArtifact>();
                launchArc = activeOrb.GetComponent<LaunchArc>();
                
            }
        }

    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") &&controller.isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }

    void Gravity()
    {


        velocity.y += gravity * Time.deltaTime;
        

        if(IsOnTheGround()&& velocity.y<0)
        {
            velocity.y = -2f;
           
        }

        velocityMagnitude = controller.velocity.magnitude;
    }
}
