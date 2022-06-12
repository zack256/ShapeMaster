using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshMaker : MonoBehaviour
{

    // Stuff might not work yet if you have a vertical slope!

    List<Vector2> triangle = new List<Vector2> {
        new Vector2(0, 0),
        new Vector2(3, 0),
        new Vector2(3, 4),
    };
    List<Vector2> convexQuadrilateral = new List<Vector2> {
        new Vector2(-3, -2),
        new Vector2(6, -2),
        new Vector2(8, 4),
        new Vector2(2, 6),
    };
    List<Vector2> kite = new List<Vector2> {
        new Vector2(-2, -3),
        new Vector2(0, 0),
        new Vector2(2, -4),
        new Vector2(0, 6),
    };
    List<Vector2> reversedKite = new List<Vector2> {
        new Vector2(-2, -3),
        new Vector2(0, 6),
        new Vector2(2, -4),
        new Vector2(0, 0),
    };
    List<float> advancedKite = new List<float> {
        0, 5,
        -5, -4,
        -3, -2,
        -1, -1,
        1, -1,
        3, -2,
        5, -4
    };
    List<float> arch = new List<float> {
        7, 8, 10, 14, 16, 11, 13.5f, 6.5f, 12, 7.5f, 13, 10, 11, 11, 9, 7
    };
    void PrintArr<T> (T[] l) {
        int n = l.Length;
        string[] pr = new string[n];
        for (int i = 0; i < n; i++) {
            pr[i] = Convert.ToString(l[i]);
        }
        print("[" + String.Join(", ", pr) + "]");
    }
    void PrintList<T> (List<T> l) {
        int n = l.Count;
        string[] pr = new string[n];
        for (int i = 0; i < n; i++) {
            pr[i] = Convert.ToString(l[i]);
        }
        print("[" + String.Join(", ", pr) + "]");
    }
    
    int Mod (int a, int b) {
        int r = a % b;
        return r < 0 ? r + b : r;
    }

    float RadiansToDegrees (float theta) {
        return (theta / Mathf.PI) * 180;
    }

    float FindAngle (float sinTheta, float cosTheta) {
        float presumedTheta = Mathf.Asin(sinTheta);
        if (cosTheta < 0) {
            return Mathf.PI - presumedTheta;
        } else {
            if (sinTheta < 0) {
                return presumedTheta + 2 * Mathf.PI;
            } else {
                return presumedTheta;
            }
        }
    }

    float CalcSlope (Vector2 A, Vector2 B) {
        return (B.y - A.y) / (B.x - A.x);
    }

    float ListSum (float[] l) {
        float su = 0;
        for (int i = 0; i < l.Length; i++) {
            su += l[i];
        }
        return su;
    }

    bool IsBetween (float end1, float end2, float x) {
        // Inclusive.
        if (end2 > end1) {
            return (x >= end1) && (x <= end2);
        } else {
            return (x >= end2) && (x <= end1);
        }
    }

    bool PointIsInTriangle (Vector2 A, Vector2 B, Vector2 C, Vector2 X) {
        // Inclusive.
        float aSlope = CalcSlope(B, C);
        float bSlope = CalcSlope(C, A);
        float cSlope = CalcSlope(A, B);
        float aYInt = B.y - aSlope * B.x;
        float bYInt = C.y - bSlope * C.x;
        float cYInt = A.y - cSlope * A.x;
        float aRes = X.y - (aSlope * X.x + aYInt);
        float bRes = X.y - (bSlope * X.x + bYInt);
        float cRes = X.y - (cSlope * X.x + cYInt);
        if (aRes == 0 || bRes == 0 || cRes == 0) {
            return true;
        }
        float aaRes = A.y - (aSlope * A.x + aYInt);
        if (Mathf.Sign(aRes) != Mathf.Sign(aaRes)) {
            return false;
        }
        float bbRes = B.y - (bSlope * B.x + bYInt);
        if (Mathf.Sign(bRes) != Mathf.Sign(bbRes)) {
            return false;
        }
        float ccRes = C.y - (cSlope * C.x + cYInt);
        if (Mathf.Sign(cRes) != Mathf.Sign(ccRes)) {
            return false;
        }
        return true;
    }

    bool CalcIntersection (Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 intersection) {
        // Calcs if the line segment AB intersects the line CD.
        // Returns bool denoting if intersection exists,
        // and populates intersection Vector2 with the
        // intersection of the two lines regardless.
        // Assumes that none of the points are equal! Weird stuff
        // will happen otherwise...
        float m1 = CalcSlope(A, B);
        float m2 = CalcSlope(C, D);
        if (m1 == m2) {
            return false;
        }
        float b1 = A.y - m1 * A.x;
        float b2 = C.y - m2 * C.x;
        float Ex = (b1 - b2) / (m2 - m1);
        float Ey = m1 * Ex + b1;
        intersection = new Vector2(Ex, Ey);
        return IsBetween(A.x, C.x, Ex);
    }

    float[] GetInteriorAngles (List<Vector2> vertices) {
        int n = vertices.Count;
        float[] thetas = new float[n];
        for (int i = 0; i < n; i++) {
            Vector2 B = vertices[i];
            Vector2 A = vertices[Mod(i - 1, n)];
            Vector2 C = vertices[Mod(i + 1, n)];
            Vector2 BA = A - B;
            Vector2 BC = C - B;
            float magBACrossBC = BA.y * BC.x - BA.x * BC.y;
            float magnitudeProduct = BA.magnitude * BC.magnitude;
            float sinTheta = magBACrossBC / magnitudeProduct;
            float cosTheta = Vector2.Dot(BA, BC) / magnitudeProduct;
            //float theta = Mathf.Asin(sinTheta);
            float theta = FindAngle(sinTheta, cosTheta);
            thetas[i] = theta;
        }
        float sumAngles = ListSum(thetas);
        float interiorAngleSum = Mathf.PI * (n - 2);
        float exteriorAngleSum = interiorAngleSum + 4 * Mathf.PI;
        bool gotInteriorAngles = Mathf.Abs(sumAngles - interiorAngleSum) < Mathf.Abs(sumAngles - exteriorAngleSum);
        if (!gotInteriorAngles) {
            for (int i = 0; i < n; i++) {
                thetas[i] = 2 * Mathf.PI - thetas[i];
            }
        }
        return thetas;
    }

    //bool[] ClassifyConvexVertices (Vector2[] vertices) {
    List<bool> ClassifyConvexVertices (List<Vector2> vertices) {
        // Classifies vertices as convex or not.
        // A convex vertex is probably just one with
        // angle < 180.
        int n = vertices.Count;
        //bool[] isConvex = new bool[n];
        List<bool> isConvex = new List<bool>();
        float[] thetas = GetInteriorAngles(vertices);
        float[] degs = new float[n];
        for (int i = 0; i < n; i++) {
            //isConvex[i] = thetas[i] < Mathf.PI;
            isConvex.Add(thetas[i] < Mathf.PI);
            degs[i] = RadiansToDegrees(thetas[i]);
        }
        //PrintArr(degs);
        //PrintList(isConvex);
        return isConvex;
    }

    bool VertexIsEar (List<Vector2> vertices, List<bool> isConvex, int idx) {
        // To check if a vertex is a leaf, we can probably just check
        // if there are no points within the triangle to be created.
        int n = vertices.Count;
        if (!isConvex[idx]) {
            // A connecting edge between the neighbors of a non-convex
            // vertex won't be in the polygon.
            return false;
        }
        for (int i = 0; i < n; i++) {
            int leftIdx = Mod(idx - 1, n);
            int rightIdx = Mod(idx + 1, n);
            if (i == idx || i == leftIdx || i == rightIdx) {
                continue;
            }
            if (PointIsInTriangle(vertices[leftIdx], vertices[idx], vertices[rightIdx], vertices[i])) {
                return false;
            }
        }
        return true;
    }

    void FindAllEars (List<Vector2> vertices) {
        // Debug func.
        int n = vertices.Count;
        List<bool> isConvex = ClassifyConvexVertices(vertices);
        bool[] isEar = new bool[n];
        for (int i = 0; i < n; i++) {
            isEar[i] = VertexIsEar(vertices, isConvex, i);
        }
        PrintArr(isEar);
    }

    List<Vector2> VertexListFromList (List<float> nums) {
        List<Vector2> res = new List<Vector2>();
        for (int i = 0; i < nums.Count; i += 2) {
            //res[i / 2] = new Vector2(nums[i], nums[i + 1]);
            res.Add(new Vector2(nums[i], nums[i + 1]));
        }
        return res;
    }

    List<int> ListOfNumbersInOrder (int n) {
        List<int> res = new List<int>();
        for (int i = 0; i < n; i++) {
            res.Add(i);
        }
        return res;
    }

    int[] FindTrianglesForMesh (List<Vector2> vertices0) {
        int n = vertices0.Count;
        int[] triangles = new int[3 * (n - 2)];
        List<Vector2> vertices = new List<Vector2>(vertices0);
        List<int> idxs = ListOfNumbersInOrder(n);
        for (int i = 0; i < n - 2; i++) {
            int verticesRemaining = vertices.Count;
            List<bool> isConvex = ClassifyConvexVertices(vertices);
            for (int j = 0; j < verticesRemaining; j++) {
                if (VertexIsEar(vertices, isConvex, j)) {
                    int leftIdx = Mod(j - 1, verticesRemaining);
                    int rightIdx = Mod(j + 1, verticesRemaining);
                    Vector2 A = vertices[leftIdx];
                    Vector2 B = vertices[j];
                    Vector2 C = vertices[rightIdx];
                    Vector2 BA = A - B;
                    Vector2 BC = C - B;
                    float signedCross = BA.y * BC.x - BA.x * BC.y;
                    triangles[3 * i + 1] = idxs[j];
                    // Fixes orientation prob.
                    if (signedCross < 0) {
                        triangles[3 * i] = idxs[leftIdx];
                        triangles[3 * i + 2] = idxs[rightIdx];
                    } else {
                        triangles[3 * i] = idxs[rightIdx];
                        triangles[3 * i + 2] = idxs[leftIdx];
                    }
                    vertices.RemoveAt(j);
                    idxs.RemoveAt(j);
                    break;
                }
            }
        }
        PrintArr(triangles);
        return triangles;
    }

    Vector3[] Vector2VertexListToVector3Arr (List<Vector2> vertices) {
        int n = vertices.Count;
        Vector3[] arr = new Vector3[n];
        for (int i = 0; i < n; i++) {
            arr[i] = new Vector3(vertices[i].x, 0, vertices[i].y);
        }
        return arr;
    }

    Vector2[] CalcNewUVs (List<Vector2> vertices) {
        // Will revisit/test maybe.
        int numVertices = vertices.Count;
        Vector2[] newUVs = new Vector2[numVertices];
        for (int i = 0; i < numVertices; i++) {
            newUVs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }
        return newUVs;
    }

    void RedoMesh (List<Vector2> vertices) {
        int[] triangles = FindTrianglesForMesh(vertices);
        Vector2[] uvs = CalcNewUVs(vertices);
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = Vector2VertexListToVector3Arr(vertices);
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        Destroy(gameObject.GetComponent<MeshCollider>());
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    void OldStart () {
        /**
        //ClassifyConvexVertices(convexQuadrilateral);
        //ClassifyConvexVertices(kite);
        //ClassifyConvexVertices(reversedKite);
        **/
        /**
        FindTrianglesForMesh(convexQuadrilateral);
        FindTrianglesForMesh(kite);
        FindTrianglesForMesh(reversedKite);
        FindTrianglesForMesh(VertexListFromList(advancedKite));
        FindTrianglesForMesh(VertexListFromList(arch));
        **/

    }

    void Start () {
        RedoMesh(reversedKite);
        //RedoMesh(VertexListFromList(advancedKite));
    }

}
