using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class John : Actor {
    public override int findNextMove(int dir)
    {

        //order to try in is straight->right->left->back

        //this is the modifies to the directions something can face
        int[] dirMods = { 0, 2, 1, -1 };
        //directions are 0,1,2,3, with 0 being up and going clockwise.
        for (int i = 0; i < 4; i++)
        {
            //make a current direction by adding the direction modifier to the direction
            int currDir = dir + dirMods[i];

            //Normalize currDir within 0 to 3
            if (currDir > 3)
            {
                currDir -= 4;
            }
            else if (currDir < 0)
            {
                currDir += 4;
            }


            //RAYCAST LASER BEAMS ♫♫♫♫♫
            //Actual comment: Raycasts are complicated.  Here it is then I'll explain
            RaycastHit2D[] raycasts = Physics2D.RaycastAll(transform.position, v2Dirs[currDir], gm.tileSize, LayerMask.GetMask("Wall"));
            //RaycastHit2D raycastEnemy = Physics2D.Raycast(transform.position, v2Dirs[currDir], gm.tileSize, LayerMask.GetMask("Enemy"));
            /*
             * RaycastHit2D is a return type of raycasts, that holds something
             * If it holds nothing, then there was nothing! woooo
             * Arg1 - position, where we start the laser beam
             * Arg2 - direction, from Vector2.up, down etc
             * Arg3 - distance, we don't want further than tile size
             * Arg4 - The layer mask, currently walls so thats all we shoot at
             */

            // using RAYCAAAAST! for enemy collision
            //if (raycastEnemy
            //if raycast doesn't return anything, then there isn't anything there!
            foreach (RaycastHit2D ray in raycasts)
            {
                // print(currDir + " " + ray.collider);
                if (ray.collider.tag == "Wall" || ray.collider.tag == "OuterWall")
                {
                    foundWall = true;
                    //print("I found a wall");
                    break;
                }
            }
            if (!foundWall)
            {
                foreach (RaycastHit2D ray in raycasts)
                {
                    //print(ray.collider.tag);
                    if (ray.collider.tag == "Enemy")
                    {
                        print("Enemy");
                        int EnemyView = ray.collider.gameObject.GetComponent<Actor>().direction;
                        switch (EnemyView)
                        {
                            case 0:
                                {
                                    if (direction == 2)
                                    {
                                        death = true;
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (direction == 3)
                                    {
                                        death = true;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (direction == 0)
                                    {
                                        death = true;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (direction == 1)
                                    {
                                        death = true;
                                    }
                                    break;
                                }
                        }
                    }
                    else if (ray.collider.tag == "Goal")
                    {
                        //Set a win varible to true
                        escaped = true;
                        //print("Escape!");
                        if (this.GetType().Name == "John")
                        {
                            gm.levelWin();
                        }
                    }
                    else if (ray.collider.tag == "Trap")
                    {
                        //print("You activated my Trap card");
                        death = true;
                    }

                }
                return currDir;
            }
            foundWall = false;
        }
        //if no moves were found
        return -1;
    }
}
