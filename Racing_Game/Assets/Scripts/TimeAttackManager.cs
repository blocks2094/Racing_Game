using UnityEngine;
using TMPro;

public class TimeAttackManager : MonoBehaviour
{
    public static TimeAttackManager instance;

    [Header("UI 텍스트 연결")]
    public TextMeshProUGUI currentTimeText;
    // 변수 이름을 best에서 last(이전 기록)로 변경했습니다.
    public TextMeshProUGUI lastTimeText;

    private float currentTime;
    private float lastTime = 0f; // 방금 전 기록을 저장할 변수 (초기값 0)
    private bool isTimerRunning = false;

    // 서킷용 핵심 변수: 맵 중간의 체크포인트를 지났는지 확인합니다.
    public bool passedCheckpoint = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        // PlayerPrefs(영구 저장) 기능을 빼고, 시작할 때 이전 기록 UI를 기본 상태로 둡니다.
        UpdateLastTimeUI();
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
        // 🚨 수정됨: 여기에 있던 StartLap(); 을 지웠습니다. (반환점에서 시간이 0으로 리셋되는 버그 해결)
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

        Debug.Log("한 바퀴 완료! 방금 기록: " + FormatTime(currentTime));

        // 👇 수정됨: 최고 기록 갱신 로직을 지우고, 방금 들어온 시간을 무조건 이전 기록(lastTime)으로 덮어씁니다.
        lastTime = currentTime;
        UpdateLastTimeUI();

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

    private void UpdateLastTimeUI()
    {
        // 완주한 기록이 한 번도 없을 때
        if (lastTime == 0f)
            lastTimeText.text = "Last: --:--.--";
        // 방금 완주한 기록이 있을 때
        else
            lastTimeText.text = "Last: " + FormatTime(lastTime);
    }
}