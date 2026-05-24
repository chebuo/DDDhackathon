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

    [Header("レーンチェンジの移動速度（横の滑らかさ）")]
    public float laneChangeSpeed = 5f;

    [Header("モデルを配置する場所")]
    public Transform modelContainer;

    [HideInInspector]
    public GameObject[] availableLanes;

    private float acceleration = 20f;
    private float maxSpeed = 40f;
    private float friction = 10f;
    private float derailThreshold = 1500f;

    public float CurrentSpeed { get; private set; }
    public float CurrentCentrifugalForce { get; private set; }
    public bool IsFinished { get; private set; }
    public bool IsDerailed { get; private set; }

    private Rigidbody rb;
    private float splineProgress = 0f;
    private int currentLaneIndex = 0; 

    private float networkSplineProgress;
    private int networkLaneIndex;
    private float networkSpeed;

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
        }

        if (currentCarData != null)
        {
            ApplyCarData(currentCarData);
        }
    }

    void FindLanesFromScene()
    {
        List<GameObject> foundLanes = new List<GameObject>();
        int index = 0;

        while (true)
        {
            GameObject lane = GameObject.Find("OnlineLane_" + index);
            if (lane != null)
            {
                foundLanes.Add(lane);
                index++;
            }
            else
            {
                break; 
            }
        }

        availableLanes = foundLanes.ToArray();

        if (availableLanes.Length == 0)
        {
            Debug.LogError("【重要】シーン内に 'OnlineLane_0' などの名前のオブジェクトが1つも見つかりません！");
        }
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

    void Update()
    {
        if (IsDerailed || IsFinished) return;

        if (photonView.IsMine)
        {
            HandleInput();
            HandleLaneChangeInput();
            UpdatePosition();
        }
        else
        {
            UpdateRemotePosition();
        }
    }

    void HandleInput()
    {
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
        }

        currentSpline.Evaluate(splineProgress, out float3 targetPos, out float3 tangent, out float3 upVector);
        
        Vector3 worldPos = currentSpline.transform.TransformPoint(targetPos);
        Vector3 worldTangent = currentSpline.transform.TransformDirection(tangent);
        Vector3 worldUp = currentSpline.transform.TransformDirection(upVector);

        Vector3 forward = math.normalize(worldTangent);
        Vector3 worldRight = Vector3.Cross(worldUp, forward).normalized;

        Vector3 diff = transform.position - worldPos;
        float rightOffset = Vector3.Dot(diff, worldRight);
        if (CurrentSpeed == 0f || diff.magnitude > 10f) rightOffset = 0f;
        rightOffset = Mathf.Lerp(rightOffset, 0f, Time.deltaTime * laneChangeSpeed);
        transform.position = worldPos + (worldRight * rightOffset);

        if (forward != Vector3.zero)
        {
            // --- ★修正：Slerp（滑らか補間）を廃止し、コースの向きに毎フレーム強制固定 ---
            transform.rotation = Quaternion.LookRotation(forward, worldUp);
        }

        if (IsFinished) return;

        float lookAheadDist = 1f;
        float lookAheadProgress = Mathf.Repeat(splineProgress + (lookAheadDist / splineLength), 1f);
        currentSpline.Evaluate(lookAheadProgress, out float3 nextPos, out float3 nextTangent, out float3 nextUp);
        
        Vector3 worldNextTangent = currentSpline.transform.TransformDirection(nextTangent);
        Vector3 nextForward = math.normalize(worldNextTangent);

        float curvatureAngle = Vector3.Angle(forward, nextForward) / lookAheadDist;
        Vector3 cross = Vector3.Cross(forward, nextForward);
        if (cross.y < 0) curvatureAngle *= -1f;

        float forceMagnitude = (CurrentSpeed * CurrentSpeed) * Mathf.Abs(curvatureAngle);
        CurrentCentrifugalForce = (CurrentSpeed * CurrentSpeed) * curvatureAngle;

        if (forceMagnitude > derailThreshold) Derail(forward);
    }

    void UpdateRemotePosition()
    {
        if (availableLanes.Length == 0 || networkLaneIndex >= availableLanes.Length || availableLanes[networkLaneIndex] == null) return;

        splineProgress = networkSplineProgress;
        currentLaneIndex = networkLaneIndex;
        CurrentSpeed = networkSpeed;

        SplineContainer currentSpline = availableLanes[currentLaneIndex].GetComponent<SplineContainer>();
        if (currentSpline == null) return;

        currentSpline.Evaluate(splineProgress, out float3 targetPos, out float3 tangent, out float3 upVector);

        Vector3 worldPos = currentSpline.transform.TransformPoint(targetPos);
        Vector3 worldTangent = currentSpline.transform.TransformDirection(tangent);
        Vector3 worldUp = currentSpline.transform.TransformDirection(upVector);

        Vector3 forward = math.normalize(worldTangent);
        Vector3 worldRight = Vector3.Cross(worldUp, forward).normalized;

        Vector3 diff = transform.position - worldPos;
        float rightOffset = Vector3.Dot(diff, worldRight);
        if (CurrentSpeed == 0f || diff.magnitude > 10f) rightOffset = 0f;
        rightOffset = Mathf.Lerp(rightOffset, 0f, Time.deltaTime * laneChangeSpeed);
        transform.position = worldPos + (worldRight * rightOffset);

        if (forward != Vector3.zero)
        {
            // --- ★修正：他人の車（リモート）も向きを強制固定 ---
            transform.rotation = Quaternion.LookRotation(forward, worldUp);
        }
    }

    void Derail(Vector3 currentForward)
    {
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
            stream.SendNext(IsDerailed);
            stream.SendNext(IsFinished);
        }
        else
        {
            networkSplineProgress = (float)stream.ReceiveNext();
            networkLaneIndex = (int)stream.ReceiveNext();
            networkSpeed = (float)stream.ReceiveNext();
            IsDerailed = (bool)stream.ReceiveNext();
            IsFinished = (bool)stream.ReceiveNext();
        }
    }
}