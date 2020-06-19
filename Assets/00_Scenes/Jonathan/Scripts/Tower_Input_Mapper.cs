using System.Collections;
using System.Collections.Generic;
using Tower_Management;
using UnityEngine;

namespace Tower_Management
{
    public class Tower_Input_Mapper : MonoBehaviour
    {
        [SerializeField] Property grow_speed;
        [SerializeField] Property split_chance;
        [SerializeField] Property generation_amount;
        [SerializeField] Property change_direction_chance;

        Tower.Player_Inputs inputs;

        public void Initialize(Tower.Player_Inputs inputs)
        {
            this.inputs = inputs;
        }

        // properties 
        public float Grow_Speed { get { return Calculate_Grow_Speed(); } }

        public Vector3 Grow_Direction { get { return inputs.player_dir; } }

        public int Split_Chance { get { return Calculate_Split_Chance(); } }

        public int Generation_Amount { get { return Calculate_Generation_Amount(); } }

        public int Change_Direction_Chance { get { return Calculate_Change_Direction_Chance(); } }

        public string Ground_Tag { get { return inputs.ground_tag; } }

        // calculations

        float Calculate_Grow_Speed()
        {
            switch (grow_speed.input)
            {
                case input_values.OrbEnergy:
                    return  Map_Value(inputs.orb_energy, grow_speed.min, grow_speed.max);
                case input_values.PlayerSpeed:
                    return Map_Value(inputs.player_speed, grow_speed.min, grow_speed.max);
                case input_values.ThrowDistance:
                    return Map_Value(inputs.orb_energy, grow_speed.min, grow_speed.max);
                case input_values.ThrowTime:
                    return Map_Value(inputs.player_speed, grow_speed.min, grow_speed.max);
                default:
                    return default;
            }
        }

        int Calculate_Split_Chance()
        {
            switch (split_chance.input)
            {
                case input_values.OrbEnergy:
                    return Map_Value(inputs.orb_energy, grow_speed.min, grow_speed.max);
                case input_values.PlayerSpeed:
                    return Map_Value(inputs.player_speed, grow_speed.min, grow_speed.max);
                case input_values.ThrowDistance:
                    return Map_Value(inputs.orb_energy, grow_speed.min, grow_speed.max);
                case input_values.ThrowTime:
                    return Map_Value(inputs.player_speed, grow_speed.min, grow_speed.max);
                default:
                    return default;
            }
        }

        int Calculate_Generation_Amount()
        {
            switch (generation_amount.input)
            {
                case input_values.OrbEnergy:
                    return Map_Value(inputs.orb_energy, grow_speed.min, grow_speed.max);
                case input_values.PlayerSpeed:
                    return Map_Value(inputs.player_speed, grow_speed.min, grow_speed.max);
                case input_values.ThrowDistance:
                    return Map_Value(inputs.orb_energy, grow_speed.min, grow_speed.max);
                case input_values.ThrowTime:
                    return Map_Value(inputs.player_speed, grow_speed.min, grow_speed.max);
                default:
                    return default;
            }
        }

        int Calculate_Change_Direction_Chance() 
        {
            switch (change_direction_chance.input)
            {
                case input_values.OrbEnergy:
                    return Map_Value(inputs.orb_energy, grow_speed.min, grow_speed.max);
                case input_values.PlayerSpeed:
                    return Map_Value(inputs.player_speed, grow_speed.min, grow_speed.max);
                case input_values.ThrowDistance:
                    return Map_Value(inputs.orb_energy, grow_speed.min, grow_speed.max);
                case input_values.ThrowTime:
                    return Map_Value(inputs.player_speed, grow_speed.min, grow_speed.max);
                default:
                    return default;
            }
        }

        // mapping 
        int Map_Value (float value, int min, int max) 
        {
            return (int) Mathf.InverseLerp(min, max, value);
        }

        enum input_values {OrbEnergy, ThrowDistance, ThrowTime, PlayerSpeed };

        [System.Serializable]
        struct Property
        {
            public input_values input;
            public int min;
            public int max;
            public int multiplier;
        }
    }
}
