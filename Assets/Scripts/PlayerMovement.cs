using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    static PlayerMovement Instance;
    public static PlayerMovement GetPlayer()
    {
        return Instance;
    }

    [SerializeField] float speed = 10f;

    Rigidbody rb;
    private GameManager gm;

    int coinsCollected = 0;

    public int GetNumCoins()
    {
        return coinsCollected;
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gm = GameManager.GetGameManager();
    }

    // Update is called once per frame
    void Update()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        Vector3 currentPos = transform.position;
        Vector3 newPos = new Vector3(currentPos.x + (hInput * speed * Time.deltaTime), currentPos.y, currentPos.z + (vInput * speed * Time.deltaTime));
        transform.position = newPos;

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = Vector3.up * 10;
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        Direction movement = TileBehavior.StringToDirection(other.gameObject.tag);
        if(movement != Direction.Null)
        {
            gm.ShiftTile(movement);
        }

        if (other.gameObject.tag.Equals("Coin"))
        {
            coinsCollected++;
            other.gameObject.GetComponent<CoinBehavior>().KillYourself();
        }
    }
}
