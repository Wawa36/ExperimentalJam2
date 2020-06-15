using UnityEngine;

namespace Tower_Management
{
    public abstract class Structure : MonoBehaviour, IGrowingBlock
    {
        private Tower _tower;
        private Collider _main_collider;

        /// <summary>
        /// Owning Tower of the Structure
        /// </summary>
        protected Tower Tower { get { return _tower; } }

        /// <summary>
        /// Collider, used physics 
        /// </summary>
        public Collider Main_Collider { get { return _main_collider; } }

        /// <summary>
        /// Sets the owning Tower of the Structure, called automatically on instantiation
        /// </summary>
        public void Initialize(Tower tower) { _tower = tower; _main_collider = GetComponent<Collider>(); }

        /// <summary>
        /// Update Method called by Tower
        /// </summary>
        public abstract void On_Update_Growth(float speed);

        /// <summary>
        /// Deactivates the Structure, no more updates called
        /// </summary>
        protected void Deactivate()
        {
            _tower.Deactivate_Block(this);
        }
    }
}
