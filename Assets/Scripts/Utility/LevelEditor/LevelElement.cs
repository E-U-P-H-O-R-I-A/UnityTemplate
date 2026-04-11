using Data.Scheme.Public;
using UnityEngine;

namespace Utility.LevelEditor
{
    public class LevelElement : MonoBehaviour
    {
        [SerializeField] private LevelElementType levelElementType;

        [Space] 
        
        [SerializeField] private Renderer renderer;
        [SerializeField] private Collider collider;

        private string elementID;
        private string materialID;

        public string ElementID => elementID;
        public string MaterialID => materialID;
        public Renderer Renderer => renderer;
        public LevelElementType LevelElementType => levelElementType;

        public void SetElementID(string id)
        {
            elementID = id;
        }
        
        public void SetStatusCollider(bool value)
        {
            collider.enabled = value;
        }

        public void SetMaterial(string id, Material material)
        {
            if (material == null || renderer == null)
                return;

            Material[] materials = renderer.sharedMaterials;
            if (materials == null || materials.Length == 0)
                return;

            materials[^1] = material;
            renderer.sharedMaterials = materials;
            materialID = id;
        }
    }
}
