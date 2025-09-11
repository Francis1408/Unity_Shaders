using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{

    [SerializeField]
    Transform pointPrefab;

    [SerializeField, Range(10, 100)]
    int resolution = 10;

    Transform[] points;


    void Awake()
    {
        float step = 2f / resolution;
        Vector3 scale = Vector3.one * step;
        Vector3 position = Vector3.zero;

        points = new Transform[resolution];

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = Instantiate(pointPrefab);
            // Make a reference to each point we have
            points[i] = point;

            position.x = (i + 0.5f) * step - 1f;
            point.localPosition = position;
            point.localScale = scale;

            point.SetParent(transform, false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        float time = Time.time;
        
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i]; // Assign the point i from the array
            Vector3 position = point.localPosition; // Retrieve its initial position
            position.y = Mathf.Sin(Mathf.PI * (position.x + Time.time)/2f);
            point.localPosition = position;
        }   
    }
}
