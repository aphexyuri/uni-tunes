using System.IO;

public static class UniTunesUtils
{
		public static string GetSCConfigPath()
		{
				return Path.Combine(Path.Combine(Path.Combine("Assets", "Scripts"), "UniTunes"), "SCConfig.asset");

		}
}
