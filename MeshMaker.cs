using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMaker : MonoBehaviour
{

    // Stuff might not work yet if you have a vertical slope!

    Vector2[] triangle = {
        new Vector2(0, 0),
        new Vector2(3, 0),
        new Vector2(3, 4),
    };
    Vector2[] convexQuadrilateral = {
        new Vector2(-3, -2),
        new Vector2(6, -2),
        new Vector2(8, 4),
        new Vector2(2, 6),
    };
    Vector2[] kite = {
        new Vector2(-2, -3),
        new Vector2(0, 0),
        new Vector2(2, -4),
        new Vector2(0, 6),
    };
    Vector2[] reversedKite = {
        new Vector2(-2, -3),
        new Vector2(0, 6),
        new Vector2(2, -4),
        new Vector2(0, 0),
    };
    float[] advancedKite = {
        0, 5,
        -5, -4,
        -3, -2,
        -1, -1,
        1, -1,
        3, -2,
        5, -4
    };
    float[] arch = {
        7, 8, 10, 14, 16, 11, 13.5f, 6.5f, 12, 7.5f, 13, 10, 11, 11, 9, 7
    };
    void PrintList<T> (T[] l) {
        int n = l.Length;
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
        //Vector2 AB = B - A;
        //Vector2 BC = C - B;
        //Vector2 CA = A - C;
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

    float[] GetInteriorAngles (Vector2[] vertices) {
        int n = vertices.Length;
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

    bool[] ClassifyConvexVertices (Vector2[] vertices) {
        // Classifies vertices as convex or not.
        // A convex vertex is probably just one with
        // angle < 180.
        int n = vertices.Length;
        bool[] isConvex = new bool[n];
        float[] thetas = GetInteriorAngles(vertices);
        float[] degs = new float[n];
        for (int i = 0; i < n; i++) {
            isConvex[i] = thetas[i] < Mathf.PI;
            degs[i] = RadiansToDegrees(thetas[i]);
        }
        //PrintList(degs);
        //PrintList(isConvex);
        return isConvex;
    }

    bool VertexIsEar (int n, Vector2[] vertices, bool[] isConvex, int idx) {
        // To check if a vertex is a leaf, we can probably just check
        // if there are no points within the triangle to be created.
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

    void FindAllEars (Vector2[] vertices) {
        int n = vertices.Length;
        bool[] isConvex = ClassifyConvexVertices(vertices);
        bool[] isEar = new bool[n];
        for (int i = 0; i < n; i++) {
            isEar[i] = VertexIsEar(n, vertices, isConvex, i);
        }
        PrintList(isEar);
    }

    Vector2[] VertexArrFromArr (float[] nums) {
        Vector2[] res = new Vector2[nums.Length / 2];
        for (int i = 0; i < nums.Length; i += 2) {
            res[i / 2] = new Vector2(nums[i], nums[i + 1]);
        }
        return res;
    }

    void Start () {
        //ClassifyConvexVertices(convexQuadrilateral);
        //ClassifyConvexVertices(kite);
        //ClassifyConvexVertices(reversedKite);
        FindAllEars(convexQuadrilateral);
        FindAllEars(kite);
        FindAllEars(reversedKite);
        FindAllEars(VertexArrFromArr(advancedKite));
        FindAllEars(VertexArrFromArr(arch));
    }

}
