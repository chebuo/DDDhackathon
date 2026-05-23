using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndlessGameManager : MonoBehaviour
{
    [Header("レーサー（車）")]
    public SlotCarController playerCar;

    [Header("連携するマネージャー")]
    public EndlessShopManager shopManager; // ★追加：ショップへの参照

    [Header("エンドレス設定")]
    public int initialMoney = 1000;
    public float initialTime = 60f;
    public float timeBonusPerGoal = 5f;
    public float refundRate = 0.5f;

    [Header("UI関連")]
    public Text moneyText;
    public Text timerText;
    public Text scoreText;
    public Text systemMessageText;
    public GameObject shopPanel;
    public GameObject racePanel;

    [Header("効果音")]
    public AudioSource audioSource;
    public AudioClip countdownSE;
    public AudioClip startSE;
    public AudioClip goalBonusSE;
    public AudioClip gameOverSE;

    public int CurrentMoney { get; private set; }
    public float CurrentTime { get; private set; }
    public int TotalGoals { get; private set; }
    public int TotalEarned { get; private set; }

    private List<CarData> raceQueue = new List<CarData>();
    private CarData currentSushi;
    private bool isRacing = false;
    private bool isProcessingTransition = false;

    void Start()
    {
        CurrentMoney = initialMoney;
        CurrentTime = initialTime;
        TotalGoals = 0;
        TotalEarned = 0;

        UpdateSystemUI();
        GoToShopPhase();
    }

    void Update()
    {
        if (isRacing && !isProcessingTransition)
        {
            CurrentTime -= Time.deltaTime;
            UpdateSystemUI();

            if (CurrentTime <= 0)
            {
                CurrentTime = 0;
                UpdateSystemUI();
                StartCoroutine(GameOver("タイムアップ！！"));
                return;
            }

            if (playerCar.IsFinished) StartCoroutine(HandleGoal());
            else if (playerCar.IsDerailed) StartCoroutine(HandleDerail());
        }
    }

    public bool TrySpendMoney(int amount)
    {
        if (CurrentMoney >= amount)
        {
            CurrentMoney -= amount;
            UpdateSystemUI();
            return true;
        }
        return false;
    }

    public void StartRacePhase(List<CarData> purchasedSushi)
    {
        raceQueue = new List<CarData>(purchasedSushi);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (racePanel != null) racePanel.SetActive(true);
        StartCoroutine(RunNextSushi());
    }

    public void GoToShopPhase()
    {
        // ★修正：一番安いネタの価格を取得し、それすら買えなければゲームオーバー
        int minPrice = shopManager != null ? shopManager.GetCheapestPrice() : 0;
        if (CurrentMoney < minPrice)
        {
            StartCoroutine(GameOver("資金ショート！！\nこれ以上ネタを買えません..."));
            return;
        }

        if (shopPanel != null) shopPanel.SetActive(true);
        if (racePanel != null) racePanel.SetActive(false);
        if (systemMessageText != null) systemMessageText.text = "ネタを購入してください";

        // ★修正：ショップ画面を確実に初期化（リセット）させる
        if (shopManager != null) shopManager.InitializeShop();
    }

    IEnumerator RunNextSushi()
    {
        isProcessingTransition = true;
        isRacing = false;

        if (raceQueue.Count == 0)
        {
            if (systemMessageText != null) systemMessageText.text = "すべて走り切った！ 次の買い出しへ…";
            yield return new WaitForSeconds(1.5f);
            GoToShopPhase();
            isProcessingTransition = false;
            yield break;
        }

        currentSushi = raceQueue[0];
        raceQueue.RemoveAt(0);

        playerCar.ResetCar(currentSushi);

        if (systemMessageText != null) systemMessageText.text = $"出走：{currentSushi.sushiName}";
        yield return new WaitForSeconds(1f);

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

        playerCar.CanDrive = true;
        isRacing = true;
        isProcessingTransition = false;

        yield return new WaitForSeconds(1f);
        if (systemMessageText != null) systemMessageText.text = "";
    }

    IEnumerator HandleGoal()
    {
        isProcessingTransition = true;
        int refundAmount = Mathf.FloorToInt(currentSushi.price * refundRate);
        
        TotalGoals++;
        TotalEarned += refundAmount;
        CurrentMoney += refundAmount;
        CurrentTime += timeBonusPerGoal;

        UpdateSystemUI();
        if (audioSource && goalBonusSE) audioSource.PlayOneShot(goalBonusSE);

        if (systemMessageText != null) systemMessageText.text = $"完走！\n+{timeBonusPerGoal}秒 / +{refundAmount}円";

        yield return new WaitForSeconds(2f);
        StartCoroutine(RunNextSushi());
    }

    IEnumerator HandleDerail()
    {
        isProcessingTransition = true;
        if (systemMessageText != null) systemMessageText.text = "脱線！";
        yield return new WaitForSeconds(2f);
        StartCoroutine(RunNextSushi());
    }

    IEnumerator GameOver(string reason)
    {
        isProcessingTransition = true;
        isRacing = false;
        playerCar.CanDrive = false;

        if (audioSource && gameOverSE) audioSource.PlayOneShot(gameOverSE);
        if (systemMessageText != null) systemMessageText.text = $"{reason}\n\n【最終スコア】\n完走：{TotalGoals} 皿\n獲得賞金：{TotalEarned} 円";

        yield break; 
    }

    void UpdateSystemUI()
    {
        if (moneyText != null) moneyText.text = $"所持金: {CurrentMoney} 円";
        if (timerText != null) timerText.text = $"{CurrentTime:F1}";
        if (scoreText != null) scoreText.text = $"完走: {TotalGoals}皿";
    }
}