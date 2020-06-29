using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Settings_Management
{
    public static class Game_Settings
    {
        // default values
        static float default_sensitivity_X = 3.5f;
        static float default_sensitivity_Y = 3.5f;

        static float default_volume = 1;

        static int default_quality = 5;

        // properties
        public static float Sensitivity_X
        {
            get
            {
                Create_Default(); return PlayerPrefs.GetFloat("Settings_Sensitivity_X");
            }

            set
            {
                PlayerPrefs.SetFloat("Settings_Sensitivity_X", value);
            }
        }

        public static float Sensitivity_Y
        {
            get
            {
                Create_Default(); return PlayerPrefs.GetFloat("Settings_Sensitivity_Y");
            }

            set
            {
                PlayerPrefs.SetFloat("Settings_Sensitivity_Y", value);
            }
        }

        public static float Volume
        {
            get
            {
                Create_Default(); return PlayerPrefs.GetFloat("Settings_Volume");
            }

            set
            {
                PlayerPrefs.SetFloat("Settings_Volume", value);
            }
        }

        public static int Quality
        {
            get
            {
                Create_Default(); return PlayerPrefs.GetInt("Settings_Qualiy");
            }

            set
            {
                PlayerPrefs.SetInt("Settings_Qualiy", value);
            }
        }

        // set default values
        static void Create_Default()
        {
            // sensitivity X
            if (!PlayerPrefs.HasKey("Settings_Sensitivity_X"))
                PlayerPrefs.SetFloat("Settings_Sensitivity_X", default_sensitivity_X);

            // sensitivity Y
            if (!PlayerPrefs.HasKey("Settings_Sensitivity_Y"))
                PlayerPrefs.SetFloat("Settings_Sensitivity_Y", default_sensitivity_Y);

            // volume
            if (!PlayerPrefs.HasKey("Settings_Volume"))
                PlayerPrefs.SetFloat("Settings_Volume", default_volume);

            // quality
            if (!PlayerPrefs.HasKey("Settings_Qualiy"))
                PlayerPrefs.SetInt("Settings_Qualiy", default_quality);
        }
    }
}