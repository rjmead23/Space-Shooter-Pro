using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;
    private float _speedMultiplier = 2.0f;
    private float _thrusterBoost = 8.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _bigShotPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _AmmoCount = 20;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isBigShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldsActive = false;
    [SerializeField]
    private int _shieldStrength = 0;

    [SerializeField]
    private int _score;

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private SpriteRenderer _shieldSpriteRenderer;

    private UIManager _uiManager;

    [SerializeField]
    private GameObject _leftEngine, _rightEngine;

    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;

    public ThrustBar thrustbar;
    [SerializeField]
    private int maxThrust = 5;


    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _shieldSpriteRenderer.GetComponent<SpriteRenderer>();

        thrustbar.SetMaxThrust(maxThrust);

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn manager is NULL");
        }

        if (_uiManager == null)
        {
            Debug.Log("The UI Manager is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The AudioSource on the player is NULL");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _AmmoCount > 0)
        {
            FireLaser();
            _AmmoCount--;
            HandleAmmo();
        }

        CalculateMovement();       
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float upperLimit = 0;
        float lowerLimit = -3.8f;

        float xBoundary = 11.2f;

        // How to move the character right or left
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        // Phase I Thrusters
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (thrustbar._isCoolDown == false)
            {
                transform.Translate(direction * (_speed + _thrusterBoost) * Time.deltaTime);
                thrustbar.ReduceThrust(1 * Time.deltaTime);
            }
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }

        // Control y boundary of the player
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, lowerLimit, upperLimit), 0);

        // Control x boundary of the player
        if (transform.position.x >= xBoundary)
        {
            transform.position = new Vector3(-xBoundary, transform.position.y, 0);
        }
        else if (transform.position.x <= -xBoundary)
        {
            transform.position = new Vector3(xBoundary, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        // spawn laser with a cool down so can only fire at pre determined fire rate
        _canFire = Time.time + _fireRate;
        Vector3 offset = new Vector3(0, 0.8f, 0);
        
        if (_isTripleShotActive && !_isBigShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);

        }
        else if (_isBigShotActive && !_isTripleShotActive)
        {
            Instantiate(_bigShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + offset, Quaternion.identity);
        }

        _audioSource.Play();
    }

    public void Damage()
    {
        // If shield is active
        if (_isShieldsActive == true && _shieldStrength > 0)
        {
            _shieldStrength--;
            ShieldSpriteRendererColor();
            return;
        }
        else if (_isShieldsActive == true && _shieldStrength == 0)
        {
            _shieldVisualizer.SetActive(false);
            _isShieldsActive = false;
        }
        else
        {
            _lives--;
        }

        HandleEngineEffect();
        
        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    // Phase I Shield Strength
    void ShieldSpriteRendererColor()
    {
        switch (_shieldStrength)
        {
            case 3: // green
                _shieldSpriteRenderer.color = new Color(0f, 1f, 0.02f, 1f);
                break;
            case 2: // green
                _shieldSpriteRenderer.color = new Color(0f, 1f, 0.02f, 1f);
                break;
            case 1: // blue
                _shieldSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
                break;
            case 0: // red
                _shieldSpriteRenderer.color = new Color(1f, 0f, 0f, 1f);
                break;
            default:
                break;
        }
    }

    public void TripleShotActive()
    {
        _isBigShotActive = false;
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isTripleShotActive = false;
    }

    public void BigShotActive()
    {
        _isTripleShotActive = false;
        _isBigShotActive = true;
        StartCoroutine(BigShotPowerDownRoutine());
    }

    IEnumerator BigShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5);
        _isBigShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }

    public void ShieldsActive()
    {
        _isShieldsActive = true;
        _shieldStrength = 3;
        _shieldSpriteRenderer.color = new Color(0f, 1f, 0.02f, 1f); // green
        _shieldVisualizer.SetActive(true);
    }

    public void RefillAmmo()
    {
        // refill ammo
        _AmmoCount = 15;
        HandleAmmo();
    }

    public void HealPlayer()
    {
        Debug.Log("Add Life called!");
        if (_lives < 3)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);
        }

        HandleEngineEffect();
    }

    private void HandleEngineEffect()
    {
         switch (_lives)
        {
            case 3:
                _leftEngine.SetActive(false);
                _rightEngine.SetActive(false);
                break;
            case 2:
                _leftEngine.SetActive(true);
                _rightEngine.SetActive(false);
                break;
            case 1:
                _leftEngine.SetActive(true);
                _rightEngine.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void HandleAmmo()
    {
        _uiManager.UpdateAmmo(_AmmoCount);
    }

}
