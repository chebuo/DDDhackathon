using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectController : MonoBehaviour
{
    [SerializeField] GameSelectData gameSelectData;
    [SerializeField] Image gameIcon;
    [SerializeField] Text titleText;
    int index=0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameIcon.sprite = gameSelectData.games[index].gameIcon;
        titleText.text = gameSelectData.games[index].gameName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextClick()
    {
        index++;
        if(index>=gameSelectData.games.Count)index=0;
        gameIcon.sprite = gameSelectData.games[index].gameIcon;
        titleText.text = gameSelectData.games[index].gameName;
    }

    public void BackClick()
    {
        index--;
        if(index<0)index=gameSelectData.games.Count-1;
        gameIcon.sprite = gameSelectData.games[index].gameIcon;
        titleText.text = gameSelectData.games[index].gameName;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSelectData.games[index].gameScene);
    }

    public void MoveTitle()
    {
        SceneManager.LoadScene("DtoSushi");
    }
}
