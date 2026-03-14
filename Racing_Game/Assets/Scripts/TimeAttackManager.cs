using UnityEngine;
using TMPro;

public class TimeAttackManager : MonoBehaviour
{
    public static TimeAttackManager instance;

    [Header("UI 텍스트 연결")]
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI bestTimeText;

    private float currentTime;
    private float bestTime;
    private bool isTimerRunning = false;

    // 👇 서킷용 핵심 변수: 맵 중간의 체크포인트를 지났는지 확인합니다.
    public bool passedCheckpoint = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        bestTime = PlayerPrefs.GetFloat("BestTime", 5999f);
        UpdateBestTimeUI();

        
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;
            currentTimeText.text = FormatTime(currentTime);
        }
    }

    // 새로운 바퀴(Lap)를 시작할 때 부르는 함수
    public void StartLap()
    {
        currentTime = 0f;
        isTimerRunning = true;
        passedCheckpoint = false; // 새 바퀴를 시작했으니 체크포인트 통과 여부 초기화
        Debug.Log("새로운 랩(Lap) 시작!");
    }

    // 체크포인트(반환점)를 통과할 때 부르는 함수
    public void PassCheckpoint()
    {
        StartLap();
        passedCheckpoint = true;
        Debug.Log("체크포인트 통과! 이제 결승선으로 가세요!");
    }

    // 결승선을 지날 때 부르는 함수
    public void FinishLap()
    {
        // 🚨 꼼수 방지: 체크포인트를 안 찍고 결승선만 밟으면 무시합니다!
        if (!passedCheckpoint)
        {
            Debug.Log("경고: 맵 중간의 체크포인트를 아직 지나지 않았습니다!");
            return;
        }

        Debug.Log("한 바퀴 완료! 기록: " + FormatTime(currentTime));

        // 최고 기록 갱신
        if (currentTime < bestTime)
        {
            bestTime = currentTime;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            PlayerPrefs.Save();
            UpdateBestTimeUI();
            Debug.Log("🎉 최고 기록 갱신! 🎉");
        }

        // 랩타임을 기록했으니, 곧바로 다음 바퀴 타이머를 처음부터 다시 시작합니다.
        StartLap();
    }

    private string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        float fraction = (time * 100) % 100;
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
    }

    private void UpdateBestTimeUI()
    {
        if (bestTime >= 5999f)
            bestTimeText.text = "Best: --:--.--";
        else
            bestTimeText.text = "Best: " + FormatTime(bestTime);
    }
}