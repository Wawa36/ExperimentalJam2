using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Settings_Management;
using UnityEngine.Video;

public class Main_Menu : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] GameObject main_panel;
    [SerializeField] GameObject settings_panel;
    [SerializeField] GameObject credits_panel;

    [Header("Buttons")]
    [SerializeField] Button[] buttons;

    [Header("Settings Elements")]
    [SerializeField] Slider quaility_slider;
    [SerializeField] Slider volume_slider;
    [SerializeField] Slider sensitivity_x_slider;
    [SerializeField] Slider sensitivity_y_slider;

    public int button_index;
    public bool controller_input_trigger = true;

    void Start()
    {
        Assign_Settings_Elements();
        Set_Panel(0);
    }

    // controller input
    private void Update()
    {
        // select button
        if (Input.GetAxis("Menu Selection") != 0)
        {
            if (controller_input_trigger)
            {
                if (Input.GetAxis("Menu Selection") < 0)
                {
                    button_index++;

                    if (button_index == buttons.Length)
                        button_index = 0;
                }
                else if (Input.GetAxis("Menu Selection") > 0)
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
            buttons[button_index].onClick.Invoke();

        if (Input.GetButtonDown("Menu Abort"))
            Set_Panel(0);
    }

    // button interaction
    public void Start_Game()
    {
        SceneManager.LoadScene("Game");        
    }

    public void Open_Main() 
    {
        Set_Panel(0);
    }

    public void Open_Settings() 
    {
        Set_Panel(1);
    }

    public void Open_Credits() 
    {
        Set_Panel(2);
    }

    public void Quit()
    {
        Application.Quit();
    }

    // panel management
    void Set_Panel(int index)
    {
        switch (index)
        {
            // main
            case 0:
                main_panel.SetActive(true);
                settings_panel.SetActive(false);
                credits_panel.SetActive(false);
                button_index = 0;
                buttons[button_index].Select();
                break;
            // settings
            case 1:
                main_panel.SetActive(false);
                settings_panel.SetActive(true);
                credits_panel.SetActive(false);
                break;
            // credits
            case 2:
                main_panel.SetActive(false);
                settings_panel.SetActive(false);
                credits_panel.SetActive(true);
                break;
        }
    }

    // settings management
    void Assign_Settings_Elements()
    {
        quaility_slider.SetValueWithoutNotify (Game_Settings.Quality);
        volume_slider.SetValueWithoutNotify (Game_Settings.Volume);
        sensitivity_x_slider.SetValueWithoutNotify (Game_Settings.Sensitivity_X);
        sensitivity_y_slider.SetValueWithoutNotify (Game_Settings.Sensitivity_Y);

        QualitySettings.SetQualityLevel(Game_Settings.Quality);
    }

    public void Override_Settings()
    {
        Game_Settings.Quality = (int)quaility_slider.value;
        Game_Settings.Volume = volume_slider.value;
        Game_Settings.Sensitivity_X = sensitivity_x_slider.value;
        Game_Settings.Sensitivity_Y = sensitivity_y_slider.value;

        QualitySettings.SetQualityLevel(Game_Settings.Quality);
    }
}
