using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tower_Management;
using System.Runtime.InteropServices;

public static class ProceduralKabiumGenerator 
{ 
    static public bool CheckBuildingPossible(Tower.Cambium newCambium, Building at_building = null)
    {
        GameObject prefab = newCambium.prefab;

        // rotate
        prefab.transform.forward = newCambium.normal;


        //pos
        prefab.transform.position = newCambium.point;


        Debug.Log("p" + prefab.transform.position);

        BoxCollider prefabBoxCollider = (BoxCollider) prefab.GetComponent<Building>().Main_Collider;
        Vector3 boxCenter = prefab.transform.TransformPoint(prefabBoxCollider.center); //---> from local to world pos

        Debug.Log("b" + boxCenter);

        RaycastHit hit;

        //debug
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = boxCenter;
        cube.transform.localScale = prefab.transform.localScale / 2;
        cube.GetComponent<MeshRenderer>().material.color = Color.magenta;

        if (Physics.BoxCast(boxCenter, prefab.transform.localScale / 2, newCambium.normal, out hit))
        {
            if(hit.collider.transform.GetComponent<Building>().Tower == at_building.Tower) //mein eigenes Gebäude
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        else //hit nothing
        {
            return true;
        }

    }


    static public Tower.Cambiums_At_Active Calculate_Kambium(KabiumAlgorithm kabiumAlgorithm, Building at_building, Tower tower)
    {
        if (KabiumAlgorithm.SimpleLSystem == kabiumAlgorithm)
        {
            return ProceduralKabiumGeneratorFelix.SimpleLSystemGrow(at_building, tower);
        }
        else if (KabiumAlgorithm.BoababTree == kabiumAlgorithm)
        {
            return ProceduralKabiumGeneratorFelix.BaobabTree(at_building, tower);
        }
        else if (KabiumAlgorithm.FrangipaniTree == kabiumAlgorithm)
        {
            return ProceduralKabiumGeneratorFelix.FrangipaniTree(at_building, tower);
        }
        else if (KabiumAlgorithm.RBunker == kabiumAlgorithm)
        {
            return JonathanAlgorithmus.RBunker(at_building, tower);
        }
        else if (KabiumAlgorithm.RBunkerBranches == kabiumAlgorithm)
        {
            return JonathanAlgorithmus.RBunkerBranch(at_building, tower);
        }
        else if (KabiumAlgorithm.StairGrow == kabiumAlgorithm)
        {
            return JonathanAlgorithmus.StairGrow(at_building, tower);
        }
        else if (KabiumAlgorithm.StreetGrow == kabiumAlgorithm)
        {
            return JonathanAlgorithmus.StreetGrow(at_building, tower);
        }
        else
            return JonathanAlgorithmus.RBunker(at_building, tower);
    }
}
public enum KabiumAlgorithm
{
    BoababTree,
    FrangipaniTree,
    SimpleLSystem,
    RBunker,
    RBunkerBranches,
    StairGrow,
    StreetGrow,
    Default
}