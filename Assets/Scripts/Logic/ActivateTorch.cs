using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
public class ActivateTorch : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Mesh enableMesh;
        [SerializeField] private Mesh disableMesh;
        [SerializeField] private new Light light;
        public bool isEnable;
        private MeshFilter _meshFilter;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isEnable)
            {
                isEnable = true;
                _meshFilter.mesh = enableMesh;
                light.enabled = true;
            }
            else
            {
                isEnable = false;
                _meshFilter.mesh = disableMesh;
                light.enabled = false;
            }
            EventManager.SendGateStateChange();
        }
    }

