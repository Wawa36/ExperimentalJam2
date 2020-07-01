using Settings_Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller_Input : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button[] buttons;
    [SerializeField] Button settings_button;
    [SerializeField] Button abort_button;

    [Header("Settings")]
    [SerializeField] Button[] settings_header;
    [SerializeField] UI_Slider[] settings_slider;
    [SerializeField] bool _is_active;

    int button_index;
    bool controller_input_trigger = true;
    bool controller_input_trigger2 = true;

    int settings_header_index;
    bool is_settings = false;

    // select first butto at start
    void Start()
    {
        buttons[0].Select();
    }

    // controller input
    void Update()
    {
        if (Is_Active)
            Inputs();
    }

    void Inputs() 
    {
        // select button
        if (!is_settings)
        {
            if (Input.GetAxis("Controller Pad Vertical") != 0)
            {
                if (controller_input_trigger)
                {
                    if (Input.GetAxis("Controller Pad Vertical") < 0)
                    {
                        button_index++;

                        if (button_index == buttons.Length)
                            button_index = 0;
                    }
                    else if (Input.GetAxis("Controller Pad Vertical") > 0)
                    {
                        button_index--;

                        if (button_index == -1)
                            button_index = buttons.Length - 1;
                    }

                    buttons[button_index].Select();
                    controller_input_trigger = false;
                }
            }
            else
                controller_input_trigger = true;

            // click
            if (Input.GetButtonDown("Menu Click"))
            {
                buttons[button_index].onClick.Invoke();

                if (buttons[button_index] == settings_button)
                {
                    settings_header[0].Select();
                    settings_header_index = 0;
                    is_settings = true;
                }
            }
        }
        else
        {
            if (Input.GetAxis("Controller Pad Vertical") != 0)
            {
                if (controller_input_trigger)
                {
                    if (Input.GetAxis("Controller Pad Vertical") < 0)
                    {
                        settings_header_index++;

                        if (settings_header_index == settings_header.Length)
                            settings_header_index = 0;
                    }
                    else if (Input.GetAxis("Controller Pad Vertical") > 0)
                    {
                        settings_header_index--;

                        if (settings_header_index == -1)
                            settings_header_index = settings_header.Length - 1;
                    }

                    settings_header[settings_header_index].Select();
                    controller_input_trigger = false;
                }
            }
            else
                controller_input_trigger = true;

            // click
            if (Input.GetAxis("Controller Pad Horizontal") != 0)
            {
                int multiplier = Input.GetAxis("Controller Pad Horizontal") > 0 ? 1 : -1;

                if (controller_input_trigger2)
                    settings_slider[settings_header_index].slider.value += settings_slider[settings_header_index].step_size * multiplier;

                controller_input_trigger2 = false;

            }
            else
                controller_input_trigger2 = true;
        }

        if (Input.GetButtonDown("Menu Abort"))
        {
            abort_button.onClick.Invoke();
            is_settings = false;

            buttons[0].Select();
            button_index = 0;
        }
    }

    public bool Is_Active { get { return _is_active; } set { buttons[0].Select();  _is_active = value; } }

    [System.Serializable]
    struct UI_Slider
    {
        public Slider slider;
        public float step_size;
    }
}
