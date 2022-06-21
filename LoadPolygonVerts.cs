using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Might not be able to import multidimensional arrays from JSON.
// Maybe look into later (SimpleJSON?).
// But for now we'll work around (...) with objects and stuff.

public class LoadPolygonVerts : MonoBehaviour
{

    string GetShapeDataPath (string filename, int format) {
        string shapesPath = Application.dataPath + "/Data/Shapes/";
        if (format == 0) {
            return shapesPath + "JSON/" + filename + ".json";
        } else {
            return shapesPath + "CSV/" + filename + ".csv";
        }
    }

    float[] CSVLineToCoords (string line) {
        string[] stringRes = line.Trim().Split(',');
        float[] res = new float[stringRes.Length];
        for (int i = 0; i < stringRes.Length; i++) {
            res[i] = float.Parse(stringRes[i].Trim());
        }
        return res;
    }

    PolygonShape JSONToShape (string filename) {
        string path = GetShapeDataPath(filename, 0);
        using (StreamReader r = new StreamReader(path)) {
            string JSONString = r.ReadToEnd();
            return JsonUtility.FromJson<PolygonShape>(JSONString);
        }
    }

    PolygonShape CSVToShape (string filename) {
        string csvPath = GetShapeDataPath(filename, 1);
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

    void ShapeToJSON (string filename, PolygonShape polygon) {
        string jsonPath = GetShapeDataPath(filename, 0);
        string jsonString = JsonUtility.ToJson(polygon, true);
        File.WriteAllText(jsonPath, jsonString);
    }

    void CSVToJSON (string filename) {
        PolygonShape polygon = CSVToShape(filename);
        ShapeToJSON(filename, polygon);
    }

    void ShapeToCSV (string filename, PolygonShape polygon) {
        string csvPath = GetShapeDataPath(filename, 1);
        string toWrite = "";
        int n = polygon.coordinates.Count;
        for (int i = 0; i < n; i++) {
            toWrite += polygon.coordinates[i].x.ToString() + ", " + polygon.coordinates[i].y.ToString();
            if (i != n - 1) {
                toWrite += "\n";
            }
        }
        File.WriteAllText(csvPath, toWrite);
    }

    void Start () {
    }

}
