using UnityEngine;

public class enemyspawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1f;
    float timer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnInterval){
            timer = 0f;
            GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            enemy.transform.position = new Vector3(12, Random.Range(-4f, 4f), 0);
        }
    }
}
