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

        private int elementID;
        private int materialID;

        public int ElementID => elementID;
        public int MaterialID => materialID;
        public Renderer Renderer => renderer;
        public LevelElementType LevelElementType => levelElementType;

        public void SetElementID(int id)
        {
            elementID = id;
        }
        
        public void SetStatusCollider(bool value)
        {
            collider.enabled = value;
        }

        public void SetMaterial(int id, Material material)
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