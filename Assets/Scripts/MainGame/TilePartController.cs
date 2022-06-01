using MetaScripts;
using UnityEngine;

namespace MainGame
{
    public class TilePartController : MonoBehaviour, IDeactivable
    {
        public int appearanceFrequency;

        public int GetAppearanceFrequency() =>
            appearanceFrequency;

        public void Deactivate() =>
            gameObject.SetActive(false);

        public void Activate() =>
            gameObject.SetActive(true);
    }
}