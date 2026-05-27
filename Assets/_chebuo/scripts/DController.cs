using UnityEngine;
using UnityEngine.SceneManagement;

public class DController : MonoBehaviour
{
    [SerializeField] GameObject neta;
    [SerializeField] GameObject sushi;
    [SerializeField] MeshRenderer[] meshRenderers;
    bool isFall=false;
    bool isDSushi=false;
    bool isFinish=false;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb=this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.eulerAngles.z <= 270&&transform.eulerAngles.z >= 90) 
        {
            rb.linearVelocity=Vector3.zero;
            isFall=true;
        }
        if (isFall&&neta.transform.localScale.y<1.112f)
        {
            neta.transform.localScale+=new Vector3(0,0.2f,0)*Time.deltaTime;
            if(neta.transform.localScale.y>=1.112f) isDSushi=true;
        }
        if (isDSushi)
        {
            rb.linearVelocity=Vector3.zero;
            if(sushi.transform.localScale.x<0.5f)sushi.transform.localScale+=new Vector3(0.2f,0,0)*Time.deltaTime;
            if(sushi.transform.localScale.y<0.7f)sushi.transform.localScale+=new Vector3(0,0.3f,0)*Time.deltaTime;
            if(sushi.transform.localScale.z<0.5f)sushi.transform.localScale+=new Vector3(0,0,0.2f)*Time.deltaTime;
            if(sushi.transform.localScale.x>=0.5f&&sushi.transform.localScale.y>=0.7f&&sushi.transform.localScale.z>=0.5f) isFinish=true;
        }
        if(isFinish)
        {
            for(int i=0;i<meshRenderers.Length;i++)
            {
                meshRenderers[i].enabled=false;
            }
        }
        if(isFinish){
            Invoke("StartGameSelect",1f);
            isFinish=false;
            }
    }

    void StartGameSelect()
    {
        SceneManager.LoadScene("MenuSelect");
    }
}
