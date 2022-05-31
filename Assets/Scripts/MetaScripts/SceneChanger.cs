using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetaScripts
{
    public static class SceneChanger
    {
        public static void ChangeScene(int id)
        {
            if (id >= 0 && id <= SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(id);
            else
                Debug.LogError("Id scene does not exist. Max of them is - " + SceneManager.sceneCountInBuildSettings +
                               " but was - " + id);
        }
    }
}

