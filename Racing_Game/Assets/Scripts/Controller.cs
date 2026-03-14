using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("Wheel Settings")]
    public WheelCollider[] wheels = new WheelCollider[4];

    // 👇 새로 추가: 눈에 보이는 실제 바퀴 오브젝트(메쉬)를 연결할 배열
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

        // 1. 가속 및 브레이크 
        if (Input.GetKey(KeyCode.Space))
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

            if (Input.GetKey(KeyCode.W))
            {
                currentTorque = Mathf.MoveTowards(currentTorque, maxMotorTorque, accelerationRate * Time.deltaTime);
                for (int i = 0; i < wheels.Length; i++) { wheels[i].motorTorque = currentTorque; }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                currentTorque = Mathf.MoveTowards(currentTorque, -maxMotorTorque, accelerationRate * Time.deltaTime);
                for (int i = 0; i < wheels.Length; i++) { wheels[i].motorTorque = currentTorque; }
            }
            else
            {
                currentTorque = Mathf.MoveTowards(currentTorque, 0f, accelerationRate * Time.deltaTime);
                for (int i = 0; i < wheels.Length; i++) { wheels[i].motorTorque = currentTorque; }
            }
        }

        // 2. 조향 
        float currentMaxSteer = Mathf.Lerp(maxSteerAngle, minSteerAngle, speed / maxSpeedForSteer);
        float targetSteerAngle = Input.GetAxis("Horizontal") * currentMaxSteer;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.deltaTime * steerSpeed);

        for (int i = 0; i < wheels.Length - 2; i++)
        {
            wheels[i].steerAngle = currentSteerAngle;
        }
        // 👇 추가할 부분: 고속 코너링 시 부드러운 궤적 보조 (Grip Assist)
        if (speed > 10f) // 시속 10km 이상일 때만 작동 (제자리 회전 방지)
        {
            // 현재 자동차의 실제 이동 방향(속도)을 가져옵니다.
            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);

            // X축(좌우로 미끄러지는 속도)을 0에 가깝게 부드럽게 깎아냅니다.
            // 👉 맨 끝의 숫자 '3f'를 조절하여 코너링 느낌을 바꿀 수 있습니다.
            // (1f = 현실적이고 많이 미끄러짐 / 5f = 카트라이더처럼 레일을 타듯 부드럽고 쫀득함)
            localVelocity.x = Mathf.Lerp(localVelocity.x, 0, Time.deltaTime * 1f);

            // 보정된 궤적을 다시 적용합니다.
            rb.velocity = transform.TransformDirection(localVelocity);
        }

        // 👇 3. 시각적 바퀴 애니메이션 적용 함수 호출
        UpdateWheelPoses();
    }

    // 👇 새로 추가된 애니메이션 처리 함수
    private void UpdateWheelPoses()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            // 배열에 바퀴 모델이 제대로 연결되어 있는지 확인
            if (wheelModels[i] != null)
            {
                Vector3 pos;
                Quaternion quat;

                // WheelCollider의 현재 물리적 위치와 회전 각도를 가져옴
                wheels[i].GetWorldPose(out pos, out quat);

                // 눈에 보이는 바퀴 모델의 위치와 회전값을 물리 연산 결과와 똑같이 맞춰줌
                wheelModels[i].position = pos;
                wheelModels[i].rotation = quat;
            }
        }
    }
}