using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tower_Management;
using System.Runtime.InteropServices;

public static class ProceduralKabiumGenerator 
{ 
    private static bool HasStillSteps(Building at_building)
    {
        if(at_building.Cambium.steps > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    static Tower.Cambiums_At_Active SimpleLSystemGrow(Building at_building, Tower tower)
    {
        float angle = 45;
        //rule F[+F]F[-F]F

        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        if (Random.value > 0.25)
        {
            kambiumList.Add(new Tower.Cambium(at_building.transform.position + (at_building.transform.up * at_building.transform.localScale.y), at_building.transform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], 0));
        }
        else
        {
            Vector3 turnAngle = at_building.transform.up * at_building.transform.localScale.y;
            turnAngle = Quaternion.Euler(angle, 0, 0) * turnAngle;

            kambiumList.Add(new Tower.Cambium(at_building.transform.position + turnAngle, turnAngle, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], 0));
        }

        return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
    }
    
    /*
    static Tower.Cambium[] FelixKabiumAlgo(Building at_building, Tower tower)
    {
        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        if (HasStillSteps(at_building))
        {
            kambiumList.Add(new Tower.Cambium(at_building.transform.position + at_building.Cambium.normal, at_building.transform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], 0));

            return kambiumList.ToArray();
        }
    }
    */

    static Tower.Cambiums_At_Active JonathansKabiumAlgo(Building at_building, Tower tower)
    {
        return JonathanAlgorithmus.JonathansKabiumAlgo(at_building, tower);
    }

    static public Tower.Cambiums_At_Active Calculate_Kambium(KabiumAlgorithm kabiumAlgorithm, Building at_building, Tower tower)
    {
        if(KabiumAlgorithm.SimpleLSystem == kabiumAlgorithm)
        {
            return SimpleLSystemGrow(at_building, tower);
        }
        else //Default
        {
            return JonathansKabiumAlgo(at_building, tower);
        }
    }
}
public enum KabiumAlgorithm
{
    SimpleLSystem,
    JonathansAlgo,
    Default
}