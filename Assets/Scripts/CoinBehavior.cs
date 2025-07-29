using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    float spinSpeed = 90;  // in degrees per second
    TileBehavior owningTile;

    void Start()
    {
        // randomize the starting rotation so the coins aren't all in sync with each other
        float yRot = Random.Range(0, 180);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    void Update()
    {
        // start spinning and never stop
        transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
    }

    public void SetOwner(TileBehavior owner)
    {
        owningTile = owner;
    }

    public void KillYourself()
    {
        UIManager.GetUIManager().UpdateCoinCount();
        owningTile.RemoveCoin();
        Destroy(gameObject);
    }
}
