using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectController : MonoBehaviour
{
    [SerializeField] GameSelectData gameSelectData;
    [SerializeField] Image gameIcon;
    [SerializeField] Text playerName;
    [SerializeField] Text titleText;
    [SerializeField] Text scoreText;
    [SerializeField] GameObject ranking;
    public static int index=0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameIcon.sprite = gameSelectData.games[index].gameIcon;
        titleText.text = gameSelectData.games[index].gameName;
        if(string.IsNullOrEmpty(gameSelectData.playerName))gameSelectData.playerName=PlayerPrefs.GetString("playerName", "Player");   
        playerName.text = gameSelectData.playerName;
        if(string.IsNullOrEmpty(gameSelectData.games[index].score)&&!string.IsNullOrEmpty(gameSelectData.games[index].scoreName))
        {
            gameSelectData.games[index].score = PlayerPrefs.GetFloat(gameSelectData.games[index].scoreName, 0f).ToString("F2");
        }
        if(!string.IsNullOrEmpty(gameSelectData.games[index].score))scoreText.text = gameSelectData.GetScoreHMS(float.Parse(gameSelectData.games[index].score));
        Debug.Log(gameSelectData.games[index].score);
    }
    void Update()
    {
        playerName.text = gameSelectData.playerName;
    }

    public void NextClick()
    {
        index++;
        if(index>=gameSelectData.games.Count)index=0;
        gameIcon.sprite = gameSelectData.games[index].gameIcon;
        titleText.text = gameSelectData.games[index].gameName;
        if(ranking.activeSelf){
            ranking.SetActive(!ranking.activeSelf);
            ranking.SetActive(!ranking.activeSelf);
        }
        if(!string.IsNullOrEmpty(gameSelectData.games[index].score))scoreText.text = gameSelectData.GetScoreHMS(float.Parse(gameSelectData.games[index].score));
    }

    public void BackClick()
    {
        index--;
        if(index<0)index=gameSelectData.games.Count-1;
        gameIcon.sprite = gameSelectData.games[index].gameIcon;
        titleText.text = gameSelectData.games[index].gameName;
        if(ranking.activeSelf){
            ranking.SetActive(!ranking.activeSelf);
            ranking.SetActive(!ranking.activeSelf);
        }
        if(!string.IsNullOrEmpty(gameSelectData.games[index].score))scoreText.text = gameSelectData.GetScoreHMS(float.Parse(gameSelectData.games[index].score));
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSelectData.games[index].gameScene);
    }

    public void MoveTitle()
    {
        SceneManager.LoadScene("DtoSushi");
    }
    public void MoveRanking()
    {
        ranking.SetActive(!ranking.activeSelf);
    }
}
