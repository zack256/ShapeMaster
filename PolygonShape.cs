using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PolygonShape {
    public List<Vector2> coordinates;
    
    public PolygonShape (List<Vector2> coordinates) {
        this.coordinates = coordinates;
    }

}