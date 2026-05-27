using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using Photon.Pun;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class NetworkCarController : MonoBehaviourPun, IPunObservable
{
    [Header("寿司のデータ")]
    public CarData currentCarData;

    [Header("レーンチェンジの移動速度")]
    public float laneChangeSpeed = 5f;

    [Header("モデルを配置する場所")]
    public Transform modelContainer;

    [HideInInspector]
    public GameObject[] availableLanes;

    [Header("効果音")]
    public AudioSource engineAudioSource; 
    public AudioSource seAudioSource;     
    public AudioClip sparkSE;
    public AudioClip derailSE;
    public AudioClip goalSE;

    [Header("エフェクト設定")]
    public GameObject sparkEffectPrefab; 
    public Transform sparkEmitter; 
    public float sparkThresholdRatio = 0.8f; 
    public float sparkEmissionMultiplier = 0.5f;

    private ParticleSystem activeSparkEffect;

    private float acceleration = 20f;
    private float maxSpeed = 40f;
    private float friction = 10f;
    public float derailThreshold = 1500f; 

    public float CurrentSpeed { get; private set; }
    public float CurrentCentrifugalForce { get; private set; }
    public float CurrentTime { get; private set; } 
    public bool IsFinished { get; private set; }
    public bool IsDerailed { get; private set; }
    
    // ★追加：ゲーム開始まで操作を無効にするフラグ
    public bool CanDrive { get; set; } = false;

    private Rigidbody rb;
    private float splineProgress = 0f;
    private int currentLaneIndex = 0; 

    private float networkSplineProgress;
    private int networkLaneIndex;
    private float networkSpeed;
    private float networkCentrifugalForce; 
    private float networkTime;             

    private float visualLaneFloat = 0f;
    private bool isFirstNetworkReceive = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        FindLanesFromScene();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            currentLaneIndex = Mathf.Clamp(PhotonNetwork.LocalPlayer.ActorNumber - 1, 0, availableLanes.Length - 1);
            visualLaneFloat = currentLaneIndex; 
        }

        if (currentCarData != null) ApplyCarData(currentCarData);
    }

    void FindLanesFromScene()
    {
        List<GameObject> foundLanes = new List<GameObject>();
        int index = 0;
        while (true)
        {
            GameObject lane = GameObject.Find("OnlineLane_" + index);
            if (lane != null) { foundLanes.Add(lane); index++; }
            else break; 
        }
        availableLanes = foundLanes.ToArray();
    }

    public void ApplyCarData(CarData data)
    {
        if (data == null) return;
        currentCarData = data;
        acceleration = data.acceleration;
        maxSpeed = data.maxSpeed;
        friction = data.friction;
        derailThreshold = data.derailThreshold;

        if (data.modelPrefab != null && modelContainer != null)
        {
            foreach (Transform child in modelContainer) Destroy(child.gameObject);
            Instantiate(data.modelPrefab, modelContainer.position, modelContainer.rotation, modelContainer);
        }
    }

    // ★追加：ネットワーク経由で他の人の画面でも自分の寿司を変更させるメソッド
    [PunRPC]
    public void RpcSetCarData(int sushiIndex)
    {
        OnlineGameManager gm = FindObjectOfType<OnlineGameManager>();
        if (gm != null && sushiIndex >= 0 && sushiIndex < gm.availableSushi.Length)
        {
            ApplyCarData(gm.availableSushi[sushiIndex]);
        }
    }

    void Update()
    {
        if (IsDerailed || IsFinished) return;

        if (photonView.IsMine)
        {
            // ★追加：CanDriveがONの時だけタイムを進める
            if (CanDrive) CurrentTime += Time.deltaTime; 
            
            HandleInput();
            HandleLaneChangeInput();
            UpdatePosition();
        }
        else
        {
            UpdateRemotePosition();
        }

        ControlSparkEffect();
        ControlEngineSound();
    }

    void HandleInput()
    {
        if (!CanDrive) return; // ★追加：スタート前は操作できない
        if (Keyboard.current == null) return;
        bool isAccelerating = Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed;

        if (isAccelerating) CurrentSpeed += acceleration * Time.deltaTime;
        else CurrentSpeed -= friction * Time.deltaTime;

        CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, maxSpeed);
    }

    void HandleLaneChangeInput()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if (currentLaneIndex > 0) currentLaneIndex--;
        }
        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if (currentLaneIndex < availableLanes.Length - 1) currentLaneIndex++;
        }
    }

    void ApplyPositionAndRotation(float progress, float targetLane)
    {
        if (availableLanes.Length == 0) return;

        visualLaneFloat = Mathf.Lerp(visualLaneFloat, targetLane, Time.deltaTime * laneChangeSpeed);

        int laneAIndex = Mathf.Clamp(Mathf.FloorToInt(visualLaneFloat), 0, availableLanes.Length - 1);
        int laneBIndex = Mathf.Clamp(Mathf.CeilToInt(visualLaneFloat), 0, availableLanes.Length - 1);
        float lerpFactor = visualLaneFloat - laneAIndex;

        SplineContainer splineA = availableLanes[laneAIndex].GetComponent<SplineContainer>();
        SplineContainer splineB = availableLanes[laneBIndex].GetComponent<SplineContainer>();
        if (splineA == null || splineB == null) return;

        splineA.Evaluate(progress, out float3 posA, out float3 tanA, out float3 upA);
        Vector3 worldPosA = splineA.transform.TransformPoint(posA);
        Vector3 worldTanA = splineA.transform.TransformDirection(tanA);
        Vector3 worldUpA = splineA.transform.TransformDirection(upA);

        splineB.Evaluate(progress, out float3 posB, out float3 tanB, out float3 upB);
        Vector3 worldPosB = splineB.transform.TransformPoint(posB);
        Vector3 worldTanB = splineB.transform.TransformDirection(tanB);
        Vector3 worldUpB = splineB.transform.TransformDirection(upB);

        transform.position = Vector3.Lerp(worldPosA, worldPosB, lerpFactor);
        Vector3 finalTan = Vector3.Lerp(worldTanA, worldTanB, lerpFactor);
        Vector3 finalUp = Vector3.Lerp(worldUpA, worldUpB, lerpFactor);
        
        Vector3 forward = math.normalize(finalTan);
        if (forward != Vector3.zero) transform.rotation = Quaternion.LookRotation(forward, finalUp);
    }

    void UpdatePosition()
    {
        if (availableLanes.Length == 0 || currentLaneIndex >= availableLanes.Length || availableLanes[currentLaneIndex] == null) return;

        SplineContainer currentSpline = availableLanes[currentLaneIndex].GetComponent<SplineContainer>();
        if (currentSpline == null) return;

        float splineLength = currentSpline.CalculateLength();

        if (CurrentSpeed > 0f)
        {
            float progressDelta = (CurrentSpeed * Time.deltaTime) / splineLength;
            splineProgress += progressDelta;
        }

        if (splineProgress >= 1f)
        {
            splineProgress = 1f;
            IsFinished = true;
            CurrentSpeed = 0f;
            CurrentCentrifugalForce = 0f;
            if (seAudioSource != null && goalSE != null && photonView.IsMine) seAudioSource.PlayOneShot(goalSE);
            if (engineAudioSource != null) engineAudioSource.Stop();
        }

        ApplyPositionAndRotation(splineProgress, currentLaneIndex);

        if (IsFinished) return;

        float lookAheadDist = 1f;
        float lookAheadProgress = Mathf.Repeat(splineProgress + (lookAheadDist / splineLength), 1f);
        
        currentSpline.Evaluate(splineProgress, out float3 currentPos, out float3 currentTan, out float3 currentUp);
        Vector3 currentForward = math.normalize(currentSpline.transform.TransformDirection(currentTan));

        currentSpline.Evaluate(lookAheadProgress, out float3 nextPos, out float3 nextTangent, out float3 nextUp);
        Vector3 worldNextTangent = currentSpline.transform.TransformDirection(nextTangent);
        Vector3 nextForward = math.normalize(worldNextTangent);

        float curvatureAngle = Vector3.Angle(currentForward, nextForward) / lookAheadDist;
        Vector3 cross = Vector3.Cross(currentForward, nextForward);
        if (cross.y < 0) curvatureAngle *= -1f;

        float forceMagnitude = (CurrentSpeed * CurrentSpeed) * Mathf.Abs(curvatureAngle);
        CurrentCentrifugalForce = (CurrentSpeed * CurrentSpeed) * curvatureAngle;

        if (forceMagnitude > derailThreshold) Derail(currentForward);
    }

    void UpdateRemotePosition()
    {
        if (availableLanes.Length == 0) return;

        splineProgress = Mathf.Lerp(splineProgress, networkSplineProgress, Time.deltaTime * 10f);
        currentLaneIndex = networkLaneIndex;
        CurrentSpeed = networkSpeed;
        CurrentCentrifugalForce = networkCentrifugalForce; 
        CurrentTime = networkTime;                         

        ApplyPositionAndRotation(splineProgress, networkLaneIndex);
    }

    void ControlSparkEffect()
    {
        if (IsFinished || IsDerailed || sparkEffectPrefab == null || sparkEmitter == null)
        {
            if (activeSparkEffect != null) 
            {
                var emission = activeSparkEffect.emission;
                emission.enabled = false;
            }
            return;
        }

        float currentGRatio = Mathf.Abs(CurrentCentrifugalForce) / derailThreshold;

        if (currentGRatio >= sparkThresholdRatio)
        {
            if (activeSparkEffect == null)
            {
                activeSparkEffect = Instantiate(sparkEffectPrefab, sparkEmitter.position, Quaternion.LookRotation(transform.right, transform.up), sparkEmitter).GetComponent<ParticleSystem>();
            }

            var emission = activeSparkEffect.emission;
            emission.enabled = true;
            emission.rateOverTime = 50f; 

            float crossY = CurrentCentrifugalForce / (CurrentSpeed * CurrentSpeed); 
            Vector3 sparkDirection = (crossY < 0) ? -transform.right : transform.right; 
            activeSparkEffect.transform.rotation = Quaternion.LookRotation(sparkDirection, transform.up);

            if (seAudioSource != null && sparkSE != null && !seAudioSource.isPlaying)
            {
                seAudioSource.PlayOneShot(sparkSE);
            }
        }
        else
        {
            if (activeSparkEffect != null) 
            {
                var emission = activeSparkEffect.emission;
                emission.enabled = false;
            }
        }
    }

    void ControlEngineSound()
    {
        if (engineAudioSource != null)
        {
            if (CurrentSpeed > 0 && !IsFinished && !IsDerailed)
            {
                if (!engineAudioSource.isPlaying) engineAudioSource.Play();
                engineAudioSource.pitch = 0.8f + (CurrentSpeed / maxSpeed); 
            }
            else
            {
                engineAudioSource.Stop();
            }
        }
    }

    void Derail(Vector3 currentForward)
    {
        if (seAudioSource != null && derailSE != null) seAudioSource.PlayOneShot(derailSE);
        if (engineAudioSource != null) engineAudioSource.Stop();
        
        if (activeSparkEffect != null) 
        {
            var emission = activeSparkEffect.emission;
            emission.enabled = false;
        }

        IsDerailed = true;
        rb.isKinematic = false;
        rb.linearVelocity = currentForward * CurrentSpeed;
        rb.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(splineProgress);
            stream.SendNext(currentLaneIndex);
            stream.SendNext(CurrentSpeed);
            stream.SendNext(CurrentCentrifugalForce); 
            stream.SendNext(CurrentTime);             
            stream.SendNext(IsDerailed);
            stream.SendNext(IsFinished);
        }
        else
        {
            networkSplineProgress = (float)stream.ReceiveNext();
            networkLaneIndex = (int)stream.ReceiveNext();
            networkSpeed = (float)stream.ReceiveNext();
            networkCentrifugalForce = (float)stream.ReceiveNext(); 
            networkTime = (float)stream.ReceiveNext();             
            IsDerailed = (bool)stream.ReceiveNext();
            IsFinished = (bool)stream.ReceiveNext();

            if (isFirstNetworkReceive)
            {
                visualLaneFloat = networkLaneIndex;
                splineProgress = networkSplineProgress;
                isFirstNetworkReceive = false;
            }
        }
    }
}