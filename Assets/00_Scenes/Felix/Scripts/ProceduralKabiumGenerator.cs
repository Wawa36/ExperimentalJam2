﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tower_Management;

public class ProceduralKabiumGenerator 
{

    Tower.Kambium[] JonathansKabiumAlgo(Building at_building)
    {
        List<Tower.Kambium> kambiumList = new List<Tower.Kambium>();

        // raycast to main collider
        var dir = new Vector3(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(0, 2) == 0 ? 1 : 0, Random.Range(0, 2) == 0 ? 1 : -1).normalized;
        var ray = new Ray(at_building.transform.position + dir, -dir);
        var hit = new RaycastHit();
        at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);

        kambiumList.Add(new Tower.Kambium(hit.point, hit.normal));

        return kambiumList.ToArray();
    }


    public Tower.Kambium[] Calculate_Kambium(KabiumAlgorithm kabiumAlgorithm, Building at_building)
    {
        if(KabiumAlgorithm.JonathansAlgo == kabiumAlgorithm)
        {
            return JonathansKabiumAlgo(at_building);
        }
        else //Default
        {
            return JonathansKabiumAlgo(at_building);
        }
    }
}

public enum KabiumAlgorithm
{
    JonathansAlgo,
    Default
}
