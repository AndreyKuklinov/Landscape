using UnityEngine;

namespace MainGame
{
    public class MaterialHandler : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;
        private Material _defaultMaterial;
    
        public void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _defaultMaterial = _meshRenderer.material;
        }

        public void SetMaterial(Material material)
        {
            _meshRenderer.material = material;
        }

        public void ResetMaterial()
        {
            _meshRenderer.material = _defaultMaterial;
        }
    }
}
