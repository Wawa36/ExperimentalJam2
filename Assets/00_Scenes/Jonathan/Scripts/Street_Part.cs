using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tower_Management
{
    public class Street_Part : MonoBehaviour
    {
        [SerializeField] List<Transform> directions = new List<Transform>();

        public Transform[] Directions { get { return directions.ToArray(); } }
    }
}
