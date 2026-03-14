using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // 카메라가 따라갈 타겟 (Car 오브젝트)

    [Header("Camera Position")]
    // 자동차를 기준으로 한 카메라의 기본 위치 (X: 좌우, Y: 높이, Z: 앞뒤 거리)
    // Z값을 마이너스(-)로 주어야 자동차 뒤에 위치합니다.
    public Vector3 offset = new Vector3(0f, 3f, -7f);

    [Header("Smoothness")]
    public float followSpeed = 10f; // 카메라가 따라가는 속도 (낮을수록 고무줄처럼 부드러움)
    public float lookSpeed = 10f;   // 카메라가 자동차를 향해 회전하는 속도

    private void LateUpdate()
    {
        // 타겟(자동차)이 연결되어 있지 않으면 실행하지 않음
        if (target == null) return;

        // 1. 목표 위치 계산: 자동차의 현재 위치 + 자동차가 바라보는 방향을 기준으로 한 오프셋
        Vector3 targetPosition = target.position + target.TransformDirection(offset);

        // 2. 부드러운 위치 이동 (Lerp 사용)
        // 현재 위치에서 목표 위치로 followSpeed에 맞춰 부드럽게 이동합니다.
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // 3. 부드러운 회전 (자동차 바라보기)
        Vector3 lookDirection = target.position - transform.position;
        if (lookDirection != Vector3.zero)
        {
            // 자동차를 바라보는 목표 회전값 계산
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            // 현재 회전값에서 목표 회전값으로 부드럽게 회전
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        }
    }
}