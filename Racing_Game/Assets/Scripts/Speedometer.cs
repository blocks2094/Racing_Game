using UnityEngine;
using TMPro; // TextMeshPro(UI 텍스트) 사용을 위한 필수 구문

public class Speedometer : MonoBehaviour
{
    [Header("연결 설정")]
    public Rigidbody carRigidbody;      // 자동차의 Rigidbody (속도를 가져오기 위함)
    public TextMeshProUGUI speedText;   // 속도를 표시할 텍스트
    public RectTransform needle;        // 회전할 바늘 이미지 (UI)

    [Header("속도계 설정")]
    public float maxSpeed = 200f;       // 계기판의 최대 속도 (km/h)

    [Header("바늘 각도 설정 (Z축)")]
    public float minSpeedAngle = 90f;   // 0 km/h 일 때 바늘의 각도 (기본: 왼쪽)
    public float maxSpeedAngle = -90f;  // 최대 속도일 때 바늘의 각도 (기본: 오른쪽, 시계방향은 숫자가 작아짐)

    [Header("바늘 애니메이션")]
    public float needleSmoothing = 10f; // 바늘이 움직이는 부드러움 정도 (낮을수록 천천히 따라감)

    private float currentNeedleAngle;

    void Start()
    {
        // 시작할 때 바늘을 최소 각도(0km/h 위치)로 세팅
        currentNeedleAngle = minSpeedAngle;
    }

    void Update()
    {
        if (carRigidbody == null) return;

        // 1. 현재 자동차 속도 계산 (km/h 단위)
        float speed = carRigidbody.velocity.magnitude * 3.6f;

        // 2. 텍스트 업데이트 (소수점 버리고 정수로만 표시)
        if (speedText != null)
        {
            speedText.text = Mathf.RoundToInt(speed).ToString();
        }

        // 3. 바늘 회전 애니메이션
        if (needle != null)
        {
            // 속도를 0 ~ 최대 속도(maxSpeed) 사이로 제한 (바늘이 한 바퀴를 넘어가는 것 방지)
            float clampedSpeed = Mathf.Clamp(speed, 0f, maxSpeed);

            // 현재 속도가 최대 속도의 몇 %인지 비율 계산 (0.0 ~ 1.0)
            float speedNormalized = clampedSpeed / maxSpeed;

            // 해당 비율에 맞춰서 바늘이 가리켜야 할 목표 각도 계산
            float targetAngle = Mathf.Lerp(minSpeedAngle, maxSpeedAngle, speedNormalized);

            // 바늘이 팍팍 꺾이지 않고 부드럽게 목표 각도로 이동하도록 처리
            currentNeedleAngle = Mathf.Lerp(currentNeedleAngle, targetAngle, Time.deltaTime * needleSmoothing);

            // 바늘 UI의 Z축 회전값 적용
            needle.eulerAngles = new Vector3(0, 0, currentNeedleAngle);
        }
    }
}