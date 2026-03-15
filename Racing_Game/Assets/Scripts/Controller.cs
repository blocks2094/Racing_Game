using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("Wheel Settings")]
    public WheelCollider[] wheels = new WheelCollider[4];
    public Transform[] wheelModels = new Transform[4];

    [Header("Engine & Brake")]
    public float maxMotorTorque = 1200f;
    public float brakeTorque = 10000f;
    public float accelerationRate = 2000f;
    private float currentTorque = 0f;

    [Header("Steering Settings")]
    public float maxSteerAngle = 15f;
    public float minSteerAngle = 5f;
    public float maxSpeedForSteer = 100f;
    public float steerSpeed = 5f;
    private float currentSteerAngle = 0f;

    [Header("Physics Settings")]
    public Transform centerOfMass;
    public float downforce = 50f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }
    }

    private void FixedUpdate()
    {
        float speed = rb.velocity.magnitude * 3.6f;
        rb.AddForce(-transform.up * downforce * rb.velocity.magnitude);

        // ==========================================
        // 🎮 1. 입력 감지 (키보드 + 게임패드 동시 지원)
        // ==========================================

        // 왼쪽 스틱의 위/아래 기울기를 가져옵니다. (아래로 당기면 음수)
        float verticalInput = Input.GetAxis("Vertical");

        bool isAccel = Input.GetKey(KeyCode.W);
        bool isBrake = Input.GetKey(KeyCode.Space);
        bool isReverse = Input.GetKey(KeyCode.S);

        try
        {
            // LT를 당기면 브레이크
            if (Input.GetAxis("LT") > 0.1f) isBrake = true;

            // RT를 당겼을 때
            if (Input.GetAxis("RT") > 0.1f)
            {
                // 👇 핵심 추가: 왼쪽 스틱을 아래쪽으로 절반 이상 당긴 상태라면?
                if (verticalInput < -0.5f)
                {
                    isReverse = true;  // 후진 켜기
                    isAccel = false;   // 전진 끄기
                }
                else
                {
                    isAccel = true;    // 스틱을 안 당겼거나 위로 밀었다면 정상적으로 전진!
                }
            }
        }
        catch { /* 세팅 전 에러 방지 */ }


        // ==========================================
        // 🏎️ 2. 가속 및 브레이크 로직 적용
        // ==========================================

        if (isBrake)
        {
            currentTorque = 0f;
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = brakeTorque;
                wheels[i].motorTorque = 0;
            }
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 0.5f);
        }
        else
        {
            for (int i = 0; i < wheels.Length; i++) { wheels[i].brakeTorque = 0; }

            if (isAccel) // 직진 (W키 or 스틱중립+RT)
            {
                currentTorque = Mathf.MoveTowards(currentTorque, maxMotorTorque, accelerationRate * Time.deltaTime);
                for (int i = 0; i < wheels.Length; i++) { wheels[i].motorTorque = currentTorque; }
            }
            else if (isReverse) // 후진 (S키 or 스틱아래+RT)
            {
                currentTorque = Mathf.MoveTowards(currentTorque, -maxMotorTorque, accelerationRate * Time.deltaTime);
                for (int i = 0; i < wheels.Length; i++) { wheels[i].motorTorque = currentTorque; }
            }
            else // 아무것도 안 누름
            {
                currentTorque = Mathf.MoveTowards(currentTorque, 0f, accelerationRate * Time.deltaTime);
                for (int i = 0; i < wheels.Length; i++) { wheels[i].motorTorque = currentTorque; }
            }
        }

        // ==========================================
        // 🛞 3. 조향 (A/D 키 & 패드 왼쪽 스틱 모두 자동 지원)
        // ==========================================

        float currentMaxSteer = Mathf.Lerp(maxSteerAngle, minSteerAngle, speed / maxSpeedForSteer);
        float targetSteerAngle = Input.GetAxis("Horizontal") * currentMaxSteer;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.deltaTime * steerSpeed);

        for (int i = 0; i < wheels.Length - 2; i++)
        {
            wheels[i].steerAngle = currentSteerAngle;
        }

        // 고속 코너링 보조 (Grip Assist)
        if (speed > 10f)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
            localVelocity.x = Mathf.Lerp(localVelocity.x, 0, Time.deltaTime * 1f);
            rb.velocity = transform.TransformDirection(localVelocity);
        }

        // 시각적 바퀴 애니메이션 적용
        UpdateWheelPoses();
    }

    private void UpdateWheelPoses()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheelModels[i] != null)
            {
                Vector3 pos;
                Quaternion quat;
                wheels[i].GetWorldPose(out pos, out quat);
                wheelModels[i].position = pos;
                wheelModels[i].rotation = quat;
            }
        }
    }
}