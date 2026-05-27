using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.SceneManagement;

public class OnlineGameManager : MonoBehaviourPunCallbacks
{
    [Header("選べる寿司の全リスト")]
    public CarData[] availableSushi; 

    [Header("UI")]
    public Text systemMessageText;
    public Text resultText;
    public GameObject returnToTitleButton;

    [Header("効果音")]
    public AudioSource audioSource;
    public AudioClip countdownSE;
    public AudioClip startSE;
    public AudioClip resultSE;

    private int readyPlayers = 0;
    private bool isRacing = false;
    private bool hasReportedGoal = false; 

    private int finishedPlayers = 0;
    private string finalResultString = "【 レース結果 】\n";

    void Start()
    {
        if (returnToTitleButton != null) returnToTitleButton.SetActive(false);
        if (resultText != null) resultText.text = "";
    }

    // 誰かが「準備完了」を押したときに呼ばれる
    public void OnPlayerReady()
    {
        photonView.RPC("RpcPlayerReady", RpcTarget.All);
    }

    [PunRPC]
    void RpcPlayerReady()
    {
        readyPlayers++;
        
        // 部屋の親（マスタークライアント）が全員揃ったか判定
        if (PhotonNetwork.IsMasterClient)
        {
            if (readyPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                photonView.RPC("RpcStartCountdown", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    IEnumerator RpcStartCountdown()
    {
        // 寿司選択メニューを非表示にする
        FindObjectOfType<OnlineMenuManager>()?.HideMenu();

        if (systemMessageText != null) systemMessageText.text = "3";
        if (audioSource && countdownSE) audioSource.PlayOneShot(countdownSE);
        yield return new WaitForSeconds(1f);
        
        if (systemMessageText != null) systemMessageText.text = "2";
        if (audioSource && countdownSE) audioSource.PlayOneShot(countdownSE);
        yield return new WaitForSeconds(1f);
        
        if (systemMessageText != null) systemMessageText.text = "1";
        if (audioSource && countdownSE) audioSource.PlayOneShot(countdownSE);
        yield return new WaitForSeconds(1f);
        
        if (systemMessageText != null) systemMessageText.text = "GO!!";
        if (audioSource && startSE) audioSource.PlayOneShot(startSE);

        // 自分自身の車の CanDrive を true にして発進させる
        NetworkCarController[] cars = FindObjectsOfType<NetworkCarController>();
        foreach(var c in cars) 
        {
            if (c.photonView.IsMine) c.CanDrive = true;
        }

        isRacing = true;
        yield return new WaitForSeconds(1f);
        if (systemMessageText != null) systemMessageText.text = ""; 
    }

    void Update()
    {
        if (!isRacing || hasReportedGoal) return;

        NetworkCarController myCar = GetMyCar();
        
        // 自分がゴール（または脱線）したら、サーバーに報告する
        if (myCar != null && (myCar.IsFinished || myCar.IsDerailed))
        {
            hasReportedGoal = true;
            string myName = "Player " + PhotonNetwork.LocalPlayer.ActorNumber;
            float finalTime = myCar.IsDerailed ? 999f : myCar.CurrentTime; // 脱線はペナルティタイム
            
            photonView.RPC("RpcPlayerGoal", RpcTarget.All, myName, finalTime, myCar.IsDerailed);
        }
    }

    NetworkCarController GetMyCar()
    {
        NetworkCarController[] cars = FindObjectsOfType<NetworkCarController>();
        foreach(var c in cars) {
            if (c.photonView.IsMine) return c;
        }
        return null;
    }

    [PunRPC]
    void RpcPlayerGoal(string playerName, float time, bool isDerailed)
    {
        finishedPlayers++;
        
        if (isDerailed) finalResultString += $"{finishedPlayers}位: {playerName} - 脱線リタイア\n";
        else finalResultString += $"{finishedPlayers}位: {playerName} - {time:F2}秒\n";

        // 全員がゴールしたらリザルト発表
        if (PhotonNetwork.IsMasterClient && finishedPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            photonView.RPC("RpcShowResult", RpcTarget.All, finalResultString);
        }
    }

    [PunRPC]
    void RpcShowResult(string result)
    {
        isRacing = false;
        if (audioSource && resultSE) audioSource.PlayOneShot(resultSE);
        
        if (systemMessageText != null) systemMessageText.text = "FINISH!!";
        if (resultText != null) resultText.text = result;
        if (returnToTitleButton != null) returnToTitleButton.SetActive(true);
    }

    public void ReturnToTitle()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("TitleScene"); // ご自身のタイトルシーン名に合わせる
    }
}