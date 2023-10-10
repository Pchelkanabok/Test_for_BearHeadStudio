using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public bool f = true;

    [Range (0f, 10f)]
    public float CraterSizeMultiplier;

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(GetComponent<Transform>().localScale.x);
        
        if (other.gameObject.tag == "Terrain" && f)
        {
            
            other.gameObject.GetComponent<TerrainDeformer>().DestroyTerrain(other.contacts[0].point, CraterSizeMultiplier * GetComponent<Transform>().localScale.x);
            f = false;
        }
    }
}
