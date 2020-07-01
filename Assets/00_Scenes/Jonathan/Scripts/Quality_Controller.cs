using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Quality_Controller : Singleton<Quality_Controller>
{
    [SerializeField] Custom_Quality_Level[] level = new Custom_Quality_Level[6];
    Volume volume;
    ParticleSystem clouds;

    public override void Awake()
    {
        base.Awake();

        var c = GameObject.FindGameObjectWithTag("postProcess");
        if (c)
            volume = c.GetComponent<Volume>();

        c = GameObject.FindGameObjectWithTag("Cloud Manager");
        if (c)
            clouds = c.GetComponent<ParticleSystem>();
    }

    public void Set_Level(int to_level) 
    {
        QualitySettings.SetQualityLevel(to_level);

        // enable/disable pp
        if (volume)
            volume.enabled = level[to_level].use_pp;

        // set coud rate
        if (clouds)
        {
            var c = clouds.emission;
            c.rateOverTime = level[to_level].cloud_rate;
        }
    }

    [System.Serializable]
    public struct Custom_Quality_Level
    {
        public bool use_pp;
        public float cloud_rate;
    }
}
