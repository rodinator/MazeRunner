using UnityEngine;
using Photon.Pun;

public class Movement : MonoBehaviourPunCallbacks
{
    public float speed = 1f; 
    public float jumpForce = 5f; 
    public float turnSensitivity = 100f; 
    public float lookSensitivity = 100f; 

    private Rigidbody rb;
    private float verticalLookRotation;
    private Transform cameraTransform; // Referenz auf das Kamera-Transform

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Die Kamera suchen
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
           // Debug.Log("Not controlling this player.");
            return;
        }

      // Debug.Log("Controlling this player.");

        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical"); 

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;
        transform.Translate(movement);

        float mouseX = Input.GetAxis("Mouse X") * turnSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
        verticalLookRotation += mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -80f, 80f);
        transform.localEulerAngles = new Vector3(verticalLookRotation, transform.localEulerAngles.y, 0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        rb.AddForce(Vector3.down * Physics.gravity.y * Time.deltaTime);
    }
}