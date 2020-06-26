using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detail_Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Detail_Prefab[] prefabs;
    [SerializeField] float spawn_rate;
    [SerializeField] float distance;
    [SerializeField] int chunk_size;
    [SerializeField] Material chunk_material;

    [Header("Debugging")]
    [SerializeField] bool draw_ray;
    [SerializeField] List<GameObject> spawned_details = new List<GameObject>();
    [SerializeField] List<GameObject> merged_chunks = new List<GameObject>();
    [SerializeField] Transform tracker;

    void Start()
    {
        StartCoroutine(Spawn_Loop());
        tracker = GameObject.FindGameObjectWithTag("Details Tracker").transform;
    }

    IEnumerator Spawn_Loop() 
    {
        while (true)
        {
            yield return new WaitForSeconds(spawn_rate);

            Search_Point(prefabs[Random.Range(0, prefabs.Length)]);
        }
    }

    void Search_Point (Detail_Prefab prefab) 
    {
        int counter = 0;

        while (counter < 50)
        {
            var ray = new Ray(transform.position, (transform.position + Random.onUnitSphere) - transform.position);
            var hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit, distance, prefab.mask, QueryTriggerInteraction.Ignore))
            {
                if (draw_ray)
                    Debug.DrawRay(transform.position, hit.point - transform.position, Color.green, 0.5f);

                Spawn_Prefab(prefab.prefab, hit.point, hit.normal);

                break;
            }
            else
                counter++;
        }
    }

    void Spawn_Prefab(GameObject prefab, Vector3 point, Vector3 normal) 
    {
        spawned_details.Add (Instantiate(prefab, point, Quaternion.FromToRotation(Vector3.up, normal), tracker));

        if (spawned_details.Count >= chunk_size)
            Merge();
    }

    void Merge() 
    {
        List<CombineInstance> combine = new List<CombineInstance>();
        int new_vertice_count = 0;

        // add building meshes
        for (int i = 0; i < spawned_details.Count; i++)
        {
            var instance = new CombineInstance();
            instance.mesh = spawned_details[i].GetComponent<MeshFilter>().sharedMesh;
            instance.transform = spawned_details[i].transform.localToWorldMatrix;
            combine.Add(instance);

            new_vertice_count += instance.mesh.vertexCount;
        }

        // first chunk
        if (merged_chunks.Count == 0)
        {
            // create new chunk
            var new_chunk = new GameObject("Chunk #" + (merged_chunks.Count));
            new_chunk.transform.SetParent(tracker);
            new_chunk.tag = "Building";
            new_chunk.isStatic = true;

            // add components
            new_chunk.AddComponent<MeshFilter>();
            new_chunk.AddComponent<MeshRenderer>();
            new_chunk.GetComponent<MeshRenderer>().material = chunk_material;

            // create mesh
            new_chunk.GetComponent<MeshFilter>().mesh = new Mesh();
            new_chunk.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            // merge
            new_chunk.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());

            merged_chunks.Add(new_chunk);
        }
        else
        {
            var filter = merged_chunks[merged_chunks.Count - 1].GetComponent<MeshFilter>();

            // add to last chunk
            if (filter.mesh.vertexCount + new_vertice_count <= 2000000000)
            {
                var instance = new CombineInstance();
                instance.mesh = filter.mesh;
                instance.transform = filter.transform.localToWorldMatrix;
                combine.Add(instance);

                filter.mesh = new Mesh();
                filter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                filter.mesh.CombineMeshes(combine.ToArray());
            }
            // create new chunk
            else
            {
                // create new chunk
                var new_chunk = new GameObject("Chunk #" + (merged_chunks.Count));
                new_chunk.transform.SetParent(tracker);
                new_chunk.tag = "Building";
                new_chunk.isStatic = true;

                // add components
                new_chunk.AddComponent<MeshFilter>();
                new_chunk.AddComponent<MeshRenderer>();
                new_chunk.GetComponent<MeshRenderer>().material = chunk_material;

                // create mesh
                new_chunk.GetComponent<MeshFilter>().mesh = new Mesh();
                new_chunk.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                // merge
                new_chunk.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
                merged_chunks.Add(new_chunk);
            }
        }

        // delet former details
        foreach (var c in spawned_details)
        {
            Destroy(c);
        }

        spawned_details.Clear();
    }

    [System.Serializable]
    struct Detail_Prefab 
    {
        public GameObject prefab;
        public LayerMask mask;
    }
}
