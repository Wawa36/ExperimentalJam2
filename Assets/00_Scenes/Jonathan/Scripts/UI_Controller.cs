using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Settings_Management;

public class UI_Controller : Singleton<UI_Controller>
{
    [Header("Panel")]
    [SerializeField] GameObject menu_panel;
    [SerializeField] GameObject settings_panel;

    [Header("Settings Elements")]
    [SerializeField] Slider quaility_slider;
    [SerializeField] Slider volume_slider;
    [SerializeField] Slider sensitivity_x_slider;
    [SerializeField] Slider sensitivity_y_slider;

    bool is_paused;

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
    }

    // button interaction
    public void Open_Menu() 
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Set_Panel(1);

        is_paused = true;
        GetComponent<UI_Controller_Input>().Is_Active = true;
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        Set_Panel(0);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        is_paused = false;
        GetComponent<UI_Controller_Input>().Is_Active = false;
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
