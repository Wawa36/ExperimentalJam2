using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Tower_Management.Details;
using UnityEngine;
using UnityEngine.Rendering;

public class Tutorial_Manager : Singleton<Tutorial_Manager>
{
    [SerializeField] Step[] steps;
    int current_step = -1;

    public void Step_Completed()
    {
        Next_Step();
    }

    void Next_Step() 
    {
        Reset_Step(current_step);
        current_step++;
        Set_Step(current_step);
    }

    void Set_Step(int index)
    {
        if (index < steps.Length)
        {
            steps[index].panel.SetActive(true);

            // details
            if (steps[index].spawn_details)
                Detail_Spawner.Instance.Activate();
            else
                Detail_Spawner.Instance.Deactivate();
        }
    }

    void Reset_Step(int index) 
    {
        if (index >= 0)
        steps[index].panel.SetActive(false);
    }

    [System.Serializable]
    struct Step 
    {
        public string name;
        public GameObject panel;
        public bool allow_movement;
        public bool allow_take_orb;
        public bool allow_throw_charge;
        public bool allow_teleport;
        public bool spawn_details;
    }
}
