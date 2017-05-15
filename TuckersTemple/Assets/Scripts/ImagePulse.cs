using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePulse : MonoBehaviour {

    private Image img;
    private float promptTimer;

	// Use this for initialization
	void Start () {
        img = this.GetComponent<Image>();
        promptTimer = 0;
    }
	
	// Update is called once per frame
	void Update () {
        promptTimer += Time.deltaTime * 2;
        float scale = (1 + Mathf.Sin(promptTimer)) / 2;
        scale = (scale * 0.2f) + 0.8f;
        Color c = img.color;
        c.a = scale;
        img.color = c;
        //if you uncomment this line, make sure to play wii shop music
        //this.transform.Rotate(new Vector3(0, 0, 1));
        
        this.transform.localScale = new Vector2(scale, scale);
    }
}
