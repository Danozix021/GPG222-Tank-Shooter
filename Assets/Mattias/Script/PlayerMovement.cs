using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 200f;

    void Update()
    {
        // Only control YOUR player
        if (!IsOwner) return;

        float move = Input.GetAxis("Vertical");
        float rotate = -Input.GetAxis("Horizontal");

        transform.Translate(Vector2.up * move * speed * Time.deltaTime);
        transform.Rotate(Vector3.forward * rotate * rotationSpeed * Time.deltaTime);
    }
}