using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

    private GameObject teleporterTarget;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!teleporterTarget)
        {
            teleporterTarget = GameObject.FindWithTag("TeleporterTarget");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("COLLIDIEASDASDASDF");
        if ((other.CompareTag("Player") || other.CompareTag("Enemy")) && other.gameObject.GetComponent<ActorFSM>().fsm.CurrentStateID == StateID.WalkA)
        {
            other.transform.position = teleporterTarget.transform.position;
            other.transform.parent = teleporterTarget.transform.parent;
            other.gameObject.GetComponent<ActorFSM>().goalPos = other.transform.position;
        }
    }
}
