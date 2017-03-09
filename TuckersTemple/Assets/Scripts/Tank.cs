using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Actor {

	private void enemyCollision(GameObject enemy)
    {
        print("Tank has killed.");
        enemy.GetComponent<Actor>().death = true;
    }


}
