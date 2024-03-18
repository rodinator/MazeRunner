using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerPosition : MonoBehaviour
{ public GameObject playerObjekt;            // The prefab of the game object to spawn in the maze.
    public GameObject monsterObjekt;           // The prefab of the game object to spawn in the maze.
    private GameObject spawnedGameObject;
    private Vector2Int mazeDimensions;

    public void SpawnObjectInMaze(MazeCell[,] maze, Vector2Int dimensions)
    {
        mazeDimensions = dimensions;

        if (spawnedGameObject == null)
        {
            spawnedGameObject = Instantiate(playerObjekt);
        }

        SetGameObjectPosition(maze);
    }

    private void SetGameObjectPosition(MazeCell[,] maze)
    {
        int randomX = Random.Range(0, mazeDimensions.x);
        int randomY = Random.Range(0, mazeDimensions.y);
        Vector3 newPosition = new Vector3(randomX, 0.5f, randomY); // Adjust Y position as needed

        // Check if the random position is not a wall in the maze
        if (!maze[randomX, randomY].topwall && !maze[randomX, randomY].lopwall)
        {
            spawnedGameObject.transform.position = newPosition;
        }
        else
        {
            // If the random position is a wall, find a new position recursively
            SetGameObjectPosition(maze);
        }
    }
}