using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Transactions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Tower_Management
{
    [RequireComponent(typeof(Tower_Input_Mapper))]
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
        [SerializeField] int _start_cambiums;
        [SerializeField] float _growth_speed;
        [SerializeField] AnimationCurve _growth_speed_over_lifetime = AnimationCurve.Linear(0, 1, 1 , 1);
        [SerializeField] float _delay;
        [SerializeField] [Range(1, 30)] int _steps;
        [SerializeField] int chunk_size;

        [Header("Debugging")]
        [SerializeField] Player_Inputs inputs;
        [SerializeField] Material default_material;
        [SerializeField] Material highlight_material;
        [SerializeField] int building_generation = 0;

        // stored growth data
        Tower_Input_Mapper mapper;
        List<IGrowingBlock> active_blocks = new List<IGrowingBlock>();
        List<IGrowingBlock> inactive_blocks = new List<IGrowingBlock>();
        List<GameObject> merged_blocks = new List<GameObject>();
        Dictionary<Building, float> building_delays = new Dictionary<Building, float>();

        // register at manager
        private void Awake()
        {
            // get mapper
            mapper = GetComponent<Tower_Input_Mapper>();

            inputs.player_dir = GameObject.FindGameObjectWithTag("Player").transform.forward;
            Initialize(inputs);

            // register tower
            Tower_Manager.Instance.Add_Tower(this);
        }

        public void Initialize(Player_Inputs inputs)
        {
            mapper.Initialize(inputs);
            Assign_Input_To_Growth_Parameter();
            Spawn_First_Building();
        }

        // change growth parameter
        public void Assign_Input_To_Growth_Parameter() 
        {
            // growth speed
            _growth_speed *= mapper.Grow_Speed;

            // generations
            var keys = _growth_speed_over_lifetime.keys;

            // remap keys
            float factor = mapper.Generation_Amount / keys[keys.Length - 1].time;
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].time *= factor;
            }

            _growth_speed_over_lifetime.keys = keys;

            // change tangetns to linear
            for (int i = 0; i < _growth_speed_over_lifetime.keys.Length; i++)
            {
                keys[i].time *= factor;
                //keys[i].inTangent = 1;
                //keys[i].outTangent = 1;
                AnimationUtility.SetKeyLeftTangentMode(_growth_speed_over_lifetime, i, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(_growth_speed_over_lifetime, i, AnimationUtility.TangentMode.Linear);
            }
        }

        // growing management
        void Spawn_First_Building() 
        {
            for (int i = 0; i < _start_cambiums; i++)
            {
                var c = new Cambium[1];
                c[0] = new Cambium(transform.position, Building_Prefabs[0]); // index 0 is always the first spawned building
                c[0].steps = Steps;
                c[0].normal = mapper.Grow_Direction;
                Create_Building(new Cambiums_At_Active(null, c));
            }
        }

        public void Update_Growth()
        {
            // call growth updates on blocks
            for (int i = 0; i < active_blocks.Count; i++) { active_blocks[i].On_Update_Growth(Growth_Speed); }

            // count delays
            List<Building> finished_buildings = new List<Building>();

            foreach (var c in building_delays.Keys.ToList())
            {
                if (c != null)
                {
                    building_delays[c] += Time.deltaTime;

                    if (building_delays[c] >= Delay)
                    {
                        Create_Building(Calculate_Cambiums(c));
                        finished_buildings.Add(c);
                        c.gameObject.GetComponentInChildren<MeshRenderer>().material = default_material;
                    }
                }
            }

            foreach (var c in finished_buildings) { building_delays.Remove(c); }

            if (inactive_blocks.Count >= chunk_size)
            {
                Merge_Chunk();
            }
        }

        private void Create_Building(Cambiums_At_Active cambiums_a)
        {
            if (Get_Current_Grow_Speed_Over_Lifetime() > 0)
            {
                List<Building> created_buildings = new List<Building>();

                for (int i = 0; i < cambiums_a.cambiums.Length; i++)
                {
                    var c = cambiums_a.cambiums[i];

                    // count steps
                    c.steps = c.steps > 0 ? --c.steps : 0;

                    // instantiate and parent building
                    var new_building = Instantiate(c.prefab, c.point, Quaternion.identity);
                    new_building.transform.SetParent(transform);

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

                    // initialite building 
                    new_building.GetComponent<IGrowingBlock>().Initialize(this, c);

                    building_generation++;
                }

                // set childs
                if (cambiums_a.active_building)
                {
                    foreach (var c in created_buildings)
                    {
                        cambiums_a.active_building.Add_Child_Building(c);
                    }
                }
            }
        }

        private void Create_Structure(Vector3 at_point)
        {
        }

        // public interfaces
        public void Create_New_Building(Building at_building) 
        {
            Create_Building(Calculate_Cambiums(at_building));
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

        // parameters & properties
        public float Growth_Speed 
        { 
            get 
            {
                return _growth_speed * Get_Current_Grow_Speed_Over_Lifetime() * Tower_Manager.Instance.Growth_Speed_Multiplier; 
            } 
        }

        public float Get_Current_Grow_Speed_Over_Lifetime()
        {
            float value;

            if (building_generation < _growth_speed_over_lifetime.keys[_growth_speed_over_lifetime.keys.Length - 1].time)
                value = _growth_speed_over_lifetime.Evaluate(building_generation);
            else
            {
                value = 0;

                foreach (var c in active_blocks.ToList())
                {
                    active_blocks.Remove(c);
                    Destroy((c as Component).gameObject);
                }
            }

            return value;
        }

        public float Delay { get { return _delay * Tower_Manager.Instance.Delay_Multiplier; } }

        public int Steps { get { return _steps; } }

        public Tower_Input_Mapper Mapper { get { return mapper; } }

        // merging
        void Merge_Chunk() 
        {
            CombineInstance[] combine = new CombineInstance[chunk_size];
            GameObject new_chunk = new GameObject("Chunk #" + (merged_blocks.Count + 1).ToString());
            new_chunk.transform.SetParent(transform);

            for (int i = 0; i < chunk_size; i++)
            {
                if (inactive_blocks[i] is Building block_as_building)
                {
                    combine[i].mesh = block_as_building.Mesh.GetComponent<MeshFilter>().sharedMesh;
                    combine[i].transform = block_as_building.Mesh.transform.localToWorldMatrix;
                }
            }

            new_chunk.AddComponent<MeshFilter>();
            new_chunk.AddComponent<MeshRenderer>();
            new_chunk.GetComponent<MeshRenderer>().material = default_material;

            new_chunk.GetComponent<MeshFilter>().mesh = new Mesh();
            new_chunk.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

            new_chunk.AddComponent<MeshCollider>().sharedMesh = new_chunk.GetComponent<MeshFilter>().sharedMesh;

            merged_blocks.Add(new_chunk);

            foreach (var c in inactive_blocks.ToList())
            {
                (c as Building).On_Merged();
                inactive_blocks.RemoveAt(0);
            }
        }

        // calculate Kambium
        Cambiums_At_Active Calculate_Cambiums(Building at_building)
        {
            return ProceduralKabiumGenerator.Calculate_Kambium(algorithm, at_building, this);
        }

        [System.Serializable]
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

        // control inspector inputs
        private void OnValidate()
        {
            var c = _growth_speed_over_lifetime.keys;

            if (c[0].time != 0)
                c[0].time = 0;

            _growth_speed_over_lifetime.keys = c;
        }

        // tower spawn parameter
        [System.Serializable]
        public struct Player_Inputs
        {
            public Vector3 player_dir;
            public float orb_energy;
            public float throw_dist;
            public float throw_time;
            public float player_speed;
            public string ground_tag;

            public Player_Inputs(Vector3 player_dir, float orb_energy, float throw_dist, float throw_time, float player_speed, string ground_tag)
            {
                this.player_dir = player_dir;
                this.orb_energy = orb_energy;
                this.throw_dist = throw_dist;
                this.throw_time = throw_time;
                this.player_speed = player_speed;
                this.ground_tag = ground_tag;
            }
        }
    }

    public interface IGrowingBlock
    {
        void Initialize(Tower tower, Tower.Cambium cambium);

        void On_Update_Growth(float speed);
    }
}