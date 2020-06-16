using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb_ThirdPerson : MonoBehaviour
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
        Cursor.visible = false;

        playerScript = playerTransform.GetComponent<PlayerMovement>();
        rigid = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// wird aufgerufen wenn die Kugel eingesammelt werden soll
    /// 
    /// </summary>
    public void GetCollected()
    {
        transform.parent = playerTransform;
        transform.localPosition = new Vector3(.8f, .3f, 0);
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
        while (true)
        {
            rigid.velocity = Vector3.zero;
            if (Vector3.Distance(playerTransform.position, transform.position) < collectingDistance)
            {
                GetCollected();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rigid.velocity = Vector3.zero;
        StartCoroutine(beeingStuck());
        rigid.useGravity = false;
        if (collision.gameObject.CompareTag("Ground"))
        {
            //hier kommt der fall hin das die Kugel den boden trifft



        }
        else if (collision.gameObject.CompareTag("Building"))
        {
            // hier kommt der fall hin das die Kugel ein Gebäude trifft

        }
    }
}
