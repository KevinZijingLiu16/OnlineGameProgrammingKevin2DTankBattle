using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{

    [SerializeField] private float lifeTime = 1f;

    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    
}
