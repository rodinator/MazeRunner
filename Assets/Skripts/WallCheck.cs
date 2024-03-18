using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public LayerMask playerLayer; // Layer, auf den der Trigger reagiert
    public bool isColliding; // Flagge, die anzeigt, ob der Spieler gegen die Wand läuft

    void OnTriggerStay(Collider other)
    {
        // Prüft, ob das Objekt, das den Trigger berührt, der Spieler ist
        if (other.gameObject.layer == playerLayer)
        {
            isColliding = true;
            print("hit");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Wenn der Spieler den Trigger verlässt, wird die Flagge zurückgesetzt
        if (other.gameObject.layer == playerLayer)
        {
            isColliding = false;
        }
    }
}
