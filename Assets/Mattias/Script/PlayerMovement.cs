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

    
    private bool hasSpeedBoost = false;
    private bool isBoostActive = false;
    private float originalSpeed;

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
        //Debug.Log("Player spawned -> " +
        //         "Object name: " + gameObject.name +
        //         " | OwnerClientId: " + OwnerClientId +
        //         " | LocalClientId: " + NetworkManager.Singleton.LocalClientId +
        //         " | IsOwner: " + IsOwner +
        //         " | IsServer: " + IsServer +
        //      
    }

    void Update()
    {
        if (!IsOwner) return;

        //Movement input
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        moveInput = new Vector2(x, y).normalized * speed;

        //Turret rotation
        if (turret != null && cam != null)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            Vector2 direction = mousePos - turret.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.rotation = Quaternion.Euler(0, 0, angle - 92f);
        }

        //Activate speed boost with Space
        if (hasSpeedBoost && Input.GetKeyDown(KeyCode.Space))
        {
            ActivateSpeedBoostServerRpc();
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        rb.MovePosition(rb.position + moveInput * Time.fixedDeltaTime);
    }

    //when picking up power-up
    [ClientRpc]
    public void GiveSpeedBoostClientRpc()
    {
        if (!IsOwner) return;

        hasSpeedBoost = true;
        Debug.Log("Picked up speed boost!");
    }

    //Tell server to activate boost
    [ServerRpc]
    private void ActivateSpeedBoostServerRpc()
    {
        ActivateSpeedBoostClientRpc();
    }

    //Activate boost on client
    [ClientRpc]
    private void ActivateSpeedBoostClientRpc()
    {
        if (!IsOwner || isBoostActive) return;

        StartCoroutine(SpeedBoostCoroutine());
    }

    //Speed boost logic
    private System.Collections.IEnumerator SpeedBoostCoroutine()
    {
        isBoostActive = true;
        hasSpeedBoost = false;

        originalSpeed = speed;
        speed = speed * 2f;

        Debug.Log("Speed boost activated!");

        yield return new WaitForSeconds(3f);

        speed = originalSpeed;
        isBoostActive = false;

        Debug.Log("Speed boost ended");
    }
}