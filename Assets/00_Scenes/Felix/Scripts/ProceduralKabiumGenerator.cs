using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tower_Management;

public static class ProceduralKabiumGenerator 
{
    static Tower.Cambium[] SimpleLSystemGrow(Building at_building)
    {
        float angle = 45;
        //rule F[+F]F[-F]F

        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        kambiumList.Add(new Tower.Cambium(at_building.transform.position + (at_building.transform.up * at_building.transform.localScale.y / 2), at_building.transform.up));

        Vector3 turnAngle = at_building.transform.up * at_building.transform.localScale.y / 2;
        turnAngle = Quaternion.Euler(0, angle, 0) * turnAngle;

        kambiumList.Add(new Tower.Cambium(at_building.transform.position + turnAngle, at_building.transform.up));

        return kambiumList.ToArray();
    }

    static Tower.Cambium[] JonathansKabiumAlgo(Building at_building)
    {
        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        // raycast to main collider
        var dir = new Vector3(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(0, 2) == 0 ? 1 : 0, Random.Range(0, 2) == 0 ? 1 : -1).normalized;
        var ray = new Ray(at_building.transform.position + dir * 100f, -dir);
        var hit = new RaycastHit();
        at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);

        kambiumList.Add(new Tower.Cambium(hit.point, hit.normal));

        return kambiumList.ToArray();
    }

    static public Tower.Cambium[] Calculate_Kambium(KabiumAlgorithm kabiumAlgorithm, Building at_building)
    {
        if(KabiumAlgorithm.SimpleLSystem == kabiumAlgorithm)
        {
            return SimpleLSystemGrow(at_building);
        }
        else //Default
        {
            return JonathansKabiumAlgo(at_building);
        }
    }
}
public enum KabiumAlgorithm
{
    SimpleLSystem,
    JonathansAlgo,
    Default
}