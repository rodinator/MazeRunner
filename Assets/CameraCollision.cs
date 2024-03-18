using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour

  {
    public float maxDistance = 10f;
    public LayerMask collisionMask;

    void Update()
    {
        RaycastHit hit;
        Vector3 direction = transform.forward;

        // Führe einen Raycast von der aktuellen Position der Kamera in Richtung ihrer Zielposition aus
        if (Physics.Raycast(transform.position, direction, out hit, maxDistance, collisionMask))
        {
            // Wenn der Raycast ein Hindernis trifft, passen Sie die Position der Kamera entsprechend an
            transform.position = hit.point - direction * 0.5f; // Positionieren Sie die Kamera auf halber Strecke zum Hindernis
        }
        else
        {
            // Wenn kein Hindernis getroffen wurde, bewegen Sie die Kamera zu ihrer Zielposition
            transform.position += direction * maxDistance;
        }
    }
}
