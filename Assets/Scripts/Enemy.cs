﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4;
    private float _lowerBoundary = -7.0f;

    [SerializeField]
    private GameObject _laserPrefab;
    private float _upperBoundary = 8.0f;

    private Player _player;
    private Animator _animator;

    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1;
    private bool _stopEnemyFiring = false;

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private SpriteRenderer _shieldSpriteRenderer;
    [SerializeField]
    private float _enemyRandomShield = 0.1f;

    [SerializeField]
    private int _shieldStrength = 0;

    void Start()
    {
        _shieldSpriteRenderer.GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("The audiosource is NULL");
        }

        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("The Player is NULL");
        }

        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogError("The Animator is NULL");
        }

        if (Random.Range(0f, 1f) >= _enemyRandomShield)
        {
            ShieldSpriteRendererColor();
            _shieldVisualizer.SetActive(true);
        }
    }

    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire && _stopEnemyFiring == false)
        {
            _fireRate = Random.Range(3f, 9f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }


        }
    }

    void CalculateMovement()
    {
        // Move down at 4 meters per second
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= _lowerBoundary)
        {
            // Create random position for enemy when they drop off the bottom of screen
            float randomX = Random.Range(-9.5f, 9.5f);
            transform.position = new Vector3(randomX, _upperBoundary, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // damage player
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            if (_shieldStrength >= 0 && _shieldVisualizer.activeSelf == true)
            {
                RemoveEnemyShield();
            }
            else
            {
                _animator.SetTrigger("OnEnemyDeath");
                StopEnemyFiring();
                _speed = 0;

                _audioSource.Play();
                Destroy(this.gameObject, 2.8f);
            }            
        }

        // BigShot Laser not destroyed when it hits first enemy to allow for multi enemy hit
        if (other.CompareTag("Laser") || other.CompareTag("BigShot_Laser"))
        {
            if (other.CompareTag("Laser"))
            {
                Destroy(other.gameObject);
            }
            
            if (_player != null && _shieldVisualizer.activeSelf == false)
            {
                _player.AddScore(10);
            }
            else
            {
                RemoveEnemyShield();
                return;
            }

            // trigger anim
            _animator.SetTrigger("OnEnemyDeath");
            StopEnemyFiring();
            _speed = 0;
            _audioSource.Play();

            Destroy(this.GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }

    }

    void ShieldSpriteRendererColor()
    {
        switch (_shieldStrength)
        {
            case 0: // red
                _shieldSpriteRenderer.color = new Color(1f, 0f, 0f, 1f);
                break;
            default:
                break;
        }
    }

    void RemoveEnemyShield()
    {
        _shieldVisualizer.SetActive(false);
        _shieldStrength = -1;
    }

    void StopEnemyFiring()
    {
        _stopEnemyFiring = true;
    }
}
