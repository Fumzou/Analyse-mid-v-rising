using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime;
using Il2CppSystem.Reflection;
using ProjectM;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;

namespace KindredExtract;

public static class ECSExtensions
{
	public unsafe static void Write<T>(this Entity entity, T componentData) where T : struct
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		ComponentType val = default(ComponentType);
		((ComponentType)(ref val))._002Ector(Il2CppType.Of<T>(), (AccessMode)0);
		byte[] array = StructureToByteArray(componentData);
		int num = Marshal.SizeOf<T>();
		fixed (byte* ptr = array)
		{
			EntityManager entityManager = Core.TheWorld.EntityManager;
			((EntityManager)(ref entityManager)).SetComponentDataRaw(entity, val.TypeIndex, (void*)ptr, num);
		}
	}

	public static byte[] StructureToByteArray<T>(T structure) where T : struct
	{
		int num = Marshal.SizeOf(structure);
		byte[] array = new byte[num];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.StructureToPtr(structure, intPtr, fDeleteOld: true);
		Marshal.Copy(intPtr, array, 0, num);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}

	public unsafe static T Read<T>(this Entity entity) where T : struct
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		ComponentType val = default(ComponentType);
		((ComponentType)(ref val))._002Ector(Il2CppType.Of<T>(), (AccessMode)0);
		if (((ComponentType)(ref val)).IsZeroSized)
		{
			return new T();
		}
		EntityManager entityManager = Core.TheWorld.EntityManager;
		return Marshal.PtrToStructure<T>(new IntPtr(((EntityManager)(ref entityManager)).GetComponentDataRawRO(entity, val.TypeIndex)));
	}

	public static bool Has<T>(this Entity entity)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		ComponentType val = default(ComponentType);
		((ComponentType)(ref val))._002Ector(Il2CppType.Of<T>(), (AccessMode)0);
		EntityManager entityManager = Core.TheWorld.EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent(entity, val);
	}

	public static string LookupName(this PrefabGUID prefabGuid)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		PrefabCollectionSystem existingSystemManaged = Core.TheWorld.GetExistingSystemManaged<PrefabCollectionSystem>();
		NativeParallelHashMap<PrefabGUID, Entity> guidToEntityMap = existingSystemManaged._PrefabLookupMap.GuidToEntityMap;
		if (!guidToEntityMap.ContainsKey(prefabGuid))
		{
			return "GUID Not Found";
		}
		PrefabLookupMap prefabLookupMap = existingSystemManaged._PrefabLookupMap;
		return ((PrefabLookupMap)(ref prefabLookupMap)).GetName(prefabGuid) + " PrefabGuid(" + ((PrefabGUID)(ref prefabGuid)).GuidHash + ")";
	}

	public static void Add<T>(this Entity entity)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		ComponentType val = default(ComponentType);
		((ComponentType)(ref val))._002Ector(Il2CppType.Of<T>(), (AccessMode)0);
		EntityManager entityManager = Core.TheWorld.EntityManager;
		((EntityManager)(ref entityManager)).AddComponent(entity, val);
	}

	public static void Remove<T>(this Entity entity)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		ComponentType val = default(ComponentType);
		((ComponentType)(ref val))._002Ector(Il2CppType.Of<T>(), (AccessMode)0);
		EntityManager entityManager = Core.TheWorld.EntityManager;
		((EntityManager)(ref entityManager)).RemoveComponent(entity, val);
	}

	public static string GetComponentString(this Entity entity)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = Core.TheWorld.EntityManager;
		NativeArray<ComponentType> componentTypes = ((EntityManager)(ref entityManager)).GetComponentTypes(entity, (Allocator)2);
		IEnumerable<string> values = ((IEnumerable<ComponentType>)componentTypes.ToArray()).Select((ComponentType type) => ((MemberInfo)TypeManager.GetType(type.TypeIndex)).Name);
		componentTypes.Dispose();
		return string.Join("+", values);
	}
}
