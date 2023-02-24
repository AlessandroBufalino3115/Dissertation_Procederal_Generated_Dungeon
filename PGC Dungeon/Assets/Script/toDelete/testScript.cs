using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
   

    /*
     * 
     *   public List<Vector2> Sample(Vector2 topRight, Vector2 topLeft, Vector2 botRight, Vector2 botLeft, int triesAfterFailure, float radius,int gridSize, bool maxPoints = false, int maxPointsAllowed = 100)
    {
        var spawnedPoints = new List<Vector2>();


        //return samples;
        _gridSize = gridSize;

        // Calculate the width and height of the grid cells
        float cellWidth = (topRight.x - topLeft.x) / _gridSize;
        float cellHeight = (topLeft.y - botLeft.y) / _gridSize;


        // Create a 2D array to store things in each grid cell
        grid = new PoissantTile[_gridSize, _gridSize];

        // Iterate over each row and column in the grid
        for (int row = 0; row < _gridSize; row++)
        {
            for (int col = 0; col < _gridSize; col++)
            {
                // Calculate the position of the current cell relative to the square's corner points
                float x = topLeft.x + (col * cellWidth) + (cellWidth / 2);
                float y = botLeft.y + (row * cellHeight) + (cellHeight / 2);

                PoissantTile cell = new PoissantTile() {coord = new Vector2(x, y) , index = new Vector2(col,row)};
                grid[row, col] = cell;
            }
        }





        int tries = triesAfterFailure;

        while (tries > 0)
        {
            var newPossiblePosIndex = new Vector2Int(Random.Range(0,_gridSize), Random.Range(0, _gridSize));


            if (grid[newPossiblePosIndex.x, newPossiblePosIndex.y].coordObjInside != new Vector2(-1, -1))
                continue;

            float quadrantWidth = cellWidth / 2;
            float quadrantHeight = cellHeight / 2;
            float quadrantX = grid[newPossiblePosIndex.x, newPossiblePosIndex.y].coord.x - quadrantWidth / 2;
            float quadrantY = grid[newPossiblePosIndex.x, newPossiblePosIndex.y].coord.y - quadrantHeight / 2;
            Vector3 objectPos = new Vector3(
                Random.Range(quadrantX, quadrantX + quadrantWidth),
                Random.Range(quadrantY, quadrantY + quadrantHeight),
                0f);

            for (int i = 0; i < spawnedPoints.Count; i++)
            {
                if (IsInsideRadiusOfOther(newPossiblePosIndex, objectPos, radius, grid))
                {
                    tries--;
                }
                else
                {
                    tries = triesAfterFailure;
                    grid[newPossiblePosIndex.x, newPossiblePosIndex.y].coordObjInside = objectPos;
                    spawnedPoints.Add(objectPos);
                }
            }


            if (maxPoints && spawnedPoints.Count >= maxPointsAllowed)
                break;

        }


        return spawnedPoints;

    }

    /// <summary>
    /// return true if it is
    /// </summary>
    /// <param name="candidate"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    private bool IsInsideRadiusOfOther(Vector2Int desiredPosition, Vector3 possiblePos , float radius, PoissantTile[,] grid)
    {
       
        if (desiredPosition.x - 1 >= 0) //left
        {
        
        }

        if (desiredPosition.x + 1 >= 0) //right
        {

        }

        if (desiredPosition.x - 1 >= 0) //top
        {

        }

        if (desiredPosition.x - 1 >= 0) //bot
        {

        }

        if (desiredPosition.x - 1 >= 0) //topright
        {

        }

        if (desiredPosition.x - 1 >= 0) //topLeft
        {

        }

        if (desiredPosition.x - 1 >= 0) //botRigth
        {

        }

        if (desiredPosition.x - 1 >= 0) //botLeft
        {

        }

    }

     * 
     */
}
