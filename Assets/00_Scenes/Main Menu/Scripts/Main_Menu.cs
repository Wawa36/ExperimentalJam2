using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Settings_Management;
using UnityEngine.Video;

public class Main_Menu : Singleton<Main_Menu>
{
    [Header("Panel")]
    [SerializeField] GameObject main_panel;
    [SerializeField] GameObject settings_panel;
    [SerializeField] GameObject controls_panel;
    [SerializeField] GameObject credits_panel;

    [Header("Settings Elements")]
    [SerializeField] Slider quaility_slider;
    [SerializeField] Slider volume_slider;
    [SerializeField] Slider sensitivity_x_slider;
    [SerializeField] Slider sensitivity_y_slider;

    [Header("Controlls")]
    [SerializeField] GameObject gamepad_controlls;
    [SerializeField] GameObject keyboard_controlls;

    void Start()
    {
        Set_Panel(0);
        Assign_Settings_Elements();
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

    public void Open_Controls() 
    {
        Set_Panel(3);
        print(Input.GetJoystickNames().Length);
        keyboard_controlls.SetActive(Input.GetJoystickNames().Length == 0);
        gamepad_controlls.SetActive(Input.GetJoystickNames().Length != 0);
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
                controls_panel.SetActive(false);
                break;
            // settings
            case 1:
                main_panel.SetActive(false);
                settings_panel.SetActive(true);
                credits_panel.SetActive(false);
                controls_panel.SetActive(false);
                break;
            // credits
            case 2:
                main_panel.SetActive(false);
                settings_panel.SetActive(false);
                credits_panel.SetActive(true);
                controls_panel.SetActive(false);
                break;
            case 3:
                main_panel.SetActive(false);
                settings_panel.SetActive(false);
                credits_panel.SetActive(false);
                controls_panel.SetActive(true);
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

        Quality_Controller.Instance.Set_Level(Game_Settings.Quality);
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
