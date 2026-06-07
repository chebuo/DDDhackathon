using System;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine.UIElements;

public class RankingController : MonoBehaviour
{
    string[] playerNames;
    string[] playerIds;
    float[] allScore;
    [SerializeField]GameSelectData gameSelectData;
    [SerializeField]UIDocument uiDocument;
    ListView rankingLabel;

    void Awake()
    {
        var panel = uiDocument.panelSettings;

        panel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        panel.match = 0.5f;
    }
    async void OnEnable()
    {
        var root=uiDocument.rootVisualElement;
        root.style.marginLeft=370;
        root.style.marginRight=30;
        root.style.marginTop=110;
        root.style.marginBottom=200;
        rankingLabel = root.Q<ListView>("ranking-list");
        rankingLabel.fixedItemHeight = 70;
        rankingLabel.makeItem = () =>
        {
            var label=new Label();
            label.style.unityTextAlign=TextAnchor.MiddleCenter;
            label.style.fontSize=40;
            label.style.height=StyleKeyword.Auto;
            return label;
        };
        
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
            var scores = await LeaderboardsService.Instance.GetScoresAsync(gameSelectData.games[SelectController.index].scoreName, new GetScoresOptions(){Limit=10});
            allScore = new float[scores.Results.Count];
            playerIds = new string[scores.Results.Count];
            playerNames = new string[scores.Results.Count];
            for (int i = 0; i < scores.Results.Count; i++)
            {
                allScore[i] = (float)scores.Results[i].Score;
                playerIds[i] = scores.Results[i].PlayerId;
                playerNames[i] = scores.Results[i].PlayerName.Split('#')[0];
                Debug.Log($"Score {i}: {scores.Results[i].Score}, PlayerId: {scores.Results[i].PlayerId}, PlayerName: {scores.Results[i].PlayerName}");
            }
            rankingLabel.bindItem = (element, index) =>
            {
                var data=$"{index+1}. {playerNames[index]} - {gameSelectData.GetScoreHMS(allScore[index])}";
                var label=element as Label;
                label.text = data;
            };
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        rankingLabel.itemsSource = playerNames;
        rankingLabel.Rebuild();
    }
}
