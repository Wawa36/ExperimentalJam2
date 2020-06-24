using UnityEngine;
using UnityEditor;

namespace Tower_Management
{
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