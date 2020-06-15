using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tower_Management
{
    public class Tower : MonoBehaviour
    {
        // prefabs
        [Header("Prefabs")]
        [SerializeField] List<GameObject> building_prefabs = new List<GameObject>();
        [SerializeField] List<GameObject> structure_prefabs = new List<GameObject>();

        // paremeter
        [Header("Growth Parameter")]
        [SerializeField] KabiumAlgorithm algorithm;
        [SerializeField] int start_kambiums;
        [SerializeField] float growth_speed;
        [SerializeField] float delay;

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
                c[0] = new Cambium(transform.position);

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
                    Create_Building(Calculate_Kambium(c));
                    finished_buildings.Add(c);
                }
            }

            foreach (var c in finished_buildings) { building_delays.Remove(c); }
        }

        // growing management
        private void Create_Building(Cambium[] cambiums)
        {
            foreach (var c in cambiums)
            {
                var new_building = Instantiate(building_prefabs[Random.Range(0, building_prefabs.Count)], c.point + c.normal * 0.5f, Quaternion.identity);
                new_building.GetComponent<IGrowingBlock>().Initialize(this);

                active_blocks.Add(new_building.GetComponent<IGrowingBlock>());
            }
        }

        private void Create_Structure(Vector3 at_point)
        {
            var c = Instantiate(structure_prefabs[0], transform.position, transform.rotation);
            c.GetComponent<IGrowingBlock>().Initialize(this);
            active_blocks.Add(c.GetComponent<IGrowingBlock>());
        }

        // calcualte parameters 
        float Calculate_Growth_Speed() { return growth_speed * Tower_Manager.Instance.Growth_Speed_Multiplier; }

        float Calculate_Delay() { return delay * Tower_Manager.Instance.Delay_Multiplier; }

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

        // calculate Kambium
        Cambium[] Calculate_Kambium(Building at_building)
        {
            return ProceduralKabiumGenerator.Calculate_Kambium(algorithm, at_building);
        }

        public struct Cambium
        {
            public Vector3 point;
            public Vector3 normal;

            // at building
            public Cambium(Vector3 point, Vector3 normal)
            {
                this.point = point;
                this.normal = normal;
            }

            // ass origin
            public Cambium(Vector3 origin)
            {
                this.point = origin;
                this.normal = Vector3.up;
            }
        }
    }

    public interface IGrowingBlock
    {
        void Initialize(Tower tower);

        void On_Update_Growth(float speed);
    }
}