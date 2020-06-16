using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tower_Management;

static class JonathanAlgorithmus
{
    static Tower.Cambiums_At_Active JonathansKabiumAlgo(Building at_building, Tower tower)
    {
        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        // raycast to main collider
        Vector3 dir;

        if (at_building.Cambium.steps > 0)
        {
            dir = at_building.Cambium.normal;
        }
        else
        {
            dir = new Vector3(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(0, 2) == 0 ? 1 : 0, Random.Range(0, 2) == 0 ? 1 : -1).normalized;
        }

        var ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
        var hit = new RaycastHit();
        at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);

        kambiumList.Add(new Tower.Cambium(hit.point, hit.normal, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], at_building.Cambium.steps > 0 ? at_building.Cambium.steps - 1 : 0));

        return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
    }
}
