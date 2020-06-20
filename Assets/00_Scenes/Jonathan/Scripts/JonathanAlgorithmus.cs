using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tower_Management;
using System.Runtime.InteropServices;

static class JonathanAlgorithmus
{
    // RBunker
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

    // RBunker Branch
    static Dictionary<Tower, int> branches_per_tower = new Dictionary<Tower, int>();

    public static Tower.Cambiums_At_Active RBunkerBranch(Building at_building, Tower tower)
    {
        bool is_first_spawn = false;

        // add tower to dic
        if (!branches_per_tower.ContainsKey(tower))
        {
            branches_per_tower.Add(tower, 1);
            is_first_spawn = true;
        }

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
                dir = new Vector3(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(0, 50) == 0 ? 1 : 0, Random.Range(0, 2) == 0 ? 1 : -1).normalized;
                ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
                at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);


                // check distance to origin
                Vector2 tower_2D = new Vector2(tower.transform.position.x, tower.transform.position.z);
                Vector2 building_2D = new Vector2(at_building.Main_Collider.transform.position.x, at_building.Main_Collider.transform.position.z);

                if (Vector2.Distance(tower_2D, building_2D) > 60)
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

        int steps = at_building.Cambium.steps > 0 ? at_building.Cambium.steps : Random.Range(0, tower.Steps);

        if (is_first_spawn)
            steps = 1;

        kambiumList.Add(new Tower.Cambium(hit.point, hit.normal, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], steps));

        // delet random cambium
        if (Random.Range(0, 50) == 0 && branches_per_tower[tower] > 1)
        {
            kambiumList.RemoveAt(0);

            branches_per_tower[tower]--;
        }

        // new random cambium
        if (Random.Range(0, 50) == 0 && branches_per_tower[tower] < 4)
        {
            // raycast with random dir
            dir = new Vector3(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(0, 5) == 0 ? 1 : 0, Random.Range(0, 2) == 0 ? 1 : -1).normalized;
            ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
            at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);
            kambiumList.Add(new Tower.Cambium(hit.point, hit.normal, tower.Building_Prefabs[Random.Range(0, tower.Building_Prefabs.Count)], Random.Range(0, tower.Steps)));

            branches_per_tower[tower]++;
        }
        

        return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
    }

    // StairGrow
    static Dictionary<Tower, int> StairGrow_Steps_Cache = new Dictionary<Tower, int>();

    public static Tower.Cambiums_At_Active StairGrow(Building at_building, Tower tower) 
    {
        bool is_first_spawn = false;

        if (!StairGrow_Steps_Cache.ContainsKey(tower))
        {
            StairGrow_Steps_Cache.Add(tower, tower.Steps);
            is_first_spawn = true;
        }

        // cambiums
        List<Tower.Cambium> kambiumList = new List<Tower.Cambium>();

        // raycast to main collider
        Vector3 dir;
        Ray ray;
        RaycastHit hit = new RaycastHit();

        // spawn stairs
        if (at_building.Cambium.steps > 0)
        {
            // raycast with random dir
            if (at_building.Cambium.steps == StairGrow_Steps_Cache[tower] - 1)
            {
                if (!is_first_spawn)
                {
                    dir = at_building.Main_Collider.transform.parent.right;
                }
                else
                {
                    dir = at_building.Main_Collider.transform.parent.forward;
                }
            }
            else
                dir = at_building.Cambium.normal;

            ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
            at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);

            kambiumList.Add(new Tower.Cambium(hit.point + new Vector3(0, at_building.Main_Collider.bounds.size.y/2, 0), hit.normal, tower.Building_Prefabs[0], at_building.Cambium.steps));

            // split
            if (!is_first_spawn && at_building.Cambium.steps == StairGrow_Steps_Cache[tower] - 1 && Random.Range(0, tower.Mapper.Split_Chance) == 0)
            {
                dir = -at_building.Main_Collider.transform.parent.right;

                ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
                at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);

                kambiumList.Add(new Tower.Cambium(hit.point + new Vector3(0, at_building.Main_Collider.bounds.size.y / 2, 0), hit.normal, tower.Building_Prefabs[0], at_building.Cambium.steps));
            }
        }
        // spawn plattform
        else
        {
            // raycast with random dir
            dir = at_building.Cambium.normal;
            ray = new Ray(at_building.Main_Collider.transform.position + dir * 100f, -dir);
            at_building.Main_Collider.Raycast(ray, out hit, Mathf.Infinity);

            // add cambium to list
            StairGrow_Steps_Cache[tower] = Random.Range(tower.Steps / 2, tower.Steps);
            kambiumList.Add(new Tower.Cambium(hit.point, hit.normal, tower.Building_Prefabs[1], StairGrow_Steps_Cache[tower]));
        }

        return new Tower.Cambiums_At_Active(at_building, kambiumList.ToArray());
    }
}
