using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalLight : MonoBehaviour
{

    float promptTimer = 0f;
    GameObject gLight;
    bool flashlight = false;

    // Use this for initialization
    void Start()
    {
        gLight = transform.Find("light").gameObject;
        Color temp = gLight.GetComponent<SpriteRenderer>().color;
        temp.a = 0;
        gLight.GetComponent<SpriteRenderer>().color = temp;
    }

    // Update is called once per frame
    void Update()
    {
        //blink prompt
        if (flashlight)
        {
            promptTimer += Time.deltaTime * 2;
            Color temp = gLight.GetComponent<SpriteRenderer>().color;
            temp.a = Mathf.Sin(promptTimer);
            gLight.GetComponent<SpriteRenderer>().color = temp;

            if (Mathf.Sin(promptTimer) < 0)
            {
                flashlight = false;
            }
        }
    }

    public void FlashLight()
    {
        flashlight = true;
        promptTimer = 0f;
    }
}