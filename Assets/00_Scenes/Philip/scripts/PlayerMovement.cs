﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rigid;
    Rigidbody orbRigid;
    SphereArtifact orbScript;
    public bool carryingTheOrb;
    [SerializeField] GameObject orb;
    [SerializeField] Transform cameraRigTransform;

    [SerializeField] float movespeed;
    [SerializeField] float rotationSpeed;

    [SerializeField] float throwForce;
    

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        orbRigid = orb.GetComponent<Rigidbody>();
        orbScript = orb.GetComponent<SphereArtifact>();
        carryingTheOrb = true;
    }
    private void Update()
    {
        Move();
        Throw();
        Teleport();
    }
    /// <summary>
    /// wandelt den input in die bewegung des players um
    /// </summary>
    void Move()
    {
        float XAxis = Input.GetAxis("Horizontal");
        float YAxis = Input.GetAxis("Vertical");

        rigid.MovePosition(transform.position + transform.forward * YAxis * Time.deltaTime * movespeed + transform.right * XAxis * Time.deltaTime * movespeed);
       

    }
    /// <summary>
    /// wirft den orb in richtung vector3.forward des Spielers
    /// </summary>
    void Throw()
    {
        if (carryingTheOrb && Input.GetMouseButtonDown(0))
        {

            carryingTheOrb = false;
            orbRigid.isKinematic = false;
            orbRigid.useGravity = true;
            orbRigid.AddForce(cameraRigTransform.forward * 100 *throwForce);
            orb.transform.parent = null;

        }
    }
    /// <summary>
    /// teleportiert den Spieler zum Orb
    /// </summary>
    void Teleport()
    {
        if (Input.GetMouseButtonDown(1) && !carryingTheOrb)
        {
            transform.position =orb.transform.position + Vector3.up;
            rigid.velocity = Vector3.zero;
            
            orbScript.GetCollected();
        }
    }
    bool IsOnTheGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down,2, 0))
        {
            return true;
        }
        else
        return false;

    }

}
