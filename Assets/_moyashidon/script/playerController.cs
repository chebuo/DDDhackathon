using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (Keyboard.current.upArrowKey.isPressed && transform.position.y < 4.5f)
        {
            transform.Translate(0, 0.02f, 0);
        }
    }

}
