using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("生成する車のPrefab名")]
    [Tooltip("Assets/Resources フォルダ内に配置したPrefabの名前を入力")]
    public string carPrefabName = "NetworkSushiCar";

    [Header("スタート位置のオフセット")]
    public float spawnOffset = 2f;

    void Start()
    {
        // 1. Photonサーバーに接続
        Debug.Log("Photonサーバーに接続中...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("マスターサーバーに接続しました。ロビーに入ります。");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("ロビーに入りました。ランダムな部屋に参加します。");
        // 定員5人のカスタムルーム設定
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 5 };
        PhotonNetwork.JoinOrCreateRoom("SushiOnlineRoom", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"ルームに参加しました。現在の参加人数: {PhotonNetwork.CurrentRoom.PlayerCount}人");

        // 自分のプレイヤー番号（1～5）に応じて、初期レーンや初期位置をずらす
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        Vector3 spawnPosition = transform.position + Vector3.right * (actorNumber * spawnOffset);

        // 2. ネットワーク上に自分の車を生成
        // ※このPrefabは必ず「Assets/Resources」フォルダ内にある必要があります
        GameObject myCar = PhotonNetwork.Instantiate(carPrefabName, spawnPosition, Quaternion.identity);
        
        // 自分の車であることをローカルのカメラ等に通知する場合はここで設定
        Debug.Log("自車の生成が完了しました。");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"他のプレイヤーが入室しました: {newPlayer.NickName}");
    }
}