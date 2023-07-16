using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    [SerializeField] private int rayCount;
    [SerializeField] private float fov;
    [SerializeField] private float viewDistance;
    [SerializeField] private Transform rayOrigin;
    public Vector3 origin;
    private float startingAngle;

    private void Start() 
    {
        mesh = new Mesh();
        origin = transform.localPosition;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate() 
    {
        var dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        startingAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + fov / 2f;

        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++) {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.TransformPoint(origin), GetVectorFromAngle(angle) * viewDistance, viewDistance * 5f);
            Debug.DrawRay(transform.TransformPoint(origin), transform.TransformDirection(GetVectorFromAngle(angle)) * viewDistance, Color.red);
            if (raycastHit2D.collider == null) {
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            } else {
                //Debug.Log("HIT");
                vertex = transform.InverseTransformPoint(raycastHit2D.point);
            }
            vertices[vertexIndex] = vertex;

            if (i > 0) {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    private Vector3 GetVectorFromAngle(float angle) 
    {
        float angleRad = angle * (Mathf.PI/180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public void SetDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }

    public void SetFoV(float fov) {
        this.fov = fov;
    }

    public void SetViewDistance(float viewDistance) {
        this.viewDistance = viewDistance;
    }

}
