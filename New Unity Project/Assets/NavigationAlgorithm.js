#pragma strict

//macros for directional variables
var N = 0; 
var E = 1;
var S = 2;
var W = 3;

function Start () {

}

function Update () {

}

//For this script, actors have row, col, and dir(Direction they are facing)
function findNextMove(actor){
	//get the actors info
	var row = actor.row;
	var col = actor.col;
	var dir = actor.dir;
	//Try Straight, right, left, then back
  	var dirMods = [0, -1, 1, 2]; //this is the modifies to the directions something can face
  	//directions are 0,1,2,3, with 0 being up and going clockwise.
  	for (var i = 0; i < 4; i++) {
    	var currDir = dir + dirMods[i]; //make a current direction by adding the direction modifier to the direction
    	//these are two checks to see if it has looped around, for example a direction of 4 is the same as 0.
    	if (currDir > 3) currDir -= 4;
    	if (currDir < 0) currDir += 4;
    	//calls a function calld validMove, which checks if the attempted move has walls in the way or is off the map.
    	if (validMove(col, row, currDir)) { //valideMove returns a boolean
      		switch (currDir) {
        		case N: //north
          			row--;
          			break;
        		case E: //east
          			col++;
          			break;
        		case S: //south
          			row++;
          			break;
        		case W: //west
          			col--;
          			break;
      		}
      		//update the actor's info
      		actor.col = col;
      		actor.row = row;
      		actor.dir = currDir;
      		break;
    	}
  	}
}

//takes in col, row, and direction
function validMove(c, r, d) {
	//isWall gets the boolean for a wall in the direction the actor is facing
	//at its current tile
 	var isWall = grid[c][r].dirs[d];
  	if (isWall === 0) { //0 means no wall
  		//now we check if the actor is trying to move off the grid
    	switch (d) {
    		//N,E,S,W are global variables for 0,1,2,3
      		case N:
        		r--;
        		if (r < 0) return false;
        		break;
      		case E:
        		c++;
        		//numCols is a global variable for how many cols there are
        		if (c >= numCols) return false;
        		break;
      		case S:
        		r++;
        		//numRows is a global variable for how many rows there are
        		if (r >= numRows) return false;
        		break;
     		case W:
        		c--;
        		if (c < 0) return false;
        		break;
    	}
    	//check if next box has a path pointing at you
    	d += 2;
    	if (d >= 4) d -= 4; //normalize dir
    	isWall = grid[c][r].dirs[d]; //row, col, and dir have been updated to be as if an actor is trying to
    	//move from destination tile to origin tile, to check if there is a wall in that tile
    	if (isWall === 0) return true; //0 means no wall, so we have a valid move.
    	return false;
  	}
  	return false;
}