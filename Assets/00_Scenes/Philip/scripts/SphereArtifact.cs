using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereArtifact : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float collectingDistance;
    PlayerMovement playerScript;
    Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = playerTransform.GetComponent<PlayerMovement>();
        rigid = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// wird aufgerufen wenn die Kugel eingesammelt werden soll
    /// 
    /// </summary>
    public void GetCollected()
    {
        transform.localPosition=Vector3.forward;
        playerScript.carryingTheOrb = true;
        rigid.isKinematic = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetCollected();
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            //hier kommt der fall hin das die Kugel den boden trifft
        }
        else if (collision.gameObject.CompareTag("Building"))
        {
            // hier kommen die fälle hin, wenn der orb ein gebäude trifft und es entweder aktiv ist oder nicht

        }
    }
}
