using System.Collections.Generic;
using UnityEngine;

public class MonsterLogic : MonoBehaviour
{
    public float speed = 5f;             // Geschwindigkeit des Monsters
    public Transform player;             // Referenz auf den Spieler

    private Rigidbody rb;                // Rigidbody-Komponente des Monsters
    private List<Vector2Int> path;       // Pfad zum Spieler
    private int currentPathIndex;        // Aktueller Index im Pfad

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        path = new List<Vector2Int>();
        currentPathIndex = 0;
    }

    void Update()
    {
        // Überprüfen, ob der Spieler existiert
        if (player != null)
        {
            // Wenn der Pfad leer ist oder das Monster das Ziel erreicht hat, generiere einen neuen Pfad
            if (path.Count == 0 || currentPathIndex >= path.Count)
            {
                GeneratePath();
                currentPathIndex = 0;
            }
            else
            {
                // Bewegung des Monsters entlang des Pfads
                Vector3 nextPosition = new Vector3(path[currentPathIndex].x, 0.5f, path[currentPathIndex].y);
                Vector3 direction = (nextPosition - transform.position).normalized;
                rb.MovePosition(transform.position + direction * speed * Time.deltaTime);

                // Überprüfen, ob das Monster das nächste Ziel erreicht hat
                if (Vector3.Distance(transform.position, nextPosition) < 0.1f)
                {
                    currentPathIndex++;
                }
            }
        }
    }

    // Generiere einen Pfad zum Spieler mithilfe des A* Algorithmus
    void GeneratePath()
    {
        MazeGenerator mazeGenerator = FindObjectOfType<MazeGenerator>();
        if (mazeGenerator != null)
        {
            MazeCell[,] maze = mazeGenerator.GetMaze();

            // Konvertiere die Positionen von Vector3 in Vector2Int
            Vector2Int startPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
            Vector2Int targetPosition = new Vector2Int(Mathf.RoundToInt(player.position.x), Mathf.RoundToInt(player.position.z));

            // Verwende den A* Algorithmus, um den Pfad zu finden
            path = AStar.FindPath(maze, startPosition, targetPosition);
        }
    }
}
