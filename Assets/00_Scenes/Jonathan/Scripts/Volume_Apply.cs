using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    public class Volume_Apply : MonoBehaviour
    {
        [SerializeField] AudioSource[] sources;

        private void Awake()
        {
            foreach (var c in sources)
                c.volume *= Settings.Volume;
        }
    }
}