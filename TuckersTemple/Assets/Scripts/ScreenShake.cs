using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour {
    Vector3 originalPosition;
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void startShaking(float speed){
        originalPosition = this.transform.position;
        InvokeRepeating("shake",0,.07f);
        Invoke("stopShaking", .5f);
    }
   private void shake(){

            float quakeAmt = .015f ;
        if (this.transform.position == originalPosition)
        {
            this.transform.position = new Vector3( transform.position.x + quakeAmt, transform.position.y + quakeAmt, transform.position.z);
        } else
        {
            this.transform.position = new Vector3(transform.position.x - quakeAmt, transform.position.y - quakeAmt, transform.position.z);
        }
    }
    private void stopShaking(){
        CancelInvoke("shake");
        this.transform.position = originalPosition;
    }
}
