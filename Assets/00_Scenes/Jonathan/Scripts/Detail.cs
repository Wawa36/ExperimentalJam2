using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tower_Management.Details
{
    public class Detail : MonoBehaviour
    { 
        [SerializeField] MeshFilter _filter;
        [SerializeField] float max_scale = 1.3f;
        [SerializeField] float min_scale = .7f;

        private void Start()
        {
            var scale = Random.Range(min_scale, max_scale);
            Filter.transform.localScale = new Vector3(scale, scale, scale);    
        }

        void On_Finished_Growing() 
        {
            Detail_Spawner.Instance.Detail_Finished_Growing(this);
        }

        public MeshFilter Filter { get { return _filter; } }
       
    }
}
