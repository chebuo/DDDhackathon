using UnityEngine;

public class enemy : MonoBehaviour
{
    public float speed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        transform.Translate(-speed * Time.deltaTime, 0, 0);
            if (transform.position.x < -15)
            {
                Destroy(gameObject);
            }
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
