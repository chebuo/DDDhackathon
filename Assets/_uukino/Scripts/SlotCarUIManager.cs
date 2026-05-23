using UnityEngine;
using UnityEngine.UI;

public class SlotCarUIManager : MonoBehaviour
{
    [Header("車のデータ取得元")]
    public SlotCarController targetCar;

    [Header("UI部品")]
    public Text speedText;   
    public Slider gMeter;    
    public Text timeText;    

    void Start()
    {
        if (targetCar != null && gMeter != null)
        {
            gMeter.minValue = -targetCar.derailThreshold;
            gMeter.maxValue = targetCar.derailThreshold;
            gMeter.value = 0f;
        }
    }

    void Update()
    {
        if (targetCar == null) return;

        if (targetCar.IsDerailed)
        {
            if (speedText != null) speedText.text = "脱線！";
            if (timeText != null) timeText.text = "RETIRE";
            if (gMeter != null) gMeter.value = 0f;
            return;
        }

        if (targetCar.IsFinished)
        {
            if (speedText != null) speedText.text = "0.0 km/h";
            if (timeText != null) timeText.text = targetCar.CurrentTime.ToString("F2") + " 秒\nGOAL!!";
            if (gMeter != null) gMeter.value = 0f;
            return;
        }

        // --- ★追加：ペナルティ（出遅れ）中の表示 ---
        if (targetCar.IsStalled)
        {
            if (speedText != null) speedText.text = "出遅れ！";
            if (gMeter != null) gMeter.value = 0f;
            // タイムはそのまま進める（ペナルティ中もタイムロスになるように）
            if (timeText != null) timeText.text = targetCar.CurrentTime.ToString("F2") + " 秒";
            return;
        }
        // ------------------------------------------

        if (speedText != null) speedText.text = targetCar.CurrentSpeed.ToString("F1") + " km/h";
        if (gMeter != null) gMeter.value = targetCar.CurrentCentrifugalForce;
        if (timeText != null) timeText.text = targetCar.CurrentTime.ToString("F2") + " 秒";
    }
}