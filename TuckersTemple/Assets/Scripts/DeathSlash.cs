using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSlash : MonoBehaviour
{
    private float timer = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            Destroy(transform.gameObject);
        }
    }
}