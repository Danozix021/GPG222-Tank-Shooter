using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!IsOwner) return; // Only owner can give input

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        moveInput = new Vector2(x, y) * speed;
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        rb.MovePosition(rb.position + moveInput * Time.fixedDeltaTime);
    }
}