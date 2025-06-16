using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable.Schematics;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace ProjectMER.Features.Serializable;

public class MapSchematic
{
	public MapSchematic() { }

	public MapSchematic(string mapName)
	{
		Name = mapName;
	}

	public string Name;

	public bool IsDirty;

	public Dictionary<string, SerializablePrimitive> Primitives { get; set; } = new Dictionary<string, SerializablePrimitive>();

	public Dictionary<string, SerializableLight> Lights { get; set; } = new Dictionary<string, SerializableLight>();

	public Dictionary<string, SerializableDoor> Doors { get; set; } = new Dictionary<string, SerializableDoor>();

	public Dictionary<string, SerializableWorkstation> Workstations { get; set; } = new Dictionary<string, SerializableWorkstation>();

	public Dictionary<string, SerializableItemSpawnpoint> ItemSpawnpoints { get; set; } = new Dictionary<string, SerializableItemSpawnpoint>();

	public Dictionary<string, SerializablePlayerSpawnpoint> PlayerSpawnpoints { get; set; } = new Dictionary<string, SerializablePlayerSpawnpoint>();

	public Dictionary<string, SerializableCapybara> Capybaras { get; set; } = new Dictionary<string, SerializableCapybara>();

	public Dictionary<string, SerializableText> Texts { get; set; } = new Dictionary<string, SerializableText>();

	public Dictionary<string, SerializableScp079Camera> Scp079Cameras { get; set; } = new Dictionary<string, SerializableScp079Camera>();

	public Dictionary<string, SerializableShootingTarget> ShootingTargets { get; set; } = new Dictionary<string, SerializableShootingTarget>();

	public Dictionary<string, SerializableSchematic> Schematics { get; set; } = new Dictionary<string, SerializableSchematic>();

	public Dictionary<string, SerializableTeleport> Teleports { get; set; } = new Dictionary<string, SerializableTeleport>();

	public List<MapEditorObject> SpawnedObjects = new List<MapEditorObject>();

	public MapSchematic Merge(MapSchematic other)
	{
		Primitives.AddRange(other.Primitives);
		Lights.AddRange(other.Lights);
		Doors.AddRange(other.Doors);
		Workstations.AddRange(other.Workstations);
		ItemSpawnpoints.AddRange(other.ItemSpawnpoints);
		PlayerSpawnpoints.AddRange(other.PlayerSpawnpoints);
		Capybaras.AddRange(other.Capybaras);
		Texts.AddRange(other.Texts);
		Schematics.AddRange(other.Schematics);
		Scp079Cameras.AddRange(other.Scp079Cameras);
		ShootingTargets.AddRange(other.ShootingTargets);
		Teleports.AddRange(other.Teleports);

		return this;
	}

	public void Reload()
	{
		foreach (MapEditorObject mapEditorObject in SpawnedObjects)
			mapEditorObject.Destroy();

		SpawnedObjects.Clear();

		Primitives.ForEach(kvp => SpawnObject(kvp.Key, kvp.Value));
		Lights.ForEach(kvp => SpawnObject(kvp.Key, kvp.Value));
		Doors.ForEach(kvp =>
		{
			Door? vanillaDoor = Door.Get(kvp.Key);
			if (vanillaDoor != null)
			{
				kvp.Value.SetupDoor(vanillaDoor.Base);
				return;
			}

			SpawnObject(kvp.Key, kvp.Value);
		});
		Workstations.ForEach(kvp => SpawnObject(kvp.Key, kvp.Value));
		ItemSpawnpoints.ForEach(kvp => SpawnObject(kvp.Key, kvp.Value));
		PlayerSpawnpoints.ForEach(kvp => SpawnObject(kvp.Key, kvp.Value));
		Capybaras.ForEach(kvp => SpawnObject(kvp.Key, kvp.Value));
		Texts.ForEach(kvp => SpawnObject(kvp.Key, kvp.Value));
		Schematics.ForEach(kvp => SpawnObject(kvp.Key, kvp.Value));
		Scp079Cameras.ForEach(kvp => SpawnObject(kvp.Key, kvp.Value));
		ShootingTargets.ForEach(kvp => SpawnObject(kvp.Key, kvp.Value));
		Teleports.ForEach(kvp => SpawnObject(kvp.Key, kvp.Value));
	}

	public void SpawnObject<T>(string id, T serializableObject) where T : SerializableObject
	{
		List<Room> rooms = serializableObject.GetRooms();
		foreach (Room room in rooms)
		{
			if (serializableObject.Index < 0 || serializableObject.Index == room.GetRoomIndex())
			{
				GameObject? gameObject = serializableObject.SpawnOrUpdateObject(room);
				if (gameObject == null)
					continue;

				MapEditorObject mapEditorObject = gameObject.AddComponent<MapEditorObject>().Init(serializableObject, Name, id, room);
				SpawnedObjects.Add(mapEditorObject);
			}
		}

		ListPool<Room>.Shared.Return(rooms);
	}

	public void DestroyObject(string id)
	{
		foreach (MapEditorObject mapEditorObject in SpawnedObjects.ToList())
		{
			if (mapEditorObject.Id != id)
				continue;

			SpawnedObjects.Remove(mapEditorObject);
			mapEditorObject.Destroy();
		}
	}

	public bool TryAddElement<T>(string id, T serializableObject) where T : SerializableObject
	{
		bool dirtyPrevValue = IsDirty;
		IsDirty = true;
		
		if (Primitives.TryAdd(id, serializableObject))
			return true;

		if (Lights.TryAdd(id, serializableObject))
			return true;

		if (Doors.TryAdd(id, serializableObject))
			return true;

		if (Workstations.TryAdd(id, serializableObject))
			return true;

		if (ItemSpawnpoints.TryAdd(id, serializableObject))
			return true;

		if (PlayerSpawnpoints.TryAdd(id, serializableObject))
			return true;

		if (Capybaras.TryAdd(id, serializableObject))
			return true;

		if (Texts.TryAdd(id, serializableObject))
			return true;

		if (Schematics.TryAdd(id, serializableObject))
			return true;

		if (Scp079Cameras.TryAdd(id, serializableObject))
			return true;

		if (ShootingTargets.TryAdd(id, serializableObject))
			return true;

		if (Teleports.TryAdd(id, serializableObject))
			return true;

		IsDirty = dirtyPrevValue;
		return false;
	}

	public bool TryRemoveElement(string id)
	{
		bool dirtyPrevValue = IsDirty;
		IsDirty = true;
		
		if (Primitives.Remove(id))
			return true;

		if (Lights.Remove(id))
			return true;

		if (Doors.Remove(id))
			return true;

		if (Workstations.Remove(id))
			return true;

		if (ItemSpawnpoints.Remove(id))
			return true;

		if (PlayerSpawnpoints.Remove(id))
			return true;

		if (Capybaras.Remove(id))
			return true;

		if (Texts.Remove(id))
			return true;

		if (Schematics.Remove(id))
			return true;

		if (Scp079Cameras.Remove(id))
			return true;

		if (ShootingTargets.Remove(id))
			return true;

		if (Teleports.Remove(id))
			return true;
		
		IsDirty = dirtyPrevValue;
		return false;
	}
}
