using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void StartGame()
    {
        // 1. 씬이 로드되었을 때 실행할 함수(OnSceneLoaded)를 이벤트에 등록합니다.
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 2. 씬을 로드합니다.
        SceneManager.LoadScene("Scenes/Stage1");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 3. 씬 전환이 완전히 완료된 직후 유니티가 이 함수를 호출합니다.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 4. 현재 Scene이 Stage인지 확인
        if (scene.name == "Stage1")
        {
            // 5. 각종 데이터 (tileData, graph, unit, spdSortUnit..) 로드 
            GameManager.Instance.StageSetUp();
            
            // 6. 이제 씬에 타일맵이 존재하므로 안전하게 배틀 상태로 전환합니다.
            GameManager.Instance.UpdateGameState(GameState.Battle);

            // 7. 이벤트가 계속 남아있으면 다음 씬 로드 시 또 호출되므로 등록을 해제합니다.
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
