using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMaker : MonoBehaviour
{
    
    int Mod (int a, int b) {
        int r = a % b;
        return r < 0 ? r + b : r;
    }

    float CalcSlope (Vector2 A, Vector2 B) {
        return (B.y - A.y) / (B.x - A.x);
    }

    bool IsBetween (float end1, float end2, float x) {
        // Inclusive.
        if (end2 > end1) {
            return (x >= end1) && (x <= end2);
        } else {
            return (x >= end2) && (x <= end1);
        }
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
    /**
    bool[] ClassifyConvexVertices (Vector2[] vertices) {
        // Classifies vertices as convex or not.
        // A convex vertex is probably just one with
        // angle < 180.
    }
    **/

}
