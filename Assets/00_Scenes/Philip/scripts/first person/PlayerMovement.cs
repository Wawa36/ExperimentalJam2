﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : Singleton<PlayerMovement>
{
    #region nonSerializable Variables
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Vector3 lookDirection;
    [HideInInspector] public float orbEnergy=0;
    [HideInInspector] public bool carryingTheOrb;
    [HideInInspector] public bool notAiming;
    Rigidbody orbRigid;
    SphereArtifact orbScript;
    GameObject activeOrb;
    Animator teleportAnim;
    [HideInInspector] public int currentOrbIndex;
    #endregion

    #region serializable Variables
    [Header("References")]
    [SerializeField] List<GameObject> orbs;
    [SerializeField] LayerMask mask;
    [SerializeField] Transform cameraRigTransform;
    [SerializeField] LaunchArc launchArc;
    

    [Header("Movement Parameter")]
    [SerializeField] float gravity;
    [SerializeField] float movespeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float orbEnergyIncrease;
    public float throwingForce;
    #endregion



    private void Start()
    {
        controller = GetComponent<CharacterController>();
        activeOrb = orbs[0];
        orbRigid = activeOrb.GetComponent<Rigidbody>();
        orbScript = activeOrb.GetComponent<SphereArtifact>();
        carryingTheOrb = true;
        teleportAnim = GameObject.FindGameObjectWithTag("postProcess").GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            Gravity();
            // Throw();

            Jump();

            Move();
            Teleport();
            SwapOrbs();
        }
    }
    private void LateUpdate()
    {
        if (Time.timeScale != 0)
        {
            Throw();
        }
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
        if (controller.enabled == true)
        {
            controller.Move(transform.right * velocity.x + transform.forward * velocity.z + Vector3.up * velocity.y * Time.deltaTime);
            if (Mathf.Approximately(transform.position.y, previousPosition.y))
            {
                velocity.y = (transform.position.y - previousPosition.y) / Time.deltaTime;
            }
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
                launchArc.target.SetActive(false);
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
                launchArc.target.SetActive(true);
                launchArc.DrawPath(cameraRigTransform.forward * throwingForce + cameraRigTransform.up * throwingForce / 4);
            }
        }
        if (!notAiming && carryingTheOrb && Input.GetButtonUp("Fire1"))
        {

            orbScript.colided = false;
            orbScript.circle.gameObject.SetActive(false);
            lookDirection = transform.forward;
            orbScript.trail.time = 4;
            launchArc.lineRenderer.enabled = false;
            launchArc.target.SetActive(false);
            carryingTheOrb = false;
            orbRigid.isKinematic = false;
            orbRigid.useGravity = true;
            orbScript.timer = 0;
            orbScript.StartCoroutine(orbScript.FlyTime());
            orbRigid.velocity= cameraRigTransform.forward  * throwingForce + cameraRigTransform.up * throwingForce / 4;
            activeOrb.transform.parent = null;
        }
        
        if (!notAiming&& !carryingTheOrb&& Input.GetButtonDown("Fire1"))
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
            orbScript.colided = true;
            StartCoroutine(Teleporting(transform.position, orbScript.transform.position + Vector3.up));

        }
    }
    public IEnumerator Teleporting(Vector3 startPosition, Vector3 endPosition)
    {
        orbEnergy = 0;
        controller.enabled = false;
        teleportAnim.SetTrigger("teleport");
        for (float f=0;f<=.5;f+=Time.deltaTime) 
        {
            transform.position = Vector3.Lerp(startPosition, endPosition  , 2*f);
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPosition ;
        controller.enabled = true;
        orbScript.GetCollected();
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
                orbScript.circle.gameObject.SetActive(false);
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
                orbScript.circle.gameObject.SetActive(false);
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
        if (Input.GetButtonDown("Jump") && IsOnTheGround())
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }

    void Gravity()
    {


        velocity.y += gravity * Time.deltaTime;
        

        if(IsOnTheGround()&& velocity.y<0)
        {
            velocity.y = -5f;
           
        }

    }
}
