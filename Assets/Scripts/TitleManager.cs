using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
            SceneManager.sceneLoaded -= OnSceneLoaded; // 중복 방지를 위해 먼저 해제
            GameManager.Instance.StartCoroutine(GameManager.Instance.InitializeStageSequence());
        }
    }
}
