using UnityEngine;
using UnityEngine.EventSystems;
using  TMPro;
using UnityEngine.SceneManagement;
public class Result : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(UIManager.result)
        {
            text.text = "WIN!";
        }
        else
        {
            text.text = "lose";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene("MenuSelect");
    }
}
