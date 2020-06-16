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


    static Tower.Cambiums_At_Active BaobabTree(Building at_building, Tower tower)
    {
        Transform buildingTransform = at_building.Main_Collider.transform;

        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        if (HasStillSteps(at_building))
        {
            
            kambiumList.Add(new Tower.Cambium(buildingTransform.position + (buildingTransform.up * buildingTransform.localScale.y / 2),
                                             buildingTransform.up,
                                             //tower.Building_Prefabs[at_building.Cambium.steps - 1],
                                             tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                             at_building.Cambium.steps)); //same steps, the tower counts them down

            return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
        }
        else //split in 4 if 0 steps
        {
            Vector3 backRightPos = buildingTransform.position + (buildingTransform.forward * buildingTransform.localScale.z / 2) + (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
            Vector3 backLeftPos = buildingTransform.position + (buildingTransform.forward * buildingTransform.localScale.z / 2) - (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
            Vector3 frontRightPos = buildingTransform.position - (buildingTransform.forward * buildingTransform.localScale.z / 2) + (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
            Vector3 frontLeftPos = buildingTransform.position - (buildingTransform.forward * buildingTransform.localScale.z / 2) - (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);

            List<Vector3> positions = new List<Vector3>();
            positions.Add(backRightPos); positions.Add(backLeftPos); positions.Add(frontRightPos); positions.Add(frontLeftPos);

            bool hasStartedOne = false;

            if (Random.value > 0.75) // 1 chance von 4 dass es weiter geht
            {
                //back right
                kambiumList.Add(new Tower.Cambium(backRightPos, buildingTransform.up, tower.Building_Prefabs[3], 4));
                hasStartedOne = true;
            }

            if (Random.value > 0.75) // 1 chance von 4 dass es weiter geht
            {
                //back right
                kambiumList.Add(new Tower.Cambium(backLeftPos, buildingTransform.up, tower.Building_Prefabs[3], 4));
                hasStartedOne = true;
            }

            if (Random.value > 0.75) // 1 chance von 4 dass es weiter geht
            {
                //back right
                kambiumList.Add(new Tower.Cambium(frontRightPos, buildingTransform.up, tower.Building_Prefabs[3], 4));
                hasStartedOne = true;
            }

            if (Random.value > 0.75) // 1 chance von 4 dass es weiter geht
            {
                //back right
                kambiumList.Add(new Tower.Cambium(frontLeftPos, buildingTransform.up, tower.Building_Prefabs[3], 4));
                hasStartedOne = true;
            }

            if(!hasStartedOne) //falls keiner gestarted wurden ist
            {
                kambiumList.Add(new Tower.Cambium(positions[Random.Range(0, positions.Count)], buildingTransform.up, tower.Building_Prefabs[3], 4));
            }
            

            return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
        }


    }


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
        else if(KabiumAlgorithm.BoababTree == kabiumAlgorithm)
        {
            return BaobabTree(at_building, tower);
        }
        else if(KabiumAlgorithm.JonathansAlgo == kabiumAlgorithm)
        {
            return JonathansKabiumAlgo(at_building, tower);
        }
        else //Default
        {
            return JonathansKabiumAlgo(at_building, tower);
        }
    }
}
public enum KabiumAlgorithm
{
    BoababTree,
    SimpleLSystem,
    JonathansAlgo,
    Default
}