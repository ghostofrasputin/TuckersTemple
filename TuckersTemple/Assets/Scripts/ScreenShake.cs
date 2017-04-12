using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour {
    Vector3 originalPosition;
    float shakeAmt = 0;
    float xChange = 0;
    float yChange = 0;
    float counter = 0;
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void startShaking(float speed){
        shakeAmt = speed ;
        originalPosition = this.transform.position;
        //xChange = xDifference;
        //yChange = yDifference;
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
        /*            if (xChange == 0)
                    {
                        changedPos.x += quakeAmt;
                    }
                    if (yChange == 0)
                    { 
                        changedPos.y += quakeAmt;
                    }*/
    }
    private void stopShaking(){
        CancelInvoke("shake");
        this.transform.position = originalPosition;
        xChange = 0;
        yChange = 0;
    }
}
