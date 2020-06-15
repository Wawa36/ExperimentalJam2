using UnityEngine;
using System.ComponentModel;

namespace Tower_Management
{
    public abstract class Building : MonoBehaviour, IGrowingBlock
    {
        private Tower _tower;
        private Collider _main_collider;

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
    }
}