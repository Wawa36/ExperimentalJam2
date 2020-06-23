﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tower_Management;

public class ProceduralKabiumGeneratorFelix 
{
    private static bool HasStillSteps(Building at_building)
    {
        if (at_building.Cambium.steps > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static float Remap(float value, float inputFrom, float inputTo, float outputFrom, float outputTo)
    {

        return (value - inputFrom) / (inputTo - inputFrom) * (outputTo - outputFrom) + outputFrom;

    }


    static Dictionary<Tower, int> towerAndNeighbours = new Dictionary<Tower, int>();
    static Dictionary<Tower, int> towerTurnNumber = new Dictionary<Tower, int>();
    static Dictionary<Tower, List<Tower.Cambium>> towerAndLatestMainCambiums = new Dictionary<Tower, List<Tower.Cambium>>();

    public static Tower.Cambiums_At_Active SimpleLSystemGrow(Building at_building, Tower tower)
    {
        float angle = 45;

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


    #region newBaobabTree
    static Dictionary<Tower, int> towerAndBranches = new Dictionary<Tower, int>();
    static Dictionary<Tower, int> towerAndCambiumAmount = new Dictionary<Tower, int>();
    static Dictionary<Tower, Dictionary<int, int>> towerAndBranchDyingGeneration = new Dictionary<Tower, Dictionary<int, int>>();

    private static int CambiumGenerations(Building at_building, int generations = 0)
    {
        if(at_building.Parent_Building == null)
        {
            return generations;
        }
        else
        {
            return CambiumGenerations(at_building.Parent_Building, generations + 1);
        }
    }

    static Dictionary<Tower, int> numberOfNos = new Dictionary<Tower, int>();
    public static Tower.Cambiums_At_Active BaobabTree(Building at_building, Tower tower)
    {
        // Init Towers Dictionarres ------------------------------------------------

        if (!towerAndBranches.ContainsKey(tower))
        {
            towerAndBranches.Add(tower, 1);
        }

        if (!towerAndCambiumAmount.ContainsKey(tower))
        {
            towerAndCambiumAmount.Add(tower, 1);
        }

        if (!towerAndBranchDyingGeneration.ContainsKey(tower))
        {
            Dictionary<int, int> branchDyingGeneration = new Dictionary<int, int>();
            branchDyingGeneration.Add(0,-1); 
            towerAndBranchDyingGeneration.Add(tower, branchDyingGeneration);
        }


        //Set Params ---------------------------------------------------------------

        int maxCambiumsPerBranch = Mathf.Clamp(tower.Mapper.Width, 1, 40); //between 1 and 40 ?
        int maxCambiums = maxCambiumsPerBranch * towerAndBranches[tower];

        int splitAfterGenerations = 30;//tower.Mapper.Split_Chance; // wie genaue werden die werte noch verarbeitet?
        

        //Prepare other Variables ------------------------------------------------

        Transform buildingTransform = at_building.Main_Collider.transform;
        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        Vector3 backRightPos = buildingTransform.position + (buildingTransform.forward * buildingTransform.localScale.z / 2) + (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
        Vector3 backLeftPos = buildingTransform.position + (buildingTransform.forward * buildingTransform.localScale.z / 2) - (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
        Vector3 frontRightPos = buildingTransform.position - (buildingTransform.forward * buildingTransform.localScale.z / 2) + (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
        Vector3 frontLeftPos = buildingTransform.position - (buildingTransform.forward * buildingTransform.localScale.z / 2) - (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);

        List<Vector3> positions = new List<Vector3>();
        positions.Add(backRightPos); positions.Add(backLeftPos); positions.Add(frontRightPos); positions.Add(frontLeftPos);


        //DEBUG -----------------------------------------------------------------
        Debug.Log("---------------------------------------- new cambium ---------------------------");
        Debug.Log("max cambiums "+ maxCambiums);
        Debug.Log("nbr of cambiums " + towerAndCambiumAmount[tower]);
        Debug.Log("nbr of branches " + towerAndBranches[tower]);
        Debug.Log("ID? " + at_building.Cambium.branch_ID);



        //Algorithm --------------------------------------------------------------

        Dictionary<int, int> thisTowersBranchGenerations = towerAndBranchDyingGeneration[tower];
        int cambiumGeneration = CambiumGenerations(at_building);

        if (cambiumGeneration != thisTowersBranchGenerations[at_building.Cambium.branch_ID])
        {
            if (cambiumGeneration % splitAfterGenerations == 0 && cambiumGeneration != 0) // Do Split
            {
                int newBranchID = towerAndBranches[tower]; //das was drin steht erhöt
                if (Random.value > 0.5) //L - R
                {
                    kambiumList.Add(new Tower.Cambium(positions[0] + ((buildingTransform.position + new Vector3(0, buildingTransform.localScale.y / 2, 0)) - positions[0]),
                                    ((buildingTransform.position + new Vector3(0, buildingTransform.localScale.y / 2, 0)) - positions[0]),
                                    tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                    5,
                                    at_building.Cambium.branch_ID)); //behalt den selben ID

                    kambiumList.Add(new Tower.Cambium(positions[3] + ((buildingTransform.position + new Vector3(0, buildingTransform.localScale.y / 2, 0)) - positions[3]),
                                    ((buildingTransform.position + new Vector3(0, buildingTransform.localScale.y / 2, 0)) - positions[3]),
                                    tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                    5,
                                    newBranchID)); //neuer ID

                }
                else //B - F
                {
                    kambiumList.Add(new Tower.Cambium(positions[1] + ((buildingTransform.position + new Vector3(0, buildingTransform.localScale.y / 2, 0)) - positions[1]),
                                    ((buildingTransform.position + new Vector3(0, buildingTransform.localScale.y / 2, 0)) - positions[1]),
                                    tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                    5,
                                    at_building.Cambium.branch_ID)); //behalt den selben ID

                    kambiumList.Add(new Tower.Cambium(positions[2] + ((buildingTransform.position + new Vector3(0, buildingTransform.localScale.y / 2, 0)) - positions[2]),
                                    ((buildingTransform.position + new Vector3(0, buildingTransform.localScale.y / 2, 0)) - positions[2]),
                                    tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                    5,
                                    newBranchID)); //neuer ID
                }

                //Die generation überschreiben, set die generation for this branch
                thisTowersBranchGenerations[at_building.Cambium.branch_ID] = cambiumGeneration;
                towerAndBranchDyingGeneration[tower] = thisTowersBranchGenerations; //write back

                //Neue Branch
                towerAndCambiumAmount[tower]++;
                towerAndBranches[tower]++;

                Dictionary<int, int> newBranchDyingGeneration = new Dictionary<int, int>();
                towerAndBranchDyingGeneration[tower].Add(newBranchID, 0);

                return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
            }
            else
            {
                if (towerAndCambiumAmount[tower] <= maxCambiums)
                {
                    if (HasStillSteps(at_building)) //fullfill steps
                    {
                        Tower.Cambium newCambium = new Tower.Cambium(buildingTransform.position + (at_building.Cambium.normal.normalized * buildingTransform.localScale.y / 2),
                                                         //Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * at_building.Cambium.normal,
                                                         at_building.Cambium.normal,
                                                         tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                                         at_building.Cambium.steps,
                                                         at_building.Cambium.branch_ID);
                        kambiumList.Add(newCambium);

                        return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
                    }
                    else //grow larger
                    {
                        int countNewCambiums = 0;

                        bool hasStartedOne = false;

                        if (Random.value > 0.25)
                        {
                            //back right
                            kambiumList.Add(new Tower.Cambium(backRightPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps, at_building.Cambium.branch_ID));
                            hasStartedOne = true;
                            countNewCambiums++;
                        }

                        if (Random.value > 0.25)
                        {
                            //back left
                            kambiumList.Add(new Tower.Cambium(backLeftPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps, at_building.Cambium.branch_ID));
                            hasStartedOne = true;
                            countNewCambiums++;
                        }

                        if (Random.value > 0.25)
                        {
                            //front right
                            kambiumList.Add(new Tower.Cambium(frontRightPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps, at_building.Cambium.branch_ID));
                            hasStartedOne = true;
                            countNewCambiums++;
                        }

                        if (Random.value > 0.25)
                        {
                            //front left
                            kambiumList.Add(new Tower.Cambium(frontLeftPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps, at_building.Cambium.branch_ID));
                            hasStartedOne = true;
                            countNewCambiums++;
                        }

                        if (!hasStartedOne) //falls keiner gestarted wurden ist
                        {
                            kambiumList.Add(new Tower.Cambium(positions[Random.Range(0, positions.Count)], buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps, at_building.Cambium.branch_ID));
                            countNewCambiums++;
                        }


                        towerAndCambiumAmount[tower] += countNewCambiums;

                        return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
                    }
                }
                else //STOP because to many cambiums
                {
                    towerAndCambiumAmount[tower]--; //dieses Kabium hört auf
                    return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
                }
            }
        }
        else //STOP because has to die
        {
           Debug.Log("I die " + at_building.Cambium.branch_ID + " and I'm from this gen " + thisTowersBranchGenerations[at_building.Cambium.branch_ID]);
            towerAndCambiumAmount[tower]--; //dieses Kabium hört auf
            return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
        }

    }

    #endregion

    #region oldBaobabTree

    /*
    public static Tower.Cambiums_At_Active BaobabTree(Building at_building, Tower tower)
    {
        
        if(!towerAndBranches.ContainsKey(tower))
        {
            towerAndBranches.Add(tower, 1);
        }

        

        Transform buildingTransform = at_building.Main_Collider.transform;

        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();


        if (towerAndBranches[tower] < 7)
        {
            if (HasStillSteps(at_building))
            {
                Tower.Cambium newCambium = new Tower.Cambium(buildingTransform.position + (buildingTransform.up * buildingTransform.localScale.y / 2),
                                                 buildingTransform.up,
                                                 //tower.Building_Prefabs[at_building.Cambium.steps - 1],
                                                 tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                                 at_building.Cambium.steps);
                kambiumList.Add(newCambium);

                return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
            }
            else //split in 4 if 0 steps
            {
                int countNewCambiums = 0;

                Vector3 backRightPos = buildingTransform.position + (buildingTransform.forward * buildingTransform.localScale.z / 2) + (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
                Vector3 backLeftPos = buildingTransform.position + (buildingTransform.forward * buildingTransform.localScale.z / 2) - (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
                Vector3 frontRightPos = buildingTransform.position - (buildingTransform.forward * buildingTransform.localScale.z / 2) + (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
                Vector3 frontLeftPos = buildingTransform.position - (buildingTransform.forward * buildingTransform.localScale.z / 2) - (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);

                List<Vector3> positions = new List<Vector3>();
                positions.Add(backRightPos); positions.Add(backLeftPos); positions.Add(frontRightPos); positions.Add(frontLeftPos);

                bool hasStartedOne = false;

                if (Random.value > 0.6) // 1 chance von 4 dass es weiter geht
                {
                    //back right
                    kambiumList.Add(new Tower.Cambium(backRightPos, buildingTransform.up, tower.Building_Prefabs[3], 4));
                    hasStartedOne = true;
                    countNewCambiums++;
                }

                if (Random.value > 0.6) // 1 chance von 4 dass es weiter geht
                {
                    //back right
                    kambiumList.Add(new Tower.Cambium(backLeftPos, buildingTransform.up, tower.Building_Prefabs[3], 4));
                    hasStartedOne = true;
                    countNewCambiums++;
                }

                if (Random.value > 0.6) // 1 chance von 4 dass es weiter geht
                {
                    //back right
                    kambiumList.Add(new Tower.Cambium(frontRightPos, buildingTransform.up, tower.Building_Prefabs[3], 4));
                    hasStartedOne = true;
                    countNewCambiums++;
                }

                if (Random.value > 0.6) // 1 chance von 4 dass es weiter geht
                {
                    //back right
                    kambiumList.Add(new Tower.Cambium(frontLeftPos, buildingTransform.up, tower.Building_Prefabs[3], 4));
                    hasStartedOne = true;
                    countNewCambiums++;
                }

                if (!hasStartedOne) //falls keiner gestarted wurden ist
                {
                    kambiumList.Add(new Tower.Cambium(positions[Random.Range(0, positions.Count)], buildingTransform.up, tower.Building_Prefabs[3], 4));
                    countNewCambiums++;
                }


                towerAndBranches[tower] += countNewCambiums - 1; //eins ist sowieso

                return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
            }
        }
        else
        {
            towerAndBranches[tower]--; //dieses Kabium hört auf
            return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
        }

        

    }
    */
    #endregion

    #region Ansatz1NewBaobabTree
    /*
    private static List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();
    public static Tower.Cambiums_At_Active BaobabTree(Building at_building, Tower tower)
    {
        //Branches
        if (!towerAndBranches.ContainsKey(tower))
        {
            towerAndBranches.Add(tower, 1);
        }
        int maxBranches = Mathf.Clamp(tower.Mapper.Split_Chance, 1, 10); //between 1 and 10

        //Neighbours
        if (!towerAndNeighbours.ContainsKey(tower))
        {
            towerAndNeighbours.Add(tower, 1);
        }
        int maxNeighbours = Mathf.Clamp(tower.Mapper.Width, 1, 4); //between 1 and 4

        //Cambiums
        if (!towerAndLatestMainCambiums.ContainsKey(tower))
        {
            List<Tower.Cambium> mainCambiums = new List<Tower.Cambium>();
            mainCambiums.Add(at_building.Cambium);
            towerAndLatestMainCambiums.Add(tower, mainCambiums);
        }


        Transform buildingTransform = at_building.Main_Collider.transform;
        BoxCollider buildingCollider = at_building.Main_Collider.GetComponent<BoxCollider>();
        kambiumList.Clear();

        //Check if Cambium from Building is a main Cambium, else don't work with it
        foreach(Tower.Cambium mainCambium in towerAndLatestMainCambiums[tower])
        {
            if(mainCambium.Equals(at_building.Cambium))
            {
                //Create the next Main Cambium (over the last one)
                Tower.Cambium newCambium = new Tower.Cambium(buildingTransform.position + (buildingTransform.up * buildingCollider.size.y / 2),
                                                 Quaternion.Euler(0, Random.Range(0,4) * 90,0) * buildingTransform.up, //random um 90 grad gedreht
                                                 tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                                 at_building.Cambium.steps);
                kambiumList.Add(newCambium);

                towerAndLatestMainCambiums[tower].Remove(mainCambium);
                towerAndLatestMainCambiums[tower].Add(newCambium);



                //create the neighbours around
                List<Building> newNeighbourBuildings = new List<Building>();

                for(int i = 0; i < maxNeighbours; i++)
                {

                    Vector3 backRightPos = buildingTransform.position + (buildingTransform.forward * buildingCollider.size.z / 2) + (buildingTransform.right * buildingCollider.size.x / 2);
                    Vector3 backLeftPos = buildingTransform.position + (buildingTransform.forward * buildingCollider.size.z / 2) - (buildingTransform.right * buildingCollider.size.x / 2);
                    Vector3 frontRightPos = buildingTransform.position - (buildingTransform.forward * buildingCollider.size.z / 2) + (buildingTransform.right * buildingCollider.size.x / 2);
                    Vector3 frontLeftPos = buildingTransform.position - (buildingTransform.forward * buildingCollider.size.z / 2) - (buildingTransform.right * buildingCollider.size.x / 2);

                    List<Vector3> positions = new List<Vector3>();
                    positions.Add(backRightPos); positions.Add(backLeftPos); positions.Add(frontRightPos); positions.Add(frontLeftPos);

                    //3 chance sur 4 
                    //make it 1 round, 2 rounds, 3 rounds
                    if (Random.value > 0.25) // 3 chance von 4 dass es weiter geht
                    {
                        //back right
                        kambiumList.Add(new Tower.Cambium(backRightPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));

                    }

                    if (Random.value > 0.25) // 3 chance von 4 dass es weiter geht
                    {
                        //back right
                        kambiumList.Add(new Tower.Cambium(backLeftPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));

                    }

                    if (Random.value > 0.25) // 3 chance von 4 dass es weiter geht
                    {
                        //back right
                        kambiumList.Add(new Tower.Cambium(frontRightPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));

                    }

                    if (Random.value > 0.25) // 3 chance von 4 dass es weiter geht
                    {
                        //back right
                        kambiumList.Add(new Tower.Cambium(frontLeftPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));

                    }

                }


                return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
            }

        }

        //if no main cambium existing
        return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());


        //todo
        
        if (towerAndBranches[tower] < maxBranches)
        {
            if (HasStillSteps(at_building))
            {
                Tower.Cambium newCambium = new Tower.Cambium(buildingTransform.position + (buildingTransform.up * buildingTransform.localScale.y / 2),
                                                 buildingTransform.up,
                                                 //tower.Building_Prefabs[at_building.Cambium.steps - 1],
                                                 tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                                 at_building.Cambium.steps);
                kambiumList.Add(newCambium);

                return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
            }
            else //split in 4 if 0 steps
            {
                int countNewCambiums = 0;

                Vector3 backRightPos = buildingTransform.position + (buildingTransform.forward * buildingTransform.localScale.z / 2) + (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
                Vector3 backLeftPos = buildingTransform.position + (buildingTransform.forward * buildingTransform.localScale.z / 2) - (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
                Vector3 frontRightPos = buildingTransform.position - (buildingTransform.forward * buildingTransform.localScale.z / 2) + (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
                Vector3 frontLeftPos = buildingTransform.position - (buildingTransform.forward * buildingTransform.localScale.z / 2) - (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);

                List<Vector3> positions = new List<Vector3>();
                positions.Add(backRightPos); positions.Add(backLeftPos); positions.Add(frontRightPos); positions.Add(frontLeftPos);

                bool hasStartedOne = false;

                if (Random.value > 0.6) // 1 chance von 4 dass es weiter geht
                {
                    //back right
                    kambiumList.Add(new Tower.Cambium(backRightPos, buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));
                    hasStartedOne = true;
                    countNewCambiums++;
                }

                if (Random.value > 0.6) // 1 chance von 4 dass es weiter geht
                {
                    //back right
                    kambiumList.Add(new Tower.Cambium(backLeftPos, buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));
                    hasStartedOne = true;
                    countNewCambiums++;
                }

                if (Random.value > 0.6) // 1 chance von 4 dass es weiter geht
                {
                    //back right
                    kambiumList.Add(new Tower.Cambium(frontRightPos, buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));
                    hasStartedOne = true;
                    countNewCambiums++;
                }

                if (Random.value > 0.6) // 1 chance von 4 dass es weiter geht
                {
                    //back right
                    kambiumList.Add(new Tower.Cambium(frontLeftPos, buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));
                    hasStartedOne = true;
                    countNewCambiums++;
                }

                if (!hasStartedOne) //falls keiner gestarted wurden ist
                {
                    kambiumList.Add(new Tower.Cambium(positions[Random.Range(0, positions.Count)], buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));
                    countNewCambiums++;
                }


                towerAndBranches[tower] += countNewCambiums - 1; //eins ist sowieso

                return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
            }
        }
        else
        {
            towerAndBranches[tower]--; //dieses Kabium hört random auf
            return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
        }

        

    }
*/

    private static void NeighboursRecursion(Building at_building, int maxNeighbours, int iterationStep, Tower tower)
    {
        Transform buildingTransform = at_building.Main_Collider.transform;
        BoxCollider buildingCollider = at_building.Main_Collider.GetComponent<BoxCollider>();
        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        Vector3 backRightPos = buildingTransform.position + (buildingTransform.forward * buildingCollider.size.z / 2) + (buildingTransform.right * buildingCollider.size.x / 2);
        Vector3 backLeftPos = buildingTransform.position + (buildingTransform.forward * buildingCollider.size.z / 2) - (buildingTransform.right * buildingCollider.size.x / 2);
        Vector3 frontRightPos = buildingTransform.position - (buildingTransform.forward * buildingCollider.size.z / 2) + (buildingTransform.right * buildingCollider.size.x / 2);
        Vector3 frontLeftPos = buildingTransform.position - (buildingTransform.forward * buildingCollider.size.z / 2) - (buildingTransform.right * buildingCollider.size.x / 2);


        //3 chance sur 4 
        //make it 1 round, 2 rounds, 3 rounds
        if (Random.value > 0.25) // 3 chance von 4 dass es weiter geht
        {
            //back right
            kambiumList.Add(new Tower.Cambium(backRightPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));
            //NeighboursRecursion();
        }

        if (Random.value > 0.25) // 3 chance von 4 dass es weiter geht
        {
            //back right
            kambiumList.Add(new Tower.Cambium(backLeftPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));

        }

        if (Random.value > 0.25) // 3 chance von 4 dass es weiter geht
        {
            //back right
            kambiumList.Add(new Tower.Cambium(frontRightPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));

        }

        if (Random.value > 0.25) // 3 chance von 4 dass es weiter geht
        {
            //back right
            kambiumList.Add(new Tower.Cambium(frontLeftPos, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0) * buildingTransform.up, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], tower.Steps));

        }

        iterationStep++;

        if (iterationStep >= maxNeighbours)
        {

        }
        else
        {
           // NeighboursRecursion(at_building, maxNeighbours, iterationStep);
        }

    }
    #endregion

    public static Tower.Cambiums_At_Active FrangipaniTree(Building at_building, Tower tower)
    {
        Transform buildingTransform = at_building.Main_Collider.transform;
        Vector3 backRightPos = buildingTransform.position + (buildingTransform.forward * buildingTransform.localScale.z / 2) + (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
        Vector3 backLeftPos = buildingTransform.position + (buildingTransform.forward * buildingTransform.localScale.z / 2) - (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
        Vector3 frontRightPos = buildingTransform.position - (buildingTransform.forward * buildingTransform.localScale.z / 2) + (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);
        Vector3 frontLeftPos = buildingTransform.position - (buildingTransform.forward * buildingTransform.localScale.z / 2) - (buildingTransform.right * buildingTransform.localScale.x / 2) + (buildingTransform.up * buildingTransform.localScale.y / 2);

        List<Vector3> positions = new List<Vector3>();
        positions.Add(backRightPos); positions.Add(backLeftPos); positions.Add(frontRightPos); positions.Add(frontLeftPos);

        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        if (at_building.Parent_Building == null)
        {
            //when 3 buildings lang nichts gebrancht wurde, dann spalte

            Vector3 firstRandomPos = positions[Random.Range(0, positions.Count)];
            positions.Remove(firstRandomPos);
            Vector3 secondRandomPos = positions[Random.Range(0, positions.Count)];
            positions.Add(firstRandomPos);

            kambiumList.Add(new Tower.Cambium(firstRandomPos,
                                             firstRandomPos - buildingTransform.position,
                                             tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                             15)); //same steps, the tower counts them down

            kambiumList.Add(new Tower.Cambium(secondRandomPos,
                                             secondRandomPos - buildingTransform.position,
                                             tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                             15)); //same steps, the tower counts them down

            return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
        }

        if(HasStillSteps(at_building))
        {
            if (at_building.Parent_Building != null)
            {
                if (at_building.Parent_Building.Parent_Building != null)
                {
                    if (at_building.Parent_Building.Parent_Building.Parent_Building != null)
                    {
                        if (at_building.Parent_Building.Parent_Building.Parent_Building.Child_Building != null)
                        {
                            if (at_building.Parent_Building.Parent_Building.Parent_Building.Child_Building.Length > 1) //3 parents?
                            {
                                //when 3 buildings lang nichts gebrancht wurde, dann spalte

                                Vector3 firstRandomPos = positions[Random.Range(0, positions.Count)];
                                positions.Remove(firstRandomPos);
                                Vector3 secondRandomPos = positions[Random.Range(0, positions.Count)];
                                positions.Add(firstRandomPos);

                                kambiumList.Add(new Tower.Cambium(firstRandomPos,
                                                 firstRandomPos - buildingTransform.position,
                                                 tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                                 at_building.Cambium.steps)); //same steps, the tower counts them down

                                kambiumList.Add(new Tower.Cambium(secondRandomPos,
                                                                 secondRandomPos - buildingTransform.position,
                                                                 tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                                                 at_building.Cambium.steps)); //same steps, the tower counts them down

                                return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
                            }
                            else
                            {
                                kambiumList.Add(new Tower.Cambium(buildingTransform.position + (buildingTransform.up * buildingTransform.localScale.y / 2),
                                                                buildingTransform.up,
                                                                tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                                                at_building.Cambium.steps)); //same steps, the tower counts them down

                                return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
                            }
                        }
                        else
                        {
                            kambiumList.Add(new Tower.Cambium(buildingTransform.position + (buildingTransform.up * buildingTransform.localScale.y / 2),
                                                            buildingTransform.up,
                                                            tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                                            at_building.Cambium.steps)); //same steps, the tower counts them down

                            return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
                        }
                    }
                    else
                    {
                        kambiumList.Add(new Tower.Cambium(buildingTransform.position + (buildingTransform.up * buildingTransform.localScale.y / 2),
                                                        buildingTransform.up,
                                                        tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                                        at_building.Cambium.steps)); //same steps, the tower counts them down

                        return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
                    }
                }
                else
                {
                    kambiumList.Add(new Tower.Cambium(buildingTransform.position + (buildingTransform.up * buildingTransform.localScale.y / 2),
                                                    buildingTransform.up,
                                                    tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                                    at_building.Cambium.steps)); //same steps, the tower counts them down

                    return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
                }
            }
            else
            {
                kambiumList.Add(new Tower.Cambium(buildingTransform.position + (buildingTransform.up * buildingTransform.localScale.y / 2),
                                                buildingTransform.up,
                                                tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)],
                                                at_building.Cambium.steps)); //same steps, the tower counts them down

                return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
            }
        }

        else
        {
            return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());

        }
       

    }

}
