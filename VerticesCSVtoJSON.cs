using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VerticesCSVtoJSON : MonoBehaviour
{

    float[] CSVLineToCoords (string line) {
        string[] stringRes = line.Trim().Split(',');
        float[] res = new float[stringRes.Length];
        for (int i = 0; i < stringRes.Length; i++) {
            res[i] = float.Parse(stringRes[i].Trim());
        }
        return res;
    }

    PolygonShape VerticesCSVToPolygonObj (string filename) {
        string csvPath = Application.dataPath + "/Data/Shapes/CSV/" + filename + ".csv";
        string jsonPath = Application.dataPath + "/Data/Shapes/JSON/" + filename + ".json";
        string csvData = File.ReadAllText(csvPath);
        string[] lines = csvData.Split('\n');
        int n = lines.Length;
        float[] formattedLine;
        List<Vector2> coords = new List<Vector2>();
        for (int i = 0; i < n; i++) {
            formattedLine = CSVLineToCoords(lines[i]);
            coords.Add(new Vector2(formattedLine[0], formattedLine[1]));
        }
        PolygonShape polygon = new PolygonShape(coords);
        return polygon;
    }

    void Start () {
        PolygonShape polygon = VerticesCSVToPolygonObj("triangle");
        gameObject.GetComponent<MeshMaker>().RedoMesh(polygon.coordinates);
    }

}
