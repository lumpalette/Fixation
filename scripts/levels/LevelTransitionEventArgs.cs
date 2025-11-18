using System;

namespace Fixation.Levels;

/// <summary>
/// Provides data for the <see cref="LevelManager.LevelLoadRequested"/> and <see cref="LevelManager.LevelUnloadRequested"/> events. This class cannot be inherited.
/// </summary>
public sealed class LevelTransitionEventArgs : EventArgs
{
	/// <summary>
	/// Creates a new <see cref="LevelTransitionEventArgs"/> object using the given level.
	/// </summary>
	/// <param name="level">The level, subject of the event being fired.</param>
	public LevelTransitionEventArgs(Level level)
	{
		Level = level;
		FilePath = level.SceneFilePath;
	}

	/// <summary>
	/// A reference to the level node. This property is read-only.
	/// </summary>
	public Level Level { get; }

	/// <summary>
	/// The absolute path where the level scene file resource is located. This property is read-only.
	/// </summary>
	public string FilePath { get; }
}
