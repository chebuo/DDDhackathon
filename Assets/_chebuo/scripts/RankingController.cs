using System.Collections.Generic;
using System.Threading.Tasks;      
using Newtonsoft.Json;
using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;

public class RankingController : MonoBehaviour
{
    bool isLoading=false;
    public float score=0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
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
        isLoading=true;
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
        await LeaderboardsService.Instance.AddPlayerScoreAsync("lookSushiTime",score);
    }
}
