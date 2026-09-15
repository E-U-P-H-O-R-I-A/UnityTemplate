using UnityEngine;
using UnityEngine.Serialization;

namespace Tools.LevelEditor
{
    public class LevelElement : MonoBehaviour
    {
        [SerializeField] private LevelElementType levelElementType;

        [Space] 
        
        [FormerlySerializedAs("renderer")]
        [SerializeField] private Renderer elementRenderer;
        [FormerlySerializedAs("collider")]
        [SerializeField] private Collider elementCollider;

        private string elementID;
        private string materialID;

        public string ElementID => elementID;
        public string MaterialID => materialID;
        public Renderer Renderer => elementRenderer;
        public LevelElementType LevelElementType => levelElementType;

        public void SetElementID(string id)
        {
            elementID = id;
        }
        
        public void SetStatusCollider(bool value)
        {
            elementCollider.enabled = value;
        }

        public void SetMaterial(string id, Material material)
        {
            if (material == null || elementRenderer == null)
                return;

            Material[] materials = elementRenderer.sharedMaterials;
            if (materials == null || materials.Length == 0)
                return;

            materials[^1] = material;
            elementRenderer.sharedMaterials = materials;
            materialID = id;
        }
    }
}
