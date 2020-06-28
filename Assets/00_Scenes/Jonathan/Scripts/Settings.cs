using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Settings
{
    public static class Settings
    {
        // default values
        static float default_sensitivity_X = 1f;
        static float default_sensitivity_Y = 1f;

        static float default_volume = 1;

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
        }
    }
}