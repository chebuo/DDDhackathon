using UnityEngine;

public class tama : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(1f * Time.deltaTime, 0.0f, 0.0f);
         if (transform.position.x > 15)
        {
             Destroy(gameObject);
        }
    }
}
