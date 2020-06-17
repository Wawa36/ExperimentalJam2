using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereArtifact : MonoBehaviour
{
    [SerializeField] GameObject TowerPrefab;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform targetPosition;
    [SerializeField] float collectingDistance;
    PlayerMovement playerScript;
    Rigidbody rigid;
    bool colided;
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
    }

    /// <summary>
    /// wird aufgerufen wenn die Kugel eingesammelt werden soll
    /// 
    /// </summary>
    public void GetCollected()
    {
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
                Instantiate(TowerPrefab, collision.GetContact(0).point - Vector3.up*.1f, Quaternion.identity);


            }
            else if (collision.gameObject.CompareTag("Building"))
            {
                // hier kommt der fall hin das die Kugel ein Gebäude trifft
                Instantiate(TowerPrefab, collision.GetContact(0).point - Vector3.up * .1f, Quaternion.identity);
            }
        }
    }

}
