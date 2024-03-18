using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRender : MonoBehaviour
{
   [SerializeField] MazeGenerator mazeGenerator;
   [SerializeField] GameObject MazeCellPrefab;


   public float Cellsize = 1f;

   private  void Start(){

        MazeCell[,] maze = mazeGenerator.GetMaze();


        for(int x= 0; x< mazeGenerator.mazeWidth; x++){
            for(int y = 0; y < mazeGenerator.mazeHeight; y++){
                GameObject newCell = Instantiate ( MazeCellPrefab, new Vector3((float)x * Cellsize, 0f, (float) y * Cellsize), Quaternion.identity, transform);

                MazeCellObj mazeCell = newCell.GetComponent<MazeCellObj>();

                bool top = maze[x,y].topwall;
                bool left = maze[x,y].lopwall;

                bool right = false;
                bool bottom = false;
                if(x==mazeGenerator.mazeWidth-1) right = true;
                if (y==0) bottom = true;

                mazeCell.Init(top,bottom,right,left);
            }
        }
   }
}
