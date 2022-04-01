using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScenesFolders.MetaScripts
{
    public class SceneChanger
    {
        public void ChangeScene(int id)
        {
            if (id >= 1 && id <= SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(id);
            else
                Debug.LogError("Id scene does not exist. Max of them is - " + SceneManager.sceneCountInBuildSettings +
                               " but was - " + id);
        }
    }
}
