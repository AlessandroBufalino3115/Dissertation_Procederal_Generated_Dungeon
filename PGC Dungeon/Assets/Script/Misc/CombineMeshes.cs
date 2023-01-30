using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMeshes : MonoBehaviour
{
    public void CombineChildrenMeshes()
    {
        if (this.transform.childCount > 0)
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            Material[] materials = new Material[meshFilters.Length];

            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                materials[i] = meshFilters[i].GetComponent<Renderer>().sharedMaterial;
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }


            Mesh mesh = new Mesh();

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            transform.GetComponent<MeshFilter>().sharedMesh = mesh;
            transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
            transform.GetComponent<MeshRenderer>().materials = materials;
            transform.gameObject.SetActive(true);




            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

        }

        this.gameObject.SetActive(false);
    }
}

