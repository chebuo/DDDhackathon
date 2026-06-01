using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;

public class RankingController : MonoBehaviour
{
    string[] playerNames;
    string[] playerIds;
    float[] allScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void OnEnable()
    {
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
        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync("lookSushiTime", new GetScoresOptions(){Limit=10});
            allScore = new float[scores.Results.Count];
            playerIds = new string[scores.Results.Count];
            playerNames = new string[scores.Results.Count];
            for (int i = 0; i < scores.Results.Count; i++)
            {
                allScore[i] = (float)scores.Results[i].Score;
                playerIds[i] = scores.Results[i].PlayerId;
                playerNames[i] = scores.Results[i].PlayerName;
                Debug.Log($"Score {i}: {scores.Results[i].Score}, PlayerId: {scores.Results[i].PlayerId}, PlayerName: {scores.Results[i].PlayerName}");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
