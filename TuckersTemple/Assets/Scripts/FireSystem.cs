using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystem : MonoBehaviour
{
	public AudioClip flamesActive;
    //private float timeDelay = 1;
    //private float onTime = 0;
    private float idleTime = 0;

    private float startScale = 0.1f;

    private bool isOn = false;

    private bool scaleFlag;
    private float scaleFactor;
    private bool[] scaleFlagI;
    private float[] scaleFactorI;

    private GameObject flame;
    private GameObject[] idleFlames;

    // Use this for initialization
    void Start()
    {

        flame = transform.Find("Flame").gameObject;
        flame.SetActive(false);
        idleFlames = new GameObject[6];
        idleFlames[0] = transform.Find("IdleFlame0").gameObject;
        idleFlames[1] = transform.Find("IdleFlame1").gameObject;
        idleFlames[2] = transform.Find("IdleFlame2").gameObject;
        idleFlames[3] = transform.Find("IdleFlame3").gameObject;
        idleFlames[4] = transform.Find("IdleFlame4").gameObject;
        idleFlames[5] = transform.Find("IdleFlame5").gameObject;

        idleSetOff();

        scaleFlag = false;
        scaleFactor = flame.transform.localScale.x;
        scaleFlagI = new bool[6];
        scaleFactorI = new float[6];
        for (int i = 0; i < idleFlames.Length; i++)
        {
            scaleFlagI[i] = false;
            scaleFactorI[i] = startScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            flameOut(0.03f, 0f, .1f, 1.2f);
            for (int i = 0; i < idleFlames.Length; i++)
            {
                idleFlames[i].SetActive(true);
                idleFlameOut(i, 0.01f, 0f, 0.05f, 0.3f);
            }
        }
        else
        {
            idleTime += Time.deltaTime;
            idleSetOff();
            /*
            if (idleTime > 1)
            {
                idleTime = 0;
                randFlame = Random.Range(0, 10);
                if (randFlame < idleFlames.Length)
                {
                    scaleFlagI[randFlame] = true;
                    scaleFactorI[randFlame] = startScale;
                }
            }
            if (randFlame < idleFlames.Length)
            {
                idleFlames[randFlame].SetActive(true);
                idleFlameOut(randFlame, 0.01f, 0f, 0.05f, 0.3f);
            }*/
            if (idleTime > 1.5)
            {
                for (int i = 0; i < idleFlames.Length; i++)
                {
                    scaleFlagI[i] = true;
                    scaleFactorI[i] = startScale;
                }
                idleTime = 0;
            }
            for (int i = 0; i < idleFlames.Length; i++)
            {
                idleFlames[i].SetActive(true);
                idleFlameOut(i, 0.005f, 0f, 0.05f, 0.25f);
            }
        }
    }

    public void setOn()
    {
        isOn = true;
        flame.SetActive(true);
        scaleFlag = true;
        flame.transform.localScale = new Vector3(0.1f, 0.1f, 0.0f);
        for (int i = 0; i < idleFlames.Length; i++)
        {
            scaleFlagI[i] = true;
            scaleFactorI[i] = startScale;
        }
    }

    public void setOff()
    {
        isOn = false;
        flame.SetActive(false);
        idleTime = 0;
    }

    public void idleSetOff()
    {
        foreach (GameObject f in idleFlames)
        {
            f.SetActive(false);
        }
    }

    public void flameOut(float scaleSpeed, float angle, float lowerLimit = .6f, float upperLimit = 0.7f)
    {
        if (scaleFlag)
        {
            scaleFactor += scaleSpeed;
        }
        else
        {
            scaleFactor -= scaleSpeed / 2;
        }

        if (scaleFactor <= lowerLimit)
        {
            setOff();
        }
        if (scaleFactor >= upperLimit)
        {
            scaleFactor = upperLimit;
            scaleFlag = false;
        }
        flame.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    public void idleFlameOut(int i, float scaleSpeed, float angle, float lowerLimit = .6f, float upperLimit = 0.7f)
    {
        if (scaleFlagI[i])
        {
            scaleFactorI[i] += scaleSpeed;
        }
        else
        {
            scaleFactorI[i] -= scaleSpeed / 2;
        }

        if (scaleFactorI[i] <= lowerLimit)
        {
            scaleFactorI[i] = lowerLimit;
        }
        if (scaleFactorI[i] >= upperLimit)
        {
            scaleFactorI[i] = upperLimit;
            scaleFlagI[i] = false;
        }
        idleFlames[i].transform.localScale = new Vector3(scaleFactorI[i], scaleFactorI[i], 1f);
    }
}