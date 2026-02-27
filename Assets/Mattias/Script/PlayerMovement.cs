using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Turret")]
    public Transform turret; // Assign in inspector

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (turret == null)
            Debug.LogWarning("Turret transform not assigned!");
    }

    void Update()
    {
        if (!IsOwner) return; // Only owner can give input

        // --- Movement Input ---
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        moveInput = new Vector2(x, y) * speed;

        // --- Turret Aim ---
        if (turret != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePos - turret.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        // Move the tank
        rb.MovePosition(rb.position + moveInput * Time.fixedDeltaTime);
    }
}