using OWML.Common;
using OWML.ModHelper;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GhostBuster
{
	public class GhostBuster : ModBehaviour
	{
		private static readonly string zone1Root = "RingWorld_Body/Sector_RingInterior/Sector_Zone1/Sector_DreamFireHouse_Zone1/Interactables_DreamFireHouse_Zone1/DreamFireChamber/MummyCircle";
		private static readonly string[] zone1Mummies = new string[]
		{
			"PAZUZU",
			"BAHOMET",
			"StoryBlue",
			"EBRIETAS",
			"ENKITWO",
			"ENKIDU",
			"7",
			"GILGAMESH",
			"AMYGDALA",
			"GERHMAN",
			"MICOLASH"
		};

		private static readonly string zone2Root = "RingWorld_Body/Sector_RingInterior/Sector_Zone2/Sector_DreamFireLighthouse_Zone2_AnimRoot/Interactibles_DreamFireLighthouse_Zone2/DreamFireChamber/MummyCircle";
		private static readonly string[] zone2Mummies = new string[]
		{
			"FALSEKNIGHT",
			"1",
			"COLLECTOR",
			"3",
			"ZOTE",
			"6",
			"StoryBlack",
			"StoryGreen",
			"HORNET",
			"NOSK",
			"11"
		};

		private static readonly string zone3Root = "RingWorld_Body/Sector_RingInterior/Sector_Zone3/Sector_HiddenGorge/Sector_DreamFireHouse_Zone3/Interactables_DreamFireHouse_Zone3/DreamFireChamber_DFH_Zone3/MummyCircle";
		private static readonly string[] zone3Mummies = new string[]
		{
			"StoryYellow",
			"KAMAJI",
			"StoryBlack",
			"3",
			"NOFACE",
			"KAONASHI",
			"7",
			"8",
			"BOU",
			"YUBABA",
			"11"
		};

		private static Dictionary<string, GhostBrain> _brainsDict;

		public static GhostBuster Instance { get; private set; }

		public static bool Debug { get; private set; }
		public static bool ShowNames { get; private set; }

		public UnityEvent ShowNamesChanged;

		public void Start()
		{
			Instance = this;

			Util.Log($"GhostBuster loaded");

			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public override void Configure(IModConfig config)
		{
			Debug = config.GetSettingsValue<bool>("Debug Logs");

			var wasShowingNames = ShowNames;
			ShowNames = config.GetSettingsValue<bool>("Show Names");
			if (wasShowingNames != ShowNames)
			{
				ShowNamesChanged?.Invoke();
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.name != "SolarSystem") return;

			Util.Log("Collecting brains");

			_brainsDict = new Dictionary<string, GhostBrain>();
			foreach (var brain in GameObject.FindObjectsOfType<GhostBrain>())
			{
				var name = brain.name.Split(new char[] { '_' }).Last().ToUpper();
				_brainsDict.Add(name, brain);
			}

			AddInteractables(zone1Mummies, zone1Root);
			AddInteractables(zone2Mummies, zone2Root);
			AddInteractables(zone3Mummies, zone3Root);

			if (_brainsDict.Count > 0)
			{
				Util.LogError($"Couldn't find mummies for {_brainsDict.Count} ghosts:");
				foreach (var brain in _brainsDict.Keys)
				{
					Util.LogError($"{brain}");
				}
			}
		}

		private void AddInteractables(string[] mummies, string root)
		{
			foreach (var ghostBird in mummies)
			{
				// Regardless of if they start dead or alive, put the component since it will extinguish the lamps from dead ones
				var mummy = GameObject.Find(root + $"/MummyPivot ({ghostBird})");
				var interactable = mummy.AddComponent<ArtifactFlameInteractable>();

				// They rly mispelled Gerhman's name or is it Gehrman?
				var brainName = ghostBird;
				if (ghostBird == "GERHMAN") brainName = "GEHRMAN";

				// Remove them from the dictionary so we can be sure we got them all in the end.
				if (_brainsDict.TryGetValue(brainName, out var brain))
				{
					var formattedName = brainName.Length > 1 ? brainName[0] + brainName.Substring(1).ToLower() : brainName;
					interactable.LinkToGhostBird(brain, formattedName);

					_brainsDict.Remove(brainName);

					Util.Log($"{brainName} can now be killed");
				}
				else
				{
					Util.Log($"{brainName} is already dead");
				}
			}
		}
	}
}
