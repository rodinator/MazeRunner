using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    // Eine einfache Implementierung des A*-Algorithmus für die Pfadfindung
    public static List<Vector2Int> FindPath(MazeCell[,] maze, Vector2Int start, Vector2Int target)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        // Wenn der Start oder das Ziel außerhalb des Labyrinths liegt oder das Ziel blockiert ist, gib einen leeren Pfad zurück
        if (!IsInsideMaze(maze, start) || !IsInsideMaze(maze, target) || maze[target.x, target.y].topwall || maze[target.x, target.y].lopwall)
        {
            Debug.LogWarning("Start or target position is outside the maze or target position is blocked.");
            return path;
        }

        // Erstelle die offene Liste und die geschlossene Liste
        List<Vector2Int> openList = new List<Vector2Int>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        // Füge den Startknoten zur offenen Liste hinzu
        openList.Add(start);

        // Erstelle ein Dictionary, um die Vorgänger für jeden besuchten Knoten zu speichern
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // Erstelle ein Dictionary, um die Kosten für jeden besuchten Knoten zu speichern
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
        gScore[start] = 0;

        // Füge den Startknoten zur offenen Liste hinzu
        openList.Add(start);

        while (openList.Count > 0)
        {
            // Wähle den Knoten mit dem niedrigsten g-Wert aus der offenen Liste aus
            Vector2Int current = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (gScore.ContainsKey(openList[i]) && gScore[openList[i]] < gScore[current])
                {
                    current = openList[i];
                }
            }

            // Entferne den aktuellen Knoten aus der offenen Liste
            openList.Remove(current);

            // Wenn der aktuelle Knoten das Ziel ist, baue den Pfad und gib ihn zurück
            if (current == target)
            {
                path.Add(current);
                while (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Insert(0, current);
                }
                return path;
            }

            // Füge den aktuellen Knoten zur geschlossenen Liste hinzu
            closedSet.Add(current);

            // Untersuche die Nachbarknoten
            foreach (var neighbour in GetNeighbours(maze, current))
            {
                // Wenn der Nachbarknoten bereits in der geschlossenen Liste ist, überspringe ihn
                if (closedSet.Contains(neighbour))
                    continue;

                // Berechne die vorläufigen g-Werte für den Nachbarknoten
                float tentativeGScore = gScore[current] + 1; // Hier wird die Annahme getroffen, dass jeder Schritt den gleichen Kosten hat

                // Wenn der Nachbarknoten nicht in der offenen Liste ist, füge ihn hinzu
                if (!openList.Contains(neighbour))
                    openList.Add(neighbour);
                else if (tentativeGScore >= gScore[neighbour])
                    continue; // Dies ist kein besserer Weg

                // Dieser Weg ist der beste bisher. Speichere den Knoten
                cameFrom[neighbour] = current;
                gScore[neighbour] = tentativeGScore;
            }
        }

        // Wenn die offene Liste leer ist und das Ziel nicht erreicht wurde, gib einen leeren Pfad zurück
        Debug.LogWarning("Could not find a path to the target.");
        return path;
    }

    // Überprüft, ob eine Position innerhalb des Labyrinths liegt
    private static bool IsInsideMaze(MazeCell[,] maze, Vector2Int position)
    {
        return position.x >= 0 && position.x < maze.GetLength(0) && position.y >= 0 && position.y < maze.GetLength(1);
    }

    // Gibt eine Liste von gültigen Nachbarknoten für einen bestimmten Knoten zurück
    private static List<Vector2Int> GetNeighbours(MazeCell[,] maze, Vector2Int position)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        if (IsInsideMaze(maze, position + Vector2Int.up) && !maze[position.x, position.y].topwall)
            neighbours.Add(position + Vector2Int.up);

        if (IsInsideMaze(maze, position + Vector2Int.down) && !maze[position.x, position.y - 1].topwall)
            neighbours.Add(position + Vector2Int.down);

        if (IsInsideMaze(maze, position + Vector2Int.right) && !maze[position.x, position.y].lopwall)
            neighbours.Add(position + Vector2Int.right);

        if (IsInsideMaze(maze, position + Vector2Int.left) && !maze[position.x - 1, position.y].lopwall)
            neighbours.Add(position + Vector2Int.left);

        return neighbours;
    }
}
