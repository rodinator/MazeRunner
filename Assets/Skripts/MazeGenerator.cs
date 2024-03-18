using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MazeGenerator : MonoBehaviourPunCallbacks
{
    [Range(5, 500)]
    public int mazeWidth = 5, mazeHeight = 5;      // The dimesions of the maze
    public int startX, startY;                     // The position the algorithm will start from.
    MazeCell[,] Maze;                              // An array of maze cells representing the maze grid
    Vector2Int CurrentCell;                        // The maze cell we are currently looking at.



    //Spawn Things
    
    

    public GameObject playerObjectPrefab;

    public GameObject hostPlayerPrefab;
    public GameObject joiningPlayerPrefab;
    private GameObject playerObjekt;            // The prefab of the game object to spawn in the maze.


    

     public static MazeGenerator Instance { get; private set; }

    // Ihre anderen Variablen und Methoden hier...

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Damit das Objekt beim Szenenwechsel erhalten bleibt
        }
        else
        {
            Destroy(gameObject); // Zerstöre das Duplikat des Skripts
        }
    }

    
   


    public MazeCell[,] GetMaze()
    {
        Maze = new MazeCell[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
        {
            for(int y = 0; y < mazeHeight; y++)
            {
                Maze[x, y] = new MazeCell(x, y);
            }
        }

        if (playerObjekt == null)
        {
            SpawnPlayerObject();
        }

        SpawnGamePlayerInMaze();

        CarvePath(startX, startY);
        
        return Maze;
    }

    // Method to set the position of the spawned game object to a random position in the maze
       void SpawnPlayerObject()
    { if (playerObjekt == null)
        {
            // Überprüfen, ob der lokale Spieler der Host ist
            if (PhotonNetwork.IsMasterClient)
            {
                // Spieler ist der Host, verwenden Sie das Host-Prefab
                playerObjekt = PhotonNetwork.Instantiate(hostPlayerPrefab.name, Vector3.zero, Quaternion.identity);
            }
            else
            {
                // Spieler tritt als Gast bei, verwenden Sie das Gast-Prefab
                playerObjekt = PhotonNetwork.Instantiate(joiningPlayerPrefab.name, Vector3.zero, Quaternion.identity);
            }

            playerObjekt.GetComponent<PlayerSetup>().InitializeLocalPlayer();
        }
    }

    void SpawnGamePlayerInMaze()
    {
        int randomX = Random.Range(0, mazeWidth);
        int randomY = Random.Range(0, mazeHeight);
        playerObjekt.transform.position = new Vector3(randomX, 0.5f, randomY);
    }

     public void InstantiateLocalPlayer(GameObject playerPrefab)
    {
        if (playerObjekt == null && PhotonNetwork.IsConnectedAndReady)
        {
            playerObjekt = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
            playerObjekt.GetComponent<PlayerSetup>().InitializeLocalPlayer();
        }
    }
    
  




    

    List<Direction> directions = new List<Direction>
    {
        Direction.Up, Direction.Down, Direction.Left, Direction.Right
    };

    List<Direction> GetRandomDirections()
    {
        // Make a copy of our directions list that we can mess around with
        List<Direction> dir = new List<Direction>(directions);

        // Make a directions list to put our randomised directions into
        List<Direction> rndDir = new List<Direction>();

        while (dir.Count > 0)                                // Loop until your rndDir list is empty
        {
            int rnd = Random.Range(0, dir.Count);            // Get random index in list.
            rndDir.Add(dir[rnd]);                            // Add the random direction to our list
            dir.RemoveAt(rnd);                               // Remove that direction so we cant choose it again
        }

        // When we got all four directions in a random order, return the queue
        return rndDir;
    }

    bool IsCellValid (int x, int y)
    {
        // If the cell is outside of the map or has already been visited, we consider it not valid
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1 || Maze[x, y].visited) return false;
        else return true;
    }

    Vector2Int CheckNeighbours()
    {
        List<Direction> rndDir = GetRandomDirections();

        for (int i = 0; i < rndDir.Count; i++)
        {

            // Set neighbour coordinates to current cell for now
            Vector2Int neighbour = CurrentCell;

            // Modify neighbour coordinates based on the random directions were currently trying
            switch (rndDir[i])
            {
                case Direction.Up:
                    neighbour.y++;
                    break;
                case Direction.Down:
                    neighbour.y--;
                    break;
                case Direction.Right:
                    neighbour.x++;
                    break;
                case Direction.Left:
                    neighbour.x--;
                    break;
            }

            // if the neighbour we just tried is valid, we can return the neighbour. If not, we go again
            if (IsCellValid(neighbour.x, neighbour.y)) return neighbour;
        }

        // If we tried all directions and didnt find a valid neighbour, we return the CurrentCell values
        return CurrentCell;
    }

    void BreakWalls (Vector2Int PrimaryCell, Vector2Int SecondaryCell)
    {
        // We can only go in one direction at a time so we cant handle this using if else statements
        if (PrimaryCell.x > SecondaryCell.x) // Primary cell left wall
        {
            Maze[PrimaryCell.x, PrimaryCell.y].lopwall = false;

        } 
        else if (PrimaryCell.x < SecondaryCell.x) // Secondary cell top wall
        {
            Maze[SecondaryCell.x, SecondaryCell.y].lopwall = false;

        } 
        else if (PrimaryCell.y < SecondaryCell.y) // Primary cell Top Wall
        {
            Maze[PrimaryCell.x, PrimaryCell.y].topwall = false;

        } 
        else if (PrimaryCell.y > SecondaryCell.y) // Secondary cell top wall
        {
            Maze[SecondaryCell.x, SecondaryCell.y].topwall = false;
        }
    }

    // starting at the x, y passed in, carves a path through the maze until it encounter a "dead end"
    // (a dead end is a cell with no valid neighbours)
    void CarvePath (int x, int y)
    {
        // Perform a quiq check to make sure our start position in within the boundaries of the map
        // if not, set them to a default (Im using 0) and throw a little warning up
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1)
        {
            x = y = 0;
            Debug.LogWarning("Starting position is out of bounds, defaulting to 0, 0");
        }

        // Set current cell to the starting position we were passed
        CurrentCell = new Vector2Int(x, y);

        // A list to keep track of our current path
        List<Vector2Int> path = new List<Vector2Int>();

        // Loop until we encounter a dead end
        bool DeadEnd = false;
        while (!DeadEnd)
        {
            Vector2Int NextCell = CheckNeighbours();

            // If that cell has no valid neighbours, set dead end to true so we break up of the loop
            if (NextCell == CurrentCell)
            {
                // If that cell has no valid neighbour, set deadend to true so we break up the loop
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    CurrentCell = path[i];              // Set CurrentCell to the next step back along our path
                    path.RemoveAt(i);                   // Remove this step from the path
                    NextCell = CheckNeighbours();       // Check that cell to see if any other neighbours are valid

                    // if we find a valid neighbour, break out of the loop
                    if (NextCell != CurrentCell) break;
                }

                if (NextCell == CurrentCell)
                    DeadEnd = true;
            }  
            else
            {
                BreakWalls(CurrentCell, NextCell);                         // Set wall flags on these two cells
                Maze[CurrentCell.x, CurrentCell.y].visited = true;         // Set cell to visited before moving on
                CurrentCell = NextCell;                                    // Set the current cell to the valid neighbour we found
                path.Add(CurrentCell);                                     // Add this cell to our path
            }
        }
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}



public class MazeCell {

    public bool visited;
    public int x, y;

    public bool topwall;
    public bool lopwall;

    // Return x and y as a vector2Int for convenience sake.
    public Vector2Int position
    {
        get
        {
            return new Vector2Int(x, y);
        }
    }

    public MazeCell (int x, int y)
    {
        // the coordinates of this cell in the maze grid
        this.x = x;
        this.y = y;

        // Wether the algorithm has visited this cell or not - false to start
        visited = false;

        // All walls are present until the algorithm removes them
        topwall = lopwall = true;
    }

}
