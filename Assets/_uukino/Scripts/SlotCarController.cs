using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SlotCarController : MonoBehaviour
{
    public enum PlayerType { Player1, Player2 }

    [Header("プレイヤー設定")]
    public PlayerType playerType = PlayerType.Player1;

    [Header("寿司のデータ")]
    public CarData currentCarData; 

    [Header("コース設定")]
    public SplineContainer splineContainer;

    [Header("モデルを配置する場所")]
    public Transform modelContainer;

    [Header("フライングペナルティ")]
    public float startDelayPenalty = 1.0f;

    // --- 効果音関連 ---
    [Header("効果音")]
    [Tooltip("ループ再生にするエンジン音用")]
    public AudioSource engineAudioSource; 
    [Tooltip("単発で鳴らすSE用")]
    public AudioSource seAudioSource;     
    
    public AudioClip sparkSE;  // 火花
    public AudioClip derailSE; // 脱線
    public AudioClip goalSE;   // ゴール
    // -------------------------

    // --- 火花エフェクト関連 ---
    [Header("エフェクト設定")]
    [Tooltip("火花のParticle System Prefab")]
    public GameObject sparkEffectPrefab; 
    [Tooltip("エフェクトを発生させる場所（車体の下部など）")]
    public Transform sparkEmitter; 
    [Tooltip("脱線限界値の何％から火花を出すか (0.0 ～ 1.0)")]
    public float sparkThresholdRatio = 0.8f; 
    [Tooltip("火花の発生量の倍率 (0.0 ～ 1.0)")]
    public float sparkEmissionMultiplier = 0.5f;

    private ParticleSystem activeSparkEffect; // 生成されたエフェクトのインスタンス
    // ------------------------------------

    [HideInInspector] public float acceleration;
    [HideInInspector] public float maxSpeed;
    [HideInInspector] public float friction;
    [HideInInspector] public float derailThreshold;
    
    public float CurrentSpeed { get; private set; }
    public float CurrentCentrifugalForce { get; private set; }
    public float CurrentTime { get; private set; }
    public bool IsFinished { get; private set; }
    public bool IsDerailed { get; private set; }
    public bool CanDrive { get; set; } = false;

    public bool IsStalled => currentPenaltyTimer > 0f; 
    private float currentPenaltyTimer = 0f;

    private Rigidbody rb;
    private float splineProgress = 0f;
    private float splineLength;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        if (splineContainer != null) splineLength = splineContainer.CalculateLength();
    }

    public void ResetCar(CarData newData)
    {
        IsFinished = false;
        IsDerailed = false;
        CanDrive = false; 
        
        CurrentTime = 0f;
        CurrentSpeed = 0f;
        CurrentCentrifugalForce = 0f;
        splineProgress = 0f;
        currentPenaltyTimer = 0f;

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (splineContainer != null)
        {
            splineContainer.Evaluate(0f, out float3 position, out float3 tangent, out float3 upVector);
            transform.position = position;
            Vector3 forward = math.normalize(tangent);
            if (forward != Vector3.zero) transform.rotation = Quaternion.LookRotation(forward, upVector);
        }

        // --- ★変更：エフェクトをリセットして古いインスタンスを削除 ---
        if (activeSparkEffect != null)
        {
            Destroy(activeSparkEffect.gameObject);
            activeSparkEffect = null;
        }
        // ------------------------------------

        ApplyCarData(newData);
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

    bool CheckInput()
    {
        if (Keyboard.current == null) return false;
        if (playerType == PlayerType.Player1) return Keyboard.current.wKey.isPressed;
        if (playerType == PlayerType.Player2) return Keyboard.current.upArrowKey.isPressed;
        return false;
    }

    void Update()
    {
        if (IsDerailed || IsFinished || splineContainer == null) return;

        bool isAccelerating = CheckInput();

        if (!CanDrive)
        {
            if (isAccelerating) currentPenaltyTimer = startDelayPenalty;
            else currentPenaltyTimer = 0f;
            return;
        }

        if (currentPenaltyTimer > 0f)
        {
            currentPenaltyTimer -= Time.deltaTime;
            isAccelerating = false; 
        }

        CurrentTime += Time.deltaTime;

        if (isAccelerating) CurrentSpeed += acceleration * Time.deltaTime;
        else CurrentSpeed -= friction * Time.deltaTime;
        
        CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, maxSpeed);

        UpdatePosition();

        ControlSparkEffect();

        // --- ★追加：エンジン音のコントロール ---
        if (engineAudioSource != null)
        {
            // スピードに応じて音の高さ（ピッチ）を変えるとリアルになります
            if (CurrentSpeed > 0)
            {
                if (!engineAudioSource.isPlaying) engineAudioSource.Play();
                engineAudioSource.pitch = 0.8f + (CurrentSpeed / maxSpeed); // 0.8倍 ～ 1.8倍に変化
            }
            else
            {
                engineAudioSource.Stop();
            }
        }
        // ------------------------------------
    }

    void UpdatePosition()
    {
        if (CurrentSpeed <= 0f) 
        {
            CurrentCentrifugalForce = 0f;
            return;
        }

        float progressDelta = (CurrentSpeed * Time.deltaTime) / splineLength;
        splineProgress += progressDelta;

        if (splineProgress >= 1f)
        {
            splineProgress = 1f;
            IsFinished = true;
            CurrentSpeed = 0f;
            CurrentCentrifugalForce = 0f;
            if (seAudioSource != null && goalSE != null) seAudioSource.PlayOneShot(goalSE);
            if (engineAudioSource != null) engineAudioSource.Stop();
        }

        splineContainer.Evaluate(splineProgress, out float3 position, out float3 tangent, out float3 upVector);
        transform.position = position;
        
        Vector3 forward = math.normalize(tangent);
        if (forward != Vector3.zero) transform.rotation = Quaternion.LookRotation(forward, upVector);

        if (IsFinished) return;

        float lookAheadDist = 1f; 
        float lookAheadProgress = Mathf.Repeat(splineProgress + (lookAheadDist / splineLength), 1f);
        splineContainer.Evaluate(lookAheadProgress, out float3 nextPos, out float3 nextTangent, out float3 nextUp);
        Vector3 nextForward = math.normalize(nextTangent);
        
        float curvatureAngle = Vector3.Angle(forward, nextForward) / lookAheadDist;
        Vector3 cross = Vector3.Cross(forward, nextForward);
        if (cross.y < 0) curvatureAngle *= -1f;

        float forceMagnitude = (CurrentSpeed * CurrentSpeed) * Mathf.Abs(curvatureAngle);
        CurrentCentrifugalForce = (CurrentSpeed * CurrentSpeed) * curvatureAngle;

        if (forceMagnitude > derailThreshold) Derail(forward);
    }

    // --- エフェクトの制御 ---
    void ControlSparkEffect()
    {
        // 走れない状態、ゴール、脱線、またはエフェクト設定がない場合は発生を止める
        if (!CanDrive || IsFinished || IsDerailed || sparkEffectPrefab == null || sparkEmitter == null)
        {
            if (activeSparkEffect != null)
            {
                var emission = activeSparkEffect.emission;
                emission.enabled = false;
            }
            return;
        }

        // 現在のGの大きさの、脱線限界値に対する割合 (0.0 ～ 1.0)
        float currentGRatio = Mathf.Abs(CurrentCentrifugalForce) / derailThreshold;

        // 火花を出すかどうかの判定
        if (currentGRatio >= sparkThresholdRatio)
        {
            // インスタンスがまだない場合は生成
            if (activeSparkEffect == null)
            {
                // 車体を親にして生成。向きは車体の横（カーブの外側）に向ける。
                activeSparkEffect = Instantiate(sparkEffectPrefab, sparkEmitter.position, Quaternion.LookRotation(transform.right, transform.up), sparkEmitter).GetComponent<ParticleSystem>();
            }

            // 火花の発生量をGの割合に応じて増やす
            // sparkThresholdRatioで0、1.0で最大になるように計算
            float normalizedG = (currentGRatio - sparkThresholdRatio) / (1f - sparkThresholdRatio); // 0.0 ～ 1.0 に正規化
            
            // Prefabで設定された最大発生量 × 倍率 × 正規化されたG
            //float finalEmissionRate = normalizedG * sparkEmissionMultiplier * activeSparkEffect.emission.rateOverTime.constantMax; 
            float finalEmissionRate = 50f;

            // Emissionコンポーネントを有効にし、Rateをセット
            var emission = activeSparkEffect.emission;
            emission.enabled = true;
            emission.rateOverTime = finalEmissionRate; 

            // エフェクトの向きをカーブの外側（Gのかかる方向）に向ける
            // CurrentCentrifugalForce の正負で左右を判定。
            float crossY = CurrentCentrifugalForce / (CurrentSpeed * CurrentSpeed); // cross.y と同等
            Vector3 sparkDirection = (crossY < 0) ? -transform.right : transform.right; // crossY<0なら左折傾向、Gは右向き(transform.right)

            activeSparkEffect.transform.rotation = Quaternion.LookRotation(sparkDirection, transform.up);

            if (seAudioSource != null && sparkSE != null && !seAudioSource.isPlaying)
            {
                seAudioSource.PlayOneShot(sparkSE);
            }
        }
        else
        {
            // 火花を出さない範囲なら、発生を止める
            if (activeSparkEffect != null)
            {
                var emission = activeSparkEffect.emission;
                emission.enabled = false;
            }
        }
    }
    // ----------------------------------------
    

    void Derail(Vector3 currentForward)
    {
        if (seAudioSource != null && derailSE != null) seAudioSource.PlayOneShot(derailSE);
        if (engineAudioSource != null) engineAudioSource.Stop();
        // --- 脱線時に火花エフェクトを即座に止める ---
        if (activeSparkEffect != null)
        {
            var emission = activeSparkEffect.emission;
            emission.enabled = false;
        }
        // ---------------------------------------------------

        IsDerailed = true;
        rb.isKinematic = false;
        rb.linearVelocity = currentForward * CurrentSpeed;
        rb.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);
        rb.angularVelocity = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f));
    }
}