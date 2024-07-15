using UnityEngine;

namespace Baracuda.Utilities
{
    public static class MeshUtility
    {
        public static Mesh CreateCapsuleMesh(CapsuleCollider collider, int resolution = 32)
        {
            return CreateCapsuleMesh(
                collider.center,
                collider.radius,
                collider.height,
                resolution,
                collider.transform.up);
        }

        public static Mesh CreateCapsuleMesh(Vector3 center, float radius, float height, int resolution, Vector3 axis)
        {
            // Compile a new mesh
            var mesh = new Mesh();

            radius = Mathf.Max(radius, 0);
            height = Mathf.Max(height, 2 * radius);

            // Calculate the number of vertices and triangles
            var longitudeSegments = resolution;
            var latitudeSegments = resolution / 2; // Half sphere for each end

            var vertexCount = (longitudeSegments + 1) * (latitudeSegments + 1) * 2 + (longitudeSegments + 1) * 2;
            var vertices = new Vector3[vertexCount];
            var triangles = new int[longitudeSegments * latitudeSegments * 6 * 2 + longitudeSegments * 6];

            // Compile vertices
            var index = 0;

            // Rotation to align with the axis
            var rotation = Quaternion.FromToRotation(Vector3.up, axis.normalized);

            // Top hemisphere
            for (var lat = 0; lat <= latitudeSegments; lat++)
            {
                var theta = lat * Mathf.PI / 2 / latitudeSegments; // Half sphere
                var sinTheta = Mathf.Sin(theta);
                var cosTheta = Mathf.Cos(theta);

                for (var lon = 0; lon <= longitudeSegments; lon++)
                {
                    var phi = lon * 2 * Mathf.PI / longitudeSegments;
                    var sinPhi = Mathf.Sin(phi);
                    var cosPhi = Mathf.Cos(phi);

                    var x = cosPhi * sinTheta;
                    var y = cosTheta;
                    var z = sinPhi * sinTheta;

                    var vertex = rotation * new Vector3(x, y, z) * radius;
                    vertices[index++] = center + vertex + axis.normalized * (height / 2 - radius);
                }
            }

            // Bottom hemisphere
            for (var lat = 0; lat <= latitudeSegments; lat++)
            {
                var theta = lat * Mathf.PI / 2 / latitudeSegments; // Half sphere
                var sinTheta = Mathf.Sin(theta);
                var cosTheta = Mathf.Cos(theta);

                for (var lon = 0; lon <= longitudeSegments; lon++)
                {
                    var phi = lon * 2 * Mathf.PI / longitudeSegments;
                    var sinPhi = Mathf.Sin(phi);
                    var cosPhi = Mathf.Cos(phi);

                    var x = cosPhi * sinTheta;
                    var y = -cosTheta; // Notice the sign change here to point outward
                    var z = sinPhi * sinTheta;

                    var vertex = rotation * new Vector3(x, y, z) * radius;
                    vertices[index++] = center + vertex - axis.normalized * (height / 2 - radius);
                }
            }

            // Cylinder vertices
            for (var lon = 0; lon <= longitudeSegments; lon++)
            {
                var phi = lon * 2 * Mathf.PI / longitudeSegments;
                var sinPhi = Mathf.Sin(phi);
                var cosPhi = Mathf.Cos(phi);

                var x = cosPhi;
                var z = sinPhi;

                var vertex = rotation * new Vector3(x, 0, z) * radius;
                vertices[index++] = center + vertex + axis.normalized * (height / 2 - radius);
                vertices[index++] = center + vertex - axis.normalized * (height / 2 - radius);
            }

            // Compile triangles
            var triIndex = 0;

            // Top hemisphere triangles
            for (var lat = 0; lat < latitudeSegments; lat++)
            {
                for (var lon = 0; lon < longitudeSegments; lon++)
                {
                    var first = lat * (longitudeSegments + 1) + lon;
                    var second = first + longitudeSegments + 1;

                    triangles[triIndex++] = first;
                    triangles[triIndex++] = second;
                    triangles[triIndex++] = first + 1;

                    triangles[triIndex++] = second;
                    triangles[triIndex++] = second + 1;
                    triangles[triIndex++] = first + 1;
                }
            }

            // Bottom hemisphere triangles
            var offset = (longitudeSegments + 1) * (latitudeSegments + 1);

            for (var lat = 0; lat < latitudeSegments; lat++)
            {
                for (var lon = 0; lon < longitudeSegments; lon++)
                {
                    var first = offset + lat * (longitudeSegments + 1) + lon;
                    var second = first + longitudeSegments + 1;

                    triangles[triIndex++] = first;
                    triangles[triIndex++] = second;
                    triangles[triIndex++] = first + 1;

                    triangles[triIndex++] = second;
                    triangles[triIndex++] = second + 1;
                    triangles[triIndex++] = first + 1;
                }
            }

            // Cylinder triangles
            offset = 2 * (longitudeSegments + 1) * (latitudeSegments + 1);

            for (var lon = 0; lon < longitudeSegments; lon++)
            {
                var first = offset + lon * 2;
                var second = first + 2;

                triangles[triIndex++] = first;
                triangles[triIndex++] = first + 1;
                triangles[triIndex++] = second;

                triangles[triIndex++] = first + 1;
                triangles[triIndex++] = second + 1;
                triangles[triIndex++] = second;
            }

            // Assign vertices and triangles to the mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}