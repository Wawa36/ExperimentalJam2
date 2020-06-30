using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb_Animation : MonoBehaviour
{
    enum animation_type { RotateRandom, RotateY, Pulse }

    [Header ("Settings")]
    [SerializeField] GameObject[] parts;
    [SerializeField] animation_type animation;
    [SerializeField] float min_speed;
    [SerializeField] float max_speed;
    [SerializeField] float pulse_strength;

    [Header("Parameter")]
    [SerializeField] float default_speed;
    [SerializeField] float charge_add_speed;

    List<Moving_Part> moving_parts = new List<Moving_Part>();
    public float current_speed;

    void Start()
    {
        foreach (var c in parts)
        {
            Moving_Part new_part = new Moving_Part();

            new_part.instance = c;
            new_part.speed = Random.Range(min_speed, max_speed);
            new_part.axis = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            new_part.direction = Random.Range(0, 2) == 0 ? true : false;

            moving_parts.Add(new_part);
        }

        current_speed = default_speed;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < moving_parts.Count; i++)
        {
            if (animation == animation_type.RotateRandom)
                moving_parts[i] = Rotate_Random(moving_parts[i]);
            if (animation == animation_type.RotateY)
                moving_parts[i] = Rotate_Y(moving_parts[i]);
            if (animation == animation_type.Pulse)
                moving_parts[i] = Pulse(moving_parts[i]);
        }
    }

    Moving_Part Rotate_Random(Moving_Part part)
    {
        part.instance.transform.RotateAround(part.instance.transform.position, part.axis, part.speed * current_speed * Time.deltaTime);

        return part;
    }

    Moving_Part Rotate_Y(Moving_Part part)
    {
        part.instance.transform.RotateAround(part.instance.transform.position, Vector3.up, part.speed * current_speed * Time.deltaTime);

        return part;
    }

    Moving_Part Pulse(Moving_Part part)
    {
        // increase
        if (part.direction)
        {
            if (part.instance.transform.localScale.x < 1f + pulse_strength)
            {
                var add_size = new Vector3(part.speed * current_speed * Time.deltaTime, part.speed * current_speed * Time.deltaTime, part.speed * current_speed * Time.deltaTime);

                part.instance.transform.localScale += add_size;
            }
            else
                part.direction = false;
        }
        // decrease
        else
        {
            if (part.instance.transform.localScale.x > 1f - pulse_strength)
            {
                var add_size = new Vector3(part.speed * current_speed * Time.deltaTime, part.speed * current_speed * Time.deltaTime, part.speed * current_speed * Time.deltaTime);
                part.instance.transform.localScale -= add_size;
            }
            else
                part.direction = true;
        }
        return part;
    }

    // public interface
    public float Speed { set { current_speed = default_speed + charge_add_speed * Mathf.InverseLerp (0, 25, value); } }

    struct Moving_Part 
    {
        public GameObject instance;
        public Vector3 axis;
        public float speed;
        public bool direction;
    }
}
