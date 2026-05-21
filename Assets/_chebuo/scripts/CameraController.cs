using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    InputAction move;
    [SerializeField]InputActionProperty vertical;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move=InputSystem.actions.FindAction("Move");
        Debug.Log(move);
        Debug.Log(vertical);
        move.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        var inputMove=move.ReadValue<Vector2>();
        var inputVertical=vertical.action.ReadValue<Vector3>();
        transform.position+=new Vector3(inputMove.x,0,inputMove.y)*Time.deltaTime*10;
        transform.position+=new Vector3(0,inputVertical.y,0)*Time.deltaTime*10;

    }
}
