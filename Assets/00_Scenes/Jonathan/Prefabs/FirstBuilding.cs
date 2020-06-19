using System.Collections.Generic;
using Tower_Management;
using UnityEngine;

public class FirstBuilding : Building
{
    [Header("First Building Parameter")]
    [SerializeField] bool lerp_animation;

    public override void On_Update_Growth(float speed)
    {
        if (Main_Collider.transform.parent.localScale.z < 1)
        {
            if (lerp_animation)
            {
                Main_Collider.transform.parent.localScale = Vector3.Lerp(Main_Collider.transform.parent.localScale, new Vector3(1, 1, 1), Time.deltaTime * speed);

                if (Main_Collider.transform.parent.localScale.z >= 0.99f)
                    Main_Collider.transform.parent.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                Main_Collider.transform.parent.localScale = Vector3.MoveTowards(Main_Collider.transform.parent.localScale, new Vector3(1, 1, 1), Time.deltaTime * speed);
            }
        }
        else
            Deactivate();
    }
}
