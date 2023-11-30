
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Flapping : UdonSharpBehaviour
{
    bool moveUp = false;

    [SerializeField]
    float speed = 1.0f;

    float starty;

    [SerializeField]
    int flapAmount = 5;
    int currentflap;
    private void Start()
    {
        starty = transform.localPosition.y;
    }

    void Update()
    {
        if (currentflap > flapAmount) return;
        if (moveUp)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + speed * Time.deltaTime, transform.localPosition.z);
            if (transform.localPosition.y > starty + 0.1f) moveUp = false;
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - speed * Time.deltaTime, transform.localPosition.z);
            if (transform.localPosition.y < starty - 0.1f)
            {
                currentflap++;
                moveUp = true;
            }
        }
    }
}
