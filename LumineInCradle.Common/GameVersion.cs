using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace LumineInCradle;

public record GameVersion(int major, int minor, [CanBeNull] string revision)
{

	static GameVersion()
	{
		VersionRegex = new Regex(@"^(\d+)\.(\d+)([a-z]+)?$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
	}

	public int major { get; } = major;

	public int minor { get; } = minor;

	[CanBeNull]
	public string revision { get; } = revision;

	[NotNull]
	public static GameVersion Parse(string versionString)
	{
		var match = VersionRegex.Match(versionString);

		if (!match.Success)
		{
			throw new FormatException($"Bad game version string: \"{versionString}\"");
		}

		var major = Convert.ToInt32(match.Groups[1].Value);
		var minor = Convert.ToInt32(match.Groups[2].Value);
		var revision = match.Groups.Count == 3 + 1 ? match.Groups[3].Value : null;

		return new GameVersion(major, minor, revision);
	}

	private static readonly Regex VersionRegex;

}
