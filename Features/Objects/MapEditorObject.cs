using LabApi.Features.Wrappers;
using MEC;
using ProjectMER.Features.Serializable;
using ProjectMER.Features.ToolGun;
using UnityEngine;

namespace ProjectMER.Features.Objects;

public class MapEditorObject : MonoBehaviour
{
	public SerializableObject Base;

	public string MapName { get; protected set; }

	public string Id { get; protected set; }

	public Room Room { get; protected set; }

	public MapSchematic Map => MapUtils.LoadedMaps[MapName];

	public MapEditorObject Init(SerializableObject serializableObject, string mapName, string id, Room room)
	{
		Base = serializableObject;
		MapName = mapName;
		Id = id;
		Room = room;

		return this;
	}

	public virtual void UpdateObjectAndCopies()
	{
		Map.IsDirty = true;

		if (Base.RequiresReloading)
		{
			Base._prevIndex = Base.Index;
			Player? player = ToolGunHandler.PlayerSelectedObjectDict.FirstOrDefault(x => x.Value.Id == Id).Key;

			if (MapUtils.LoadedMaps.TryGetValue(MapName, out MapSchematic map))
			{
				map.DestroyObject(Id);
			}

			Timing.CallDelayed(0.1f, () =>
			{
				map.SpawnObject(Id, Base);

				if (player is not null)
					ToolGunHandler.SelectObject(player, Map.SpawnedObjects.Find(x => x.Id == Id));
			});

			return;
		}

		foreach (var copy in Map.SpawnedObjects.ToList().Where(copy => copy.Id == Id))
		{
			copy.UpdateCopy();
		}
	}

	private void UpdateCopy()
	{
		IndicatorObject.TrySpawnOrUpdateIndicator(this);
		Base.SpawnOrUpdateObject(Room, gameObject);
	}

	/// <summary>
	/// Destroys the object.
	/// </summary>
	public void Destroy()
	{
		IndicatorObject.TryDestroyIndicator(this);
		Destroy(gameObject);
	}
}
