using System.Collections.Generic;
using System.Linq;

namespace Backend.Util.Data
{
    public static class AddressData
    {
        public const string Assets_Prefab_Sphere_Prefab = "Assets/Prefab/Sphere.prefab";
        public const string Assets_Prefab_Capsule_Prefab = "Assets/Prefab/Capsule.prefab";
        public const string Assets_Prefab_Plane_Prefab = "Assets/Prefab/Plane.prefab";
        public const string Assets_Prefab_Cube_Prefab = "Assets/Prefab/Cube.prefab";
        public const string Assets_Prefab_Cylinder_Prefab = "Assets/Prefab/Cylinder.prefab";
        public const string Assets_Prefab_Quad_Prefab = "Assets/Prefab/Quad.prefab";

        public static Dictionary<string, HashSet<string>> Groups = new()
        {
            { "aaa", new HashSet<string> { "Assets/Prefab/Sphere.prefab","Assets/Prefab/Capsule.prefab","Assets/Prefab/Plane.prefab" } },
            { "bbb", new HashSet<string> { "Assets/Prefab/Plane.prefab","Assets/Prefab/Cube.prefab","Assets/Prefab/Cylinder.prefab" } },
            { "ccc", new HashSet<string> { "Assets/Prefab/Cylinder.prefab","Assets/Prefab/Quad.prefab" } },
        };
    }
}
