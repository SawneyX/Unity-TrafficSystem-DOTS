using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using IO = System.IO;
using Debug = UnityEngine.Debug;
using JsonUtility = UnityEngine.JsonUtility;
using Application = UnityEngine.Application;
using Random = Unity.Mathematics.Random;
using MenuItem = UnityEditor.MenuItem;

public static class SerializationTester
{

    const string k_entitySerializationTestWorldName = "Default World";//nameof(SerializationTester);
    static readonly ComponentType[] _testArchetypeComponents = new ComponentType[] { typeof(Translation), typeof(Rotation), typeof(RenderMesh) };

    [MenuItem("Test/Entity Serialization/1. Create World", false, 1)]
    [MenuItem("Test/Entity Serialization/5. Create World", false, 5)]
    static void CreateEntitySerializationTestWorld() => new World(k_entitySerializationTestWorldName);

    [MenuItem("Test/Entity Serialization/2. Create Entities", false, 2)]
    static void TestEntitySerializationCreateEntities()
    {
        var world = FindWorld(k_entitySerializationTestWorldName);
        if (world == null) { Debug.Log($"world '{k_entitySerializationTestWorldName}' doesn't exist, aborted"); return; }
        var command = world.EntityManager;
        var archetype = command.CreateArchetype(_testArchetypeComponents);

        var random = new Random((uint)System.DateTime.Now.GetHashCode());
        for (int i = 0; i < 10000; i++)
        {
            var entity = command.CreateEntity(archetype);
            command.SetComponentData(entity, new Translation
            {
                Value = random.NextFloat3()
            });
            command.SetComponentData(entity, new Rotation
            {
                Value = random.NextQuaternionRotation()
            });
            /*
            command.SetComponentData(entity, new Scale
            {
                Value = random.NextFloat()
            });
            */
        }
    }

    [MenuItem("Test/Entity Serialization/3. Serialize World", false, 3)]
    static void TestEntitySerialization()
    {
        var world = FindWorld(k_entitySerializationTestWorldName);
        if (world == null) { Debug.Log($"world '{k_entitySerializationTestWorldName}' doesn't exist, aborted"); return; }
        var command = world.EntityManager;
        var archetype = command.CreateArchetype(_testArchetypeComponents);
        var query = command.CreateEntityQuery(_testArchetypeComponents);
        if (query.CalculateEntityCount() == 0) { Debug.Log($"world '{k_entitySerializationTestWorldName}' contains no matching entities, aborted"); query.Dispose(); return; }
        {
            var entities = query.ToEntityArray(Allocator.TempJob);
            string dir = Application.temporaryCachePath;
            string saveID = $"{world.Name}-save-#{1}";
            CreateSaveHeaderFile(directoryPath: dir, saveID: saveID, numEntities: entities.Length);
            SaveComponentDataToJson<Translation>(entities, command, directoryPath: dir, saveID: saveID);
            SaveComponentDataToJson<Rotation>(entities, command, directoryPath: dir, saveID: saveID);
            //SaveComponentDataToJson<ECSPathFollow>(entities, command, directoryPath: dir, saveID: saveID);
            entities.Dispose();
        }
        query.Dispose();
    }

    [MenuItem("Test/Entity Serialization/4. Dispose World", false, 4)]
    static void DisposeEntitySerializationTestWorld() => FindWorld(k_entitySerializationTestWorldName)?.Dispose();

    [MenuItem("Test/Entity Serialization/6. Deserialize", false, 6)]
    static void TestEntityDeserialization()
    {
        var world = FindWorld(k_entitySerializationTestWorldName);
        if (world == null) world = new World(k_entitySerializationTestWorldName);
        var command = world.EntityManager;
        var archetype = command.CreateArchetype(_testArchetypeComponents);

        string dir = Application.temporaryCachePath;
        string saveID = $"{world.Name}-save-#{1}";
        if (ReadSaveHeaderFile(directoryPath: dir, saveID: saveID, out var header))
        {
            var entities = command.CreateEntity(archetype, header.numEntities, Allocator.TempJob);
            {
                LoadComponentDataFromJson<Translation>(directoryPath: dir, saveID: saveID, command, entities);
                LoadComponentDataFromJson<Rotation>(directoryPath: dir, saveID: saveID, command, entities);
                //LoadComponentDataFromJson<Scale>(directoryPath: dir, saveID: saveID, command, entities);
            }
            entities.Dispose();
        }
        else Debug.LogWarning($"no header fle found {GetHeaderFileName(saveID)}");
    }

    static World FindWorld(string name)
    {
        var worlds = World.All;
        for (int i = 0; i < worlds.Count; i++)
            if (worlds[i].Name == name) return worlds[i];
        return null;
    }

    static void CreateSaveHeaderFile(string directoryPath, string saveID, int numEntities)
    {
        string fileDir = IO.Path.Combine(directoryPath, saveID);
        string filePath = IO.Path.Combine(fileDir, GetHeaderFileName(saveID));
        if (IO.Directory.Exists(fileDir))
            IO.Directory.Delete(fileDir, true);
        if (!IO.Directory.Exists(fileDir))
            IO.Directory.CreateDirectory(fileDir);
        string json = JsonUtility.ToJson(new SaveHeaderFile { numEntities = numEntities });
        IO.File.WriteAllText(filePath, json);
    }

    static bool ReadSaveHeaderFile(string directoryPath, string saveID, out SaveHeaderFile header)
    {
        string fileDir = IO.Path.Combine(directoryPath, saveID);
        string filePath = IO.Path.Combine(fileDir, GetHeaderFileName(saveID));
        if (IO.File.Exists(filePath))
        {
            string json = IO.File.ReadAllText(filePath);
            header = JsonUtility.FromJson<SaveHeaderFile>(json);
            return true;
        }
        else
        {
            header = null;
            return false;
        }
    }

    static void SaveComponentDataToJson<T>(NativeArray<Entity> entities, EntityManager entityManager, string directoryPath, string saveID)
        where T : unmanaged, IComponentData
    {
        var list = new List<T>(capacity: entities.Length);
        for (int i = 0; i < entities.Length; i++)
            list.Add(entityManager.GetComponentData<T>(entities[i]));
        var collection = new ArrayContainer<T> { array = list.ToArray() };
        string fileName = GetComponentFileName<T>(saveID);
        string fileDir = IO.Path.Combine(directoryPath, saveID);
        string filePath = IO.Path.Combine(fileDir, fileName);
        string json = JsonUtility.ToJson(collection);
        if (!IO.Directory.Exists(filePath)) IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(filePath));
        IO.File.WriteAllText(filePath, json);
        Debug.Log($"entity data saved: {filePath}\nDATA:\n{json}");
    }

    static void LoadComponentDataFromJson<T>(string directoryPath, string saveID, EntityManager entityManager, NativeArray<Entity> entities)
        where T : unmanaged, IComponentData
    {
        string fileName = GetComponentFileName<T>(saveID);
        string fileDir = IO.Path.Combine(directoryPath, saveID);
        string filePath = IO.Path.Combine(fileDir, fileName);
        if (!IO.File.Exists(filePath))
        {
            Debug.LogError($"save file not found: {filePath}");
            return;
        }
        string json = IO.File.ReadAllText(filePath);
        ArrayContainer<T> dataContainer = JsonUtility.FromJson<ArrayContainer<T>>(json);
        T[] components = dataContainer.array;
        for (int i = 0; i < components.Length; i++)
            entityManager.SetComponentData<T>(entities[i], components[i]);
        Debug.Log($"{components.Length} component data loaded: {filePath}");
    }

    static string GetHeaderFileName(string saveID) => $"{saveID}.json";
    static string GetComponentFileName<T>(string saveID) => $"{saveID}@{typeof(T).Name}.json";

    [System.Serializable] public class SaveHeaderFile { public int numEntities; }
    [System.Serializable] public class ArrayContainer<T> { public T[] array; }

}