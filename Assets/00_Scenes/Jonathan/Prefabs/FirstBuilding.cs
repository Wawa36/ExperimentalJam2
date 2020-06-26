using System.Collections.Generic;
using Tower_Management;
using UnityEngine;

public class FirstBuilding : Building
{
    [Header("First Building Parameter")]
    [SerializeField] bool lerp_animation;

    public override void On_Update_Growth(float speed)
    {
        if (Mesh.transform.parent.localScale.z < 1)
        {
            if (lerp_animation)
            {
                Mesh.transform.parent.localScale = Vector3.Lerp(Mesh.transform.parent.localScale, new Vector3(1, 1, 1), Time.deltaTime * speed);

                if (Mesh.transform.parent.localScale.z >= 0.99f)
                    Mesh.transform.parent.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                Mesh.transform.parent.localScale = Vector3.MoveTowards(Mesh.transform.parent.localScale, new Vector3(1, 1, 1), Time.deltaTime * speed);
            }
        }
        else
            Deactivate();
    }
}
