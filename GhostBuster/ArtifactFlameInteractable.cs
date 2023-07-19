using System;
using UnityEngine;

namespace GhostBuster
{
	public class ArtifactFlameInteractable : MonoBehaviour
	{
		public const float EXTINGUISH_TIME = 0.5f;

		private GhostBrain _ghost;
		private string _name;

		private Light _light1;
		private Light _light2;
		private GameObject _flame;

		private bool _extinguishing = false;
		private float _timer = 0f;

		private float _light1StartIntensity;
		private float _light2StartIntensity;

		private InteractReceiver _interactReceiver;

		public void Start()
		{
			try
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
				UpdatePrompt();
				_interactReceiver.SetInteractionEnabled(true);

				_interactReceiver.OnPressInteract += Extinguish;

				GhostBuster.Instance.ShowNamesChanged.AddListener(UpdatePrompt);

				if (_ghost == null)
				{
					FinishExtinguish();
				}
			}
			catch (Exception ex)
			{
				Util.Log($"Problem with {name}: {ex.Message}, {ex.StackTrace}");
				FinishExtinguish();
			}
		}

		public void OnDestroy()
		{
			_interactReceiver.OnPressInteract -= Extinguish;
			GhostBuster.Instance.ShowNamesChanged.RemoveListener(UpdatePrompt);
		}

		private void UpdatePrompt()
		{
			if (GhostBuster.ShowNames)
			{
				var prompt = $"{UITextLibrary.GetString(UITextType.RoastingExtinguishPrompt)} {_name}'s {UITextLibrary.GetString(UITextType.ItemSimpleLanternPrompt)}";
				_interactReceiver._screenPrompt.SetText("<CMD> " + prompt);
				_interactReceiver._noCommandIconPrompt.SetText(prompt);
				_interactReceiver.ResetInteraction();
			}
			else
			{
				_interactReceiver.SetPromptText(UITextType.RoastingExtinguishPrompt);
			}
		}

		public void LinkToGhostBird(GhostBrain ghost, string ghostName)
		{
			_ghost = ghost;
			_name = ghostName;
		}

		public void Extinguish()
		{
			// If it has no connected ghost do it instantly
			if (_ghost == null)
			{
				FinishExtinguish();
			}
			else
			{
				_extinguishing = true;

				Locator.GetPlayerAudioController().PlayMarshmallowBlowOut();

				_ghost.Die();

				_extinguishing = true;

				_interactReceiver.ResetInteraction();
				_interactReceiver.DisableInteraction();

				Util.Log($"Extinguishing {_name}");
			}
		}

		private void FinishExtinguish()
		{
			_extinguishing = false;

			if (_flame) _flame.SetActive(false);
			if (_light1) _light1.gameObject.SetActive(false);
			if (_light2) _light2.gameObject.SetActive(false);

			if (_interactReceiver)
			{
				_interactReceiver.ResetInteraction();
				_interactReceiver.DisableInteraction();
			}

			enabled = false;
		}

		public void Update()
		{
			if (_extinguishing)
			{
				_timer += Time.deltaTime;

				var t = (1f - _timer / EXTINGUISH_TIME);
				_flame.transform.localScale = Vector3.one * t;
				_light1.intensity = _light1StartIntensity * t;
				_light2.intensity = _light2StartIntensity * t;

				if (_timer > EXTINGUISH_TIME)
				{
					FinishExtinguish();
				}
			}
		}
	}
}
