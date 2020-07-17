using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World_End : MonoBehaviour
{
    [SerializeField] float at_y_position;

    private void Update()
    {
        if (PlayerMovement.Instance.transform.position.y < at_y_position)
            StartCoroutine(PlayerMovement.Instance.Teleporting(PlayerMovement.Instance.transform.position, PlayerMovement.Instance.LastGroundedPlace + new Vector3(0, 5, 0)));
    }
}
