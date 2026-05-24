using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndlessShopManager : MonoBehaviour
{
    [Header("連携するマネージャー")]
    public EndlessGameManager gameManager;

    [Header("選べる寿司のリスト")]
    public CarData[] availableSushi;

    [Header("UI要素")]
    public Text cartText;
    public Text errorText;
    public Button startRaceButton;
    public Text specText; // ★追加：スペック表示用

    [Header("効果音")]
    public AudioSource audioSource;
    public AudioClip buySE;
    public AudioClip errorSE;

    private List<CarData> shoppingCart = new List<CarData>();
    private Coroutine errorCoroutine; // ★追加：エラーメッセージ消去用

    // ★追加：一番安い寿司の値段を調べる（ゲームマネージャーから呼ばれる）
    public int GetCheapestPrice()
    {
        if (availableSushi.Length == 0) return 0;
        int min = availableSushi[0].price;
        foreach (var s in availableSushi)
        {
            if (s.price < min) min = s.price;
        }
        return min;
    }

    // ★修正：ショップが開くたびにGameManagerから確実に呼ばれる初期化処理
    public void InitializeShop()
    {
        shoppingCart.Clear();
        if (errorText != null) errorText.text = "";
        if (specText != null) specText.text = "";
        UpdateShopUI();
    }

    public void OnBuySushiClicked(int sushiIndex)
    {
        if (sushiIndex < 0 || sushiIndex >= availableSushi.Length) return;

        CarData data = availableSushi[sushiIndex];

        if (gameManager.TrySpendMoney(data.price))
        {
            shoppingCart.Add(data);
            if (audioSource != null && buySE != null) audioSource.PlayOneShot(buySE);
            if (errorText != null) errorText.text = ""; 
        }
        else
        {
            if (audioSource != null && errorSE != null) audioSource.PlayOneShot(errorSE);
            if (errorText != null) errorText.text = "お金が足りません！";
            
            // ★追加：1.5秒後にエラーメッセージを消す
            if (errorCoroutine != null) StopCoroutine(errorCoroutine);
            errorCoroutine = StartCoroutine(ClearErrorText());
        }

        UpdateShopUI();
    }

    // ★追加：エラーメッセージ消去コルーチン
    private IEnumerator ClearErrorText()
    {
        yield return new WaitForSeconds(1.5f);
        if (errorText != null) errorText.text = "";
    }

    public void OnStartRaceClicked()
    {
        if (shoppingCart.Count == 0)
        {
            if (errorText != null) errorText.text = "最低1つはネタを買ってください！";
            if (audioSource != null && errorSE != null) audioSource.PlayOneShot(errorSE);
            
            if (errorCoroutine != null) StopCoroutine(errorCoroutine);
            errorCoroutine = StartCoroutine(ClearErrorText());
            return;
        }

        gameManager.StartRacePhase(shoppingCart);
    }

    void UpdateShopUI()
    {
        if (cartText != null)
        {
            cartText.text = "出走リスト\n";
            foreach (var sushi in shoppingCart) cartText.text += $"[{sushi.sushiName}] ";
        }

        if (startRaceButton != null)
        {
            startRaceButton.interactable = shoppingCart.Count > 0;
        }
    }

    // --- ★追加：カーソルが乗った時のスペック表示 ---
    public void ShowSushiSpecs(int sushiIndex)
    {
        if (sushiIndex < 0 || sushiIndex >= availableSushi.Length) return;
        CarData data = availableSushi[sushiIndex];
        if (specText != null)
        {
            specText.text = $"【{data.sushiName}】価格: {data.price} 円 最高速度: {data.maxSpeed} 加速力: {data.acceleration} 安定感: {data.derailThreshold}";
        }
    }

    public void HideSushiSpecs()
    {
        if (specText != null) specText.text = ""; 
    }
}