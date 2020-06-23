using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    [SerializeField] Text display;
    [SerializeField] [Range(0, 5)] int rate;

    float counter = 100;

    // Update is called once per frame
    void Update()
    {
        if (counter >= rate)
        {
            display.text = Mathf.Round(1 / Time.deltaTime).ToString() + " fps";
            counter = 0;
        }
        else
            counter+= Time.deltaTime;
    }
}
