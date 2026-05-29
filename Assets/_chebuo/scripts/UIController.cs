using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField]InputActionProperty esc;
    void Start()=>canvas.SetActive(false);
    void Update()
    {
        if(esc.action.WasPressedThisFrame())canvas.SetActive(true);
    }
   
    public void CloseGame(){
        SceneManager.LoadScene("MenuSelect");
    }
    public void ContinueGame(){
        canvas.SetActive(false);
    }
}
