using UnityEngine;

public class NetworkCameraFollow : MonoBehaviour
{
    [Header("追従するターゲット（自動でセットされます）")]
    public Transform target;

    [Header("カメラの位置調整（車から見てどの位置か）")]
    public Vector3 offset = new Vector3(0f, 4f, -6f); // 上に4、後ろに6

    [Header("追従の滑らかさ")]
    public float followSpeed = 10f;
    public float rotationSpeed = 10f;

    void LateUpdate()
    {
        // ターゲット（自分の車）がセットされるまでは何もしない
        if (target == null) return;

        // 車の向きに合わせたカメラの目標位置を計算
        Vector3 targetPos = target.position + target.TransformDirection(offset);
        
        // カメラを滑らかに移動
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        // カメラを滑らかに回転（車の少し前方を向くようにする）
        Vector3 lookDirection = (target.position + target.forward * 3f) - transform.position;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }
}