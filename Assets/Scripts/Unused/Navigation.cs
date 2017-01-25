/*
 * Tucker's Temple
 * Andrew Cousins
 * Last edit: 1/18/17
 * 
 * Navigation.cs
 * contains functions to determine what an actor's next move should be
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAlg : MonoBehaviour {

	//macros for directional variables
	const int N = 0; 
	const int E = 1;
	const int S = 2;
	const int W = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//row,col,dir of actor to find next move for
	//returns 0,1,2,3 for which direction they should move
	//returns -1 if no valid move found
	public int findNextMove(int row, int col, int dir){

		//order to try in is straight->right->left->back

		//this is the modifies to the directions something can face
		int[] dirMods = {0, 1, -1, 2};
		//directions are 0,1,2,3, with 0 being up and going clockwise.
		for (int i = 0; i < 4; i++) {
			//make a current direction by adding the direction modifier to the direction
			int currDir = dir + dirMods[i]; 

			//Normalize currDir within 0 to 3
			if (currDir > 3) {
				currDir -= 4;
			}
			else if (currDir < 0) {
				currDir += 4;
			}

			//calls a function calld validMove, which checks if the attempted move has walls in the way or is off the map.
			if (validMove(col, row, currDir)) { //valideMove returns a boolean
				//return the first valid move found
				return currDir;
			}
		}
		//if no moves were found
		return -1;
	}

	//takes in col, row, and direction
	private bool validMove(int col, int row, int dir) {
		
		//isWall gets the int(0 or 1) for a wall in the direction the actor is facing
		//at its current tile

		//TODO: set isWall to the proper getter
		int isWall = 0;
		//isWall = grid[c][r].dirs[d]; <-- old code from prototype

		if (isWall == 0) { //0 means no wall
			
			//now check if the actor is trying to move off the grid
			switch (dir) {

				//N,E,S,W are global variables for 0,1,2,3
				//the row/col values are modified so we can easily use their new value later
				case N:
					row--;
					if (row < 0) return false;
					break;
				case E:
					col++;
					//numCols is a global variable for how many cols there are
					if (col >= 3) { //TODO: set to check vs numCols
						return false;
					}
					break;
				case S:
					row++;
					//numRows is a global variable for how many rows there are
					if (row >= 3) { //TODO: set to check vs numRows
						return false;
					}
					break;
				case W:
					col--;
					if (col < 0) return false;
					break;
				}
			//check if next box has a path pointing at you, by fliping the path
			//row/col are already changed appropriatly above
			dir += 2;

			//normalize dir
			if (dir >= 4) dir -= 4; 

			//now row, col, and dir have been updated to be as if an actor is trying to
			//move from destination tile to origin tile, to check if there is a wall in that tile
			//isWall = grid[c][r].dirs[d]; TODO: add in proper code here, this is legacy code

			//0 means no wall, so we have a valid move.
			if (isWall == 0) {
				return true;
			}
			return false;
		}
		return false;
	}
}
