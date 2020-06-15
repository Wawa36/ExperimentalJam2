using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereArtifact : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] float collectingDistance;
    PlayerMovement playerScript;
    Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible=false;
            
        playerScript = playerTransform.GetComponent<PlayerMovement>();
        rigid = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// wird aufgerufen wenn die Kugel eingesammelt werden soll
    /// 
    /// </summary>
    public void GetCollected()
    {
        transform.parent = cameraTransform;
        transform.localPosition=Vector3.forward;
        playerScript.carryingTheOrb = true;
        rigid.useGravity = false;
        rigid.isKinematic = true;
        StopAllCoroutines();
    }

    IEnumerator beeingStuck()
    {
        while (true) 
        {


            rigid.velocity = Vector3.zero;
            yield return new WaitForEndOfFrame();
        }
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
            rigid.velocity = Vector3.zero;
            StartCoroutine(beeingStuck());
            rigid.useGravity = false;


        }
        else if (collision.gameObject.CompareTag("Building"))
        {
            // hier kommt der fall hin das die Kugel ein Gebäude trifft
            rigid.velocity = Vector3.zero;
            StartCoroutine(beeingStuck());
            rigid.useGravity = false;
        }
    }

}
