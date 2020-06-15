﻿using UnityEngine;
using System.ComponentModel;
using System.Collections.Generic;
using TMPro;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Tower_Management
{
    public abstract class Building : MonoBehaviour, IGrowingBlock
    {
        private Tower _tower;
        
        [Header("Building Parameter")]
        [SerializeField] GameObject mesh;
        [SerializeField] Collider _main_collider;
        [SerializeField] [Range(0, 1)] float origin_turn;
        [SerializeField] List<Transform> horizontal_origins;
        [SerializeField] List<Transform> vertical_origins;

        /// <summary>
        /// Owning Tower of the Building
        /// </summary>
        public Tower Tower { get { return _tower; } }

        /// <summary>
        /// Collider for physics 
        /// </summary>
        public Collider Main_Collider { get { return _main_collider; } }

        /// <summary>
        /// Sets the owning Tower of the Building, called automatically on instantiation
        /// </summary>
        public void Initialize(Tower tower) { _tower = tower; _main_collider = GetComponent<Collider>(); }

        /// <summary>
        /// Update Method called by owning Tower
        /// </summary>
        public abstract void On_Update_Growth(float speed);

        /// <summary>
        /// Deactivates the Building, no more updates called
        /// </summary>
        protected void Deactivate()
        {
            _tower.Deactivate_Block(this);
        }

        /// <summary>
        /// Devide weather the origin is on the horizontal or vertical side of the Building
        /// </summary>
        public Transform Origin_From_Normal(Vector3 normal)
        {
            float dot = Vector3.Dot(Vector3.up, normal);
            Transform origin = Mathf.Abs(dot) > origin_turn ? horizontal_origins[Random.Range(0, horizontal_origins.Count)] : vertical_origins[Random.Range(0, vertical_origins.Count)];
            mesh.transform.SetParent(origin);
            return origin;
        }
    }
}