using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WirePhysics : MonoBehaviour {
    public Transform startPoint;
    public Transform endPoint;
    public int numNodes = 30;
    public float spring = 1f;
    public float damper = 1f;
    public float maxDistance = 1f;
    public float minDistance = 1f;
    public float tolerance = 0.5f;
    public float lineWidth = 0.05f;
    public Color lineColor = Color.white;

    public float tension = 10f;

    private List<GameObject> nodes = new List<GameObject>();
    private LineRenderer lineRenderer;
    private bool isReady;

    public float Tension {
        get {
            if (tension == 0) {
                tension = 0.1f;
                return tension;
            }
            return tension;
        }
    }

    void Start() {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = numNodes;
        SetNode(startPoint, 0);
        nodes[0].GetComponent<Rigidbody>().isKinematic = true;
        for (int i = 1; i <= numNodes / 2f; i++) {
            GameObject node = new GameObject("Node " + i);            
            SetNode(node.transform, i);            
            nodes[i - 1].GetComponent<SpringJoint>().connectedBody = nodes[i].GetComponent<Rigidbody>();
        }
        var mid = nodes.Count - 1;
        if (numNodes % 2 > 0) {            
            Destroy(nodes[nodes.Count - 1].GetComponent<SpringJoint>());
        }
        else {            
            nodes[mid].GetComponent<SpringJoint>().connectedBody = nodes[mid - 1].GetComponent<Rigidbody>();
        }
        for (int i = mid + 1; i < numNodes; i++) {
            GameObject node = new GameObject("Node " + i);
            SetNode(node.transform, i);
            nodes[i].GetComponent<SpringJoint>().connectedBody = nodes[i - 1].GetComponent<Rigidbody>();
        }
        var final = numNodes - 1;
        SetNode(endPoint, final);
        nodes[final].GetComponent<SpringJoint>().connectedBody = nodes[final - 1].GetComponent<Rigidbody>();
        nodes[final].GetComponent<Rigidbody>().isKinematic = true;
        isReady = true;
    }

    void SetNode(Transform node, int i) {
        float t = (float)i / (numNodes - 1);
        node.position = Vector3.Lerp(startPoint.position, endPoint.position, t);
        node.parent = transform;
        Rigidbody rb = node.gameObject.AddComponent<Rigidbody>();
        rb.drag = Tension;
        rb.mass = 1 / Tension;
        SpringJoint joint = node.gameObject.AddComponent<SpringJoint>();
        joint.spring = spring;
        joint.damper = damper;
        joint.maxDistance = maxDistance;
        joint.minDistance = minDistance;
        joint.tolerance = tolerance;
        nodes.Add(node.gameObject);
    }

    void Update() {
        if (!isReady) return;

        for (int i = 0; i < numNodes; i++) {
            SpringJoint joint = nodes[i].GetComponent<SpringJoint>();
            lineRenderer.SetPosition(i, nodes[i].transform.position);
            if (!joint) continue;
            var rb = nodes[i].gameObject.GetComponent<Rigidbody>();
            rb.drag = Tension;
            rb.mass = 1 / Tension;
            joint.spring = spring;
            joint.damper = damper;
            joint.maxDistance = maxDistance;
            joint.minDistance = minDistance;
            joint.tolerance = tolerance;
        }
    }
}