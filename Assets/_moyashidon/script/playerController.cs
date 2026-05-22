using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    InputAction move,shot;
    Rigidbody rb;
    void Start()
    {
        move = InputSystem.actions.FindAction("Move");
        move.Enable();

        shot = InputSystem.actions.FindAction("Shot");
        shot.Enable();

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector2 moveValue = move.ReadValue<Vector2>();
        rb.linearVelocity = new Vector3(moveValue.x * 5f, moveValue.y, 0f);
            if (shot.triggered)
            {
                GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
                Destroy(bullet, 5f);
                Debug.Log("Shot!");
            }
    }

}
