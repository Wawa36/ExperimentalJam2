using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tower_Management
{
    public class Tower_Manager : Singleton<Tower_Manager>
    {
        [SerializeField] float _growth_speed_multiplier = 1;
        [SerializeField] float _delay_multiplier = 1;
        [SerializeField] bool _enable_rotation = true;

        [SerializeField] List<Tower> active_towers = new List<Tower>();

        public float Growth_Speed_Multiplier { get { return _growth_speed_multiplier; } }

        public float Delay_Multiplier { get { return _delay_multiplier; } }

        public bool Enable_Rotation { get { return _enable_rotation; } }

        public void Add_Tower(Tower tower) { active_towers.Add(tower); }

        void Update()
        {
            foreach (Tower c in active_towers) { c.Update_Growth(); }
        }
    }
}