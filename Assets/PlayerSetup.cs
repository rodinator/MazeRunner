using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Movement movement;
    public GameObject camera;

    // Wird von Photon aufgerufen, wenn das lokale Spielerobjekt erstellt wird
    public void InitializeLocalPlayer()
    {
            // Aktiviere die Steuerung nur für den lokalen Spieler
        if (photonView.IsMine)
        {
            movement.enabled = true;
            camera.SetActive(true);
        }
        else
        {
            movement.enabled = false;
            camera.SetActive(false);
        }
    }
}
