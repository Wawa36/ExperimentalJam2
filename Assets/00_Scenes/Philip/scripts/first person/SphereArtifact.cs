﻿using System.Collections;
using System.Collections.Generic;
using Tower_Management;
using UnityEngine;

public class SphereArtifact : MonoBehaviour
{
    [SerializeField] GameObject TowerPrefab;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform targetPosition;
    [SerializeField] float collectingDistance;
    [HideInInspector] public SphereCollider collider;
    PlayerMovement playerScript;
    Rigidbody rigid;
    bool colided;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible=false;
        Vector3 cameraPosition = Camera.main.ViewportToWorldPoint(new Vector3(1 , 0,1));
        targetPosition.position = cameraPosition;
        transform.localPosition = Vector3.zero;
        playerScript = playerTransform.GetComponent<PlayerMovement>();
        rigid = GetComponent<Rigidbody>();
        collider = GetComponent<SphereCollider>();
    }

    /// <summary>
    /// wird aufgerufen wenn die Kugel eingesammelt werden soll
    /// 
    /// </summary>
    public void GetCollected()
    {
        playerScript.throwForce = 0;
        collider.enabled = true;
        transform.parent = targetPosition;
        transform.localPosition = Vector3.zero;
        playerScript.carryingTheOrb = true;
        rigid.useGravity = false;
        rigid.isKinematic = true;
        colided = false;
        StopAllCoroutines();
    }
    /// <summary>
    /// bindet den orb an den ort des auftreffens
    /// </summary>
    /// <returns></returns>
    IEnumerator beeingStuck()
    {
        StopCoroutine(FlyTime());
        while (true) 
        {
            rigid.velocity = Vector3.zero;
            if (Vector3.Distance(playerTransform.position, transform.position) < collectingDistance*2)
            {
                GetCollected();
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator FlyTime()
    {

        while (true)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }

    public IEnumerator FlyToPlayer()
    {
        while (!playerScript.carryingTheOrb)
        {
            transform.position = Vector3.Lerp(transform.position, playerScript.transform.position, .2f);
            if (Vector3.Distance(playerTransform.position, transform.position) < collectingDistance*2)
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

            colided = true;
            rigid.velocity = Vector3.zero;
            transform.position = collision.GetContact(0).point;
            StartCoroutine(beeingStuck());
            rigid.useGravity = false;

            if (collision.gameObject.CompareTag("Ground"))
            {
                //hier kommt der fall hin das die Kugel den boden trifft
                GameObject tower = Instantiate(TowerPrefab, collision.GetContact(0).point - Vector3.up*.1f, Quaternion.identity);
                Tower.Player_Inputs inputs;
                Tower towerscript = tower.GetComponent<Tower>();
                RaycastHit hit;
                Physics.Raycast(playerTransform.position, Vector3.down, out hit, 2);
                inputs.throw_time = timer;
                inputs.throw_dist = Vector3.Distance(transform.position, playerTransform.position);
                inputs.player_dir = Camera.main.transform.forward;
                inputs.player_speed = Vector3.Magnitude(playerScript.GetComponent<Rigidbody>().velocity);
                if (hit.collider != null)
                {
                    inputs.ground_tag = hit.collider.tag;
                }
                else
                {
                    inputs.ground_tag = null;
                }
                inputs.orb_energy = playerScript.throwForce;
                towerscript.Initialize(inputs);


            }
            else if (collision.gameObject.CompareTag("Building"))
            {
                // hier kommt der fall hin das die Kugel ein Gebäude trifft
                Instantiate(TowerPrefab, collision.GetContact(0).point - Vector3.up * .1f, Quaternion.identity);
            }
        }
    }

}
