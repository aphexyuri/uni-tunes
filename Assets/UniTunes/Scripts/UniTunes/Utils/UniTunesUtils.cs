using System.IO;

public static class UniTunesUtils
{
		public static string GetSCConfigPath()
		{
				return Path.Combine(Path.Combine("Assets", "UniTunes"), "SCConfig.asset");

		}
}
