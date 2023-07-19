namespace GhostBuster
{
	public static class Util
	{
		public static void Log(string msg)
		{
			if (GhostBuster.Debug) GhostBuster.Instance.ModHelper.Console.WriteLine(msg, OWML.Common.MessageType.Info);
		}

		public static void LogError(string msg)
		{
			GhostBuster.Instance.ModHelper.Console.WriteLine("Error: " + msg, OWML.Common.MessageType.Error);
		}
	}
}
