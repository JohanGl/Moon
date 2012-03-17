using System.IO.IsolatedStorage;
using MoonLib.IsolatedStorage.Entities;

namespace MoonLib.IsolatedStorage
{
	public class StorageHandler
	{
		private readonly IsolatedStorageSettings settings;

		private const string challengesKey = "Challenges";
		private const string levelsKey = "Levels";

		public StorageHandler()
		{
			settings = IsolatedStorageSettings.ApplicationSettings;

			if (!settings.Contains(levelsKey))
			{
				settings.Add(levelsKey, new LevelsInfo());
			}

			if (!settings.Contains(challengesKey))
			{
				settings.Add(challengesKey, new ChallengesInfo());
			}
		}

		public bool IsLevelCompleted(int levelId)
		{
			var levels = (LevelsInfo)settings[levelsKey];
			return levels.Completed.ContainsKey(levelId);
		}

		public bool IsChallengeCompleted(int challengeId)
		{
			var challenges = (ChallengesInfo)settings[challengesKey];
			return challenges.Completed.ContainsKey(challengeId);
		}

		public void SetChallengeCompleted(int challengeId)
		{
			var challenges = (ChallengesInfo)settings[challengesKey];
			challenges.Completed[challengeId] = true;
		}
	}
}