using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_thirdPerson : MonoBehaviour
{
    Rigidbody rigid;
    Rigidbody orbRigid;
    Animator anim;
    Orb_ThirdPerson orbScript;
    public bool carryingTheOrb;
    int currentOrbIndex;
    [SerializeField] List<GameObject> orbs;
    [SerializeField] Transform cameraRigTransform;
    GameObject activeOrb;

    LaunchArc launchArc;

    [SerializeField] float movespeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float throwForce;


    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        activeOrb = orbs[0];
        launchArc = activeOrb.GetComponent<LaunchArc>();
        orbRigid = activeOrb.GetComponent<Rigidbody>();
        orbScript = activeOrb.GetComponent<Orb_ThirdPerson>();
        carryingTheOrb = true;
    }
    private void Update()
    {
        Move();
        
        Teleport();
        Jump();
        if (carryingTheOrb)
        {
            //launchArc.DrawPath(cameraRigTransform.forward * throwForce);
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
            {
                SwapOrbs();
            }
        }

    }
    void Aim()
    {

        if(carryingTheOrb && Input.GetMouseButton(0))
        {
            anim.SetBool("shooting", true);
            launchArc.enabled = true;
        }
        if(carryingTheOrb && Input.GetMouseButtonUp(0))
        {
            Throw();
        }

    }
    /// <summary>
    /// wandelt den input in die bewegung des players um
    /// </summary>
    void Move()
    {
        float XAxis = Input.GetAxis("Horizontal");
        float YAxis = Input.GetAxis("Vertical");

        rigid.MovePosition(transform.position + transform.forward * YAxis * Time.deltaTime * movespeed +transform.right*XAxis*Time.deltaTime*movespeed*.9f);
        rigid.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(cameraRigTransform.forward.x,0,cameraRigTransform.forward.z), Vector3.up), YAxis));
    }
    /// <summary>
    /// wirft den orb in richtung vector3.forward des Spielers
    /// </summary>
    void Throw()
    {
        
            carryingTheOrb = false;
            orbRigid.isKinematic = false;
            orbRigid.useGravity = true;
            //orbRigid.velocity = cameraRigTransform.forward * throwForce;
            activeOrb.transform.parent = null;
        
    }
    /// <summary>
    /// teleportiert den Spieler zum Orb
    /// </summary>
    void Teleport()
    {
        if (Input.GetMouseButtonDown(1) && !carryingTheOrb && IsOnTheGround())
        {
            transform.position = activeOrb.transform.position + Vector3.up;
            rigid.velocity = Vector3.zero;

            orbScript.GetCollected();
        }
    }
    bool IsOnTheGround()
    {
        if (Physics.Raycast(transform.position + Vector3.right * 0.5f, Vector3.down, 1.2f) || Physics.Raycast(transform.position + Vector3.right * -0.5f, Vector3.down, 1.2f) || Physics.Raycast(transform.position + Vector3.forward * 0.5f, Vector3.down, 1.2f) || Physics.Raycast(transform.position + Vector3.forward * -0.5f, Vector3.down, 1.2f))
        {

            return true;
        }
        else
            return false;

    }

    void SwapOrbs()
    {
        orbs[currentOrbIndex].SetActive(false);
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
        orbScript = activeOrb.GetComponent<Orb_ThirdPerson>();
        launchArc = activeOrb.GetComponent<LaunchArc>();

    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsOnTheGround())
        {
            rigid.velocity += Vector3.up * jumpForce;
        }
    }
}
