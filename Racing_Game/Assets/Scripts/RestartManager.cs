using UnityEngine;
using UnityEngine.SceneManagement; // 씬(화면)을 불러오기 위한 필수 구문!

public class RestartManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        // 혹시 게임 오버나 일시 정지 때문에 시간이 멈춰(0)있었다면, 다시 정상 속도(1)로 돌려놓습니다.
        // (이 코드가 없으면 재시작해도 화면이 멈춰있는 버그가 자주 생깁니다!)
        Time.timeScale = 1f;

        // 현재 실행 중인 씬의 이름을 가져옵니다.
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 그 씬을 처음부터 다시 불러옵니다.
        SceneManager.LoadScene(currentSceneName);

        Debug.Log("게임을 재시작합니다!");
    }
}