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
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        Vector2 moveValue = move.ReadValue<Vector2>();
        rb.linearVelocity = new Vector3(moveValue.x * 10f * Time.deltaTime, moveValue.y * 10f * Time.deltaTime, 0f);
            if (shot.triggered)
            {
                GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
                Destroy(bullet, 5f);
                Debug.Log("Shot!");
            }
    }

}
