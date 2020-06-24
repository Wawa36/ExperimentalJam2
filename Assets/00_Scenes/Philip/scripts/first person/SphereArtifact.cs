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
    [SerializeField] float collectingDistance;

    [SerializeField] Color color1;
    [SerializeField] Color color2;

    [HideInInspector] public new SphereCollider collider;
    [SerializeField] PlayerMovement playerScript;
    Rigidbody rigid;
    bool colided;
    public float timer;
    Material material;
    [SerializeField] MeshRenderer meshR;
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
    private void OnEnable()
    {

        changeColor();
    }

    public void changeColor()
    {
            meshR.material.color = Color.Lerp(color1, color2, playerScript.orbEnergy / playerScript.throwingForce);

    }

    /// <summary>
    /// wird aufgerufen wenn die Kugel eingesammelt werden soll
    /// 
    /// </summary>
    public void GetCollected()
    {
        meshR.material.color = color1;
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
        StopCoroutine("Flytime");
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
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, .4f);
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
               calculateAlleParameter( Instantiate(TowerPrefab, collision.GetContact(0).point - Vector3.up*.1f, Quaternion.identity),collision.contacts[0].normal);
               

            }
            else if (collision.gameObject.CompareTag("Building"))
            {
                // hier kommt der fall hin das die Kugel ein Gebäude trifft
               calculateAlleParameter( Instantiate(TowerPrefab, collision.GetContact(0).point - Vector3.up * .1f, Quaternion.identity),collision.contacts[0].normal);

            }
            
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
        towerscript.Initialize(inputs);
    }
}
