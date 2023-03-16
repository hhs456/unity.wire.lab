using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour {
    public float wireLength = 10f;
    public int numNodes = 10;
    public float nodeDistance = 1f;
    public float springiness = 0.1f;
    public float damping = 0.01f;
    public float gravity = 9.81f;
    public float mass = 0.1f;

    private List<Vector3> nodes;
    private List<Vector3> velocities;

    private void Start() {
        // Initialize the nodes list
        nodes = new List<Vector3>();
        for (int i = 0; i < numNodes; i++) {
            nodes.Add(Vector3.zero);
            GameObject.Instantiate(new GameObject(), transform);
        }

        // Initialize the velocities list
        velocities = new List<Vector3>();
        for (int i = 0; i < numNodes; i++) {
            velocities.Add(Vector3.zero);
        }

        // Set the initial position of the nodes
        for (int i = 0; i < numNodes; i++) {
            float t = (float)i / (numNodes - 1);
            nodes[i] = Vector3.Lerp(transform.position, transform.position + Vector3.up * wireLength, t);
        }
    }

    private void Update() {
        // Apply forces to the nodes
        for (int i = 1; i < numNodes - 1; i++) {
            Vector3 force = Vector3.zero;

            // Add spring force
            Vector3 toPrev = nodes[i] - nodes[i - 1];
            Vector3 toNext = nodes[i + 1] - nodes[i];
            float distPrev = toPrev.magnitude;
            float distNext = toNext.magnitude;
            float stretchPrev = distPrev - nodeDistance;
            float stretchNext = distNext - nodeDistance;
            force += -springiness * stretchPrev * toPrev.normalized;
            force += springiness * stretchNext * toNext.normalized;

            // Add damping force
            Vector3 vel = velocities[i];
            force += -damping * vel;

            // Add gravity force
            force += mass * gravity * Vector3.down;

            // Apply force to node
            Vector3 acc = force / mass;
            velocities[i] += acc * Time.deltaTime;
            nodes[i] += velocities[i] * Time.deltaTime;
        }

        // Set the new positions of the nodes
        for (int i = 0; i < numNodes; i++) {
            transform.GetChild(i).position = nodes[i];
        }
    }
}