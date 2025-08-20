using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigator : MonoBehaviour
{
    public void Load(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
        else
            Debug.LogWarning($"Índice fuera de rango: {sceneIndex}");
    }
}
