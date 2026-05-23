using UnityEngine;

// 右クリックメニューから簡単にデータを作成できるようにする属性
[CreateAssetMenu(fileName = "NewCarData", menuName = "SushiRace/CarData")]
public class CarData : ScriptableObject
{
    [Header("基本情報")]
    public string sushiName = "マグロ";
    public int price = 100;           // 皿の値段（勝敗判定用）
    public GameObject modelPrefab;    // 寿司の3Dモデル（見た目）

    [Header("走行性能")]
    public float maxSpeed = 40f;      // 最高速度
    public float acceleration = 20f;  // 加速力
    public float friction = 10f;      // アクセルオフ時の減衰
    public float derailThreshold = 1500f; // 脱線限界値（重さ/安定感）
}