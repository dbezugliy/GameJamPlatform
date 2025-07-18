using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    private float fixedYPosition;
    public GameObject cam;

    public float parallaxEffect;

    void Start()
    {
        startpos = transform.position.x;
        fixedYPosition = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startpos + dist, fixedYPosition, transform.position.z);

        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}
