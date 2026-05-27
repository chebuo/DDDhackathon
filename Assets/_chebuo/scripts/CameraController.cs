using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    InputAction move;
    [SerializeField]InputActionProperty vertical;
    [SerializeField]InputActionProperty angle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move=InputSystem.actions.FindAction("Move");
        move.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        var inputMove=move.ReadValue<Vector2>();
        var inputVertical=vertical.action.ReadValue<Vector3>();
        var inputAngle=angle.action.ReadValue<Vector3>();
        Vector3 moveDir = transform.forward * inputMove.y + transform.right * inputMove.x;
        transform.position += moveDir * Time.deltaTime * 10;
        transform.position+=new Vector3(0,inputVertical.y,0)*Time.deltaTime*10;
        transform.Rotate(new Vector3(inputAngle.x, inputAngle.y,0) * Time.deltaTime * 20);
        Vector3 rot=transform.eulerAngles;
        rot.z=0;
        transform.eulerAngles=rot;
    }
}
