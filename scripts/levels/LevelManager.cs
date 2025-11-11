using Godot;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fixation.Levels;

/// <summary>
/// A global node responsible for loading and managing levels at runtime. This class cannot be inherited.
/// </summary>
public partial class LevelManager : Node
{
	/// <summary>
	/// Occurs when the user requests that a level be loaded to the scene tree at the end of the frame.
	/// </summary>
	public static event EventHandler<LevelTransitionEventArgs> LevelLoadRequested;

	/// <summary>
	/// Occurs when the user requests that a level be unloaded from the scene tree at the end of the frame.
	/// </summary>
	public static event EventHandler<LevelTransitionEventArgs> LevelUnloadRequested;

	/// <summary>
	/// Number of levels currently loaded, including disabled ones. This property is read-only.
	/// </summary>
	public static int LoadedLevelCount => s_instance._loadedLevels.Count;

	/// <summary>
	/// Loads a level from a packed scene resource.
	/// </summary>
	/// <param name="scene">The packed scene containing the level to load.</param>
	/// <exception cref="InvalidCastException">Thrown when <paramref name="scene"/> doesn't contain a level.</exception>
	public static void Load(PackedScene scene)
	{
		Level level = scene.Instantiate<Level>();

		// Allow level reloading.
		if (IsLoaded(level.SceneFilePath, out Level previousLevel))
		{
			s_instance.RequestLevelRemoval(previousLevel);
		}

		s_instance.RequestLevelAddition(level);
	}

	/// <summary>
	/// Loads a level from the given resource path.
	/// </summary>
	/// <param name="path">The absolute resource path of the packed scene containing the level to load.</param>
	/// <exception cref="FileNotFoundException">Thrown when the resource file at <paramref name="path"/> doesn't exist.</exception>
	/// <exception cref="InvalidCastException">Thrown either when the resource is not a <see cref="PackedScene"/> or when it doesn't contain a level.</exception>
	public static void Load(string path)
	{
		if (!ResourceLoader.Exists(path))
		{
			throw new FileNotFoundException($"Level file not found at path '{path}'", path);
		}

		Level currentLevel;
		if (s_instance._cachedLevelScenes.TryGetValue(path, out PackedScene cachedScene))
		{
			// Allow level reloading.
			if (IsLoaded(path, out Level previousLevel))
			{
				s_instance.RequestLevelRemoval(previousLevel);
			}
		}
		else
		{
			cachedScene = ResourceLoader.Load<PackedScene>(path, cacheMode: ResourceLoader.CacheMode.IgnoreDeep);
			s_instance._cachedLevelScenes.Add(path, cachedScene);
		}

		currentLevel = cachedScene.Instantiate<Level>();
		s_instance.RequestLevelAddition(currentLevel);
	}

	/// <summary>
	/// Checks whether a given level is currently loaded, identified by its resource path.
	/// </summary>
	/// <param name="path">The absolute resource path of the level to check.</param>
	/// <returns><see langword="true"/> if the level is currently loaded; <see langword="false"/> otherwise.</returns>
	public static bool IsLoaded(string path)
	{
		return s_instance._loadedLevels.ContainsKey(path);
	}

	/// <summary>
	/// Checks whether a given level is currently loaded, identified by its resource path.
	/// </summary>
	/// <param name="path">The absolute resource path of the level to check.</param>
	/// <param name="level">If the level is loaded, this parameter is assigned a reference to that level.</param>
	/// <returns><see langword="true"/> if the level is currently loaded; <see langword="false"/> otherwise.</returns>
	public static bool IsLoaded(string path, out Level level)
	{
		return s_instance._loadedLevels.TryGetValue(path, out level);
	}

	/// <summary>
	/// Checks whether the given level is currently loaded.
	/// </summary>
	/// <param name="level">The level to check.</param>
	/// <returns><see langword="true"/> if the level is currently loaded; <see langword="false"/> otherwise.</returns>
	public static bool IsLoaded(Level level)
	{
		return s_instance._loadedLevels.ContainsValue(level);
	}

	/// <summary>
	/// Unloads the level located at the specified resource path.
	/// </summary>
	/// <param name="path">The absolute resource path of the level to unload.</param>
	public static void Unload(string path)
	{
		if (IsLoaded(path, out Level level))
		{
			s_instance.RequestLevelRemoval(level);
		}
	}
	
	/// <summary>
	/// Unloads the specified level.
	/// </summary>
	/// <param name="level">The level to unload.</param>
	public static void Unload(Level level)
	{
		if (IsLoaded(level))
		{
			s_instance.RequestLevelRemoval(level);
		}
	}

	public override void _Ready()
	{
		_rootNode = GetTree().CurrentScene.GetNode("Levels");

		// In the scene tree editor, we can define an initial level by adding it as a child of "Levels".
		// This ensures the level is registered correctly.
		if (_rootNode.GetChildCount() > 0)
		{
			Level startLevel = _rootNode.GetChild<Level>(0);
			_loadedLevels.Add(startLevel.SceneFilePath, startLevel);
		}
	}
	
	private LevelManager()
	{
		if (s_instance is not null)
		{
			QueueFree();
			return;
		}

		s_instance = this;

		_loadedLevels = [];
		_cachedLevelScenes = [];
	}

	private void RequestLevelAddition(Level level)
	{
		void DeferredCall()
		{
			_rootNode.AddChild(level);
			_loadedLevels.Add(level.SceneFilePath, level);
		}

		Callable.From(DeferredCall).CallDeferred();
		LevelLoadRequested?.Invoke(null, new LevelTransitionEventArgs(level));
	}

	private void RequestLevelRemoval(Level level)
	{
		void DeferredCall()
		{
			_loadedLevels.Remove(level.SceneFilePath);
			level.Free();
		}

		Callable.From(DeferredCall).CallDeferred();
		LevelUnloadRequested?.Invoke(null, new LevelTransitionEventArgs(level));
	}

	private static LevelManager s_instance;
	private Node _rootNode;
	private readonly Dictionary<string, Level>  _loadedLevels;
	private readonly Dictionary<string, PackedScene> _cachedLevelScenes;
}