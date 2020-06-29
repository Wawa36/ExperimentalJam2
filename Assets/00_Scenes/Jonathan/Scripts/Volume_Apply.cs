using Settings_Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Settings_Management
{
    public class Volume_Apply : MonoBehaviour
    {
        [SerializeField] AudioSource[] sources;

        private void Awake()
        {
            foreach (var c in sources)
                c.volume *= Game_Settings.Volume;
        }
    }
}