using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tower_Management.Details
{
    public class Detail : MonoBehaviour
    { 
        [SerializeField] MeshFilter _filter;

        void On_Finished_Growing() 
        {
            Detail_Spawner.Instance.Detail_Finished_Growing(this);
        }

        public MeshFilter Filter { get { return _filter; } }
       
    }
}
