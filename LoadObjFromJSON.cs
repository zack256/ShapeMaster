using UnityEngine;
using System.IO;

// Might not be able to import multidimensional arrays from JSON.
// Maybe look into later (SimpleJSON?).
// But for now we'll work around (...) with objects and stuff.

public class LoadObjFromJSON : MonoBehaviour
{
    PolygonShape LoadPolygonShapeFromJSONFile (string filename) {
        string path = Application.dataPath + "/Data/Shapes/" + filename + ".json";
        using (StreamReader r = new StreamReader(path)) {
            string JSONString = r.ReadToEnd();
            return JsonUtility.FromJson<PolygonShape>(JSONString);
        }
    }

    void Start () {
        PolygonShape polygon = LoadPolygonShapeFromJSONFile("shapey");
        gameObject.GetComponent<MeshMaker>().RedoMesh(polygon.coordinates);
    }

}
