using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    [Header("連携するスクリプトとUI")]
    public CarGameManager gameManager;
    public GameObject menuPanel;
    public GameObject gameUIPanel;

    [Header("選べる寿司のリスト")]
    public CarData[] availableSushi;

    [Header("メニュー用テキスト")]
    public Text instructionText;
    public Text p1DeckText;
    public Text p2DeckText;
    public Text specText; 

    // --- 効果音 ---
    [Header("効果音")]
    public AudioSource audioSource;
    public AudioClip selectSE; // メニュー選択音
    // --------------------

    private List<CarData> p1Selection = new List<CarData>();
    private List<CarData> p2Selection = new List<CarData>();

    void Start()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (gameUIPanel != null) gameUIPanel.SetActive(false);
        if (specText != null) specText.text = ""; 
        UpdateUI();
    }

    public void OnSushiButtonClicked(int sushiIndex)
    {
        if (sushiIndex < 0 || sushiIndex >= availableSushi.Length) return;

        // ボタンを押した時に音を鳴らす
        if (audioSource != null && selectSE != null) audioSource.PlayOneShot(selectSE);

        CarData selectedData = availableSushi[sushiIndex];

        if (p1Selection.Count < 5) p1Selection.Add(selectedData);
        else if (p2Selection.Count < 5) p2Selection.Add(selectedData);

        UpdateUI();
        if (p1Selection.Count == 5 && p2Selection.Count == 5) StartGame();
    }

    // (UpdateUI, ShowSushiSpecs, HideSushiSpecs は前回と同じなので省略せずにそのまま残してください)
    void UpdateUI()
    {
        if (p1Selection.Count < 5) instructionText.text = $"プレイヤー1 の番です\nあと {5 - p1Selection.Count} 皿選んでください";
        else if (p2Selection.Count < 5) instructionText.text = $"プレイヤー2 の番です\nあと {5 - p2Selection.Count} 皿選んでください";
        else instructionText.text = "バトルスタート！";

        p1DeckText.text = "1P デッキ: ";
        foreach (var s in p1Selection) p1DeckText.text += $"[{s.sushiName}] ";

        p2DeckText.text = "2P デッキ: ";
        foreach (var s in p2Selection) p2DeckText.text += $"[{s.sushiName}] ";
    }

    public void ShowSushiSpecs(int sushiIndex)
    {
        if (sushiIndex < 0 || sushiIndex >= availableSushi.Length) return;
        CarData data = availableSushi[sushiIndex];
        if (specText != null) specText.text = $"【 {data.sushiName} 】価格 : {data.price} 円 最高速度 : {data.maxSpeed} 加速力 : {data.acceleration} 安定感 : {data.derailThreshold}";
    }

    public void HideSushiSpecs()
    {
        if (specText != null) specText.text = ""; 
    }

    void StartGame()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (gameUIPanel != null) gameUIPanel.SetActive(true);
        gameManager.StartGameWithDecks(p1Selection.ToArray(), p2Selection.ToArray());
    }
}