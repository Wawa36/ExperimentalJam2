using System.Collections.Generic;
using Tower_Management;
using UnityEngine;

public class FirstBuilding : Building
{
    float timer = 0;

    public override void On_Update_Growth(float speed)
    {
        if (timer >= 0.01f)
            Deactivate();
        else
            timer += Time.deltaTime;
    }
}
