using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coll : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.name.Contains("wall")) return;
        if (coll.transform.name.Contains("Player")
            ||coll.transform.name.Contains("Enemy")
            ||ReadyBtn.isMultiGame)
            transform.GetComponent<CircleCollider2D>().radius = 0.2f;

    }
    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.transform.name.Contains("wall")) return;
        if (coll.transform.name.Contains("Player")
            || coll.transform.name.Contains("Enemy")
            || ReadyBtn.isMultiGame)
            transform.GetComponent<CircleCollider2D>().radius = 1; 
    }
}
