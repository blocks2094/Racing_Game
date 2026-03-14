using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Anti-Roll Force")]
    // 차체가 기울어지는 것을 막아주는 힘 (차량 질량에 따라 조절 필요, 보통 3000~5000)
    public float antiRollForce = 5000f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 앞바퀴 축 수평 유지
        ApplyAntiRoll(frontLeft, frontRight);
        // 뒷바퀴 축 수평 유지
        ApplyAntiRoll(rearLeft, rearRight);
    }

    // 좌우 바퀴의 서스펜션 압축 정도를 비교하여 수평을 맞추는 힘을 가하는 함수
    void ApplyAntiRoll(WheelCollider wheelL, WheelCollider wheelR)
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        // 왼쪽 바퀴가 땅에 닿아있으면 얼마나 눌렸는지 계산
        bool groundedL = wheelL.GetGroundHit(out hit);
        if (groundedL)
            travelL = (-wheelL.transform.InverseTransformPoint(hit.point).y - wheelL.radius) / wheelL.suspensionDistance;

        // 오른쪽 바퀴가 땅에 닿아있으면 얼마나 눌렸는지 계산
        bool groundedR = wheelR.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-wheelR.transform.InverseTransformPoint(hit.point).y - wheelR.radius) / wheelR.suspensionDistance;

        // 좌우 눌림 차이를 바탕으로 가해야 할 복원력 계산
        float rollForce = (travelL - travelR) * antiRollForce;

        // 왼쪽이 들리면 아래로 누르고, 눌리면 위로 들어올림
        if (groundedL)
            rb.AddForceAtPosition(wheelL.transform.up * -rollForce, wheelL.transform.position);

        // 오른쪽이 들리면 아래로 누르고, 눌리면 위로 들어올림
        if (groundedR)
            rb.AddForceAtPosition(wheelR.transform.up * rollForce, wheelR.transform.position);
    }
}