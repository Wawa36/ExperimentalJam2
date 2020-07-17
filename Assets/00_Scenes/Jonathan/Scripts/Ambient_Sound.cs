using Settings_Management;
using UnityEngine;

public class Ambient_Sound : MonoBehaviour
{
    [SerializeField] float start_height;
    [SerializeField] float max_height;
    [SerializeField] AudioSource source;
    public float value;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        source.clip = Sound_Manager.Instance.Get_Clip("Wind Atmo").clip;
    }

    // Update is called once per frame
    void Update()
    {
        float player_y_pos = player.position.y;

        float volume = Mathf.InverseLerp(start_height, max_height, player_y_pos);
        value = Mathf.InverseLerp(start_height, max_height, player_y_pos);
        source.volume = volume * Sound_Manager.Instance.Get_Clip("Wind Atmo").volume * Game_Settings.Volume;
    }
}
