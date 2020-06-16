using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tower_Management;
using System.Runtime.InteropServices;

public static class ProceduralKabiumGenerator 
{ 
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
    Default
}