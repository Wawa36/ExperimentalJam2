using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tower_Management
{
    public class Tower : MonoBehaviour
    {
        // prefabs
        [Header("Prefabs")]
        [SerializeField] List<GameObject> _building_prefabs = new List<GameObject>();
        [SerializeField] List<GameObject> _structure_prefabs = new List<GameObject>();

        public List<GameObject> Building_Prefabs { get { return _building_prefabs; } }
        public List<GameObject> Structure_Prefabs { get { return _structure_prefabs; } }

        // paremeter
        [Header("Growth Parameter")]
        [SerializeField] KabiumAlgorithm algorithm;
        [SerializeField] int start_kambiums;
        [SerializeField] float growth_speed;
        [SerializeField] float delay;
        [SerializeField] [Range(1, 10)] int steps;

        [Header("Debugging")]
        [SerializeField] Material default_material;
        [SerializeField] Material highlight_material;

        // stored growth data
        List<IGrowingBlock> active_blocks = new List<IGrowingBlock>();
        List<IGrowingBlock> inactive_blocks = new List<IGrowingBlock>();
        Dictionary<Building, float> building_delays = new Dictionary<Building, float>();

        // register at manager
        private void Awake()
        {
            Tower_Manager.Instance.Add_Tower(this);

            for (int i = 0; i < start_kambiums; i++)
            {
                var c = new Cambium[1];
                c[0] = new Cambium(transform.position, Building_Prefabs[0]); // index 0 is always the first spawned building

                Create_Building(c);
            }
        }

        public void Update_Growth()
        {
            // call growth updates on blocks
            for (int i = 0; i < active_blocks.Count; i++) { active_blocks[i].On_Update_Growth(Calculate_Growth_Speed()); }

            // count delays
            List<Building> finished_buildings = new List<Building>();

            foreach (var c in building_delays.Keys.ToList())
            {
                building_delays[c] += Time.deltaTime;

                if (building_delays[c] >= Calculate_Delay())
                {
                    Create_Building(Calculate_Cambiums(c));
                    finished_buildings.Add(c);
                    c.gameObject.GetComponentInChildren<MeshRenderer>().material = default_material;
                }
            }

            foreach (var c in finished_buildings) { building_delays.Remove(c); }
        }

        // growing management
        private void Create_Building(Cambiums_At_Active cambiums_a)
        {
            List<Building> created_buildings = new List<Building>();

            // create buildings
            foreach (var c in cambiums_a.cambiums)
            {
                // instantiate and initialize
                var new_building = Instantiate(c.prefab, c.point, Quaternion.identity);
                new_building.GetComponent<IGrowingBlock>().Initialize(this, c);
                new_building.transform.SetParent (transform);

                // rotate
                var origin = new_building.GetComponent<Building>().Origin_From_Normal(c.normal);
                origin.transform.position = c.point;
                origin.forward = c.normal;

                // link to parent
                new_building.GetComponent<Building>().Set_Parent_Building(cambiums_a.active_building);
                created_buildings.Add(new_building.GetComponent<Building>());

                // highlight color
                new_building.gameObject.GetComponentInChildren<MeshRenderer>().material = highlight_material;

                // add to active blocks
                active_blocks.Add(new_building.GetComponent<IGrowingBlock>());
            }

            // set childs
            foreach (var c in created_buildings)
            {
                cambiums_a.active_building.Add_Child_Building(c);
            }
        }

        private void Create_Structure(Vector3 at_point)
        {
        }

        // public interfaces
        public void Create_New_Building(Building at_building) 
        {
            Create_Building(Calculate_Kambium(at_building));
        }

        public void Deactivate_Block(IGrowingBlock block) 
        {
            // add buildings to delay dic
            if (block is Building block_as_building)
                building_delays.Add(block_as_building, 0);

            // move block to inactive blocks
            active_blocks.Remove(block);
            inactive_blocks.Add(block);
        }

        // calcualte parameters 
        float Calculate_Growth_Speed() { return growth_speed * Tower_Manager.Instance.Growth_Speed_Multiplier; }

        float Calculate_Delay() { return delay * Tower_Manager.Instance.Delay_Multiplier; }

        // calculate Kambium
        Cambiums_At_Active Calculate_Cambiums(Building at_building)
        {
            return ProceduralKabiumGenerator.Calculate_Kambium(algorithm, at_building, this);
        }

        public struct Cambium
        {
            public Vector3 point;
            public Vector3 normal;
            public GameObject prefab;
            public int steps;

            // at building
            public Cambium(Vector3 point, Vector3 normal, GameObject prefab, int steps)
            {
                this.point = point;
                this.normal = normal;
                this.prefab = prefab;
                this.steps = steps;
            }

            // as origin
            public Cambium(Vector3 origin, GameObject prefab)
            {
                this.point = origin;
                this.normal = Vector3.up;
                this.prefab = prefab;
                this.steps = 0;
            }
        }

        public struct Cambiums_At_Active
        {
            public Building active_building;
            public Cambium[] cambiums;

            public Cambiums_At_Active(Building active_building, Cambium[] cambiums)
            {
                this.active_building = active_building;
                this.cambiums = cambiums;
            }
        }
    }

    public interface IGrowingBlock
    {
        void Initialize(Tower tower, Tower.Cambium cambium);

        void On_Update_Growth(float speed);
    }
}