using System.Collections;
using System.Collections.Generic;
using Tower_Management;
using UnityEngine;

namespace Tower_Management
{
    public class Tower_Input_Mapper : MonoBehaviour
    {
        Tower.Player_Inputs inputs;

        public void Initialize(Tower.Player_Inputs inputs)
        {
            this.inputs = inputs;
        }

        // properties 
        public float Grow_Speed { get { return Calculate_Grow_Speed(); } }

        public Vector3 Grow_Direction { get { return Calculate_Grow_Direction(); } }

        public int Split_Chance { get { return Calculate_Split_Chance(); } }

        public int Generation_Amount { get { return Calculate_Generation_Amount(); } }

        public int Change_Direction_Chance { get { return Calculate_Change_Direction_Chance(); } }

        // calculations

        float Calculate_Grow_Speed()
        {
            return 1;
        }

        Vector3 Calculate_Grow_Direction()
        {
            return inputs.player_dir;
        }

        int Calculate_Split_Chance()
        {
            return default;
        }

        int Calculate_Generation_Amount()
        {
            return default;
        }

        int Calculate_Change_Direction_Chance() 
        {
            return default;
        } 
    }
}
