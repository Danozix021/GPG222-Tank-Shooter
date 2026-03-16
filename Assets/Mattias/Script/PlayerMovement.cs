using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Turret")]
    public Transform turret;

    private Camera cam;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            cam = Camera.main;
        }

        if (turret == null)
        {
            Debug.LogWarning("Turret transform not assigned!");
        }

        Debug.Log("Player spawned -> " +
                 "Object name: " + gameObject.name +
                 " | OwnerClientId: " + OwnerClientId +
                 " | LocalClientId: " + NetworkManager.Singleton.LocalClientId +
                 " | IsOwner: " + IsOwner +
                 " | IsServer: " + IsServer +
                 " | IsClient: " + IsClient);
    }

    void Update()
    {
        if (!IsOwner) return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        moveInput = new Vector2(x, y).normalized * speed;

        if (turret != null && cam != null)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            Vector2 direction = mousePos - turret.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        rb.MovePosition(rb.position + moveInput * Time.fixedDeltaTime);
    }
}