using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GhostBuster
{
    public class ArtifactFlameInteractable : MonoBehaviour
    {
        public const float EXTINGUISH_TIME = 0.5f;

        private GhostBrain _ghost;

        private Light _light1;
        private Light _light2;
        private GameObject _flame;

        private bool _extinguishing = false;
        private float _timer = 0f;

        private float _light1StartIntensity;
        private float _light2StartIntensity;

        private InteractReceiver _interactReceiver;

        private void Start()
        {
            _light1 = transform.Find("Prefab_IP_SleepingMummy_v2/Pointlight_IP_Mummy").GetComponent<Light>();
            _light2 = transform.Find("Prefab_IP_SleepingMummy_v2/Mummy_IP_ArtifactAnim/ArtifactPivot/Pointlight_IP_MummyArtifact").GetComponent<Light>();
            _flame = transform.Find("Prefab_IP_SleepingMummy_v2/Mummy_IP_ArtifactAnim/ArtifactPivot/Flame").gameObject;

            _light1StartIntensity = _light1.intensity;
            _light2StartIntensity = _light2.intensity; 

            GameObject interactObject = new GameObject("InteractReceiver");
            interactObject.transform.parent = transform;
            interactObject.transform.position = _flame.transform.position;
            interactObject.layer = LayerMask.NameToLayer("Interactible");

            var sphere = interactObject.AddComponent<SphereCollider>();
            sphere.radius = 0.3f;
            interactObject.AddComponent<OWCollider>();
            
            _interactReceiver = interactObject.AddComponent<InteractReceiver>();
            _interactReceiver.SetPromptText(UITextType.RoastingExtinguishPrompt);
            _interactReceiver.SetInteractionEnabled(true);

            _interactReceiver.OnPressInteract += Extinguish;

            if(_ghost == null)
            {
                Extinguish();
            }
        }

        private void OnDestroy()
        {
            _interactReceiver.OnPressInteract -= Extinguish;
        }

        public void LinkToGhostBird(GhostBrain ghost)
        {
            _ghost = ghost;
        }

        public void Extinguish()
        {
            _extinguishing = true;

            // If it has no connected ghost do it instantly
            if (_ghost == null) 
            {
                _timer = EXTINGUISH_TIME;
            }
            else
            {
                Locator.GetPlayerAudioController().PlayMarshmallowBlowOut();

                _ghost.Die();

                _extinguishing = true;

                _interactReceiver.ResetInteraction();
                _interactReceiver.DisableInteraction();
            }
        }

        private void Update()
        {
            if(_extinguishing)
            {
                _timer += Time.deltaTime;

                var t = (1f - _timer / EXTINGUISH_TIME);
                _flame.transform.localScale = Vector3.one * t;
                _light1.intensity = _light1StartIntensity * t;
                _light2.intensity = _light2StartIntensity * t;

                if(_timer > EXTINGUISH_TIME)
                {
                    _extinguishing = false;

                    _flame.SetActive(false);
                    _light1.gameObject.SetActive(false);
                    _light2.gameObject.SetActive(false);

                    base.enabled = false;
                }
            }
        }
    }
}
