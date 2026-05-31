using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;

public class RankingController : MonoBehaviour
{
    public float score=0;
    bool isLoading=false;
    [SerializeField]GameSelectData gameSelectData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        score = PlayerPrefs.GetFloat("lookSushiTime", 0f);
        try
		{
			await UnityServices.InitializeAsync();
            Debug.Log("Unity Services Initialized");
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
        if(!AuthenticationService.Instance.IsSignedIn)await AuthenticationService.Instance.SignInAnonymouslyAsync();
        isLoading=true; 
        try
        {
            var playerScore = await LeaderboardsService.Instance.GetPlayerScoreAsync("lookSushiTime");
            Debug.Log($"Player score: {playerScore.Score}");
            gameSelectData.games[0].score = playerScore.Score.ToString();
            if(score<float.Parse(gameSelectData.games[0].score))score=float.Parse(gameSelectData.games[0].score);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    void Update()
    {
        score+=Time.deltaTime;
        score=Mathf.Round(score*100)/100;
    }

    // Update is called once per frame
    public async void SendScore()
    {
        if(!isLoading)
        {
            Debug.Log("Not loading, cannot send score");
            return;
        }
        PlayerPrefs.SetFloat("lookSushiTime", score);
        gameSelectData.games[0].score = score.ToString();
        await LeaderboardsService.Instance.AddPlayerScoreAsync("lookSushiTime",score);
    }
}
