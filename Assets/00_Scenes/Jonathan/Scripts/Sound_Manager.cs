using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Settings
{
    public class Sound_Manager : Singleton<Sound_Manager>
    {
        // container
        [SerializeField] Sound_Clip_Container container;

        // public interface
        public Sound_Clip Get_Clip(string ID)
        {
            foreach (var c in container.clips)
            {
                if (c.ID == ID)
                    return c;
            }

            Debug.LogError("No clip \"" + ID + "\" found!");

            return default;
        }

        public void Play_At(string ID, AudioSource at_source, bool if_possible = false) 
        {
            if (if_possible && !at_source.isPlaying || !if_possible)
            {
                var clip = Get_Clip(ID);

                at_source.volume = clip.volume * Settings.Volume;
                at_source.clip = clip.clip;
                at_source.Play();
            }
        } 

        [System.Serializable]
        public struct Sound_Clip
        {
            public string ID;
            public AudioClip clip;
            [Range(0, 1)] public float volume;
        }
    }
}