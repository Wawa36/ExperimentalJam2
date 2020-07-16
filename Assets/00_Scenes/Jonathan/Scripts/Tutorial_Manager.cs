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
    PlayerMovement pm;
    bool step_changed;

    void Start()
    {
        PlayerMovement.Instance.allowedToCallBackOrb = false;
        PlayerMovement.Instance.allowedToJump = false;
        PlayerMovement.Instance.allowedToMoveCamera = false;
        PlayerMovement.Instance.allowedToRun = false;
        PlayerMovement.Instance.allowedToSwapOrbs = false;
        PlayerMovement.Instance.allowedToTeleport = false;
        PlayerMovement.Instance.allowedToThrow = false;
        pm = PlayerMovement.Instance;

        Step_Completed();
    }

    void Update()
    {
        if (step_changed)
        {
            if (current_step == 0)
            {
                if (pm.didTheFirstMoveX && pm.didTheFirstCameraMove || pm.didTheFirstCameraMove && pm.didTheFirstMoveZ)
                    Step_Completed();
            }
            else if (current_step == 1)
            {
                if (Vector3.Distance(pm.transform.position, pm.activeOrb.transform.position) < 5)
                    Step_Completed();
            }
            else if (current_step == 2)
            {
                if (pm.didTheFirstCallBack)
                    Step_Completed();
            }
            else if (current_step == 3)
            {
                if (pm.didTheFirstThrow)
                    Step_Completed();
            }
            else if (current_step == 4)
            {
                if (pm.didTheFirstTeleport && pm.didTheFirstCallBack)
                    Step_Completed();
            }
            else if (current_step == 5)
            {
                if (pm.didTheFirstOrbSwap)
                    Step_Completed();
            }
        }
    }

    public void Step_Completed()
    {
        Next_Step();
        step_changed = false;
    }

    void Next_Step() 
    {
        StartCoroutine(Step_Delay());
    }

    IEnumerator Step_Delay() 
    {
        if (current_step >= 0)
            yield return new WaitForSecondsRealtime(steps[current_step].disappear_delay);

        Reset_Step(current_step);
        
        yield return new WaitForSecondsRealtime(steps[current_step + 1].popup_delay);

        current_step++;
        Set_Step(current_step);

        step_changed = true;
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

            PlayerMovement.Instance.allowedToCallBackOrb = steps[index].allow_take_orb;
            PlayerMovement.Instance.allowedToJump = steps[index].allow_movement;
            PlayerMovement.Instance.allowedToMoveCamera = steps[index].allow_movement;
            PlayerMovement.Instance.allowedToRun = steps[index].allow_movement;
            PlayerMovement.Instance.allowedToSwapOrbs = steps[index].allow_switch;
            PlayerMovement.Instance.allowedToTeleport = steps[index].allow_teleport;
            PlayerMovement.Instance.allowedToThrow = steps[index].allow_throw_charge;
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
        public float popup_delay;
        public float disappear_delay;
        public bool allow_movement;
        public bool allow_take_orb;
        public bool allow_throw_charge;
        public bool allow_teleport;
        public bool allow_switch;
        public bool spawn_details;
    }
}
