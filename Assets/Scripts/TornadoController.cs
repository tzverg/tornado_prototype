using System;
using UnityEngine;

public class TornadoController : MonoBehaviour
{
    [Range(1F, 2F)][SerializeField] private float tornadoScale = 1F;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckLocalScale();
    }

    private void CheckLocalScale()
    {
        if (transform.localScale.x != tornadoScale || transform.localScale.y != tornadoScale)
        {
            transform.localScale = new Vector3(tornadoScale, 1F, tornadoScale);
        }
    }
}
