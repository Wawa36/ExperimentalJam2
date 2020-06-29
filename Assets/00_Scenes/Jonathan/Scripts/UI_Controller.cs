using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Settings_Management;

public class UI_Controller : Singleton<UI_Controller>
{
    [Header("Panel")]
    [SerializeField] GameObject menu_panel;
    [SerializeField] GameObject settings_panel;

    [Header("Buttons")]
    [SerializeField] Button[] buttons;

    [Header("Settings Elements")]
    [SerializeField] Slider quaility_slider;
    [SerializeField] Slider volume_slider;
    [SerializeField] Slider sensitivity_x_slider;
    [SerializeField] Slider sensitivity_y_slider;

    bool is_paused;
    public int button_index = 0;
    public bool controller_input_trigger = true;

    void Start()
    {
        Assign_Settings_Elements();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (is_paused)
                Continue();
            else
                Open_Menu();
        }

        // select button
        if (Input.GetAxis("Menu Selection") != 0 && is_paused)
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
        if (Input.GetButtonDown("Menu Click") && is_paused)
            buttons[button_index].onClick.Invoke();

        if (Input.GetButtonDown("Menu Abort") && is_paused)
            Set_Panel(1);
    }

    // button interaction
    public void Open_Menu() 
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Set_Panel(1);

        is_paused = true;
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        Set_Panel(0);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        is_paused = false;
    }

    public void Open_Settings()
    {
        Set_Panel(2);
    }

    public void Go_Main_Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    // panel management
    void Set_Panel(int index)
    {
        switch (index)
        {
            // nothing
            case 0:
                menu_panel.SetActive(false);
                settings_panel.SetActive(false);
                break;
            // menu
            case 1:
                menu_panel.SetActive(true);
                settings_panel.SetActive(false);
                button_index = 0;
                buttons[button_index].Select();
                break;
            // settings
            case 2:
                menu_panel.SetActive(false);
                settings_panel.SetActive(true);
                break;
        }
    }

    // settings management
    void Assign_Settings_Elements()
    {
        quaility_slider.SetValueWithoutNotify(Game_Settings.Quality);
        volume_slider.SetValueWithoutNotify(Game_Settings.Volume);
        sensitivity_x_slider.SetValueWithoutNotify(Game_Settings.Sensitivity_X);
        sensitivity_y_slider.SetValueWithoutNotify(Game_Settings.Sensitivity_Y);

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
