using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tower_Management;

public static class ProceduralKabiumGenerator 
{

    Kambium JonathansKabiumAlgo(Building at_building)
    {
        // raycast to main collider
        var dir = new Vector3(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(0, 2) == 0 ? 1 : 0, Random.Range(0, 2) == 0 ? 1 : -1).normalized;
        var ray = new Ray(at_building.transform.position + dir, -dir);
        var hit = new RaycastHit();
        at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);

        return new Kambium(hit.point, hit.normal);
    }


    public Kambium Calculate_Kambium(KabiumAlgorithm kabiumAlgorithm, Building at_building)
    {
        if(KabiumAlgorithm.JonathansAlgo == kabiumAlgorithm)
        {
            JonathansKabiumAlgo(at_building);
        }
    }
}

public enum KabiumAlgorithm
{
    JonathansAlgo;
}
