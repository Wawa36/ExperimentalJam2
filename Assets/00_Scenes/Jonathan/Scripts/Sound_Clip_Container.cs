using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "Sound Clips", menuName = "Settings/Sound Clip Container", order = 2)]
    public class Sound_Clip_Container : ScriptableObject
    {
        public Sound_Manager.Sound_Clip[] clips;
    }
}