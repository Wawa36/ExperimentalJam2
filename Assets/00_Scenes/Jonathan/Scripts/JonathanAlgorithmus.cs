using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tower_Management;
using System.Runtime.InteropServices;

static class JonathanAlgorithmus
{
    public static Tower.Cambiums_At_Active RBunker(Building at_building, Tower tower)
    {
        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        // raycast to main collider
        Vector3 dir;
        Ray ray;
        RaycastHit hit;

        if (at_building.Cambium.steps > 0)
        {
            dir = at_building.Cambium.normal;
            ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
            hit = new RaycastHit();
            at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);
        }
        else
        {
            while (true)
            {
                // raycast with random dir
                dir = new Vector3(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(0, 5) == 0 ? 1 : 0, Random.Range(0, 2) == 0 ? 1 : -1).normalized;
                ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
                at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);


                // check distance to origin
                Vector2 tower_2D = new Vector2(tower.transform.position.x, tower.transform.position.z);
                Vector2 building_2D = new Vector2(at_building.Main_Collider.transform.position.x, at_building.Main_Collider.transform.position.z);

                if (Vector2.Distance(tower_2D, building_2D) > 30)
                {            
                    dir = tower.transform.position - at_building.transform.position;
                    dir.y = 0;

                    ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
                    at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);
                    break;
                }

                // break if normal goes to opposite dir of former ray 
                if (Vector3.Dot(hit.normal, at_building.Cambium.normal) >= -0.5f)
                    break;
            }
        }

        kambiumList.Add(new Tower.Cambium(hit.point, hit.normal, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], at_building.Cambium.steps > 0 ? at_building.Cambium.steps : Random.Range(0, tower.Steps)));

        return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
    }

    static int branch_amount = 1;

    public static Tower.Cambiums_At_Active RBunkerBranch(Building at_building, Tower tower)
    {
        // cambiums
        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        // raycast to main collider
        Vector3 dir;
        Ray ray;
        RaycastHit hit;

        // at active building
        if (at_building.Cambium.steps > 0)
        {
            dir = at_building.Cambium.normal;
            ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
            at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);
        }
        else
        {
            while (true)
            {
                // raycast with random dir
                dir = new Vector3(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(0, 5) == 0 ? 1 : 0, Random.Range(0, 2) == 0 ? 1 : -1).normalized;
                ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
                at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);


                // check distance to origin
                Vector2 tower_2D = new Vector2(tower.transform.position.x, tower.transform.position.z);
                Vector2 building_2D = new Vector2(at_building.Main_Collider.transform.position.x, at_building.Main_Collider.transform.position.z);

                if (Vector2.Distance(tower_2D, building_2D) > 30)
                {   
                    dir = tower.transform.position - at_building.transform.position;
                    dir.y = 0;
                    ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);

                    at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);
                    break;
                }

                // break if normal goes to opposite dir of former ray 
                if (Vector3.Dot(hit.normal, at_building.Cambium.normal) >= -0.5f)
                    break;
            }
        }

        kambiumList.Add(new Tower.Cambium(hit.point, hit.normal, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], at_building.Cambium.steps > 0 ? at_building.Cambium.steps : Random.Range(0, tower.Steps)));

        // delet random cambium
        if (Random.Range(0, 50) == 0 && branch_amount > 1)
        {
            kambiumList.RemoveAt(0);

            branch_amount--;
        }

        // new random cambium
        if (Random.Range(0, 50) == 0 && branch_amount < 4)
        {
            // raycast with random dir
            dir = new Vector3(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(0, 5) == 0 ? 1 : 0, Random.Range(0, 2) == 0 ? 1 : -1).normalized;
            ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
            at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);
            kambiumList.Add(new Tower.Cambium(hit.point, hit.normal, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], Random.Range(0, tower.Steps)));

            branch_amount++;
        }
        

        return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
    }
}
