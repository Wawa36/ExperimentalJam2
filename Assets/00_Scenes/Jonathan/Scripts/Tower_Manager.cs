using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace Tower_Management
{
    public class Tower_Manager : Singleton<Tower_Manager>
    {
        [SerializeField] float _growth_speed_multiplier = 1;
        [SerializeField] float _delay_multiplier = 1;
        [SerializeField] bool _enable_rotation = true;
        bool _is_paused;

        List<Tower> active_towers = new List<Tower>();

        public float Growth_Speed_Multiplier { get { return _growth_speed_multiplier; } }

        public float Delay_Multiplier { get { return _delay_multiplier; } }

        public bool Enable_Rotation { get { return _enable_rotation; } }

        public bool Is_Paused { get { return _is_paused; } set { _is_paused = value; } }

        public void Add_Tower(Tower tower) { active_towers.Add(tower); }

        void Update()
        {
            if (!Is_Paused)
            {
                foreach (Tower c in active_towers) { c.Update_Growth(); }
            }
        }
    }

    [CustomEditor(typeof(Tower_Manager))]
    public class Tower_Manager_Editor : Editor 
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = target as Tower_Manager;

            if (Application.isPlaying)
            {
                if (script.Is_Paused)
                {
                    if (GUILayout.Button("Resume"))
                        script.Is_Paused = false;
                }
                else
                {
                    if (GUILayout.Button("Pause"))
                        script.Is_Paused = true;
                }
            }
        }
    }
}