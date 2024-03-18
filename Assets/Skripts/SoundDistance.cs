using UnityEngine;

public class SoundDistance : MonoBehaviour
{
    private AudioSource audioSource; // Referenz auf die AudioSource-Komponente
    public float maxDistance = 5f; // Maximale Distanz, in der der Sound hörbar ist
    public float minVolume = 0.1f; // Minimale Lautstärke
    public float maxVolume = 2f; // Maximale Lautstärke
    public AudioClip audioClip; // Referenz auf den Audioclip

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Wenn ein Audioclip per Drag-and-Drop hinzugefügt wurde, spiele ihn ab
        if (audioClip == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    void Update()
    {
        // Suche alle Objekte im Spiel
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        // Initialisiere die Entfernung als maximale Entfernung
        float closestDistance = maxDistance;
        GameObject closestObject = null;

        // Iteriere durch alle gefundenen Objekte
        foreach (GameObject obj in allObjects)
        {
            if (obj != gameObject) // Überspringe das aktuelle Objekt (das Sound-Objekt selbst)
            {
                // Berechne die Entfernung zwischen dem Objekt und dem Sound-Objekt
                float distance = Vector3.Distance(transform.position, obj.transform.position);

                // Überprüfe, ob das Objekt näher ist als das bisher am nächsten gelegene Objekt
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                }
            }
        }

        if (closestObject != null)
        {
            // Berechne die skalierte Lautstärke basierend auf der Entfernung zum am nächsten gelegenen Objekt
            float scaledVolume = Mathf.Lerp(minVolume, maxVolume, 1 - (closestDistance / maxDistance));

            // Wende die skalierte Lautstärke an
            audioSource.volume = scaledVolume;
        }
    }
}
