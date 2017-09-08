using UnityEngine;
using System.Collections;

public class DeactivateMe : MonoBehaviour {

    void Awake()
    {
        gameObject.SetActive(false);
    }
}
