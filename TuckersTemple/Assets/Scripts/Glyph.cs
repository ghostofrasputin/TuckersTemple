using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glyph : MonoBehaviour {

    private GameObject goal;
    private GameObject gm;
    private bool isPaired;

    public Vector2 direction;
    

    // Use this for initialization
    void Start () {
        goal = GameObject.FindWithTag("Goal");
        gm = GameObject.FindWithTag("GameController");
	}

    private void Update()
    {
        if (!goal)
        {
            goal = GameObject.FindWithTag("Goal");
        }
    }

    public bool getIsPaired()
    {
        return isPaired;
    }

    public void checkConnected()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, gm.GetComponent<GameMasterFSM>().tileSize, LayerMask.GetMask("Glyphs"));
        isPaired = ray.collider != null;
    }
}
