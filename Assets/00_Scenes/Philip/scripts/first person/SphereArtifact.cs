﻿using Settings_Management;
using System.Collections;
using System.Collections.Generic;
using Tower_Management;
using UnityEngine;

public class SphereArtifact : MonoBehaviour
{
    [SerializeField] GameObject TowerPrefab;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform targetPosition;
    [SerializeField] Orb_Animation orbAnim;
    [SerializeField] float collectingDistance;
    

    [HideInInspector] public new SphereCollider collider;
    [SerializeField] PlayerMovement playerScript;
    [SerializeField] MeshRenderer meshR;

    [Header("Particles")]
    
    public ParticleSystem zoom;
    public ParticleSystem circle;
    public TrailRenderer trail;

    [HideInInspector] public AudioSource audio1;
    AudioSource audio2;
    int zoomCount=15;
    int CircleCount=1;

    float particleTimer;

    Rigidbody rigid;
    [HideInInspector] public bool colided;
    public float timer;
    Material material;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible=false;
        Vector3 cameraPosition = Camera.main.ViewportToWorldPoint(new Vector3(.95f , 0.1f, 1));
        targetPosition.position = cameraPosition;
        transform.localPosition = Vector3.zero;
        playerScript = PlayerMovement.Instance;
        rigid = GetComponent<Rigidbody>();
        collider = GetComponent<SphereCollider>();
        audio1 = GetComponents<AudioSource>()[0];
        audio2 = GetComponents<AudioSource>()[1];
        circle.GetComponent<AudioSource>().volume *= Sound_Manager.Instance.Get_Clip("Pling").volume;
    }

    private void Update()
    {
        ManageParticles();
        orbAnim.Speed = playerScript.orbEnergy;
    }

    /// <summary>
    /// kümmert sich um das scaling der Particles
    /// </summary>
    private void ManageParticles()
    {
        particleTimer += Time.deltaTime;
        if (particleTimer >= .2f)
        {

            
            zoomCount = Mathf.RoundToInt(Mathf.Lerp(.25f, 8, playerScript.orbEnergy / playerScript.throwingForce) * 4);

            if (playerScript.orbEnergy != 0)
            {

               
                ParticleEmission(zoom, zoomCount);
               
            }
            if (playerScript.orbEnergy >= 25)
            {
                
                //ParticleEmission(circle, CircleCount);
            }
            particleTimer = 0;
        }
    }


    void ParticleEmission(ParticleSystem target, int emissionCount)
    {
        target.Emit(emissionCount);

    }
    /// <summary>
    /// wird aufgerufen wenn die Kugel eingesammelt werden soll
    /// 
    /// </summary>
    public void GetCollected()
    {
        trail.time = 0;
        collider.enabled = true;
        transform.parent = targetPosition;
        transform.localPosition = Vector3.zero;
        playerScript.carryingTheOrb = true;
        rigid.useGravity = false;
        rigid.isKinematic = true;
        StopAllCoroutines();
    }
    /// <summary>
    /// bindet den orb an den ort des auftreffens
    /// </summary>
    /// <returns></returns>
    IEnumerator beeingStuck()
    {
        //sound aufschlag vom Orb;
        trail.time = 0;
        StopCoroutine("Flytime");
        while (true) 
        {
            
            rigid.velocity = Vector3.zero;
            if (Vector3.Distance(playerTransform.position, transform.position) < collectingDistance*2)
            {
//GetCollected();
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator FlyTime()
    {
        Sound_Manager.Instance.Play_At("Orb Throw", audio1, true);
        while (true)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }

    public IEnumerator FlyToPlayer()
    {
        Sound_Manager.Instance.Play_At("Orb Call Back", audio1, true);
        while (!playerScript.carryingTheOrb)
        {
            playerScript.orbEnergy = 0;
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, 16*Time.deltaTime);
            if (Vector3.Distance(playerTransform.position, transform.position) < collectingDistance*3)
            {
                
                GetCollected();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!colided)
        {
            Sound_Manager.Instance.Play_At("Orb Hit", audio1, true);
            colided = true;
            rigid.velocity = Vector3.zero;
            transform.position = collision.GetContact(0).point;
            StartCoroutine(beeingStuck());
            rigid.useGravity = false;
            //hier kommt der fall hin das die Kugel den boden trifft
            calculateAlleParameter( Instantiate(TowerPrefab, collision.GetContact(0).point - Vector3.up*.3f, Quaternion.identity),collision.contacts[0].normal);
        }
    }

    void calculateAlleParameter(GameObject tower,Vector3 normal)
    {
        Tower.Player_Inputs inputs;
        Tower towerscript = tower.GetComponent<Tower>();
        RaycastHit hit;
        Physics.Raycast(playerTransform.position, Vector3.down, out hit, 2);
        inputs.throw_time = timer;
        inputs.throw_dist = Vector3.Distance(transform.position, playerTransform.position);
        inputs.player_dir = playerScript.lookDirection;
        inputs.player_speed = Vector3.Magnitude(playerScript.velocity);
        inputs.hit_normal = normal;
        if (hit.collider != null)
        {
            inputs.ground_tag = hit.collider.tag;
        }
        else
        {
            inputs.ground_tag = null;
        }
        inputs.orb_energy = playerScript.orbEnergy;
        playerScript.orbEnergy = 0;
        towerscript.Initialize(inputs);
    }
}
