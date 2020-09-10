using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    private float _lowerBoundary = -8.0f;
    [SerializeField] // 0 = Triple shot, 1 = Speed, 2 = Shields, 3 = Ammo refill, 4 = Health, 5 = Big_Shot, 6 = Skull
    private int powerupID;

    [SerializeField]
    private AudioClip _clip;

    private GameObject _player;
    private int _pickupSpeed = 4;

    private void Start()
    {
        _player = GameObject.Find("Player");

        if (_player == null)
        {
            Debug.LogError("The Player is NULL");
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            float moveSpeed = _speed * Time.deltaTime;

            this.transform.position = Vector2.MoveTowards(this.transform.position, _player.transform.position, moveSpeed * _pickupSpeed);    
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }

        if (transform.position.y <= _lowerBoundary)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null)
            {
                switch (powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.RefillAmmo();
                        break;
                    case 4:
                        player.HealPlayer();
                        break;
                    case 5:
                        player.BigShotActive();
                        break;
                    case 6:
                        player.SkullCollected();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }
}
