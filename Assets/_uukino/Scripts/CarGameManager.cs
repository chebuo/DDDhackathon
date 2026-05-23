using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CarGameManager : MonoBehaviour
{
    [Header("レーサー（車）")]
    public SlotCarController p1Car;
    public SlotCarController p2Car;

    [Header("選択した5つの寿司（デッキ）")]
    public CarData[] p1Deck = new CarData[5];
    public CarData[] p2Deck = new CarData[5];

    [Header("システムUI")]
    public Text systemMessageText; 

    // --- ★効果音 ---
    [Header("効果音")]
    public AudioSource audioSource;
    public AudioClip countdownSE; // 「ピッ」
    public AudioClip startSE;     // 「ポーン！」
    public AudioClip resultSE;    // 「ジャジャーン！」
    // --------------------

    private int currentRound = 0;
    private bool isRacing = false;

    private int p1FinishCount = 0;
    private int p1TotalPrice = 0;
    private float p1TotalTime = 0f;

    private int p2FinishCount = 0;
    private int p2TotalPrice = 0;
    private float p2TotalTime = 0f;

    public void StartGameWithDecks(CarData[] p1, CarData[] p2)
    {
        p1Deck = p1;
        p2Deck = p2;
        StartCoroutine(StartRound(0));
    }

    void Update()
    {
        if (!isRacing) return;
        bool p1Done = p1Car.IsFinished || p1Car.IsDerailed;
        bool p2Done = p2Car.IsFinished || p2Car.IsDerailed;
        if (p1Done && p2Done)
        {
            isRacing = false;
            StartCoroutine(RoundEndProcess());
        }
    }

    IEnumerator StartRound(int roundIndex)
    {
        currentRound = roundIndex;
        if (currentRound >= 5)
        {
            ShowFinalResult();
            yield break;
        }

        if (systemMessageText != null) systemMessageText.text = $"第 {currentRound + 1} 皿目！";
        p1Car.ResetCar(p1Deck[currentRound]);
        p2Car.ResetCar(p2Deck[currentRound]);

        yield return new WaitForSeconds(1.5f);

        // カウントダウン音
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
        if (audioSource && startSE) audioSource.PlayOneShot(startSE); // スタート音

        p1Car.CanDrive = true;
        p2Car.CanDrive = true;
        isRacing = true;

        yield return new WaitForSeconds(1f);
        if (systemMessageText != null) systemMessageText.text = ""; 
    }

    IEnumerator RoundEndProcess()
    {
        if (p1Car.IsFinished) { p1FinishCount++; p1TotalPrice += p1Deck[currentRound].price; p1TotalTime += p1Car.CurrentTime; }
        if (p2Car.IsFinished) { p2FinishCount++; p2TotalPrice += p2Deck[currentRound].price; p2TotalTime += p2Car.CurrentTime; }
        if (systemMessageText != null) systemMessageText.text = "決着！ 次の皿へ...";
        yield return new WaitForSeconds(2f);
        StartCoroutine(StartRound(currentRound + 1));
    }

    void ShowFinalResult()
    {
        // リザルト音
        if (audioSource && resultSE) audioSource.PlayOneShot(resultSE);

        string resultMsg = "【最終結果】\n";
        resultMsg += $"1P: 完走 {p1FinishCount}皿 / 計 {p1TotalPrice}円 / タイム {p1TotalTime:F2}秒\n";
        resultMsg += $"2P: 完走 {p2FinishCount}皿 / 計 {p2TotalPrice}円 / タイム {p2TotalTime:F2}秒\n\n";

        if (p1FinishCount > p2FinishCount) resultMsg += "プレイヤー1 の勝利！（完走数）";
        else if (p2FinishCount > p1FinishCount) resultMsg += "プレイヤー2 の勝利！（完走数）";
        else
        {
            if (p1TotalPrice > p2TotalPrice) resultMsg += "プレイヤー1 の勝利！（高額）";
            else if (p2TotalPrice > p1TotalPrice) resultMsg += "プレイヤー2 の勝利！（高額）";
            else
            {
                if (p1TotalTime < p2TotalTime) resultMsg += "プレイヤー1 の勝利！（最速）";
                else if (p2TotalTime < p1TotalTime) resultMsg += "プレイヤー2 の勝利！（最速）";
                else resultMsg += "完全引き分け！！！";
            }
        }
        if (systemMessageText != null) systemMessageText.text = resultMsg;
    }
}