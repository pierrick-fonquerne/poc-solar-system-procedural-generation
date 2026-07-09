using UnityEngine;

/// <summary>
/// Creates celestial object GameObjects (stars, planets, moons) from generated meshes.
/// </summary>
public static class CelestialObjectFactory
{
    private const string IcoSpherePrefabPath = "Prefabs/IcoSphere";

    /// <summary>
    /// Instantiates the icosphere prefab and assigns the given mesh to it.
    /// </summary>
    /// <param name="mesh">The mesh to display on the created object.</param>
    /// <param name="name">The name of the created GameObject.</param>
    /// <returns>The created GameObject, or null if the prefab could not be loaded.</returns>
    public static GameObject CreateObject(Mesh mesh, string name = "Celestial Object")
    {
        GameObject icoSpherePrefab = Resources.Load<GameObject>(IcoSpherePrefabPath);
        if (icoSpherePrefab == null)
        {
            Debug.LogError($"Failed to load the IcoSphere prefab from Resources/{IcoSpherePrefabPath}!");
            return null;
        }

        GameObject obj = Object.Instantiate(icoSpherePrefab);
        obj.name = name;
        obj.GetComponent<MeshFilter>().sharedMesh = mesh;

        return obj;
    }
}
