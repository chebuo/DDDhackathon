using UnityEngine;
// ★シーン移動に必須の1行
using UnityEngine.SceneManagement; 

public class TtoE : MonoBehaviour
{
    // ボタンから呼び出せるように public にする
    public void GoToGameScene()
    {
        // 括弧の中に、移動したいシーンの名前を一言一句間違えずに書きます
        SceneManager.LoadScene("Endless"); 
    }
}