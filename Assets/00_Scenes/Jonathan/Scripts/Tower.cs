using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
        [SerializeField] LayerMask _layer;
        [SerializeField] int _start_cambiums;
        [SerializeField] float _growth_speed;
        [SerializeField] Vector3 override_start_direction;
        [SerializeField] AnimationCurve _growth_speed_over_lifetime = AnimationCurve.Linear(0, 1, 1 , 1);
        [SerializeField] float _delay;
        [SerializeField] [Range(1, 30)] int _steps;
        [SerializeField] bool decrement_steps = true;
        [SerializeField] bool start_steps_zero = false;
        [SerializeField] int chunk_size;
        [SerializeField] int deep_chunk_size = 10;
        [SerializeField] Material[] materials;
        [SerializeField] Color growth_color;
        [SerializeField] [Range(0.1f, 10f)] float material_lerp_speed;

        [Header("Debugging")]
        [SerializeField] Player_Inputs inputs;
        [SerializeField] int _building_generation = 0;
        [SerializeField] bool can_finish;
        [SerializeField] bool finished_growing;

        // stored growth data
        Tower_Input_Mapper mapper;
        List<IGrowingBlock> active_blocks = new List<IGrowingBlock>();
        List<IGrowingBlock> inactive_blocks = new List<IGrowingBlock>();
        List<GameObject> merged_chunks = new List<GameObject>();
        List<GameObject> deep_merged_chunk = new List<GameObject>();

        Dictionary<Building, float> building_color_lersp = new Dictionary<Building, float>();

        // register at manager
        private void Awake()
        {
            // get mapper
            mapper = GetComponent<Tower_Input_Mapper>();

            // register tower
            Tower_Manager.Instance.Add_Tower(this);
        }

        public void Initialize(Player_Inputs inputs)
        {
            // save input for debbuging purpose
            this.inputs = inputs;

            // initialize mapper with inputs
            mapper.Initialize(inputs);

            // assign growth parameter
            Assign_Input_To_Growth_Parameter();

            // spawn first building(s)
            Spawn_First_Building();

            // choose random materials
            materials = new Material[] { materials[Random.Range(0, materials.Length)] };
        }

        // change growth parameter
        public void Assign_Input_To_Growth_Parameter() 
        {
            // growth speed
            _growth_speed *= mapper.Grow_Speed;

            // generations
            var keys = _growth_speed_over_lifetime.keys;

            // remap keys
            float factor = mapper.Generation_Amount * mapper.Width / keys[keys.Length - 1].time;

            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].time *= factor;
            }

            _growth_speed_over_lifetime.keys = keys;

            // change tangents to linear
            for (int i = 0; i < _growth_speed_over_lifetime.keys.Length; i++)
            {
                keys[i].time *= factor;
                keys[i].tangentMode = 2; 
            }
        }

        // growing management
        void Spawn_First_Building() 
        {
            for (int i = 0; i < _start_cambiums; i++)
            {
                var c = new Cambium[1];
                c[0] = new Cambium(transform.position, Building_Prefabs[0]); // index 0 is always the first spawned building
                c[0].steps = start_steps_zero? 0 : Steps;
                c[0].normal = override_start_direction.magnitude == 0? Calculate_Grow_Direction(mapper.Player_Direction, mapper.Normal_Direction) : override_start_direction;
                Create_Building(new Cambiums_At_Active(null, c));
            }
        }

        public void Update_Growth()
        {
            // call growth updates on blocks
            for (int i = 0; i < active_blocks.Count; i++) { active_blocks[i].On_Update_Growth(Growth_Speed == 0 ? 0.2f : Growth_Speed); }

            // count delays
            List<Building> finished_buildings = new List<Building>();

            foreach (var c in building_color_lersp.Keys.ToList())
            {
                if (c != null)
                {
                    building_color_lersp[c] += Time.deltaTime * material_lerp_speed;
                    c.Renderer.material.color = Color.Lerp(growth_color, materials[0].color, building_color_lersp[c]);

                    if (building_color_lersp[c] >= 1)
                    {
                        c.Renderer.material.color = materials[0].color;

                        finished_buildings.Add(c);

                        // move block to inactive blocks
                        inactive_blocks.Add(c as IGrowingBlock);
                    }
                }
            }

            foreach (var c in finished_buildings) { building_color_lersp.Remove(c); }

            // merge chunks
            if (inactive_blocks.Count >= chunk_size)
            {
                Merge_Chunk();
            }

            // finish
            if (active_blocks.Count == 0 && building_color_lersp.Count == 0 && can_finish)
                Finish_Building_Growth();
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
                    if (decrement_steps)
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
                    new_building.GetComponent<Building>().Renderer.material = materials[0];
                    new_building.GetComponent<Building>().Renderer.material.color = growth_color;

                    // add to active blocks
                    active_blocks.Add(new_building.GetComponent<IGrowingBlock>());

                    // initialite building 
                    new_building.GetComponent<IGrowingBlock>().Initialize(this, c);

                    _building_generation++;
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

        private void Finish_Building_Growth() 
        {   
            if (!finished_growing)
            {
                Merge_Chunk(true);
                finished_growing = true;
            }
        }

        // public interfaces

        public void Deactivate_Block(IGrowingBlock block) 
        {
            // add buildings to delay dic
            if (block is Building block_as_building)
            {
                building_color_lersp.Add(block_as_building, 0);

                // create new building
                if (!can_finish)
                    Create_Building(Calculate_Cambiums(block_as_building));

                active_blocks.Remove(block);
            }
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

            if (_building_generation < _growth_speed_over_lifetime.keys[_growth_speed_over_lifetime.keys.Length - 1].time)
                value = _growth_speed_over_lifetime.Evaluate(_building_generation);
            else
            {
               can_finish = true;
               value = _growth_speed_over_lifetime.Evaluate(_growth_speed_over_lifetime.keys[_growth_speed_over_lifetime.keys.Length - 1].time);
            }

            return value;
        }

        public float Delay { get { return _delay * Tower_Manager.Instance.Delay_Multiplier; } }

        public int Steps { get { return _steps; } }

        public Tower_Input_Mapper Mapper { get { return mapper; } }

        public int Building_Generation { get { return _building_generation; } }

        public LayerMask Layer { get { return _layer; } }

        public static Vector3 Calculate_Grow_Direction(Vector3 player_dir, Vector3 normal_dir) 
        {
            if (Vector3.Dot(player_dir, normal_dir) >= 0)
            {
                return player_dir;
            }
            else
            {
                return -player_dir;
            }

        }

        // merging
        void Merge_Chunk(bool merge_all = false)
        {
            List<CombineInstance> combine = new List<CombineInstance>();

            // create new chunk
            var new_chunk = new GameObject("Chunk #" + (merged_chunks.Count));
            new_chunk.transform.SetParent(transform);
            new_chunk.tag = "Building";
            new_chunk.isStatic = true;
            new_chunk.AddComponent<MeshFilter>();
            new_chunk.AddComponent<MeshRenderer>();
            new_chunk.GetComponent<MeshRenderer>().material = materials[0];

            // add building meshes
            for (int i = 0; i < (merge_all ? inactive_blocks.Count : chunk_size); i++)
            {
                if (inactive_blocks[i] is Building block_as_building)
                {
                    var instance = new CombineInstance();
                    instance.mesh = block_as_building.Renderer.GetComponent<MeshFilter>().sharedMesh;
                    instance.transform = block_as_building.Renderer.transform.localToWorldMatrix;
                    combine.Add(instance);
                }
            }

            // combine
            new_chunk.GetComponent<MeshFilter>().mesh = new Mesh();
            new_chunk.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
            new_chunk.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
            merged_chunks.Add(new_chunk);

            // delet blocks
            foreach (var c in inactive_blocks.ToList())
            {
                (c as Building).On_Merged();
                inactive_blocks.RemoveAt(0);
            }

            // do a deep merge
            if (merged_chunks.Count >= deep_chunk_size || merge_all)
                Deep_Merge();
        }

        void Deep_Merge() 
        {
            List<CombineInstance> combine = new List<CombineInstance>();

            // create new chunk
            var new_chunk = new GameObject("Deep Chunk #" + (deep_merged_chunk.Count));
            new_chunk.transform.SetParent(transform);
            new_chunk.tag = "Building";
            new_chunk.isStatic = true;
            new_chunk.AddComponent<MeshFilter>();
            new_chunk.AddComponent<MeshRenderer>();
            new_chunk.GetComponent<MeshRenderer>().material = materials[0];

            // add building meshes
            for (int i = 0; i < merged_chunks.Count; i++)
            {
                var instance = new CombineInstance();
                instance.mesh = merged_chunks[i].GetComponent<MeshFilter>().sharedMesh;
                instance.transform = merged_chunks[i].GetComponent<MeshRenderer>().transform.localToWorldMatrix;
                combine.Add(instance);
            }

            // combine
            new_chunk.GetComponent<MeshFilter>().mesh = new Mesh();
            new_chunk.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            new_chunk.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
            deep_merged_chunk.Add(new_chunk);

            foreach (var c in merged_chunks.ToList())
            {
                Destroy(c);
                merged_chunks.RemoveAt(0);
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
            public int branch_ID;

            // at building
            public Cambium(Vector3 point, Vector3 normal, GameObject prefab, int steps, int branch_ID = 0)
            {
                this.point = point;
                this.normal = normal;
                this.prefab = prefab;
                this.steps = steps;
                this.branch_ID = branch_ID;
            }

            // as origin
            public Cambium(Vector3 origin, GameObject prefab)
            {
                this.point = origin;
                this.normal = Vector3.up;
                this.prefab = prefab;
                this.steps = 0;
                this.branch_ID = 0;
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
            public Vector3 hit_normal;
            public float orb_energy;
            public float throw_dist;
            public float throw_time;
            public float player_speed;
            public string ground_tag;

            public Player_Inputs(Vector3 player_dir, Vector3 hit_normal, float orb_energy, float throw_dist, float throw_time, float player_speed, string ground_tag)
            {
                this.player_dir = player_dir;
                this.hit_normal = hit_normal;
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

/*
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
        [SerializeField] LayerMask _layer;
        [SerializeField] int _start_cambiums;
        [SerializeField] float _growth_speed;
        [SerializeField] Vector3 override_start_direction;
        [SerializeField] AnimationCurve _growth_speed_over_lifetime = AnimationCurve.Linear(0, 1, 1 , 1);
        [SerializeField] float _delay;
        [SerializeField] [Range(1, 30)] int _steps;
        [SerializeField] bool decrement_steps = true;
        [SerializeField] bool start_steps_zero = false;
        [SerializeField] int chunk_size;
        [SerializeField] int deep_chunk_size = 10;
        [SerializeField] Material[] materials;
        [SerializeField] Material growth_material;

        [Header("Debugging")]
        [SerializeField] Player_Inputs inputs;
        [SerializeField] int _building_generation = 0;
        [SerializeField] bool can_finish;
        [SerializeField] bool finished_growing;

        // stored growth data
        Tower_Input_Mapper mapper;
        List<IGrowingBlock> active_blocks = new List<IGrowingBlock>();
        List<IGrowingBlock> inactive_blocks = new List<IGrowingBlock>();
        List<GameObject> merged_chunks = new List<GameObject>();
        List<GameObject> deep_merged_chunk = new List<GameObject>();

        Dictionary<Building, float> building_delays = new Dictionary<Building, float>();

        // register at manager
        private void Awake()
        {
            // get mapper
            mapper = GetComponent<Tower_Input_Mapper>();

            // register tower
            Tower_Manager.Instance.Add_Tower(this);
        }

        public void Initialize(Player_Inputs inputs)
        {
            // save input for debbuging purpose
            this.inputs = inputs;

            // initialize mapper with inputs
            mapper.Initialize(inputs);

            // assign growth parameter
            Assign_Input_To_Growth_Parameter();

            // spawn first building(s)
            Spawn_First_Building();

            // choose random materials
            materials = new Material[] { materials[Random.Range(0, materials.Length)] };
        }

        // change growth parameter
        public void Assign_Input_To_Growth_Parameter() 
        {
            // growth speed
            _growth_speed *= mapper.Grow_Speed;

            // generations
            var keys = _growth_speed_over_lifetime.keys;

            // remap keys
            float factor = mapper.Generation_Amount * mapper.Width / keys[keys.Length - 1].time;

            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].time *= factor;
            }

            _growth_speed_over_lifetime.keys = keys;

            // change tangents to linear
            for (int i = 0; i < _growth_speed_over_lifetime.keys.Length; i++)
            {
                keys[i].time *= factor;
                keys[i].tangentMode = 2; 
            }
        }

        // growing management
        void Spawn_First_Building() 
        {
            for (int i = 0; i < _start_cambiums; i++)
            {
                var c = new Cambium[1];
                c[0] = new Cambium(transform.position, Building_Prefabs[0]); // index 0 is always the first spawned building
                c[0].steps = start_steps_zero? 0 : Steps;
                c[0].normal = override_start_direction.magnitude == 0? Calculate_Grow_Direction(mapper.Player_Direction, mapper.Normal_Direction) : override_start_direction;
                Create_Building(new Cambiums_At_Active(null, c));
            }
        }

        public void Update_Growth()
        {
            // call growth updates on blocks
            for (int i = 0; i < active_blocks.Count; i++) { active_blocks[i].On_Update_Growth(Growth_Speed == 0 ? 0.2f : Growth_Speed); }

            // count delays
            List<Building> finished_buildings = new List<Building>();

            foreach (var c in building_delays.Keys.ToList())
            {
                if (c != null)
                {
                    building_delays[c] += Time.deltaTime;

                    if (building_delays[c] >= Delay)
                    {
                        if (!can_finish)
                            Create_Building(Calculate_Cambiums(c));
                        finished_buildings.Add(c);

                        if (c.Renderer)
                            c.Renderer.sharedMaterial = materials[0];
                    }
                }
            }

            foreach (var c in finished_buildings) { building_delays.Remove(c); }

            // merge chunks
            if (inactive_blocks.Count >= chunk_size)
            {
                Merge_Chunk();
            }

            // finish
            if (active_blocks.Count == 0 && building_delays.Count == 0 && can_finish)
                Finish_Building_Growth();
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
                    if (decrement_steps)
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
                    new_building.GetComponent<Building> ().Renderer.material = growth_material;

                    // add to active blocks
                    active_blocks.Add(new_building.GetComponent<IGrowingBlock>());

                    // initialite building 
                    new_building.GetComponent<IGrowingBlock>().Initialize(this, c);

                    _building_generation++;
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

        private void Finish_Building_Growth() 
        {
      
            if (!finished_growing)
            {
                Merge_Chunk(true);
                finished_growing = true;
            }
        }

        // public interfaces

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

            if (_building_generation < _growth_speed_over_lifetime.keys[_growth_speed_over_lifetime.keys.Length - 1].time)
                value = _growth_speed_over_lifetime.Evaluate(_building_generation);
            else
            {
                can_finish = true;
               value = _growth_speed_over_lifetime.Evaluate(_growth_speed_over_lifetime.keys[_growth_speed_over_lifetime.keys.Length - 1].time);
            }

            return value;
        }

        public float Delay { get { return _delay * Tower_Manager.Instance.Delay_Multiplier; } }

        public int Steps { get { return _steps; } }

        public Tower_Input_Mapper Mapper { get { return mapper; } }

        public int Building_Generation { get { return _building_generation; } }

        public LayerMask Layer { get { return _layer; } }

        public static Vector3 Calculate_Grow_Direction(Vector3 player_dir, Vector3 normal_dir) 
        {
            if (Vector3.Dot(player_dir, normal_dir) >= 0)
            {
                return player_dir;
            }
            else
            {
                return -player_dir;
            }

        }

        // merging
        void Merge_Chunk(bool merge_all = false)
        {
            List<CombineInstance> combine = new List<CombineInstance>();

            // create new chunk
            var new_chunk = new GameObject("Chunk #" + (merged_chunks.Count));
            new_chunk.transform.SetParent(transform);
            new_chunk.tag = "Building";
            new_chunk.isStatic = true;
            new_chunk.AddComponent<MeshFilter>();
            new_chunk.AddComponent<MeshRenderer>();
            new_chunk.GetComponent<MeshRenderer>().material = materials[0];

            // add building meshes
            for (int i = 0; i < (merge_all ? inactive_blocks.Count : chunk_size); i++)
            {
                if (inactive_blocks[i] is Building block_as_building)
                {
                    var instance = new CombineInstance();
                    instance.mesh = block_as_building.Renderer.GetComponent<MeshFilter>().sharedMesh;
                    instance.transform = block_as_building.Renderer.transform.localToWorldMatrix;
                    combine.Add(instance);
                }
            }

            // combine
            new_chunk.GetComponent<MeshFilter>().mesh = new Mesh();
            new_chunk.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
            new_chunk.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
            merged_chunks.Add(new_chunk);

            // delet blocks
            foreach (var c in inactive_blocks.ToList())
            {
                (c as Building).On_Merged();
                inactive_blocks.RemoveAt(0);
            }

            // do a deep merge
            if (merged_chunks.Count >= deep_chunk_size || merge_all)
                Deep_Merge();
        }

        void Deep_Merge() 
        {
            List<CombineInstance> combine = new List<CombineInstance>();

            // create new chunk
            var new_chunk = new GameObject("Deep Chunk #" + (deep_merged_chunk.Count));
            new_chunk.transform.SetParent(transform);
            new_chunk.tag = "Building";
            new_chunk.isStatic = true;
            new_chunk.AddComponent<MeshFilter>();
            new_chunk.AddComponent<MeshRenderer>();
            new_chunk.GetComponent<MeshRenderer>().material = materials[0];

            // add building meshes
            for (int i = 0; i < merged_chunks.Count; i++)
            {
                var instance = new CombineInstance();
                instance.mesh = merged_chunks[i].GetComponent<MeshFilter>().sharedMesh;
                instance.transform = merged_chunks[i].GetComponent<MeshRenderer>().transform.localToWorldMatrix;
                combine.Add(instance);
            }

            // combine
            new_chunk.GetComponent<MeshFilter>().mesh = new Mesh();
            new_chunk.GetComponent<MeshFilter>().mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            new_chunk.GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
            deep_merged_chunk.Add(new_chunk);

            foreach (var c in merged_chunks.ToList())
            {
                Destroy(c);
                merged_chunks.RemoveAt(0);
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
            public int branch_ID;

            // at building
            public Cambium(Vector3 point, Vector3 normal, GameObject prefab, int steps, int branch_ID = 0)
            {
                this.point = point;
                this.normal = normal;
                this.prefab = prefab;
                this.steps = steps;
                this.branch_ID = branch_ID;
            }

            // as origin
            public Cambium(Vector3 origin, GameObject prefab)
            {
                this.point = origin;
                this.normal = Vector3.up;
                this.prefab = prefab;
                this.steps = 0;
                this.branch_ID = 0;
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
            public Vector3 hit_normal;
            public float orb_energy;
            public float throw_dist;
            public float throw_time;
            public float player_speed;
            public string ground_tag;

            public Player_Inputs(Vector3 player_dir, Vector3 hit_normal, float orb_energy, float throw_dist, float throw_time, float player_speed, string ground_tag)
            {
                this.player_dir = player_dir;
                this.hit_normal = hit_normal;
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
*/