using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class OnlineMenuManager : MonoBehaviour
{
    [Header("連携設定")]
    public GameObject menuPanel;
    public OnlineGameManager gameManager;
    
    [Header("UI")]
    public Text statusText;
    public Button readyButton;

    private bool isReady = false;

    void Start()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (statusText != null) statusText.text = "好きな寿司を選んでください";
    }

    // 各寿司ボタンから呼ばれる
    public void OnSushiSelected(int index)
    {
        if (isReady) return; // 準備完了後は変更不可
        
        // 自分の車を探して、見た目変更のRPCを送信する
        NetworkCarController[] cars = FindObjectsOfType<NetworkCarController>();
        foreach(var c in cars) 
        {
            if (c.photonView.IsMine)
            {
                // 自分だけでなく、同じ部屋にいる他人の画面でも見た目が変わる
                c.photonView.RPC("RpcSetCarData", RpcTarget.AllBuffered, index);
                break;
            }
        }
    }

    // 「準備完了」ボタンから呼ばれる
    public void OnReadyClicked()
    {
        isReady = true;
        if (readyButton != null) readyButton.interactable = false;
        if (statusText != null) statusText.text = "他のプレイヤーを待機中...";
        
        if (gameManager != null) gameManager.OnPlayerReady();
    }

    public void HideMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
    }
}