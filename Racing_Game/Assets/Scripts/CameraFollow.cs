using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // 카메라가 따라갈 타겟 (Car 오브젝트)

    [Header("Camera Position (카메라 위치)")]
    // 자동차를 기준으로 한 카메라의 기본 위치 (X: 좌우, Y: 높이, Z: 앞뒤 거리)
    public Vector3 offset = new Vector3(0f, 3f, -7f);

    // 👇 새로 추가된 부분: 카메라가 쳐다볼 타겟의 높이 보정값
    [Header("Look Offset (시선 보정)")]
    // Y값을 올릴수록 카메라가 위를 쳐다보게 되어 차가 화면 아래로 내려갑니다.
    public Vector3 lookOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Smoothness")]
    public float followSpeed = 10f; // 카메라가 따라가는 속도
    public float lookSpeed = 10f;   // 카메라가 자동차를 향해 회전하는 속도

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. 목표 위치 계산: 자동차의 현재 위치 + 자동차가 바라보는 방향을 기준으로 한 오프셋
        Vector3 targetPosition = target.position + target.TransformDirection(offset);

        // 2. 부드러운 위치 이동 (Lerp 사용)
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // 👇 3. 부드러운 회전 (수정됨: 자동차 정중앙이 아닌 보정된 위치를 바라봄)
        // 자동차의 위치에서 lookOffset(위쪽)만큼 더한 허공의 지점을 쳐다보게 계산합니다.
        Vector3 focusPoint = target.position + target.TransformDirection(lookOffset);
        Vector3 lookDirection = focusPoint - transform.position;

        if (lookDirection != Vector3.zero)
        {
            // 목표 회전값 계산 후 부드럽게 회전
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        }
    }
}