using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MapGenEditor : MonoBehaviour
{
    [CustomEditor(typeof(MeshGenerator))]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MeshGenerator meshGeneratorTwo = (MeshGenerator)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                meshGeneratorTwo.GenerateMap();
            }
        }
    }
}
