using UnityEngine;

public class TrackTrigger : MonoBehaviour
{
    // 유니티 인스펙터에서 역할을 고를 수 있게 만드는 목록(Enum)
    public enum TriggerType { FinishLine, Checkpoint }

    [Header("이 투명 큐브의 역할을 선택하세요")]
    public TriggerType triggerType;

    private void OnTriggerEnter(Collider other)
    {
        // 부딪힌 물체가 자동차인지 확인
        if (other.GetComponentInParent<Controller>() != null)
        {
            // 1. 만약 이 큐브가 결승선이라면?
            if (triggerType == TriggerType.FinishLine)
            {
                TimeAttackManager.instance.FinishLap();
            }
            // 2. 만약 이 큐브가 체크포인트(반환점)라면?
            else if (triggerType == TriggerType.Checkpoint)
            {
                TimeAttackManager.instance.PassCheckpoint();
            }
        }
    }
}