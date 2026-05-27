using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NetworkCarUIManager : MonoBehaviour
{
    [Header("UI部品")]
    public Text speedText;   
    public Slider gMeter;    
    public Text timeText;    

    // インスペクターからはセットせず、プログラムで自動取得する
    private NetworkCarController myCar;

    void Update()
    {
        // 自分の車がまだ見つかっていない場合、シーン内を探す
        if (myCar == null)
        {
            NetworkCarController[] cars = FindObjectsOfType<NetworkCarController>();
            foreach (var car in cars)
            {
                // 「自分が操作権限を持っている車」を見つけたらターゲットにする
                if (car.photonView.IsMine)
                {
                    myCar = car;
                    
                    // Gメーターの初期設定
                    if (gMeter != null && myCar.currentCarData != null)
                    {
                        gMeter.minValue = -myCar.derailThreshold;
                        gMeter.maxValue = myCar.derailThreshold;
                        gMeter.value = 0f;
                    }
                    break; // 見つけたらループを抜ける
                }
            }
            
            // まだ誰もネットワークに生成されていない場合は何もしない
            if (myCar == null) return; 
        }

        // 自分の車が見つかったら、毎フレームUIを更新する
        if (myCar.IsDerailed)
        {
            if (speedText != null) speedText.text = "脱線！";
            if (timeText != null) timeText.text = "RETIRE";
            if (gMeter != null) gMeter.value = 0f;
            return;
        }

        if (myCar.IsFinished)
        {
            if (speedText != null) speedText.text = "0.0 km/h";
            if (timeText != null) timeText.text = myCar.CurrentTime.ToString("F2") + " 秒\nGOAL!!";
            if (gMeter != null) gMeter.value = 0f;
            return;
        }

        if (speedText != null) speedText.text = myCar.CurrentSpeed.ToString("F1") + " km/h";
        if (gMeter != null) gMeter.value = myCar.CurrentCentrifugalForce;
        if (timeText != null) timeText.text = myCar.CurrentTime.ToString("F2") + " 秒";
    }
}