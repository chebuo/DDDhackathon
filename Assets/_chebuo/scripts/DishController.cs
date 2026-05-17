using UnityEngine;

public class DishController : MonoBehaviour
{
    float speed=1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.right*speed*Time.deltaTime);
    }
}
