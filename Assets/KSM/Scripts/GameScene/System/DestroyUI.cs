using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyUI : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }
}
