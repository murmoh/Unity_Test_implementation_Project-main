using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Enemy.Attack
{
    public class AttackController : MonoBehaviour
    {
        // GunfireController Variables
        public AudioClip GunShotClip;
        public AudioSource source;
        public Vector2 audioPitch = new Vector2(.9f, 1.1f);
        public GameObject muzzlePrefab;
        public GameObject muzzlePosition;
        public bool autoFire;
        public float shotDelay = .5f;
        public bool rotate = true;
        public float rotationSpeed = .25f;
        public GameObject scope;
        public bool scopeActive = false;
        private bool lastScopeState;
        public GameObject projectilePrefab;
        public GameObject projectileToDisableOnFire;
        private float timeLastFired;
        public Transform weapon;

        // Variables kept from AttackController
        private float shootingRate = 0.1f;
        private int maxMagazineSize = 30;
        private float reloadTime = 2f;
        private int currentMagazineSize;
        private bool isReloading;
        [SerializeField] private float bulletSpeed = 50f;
        private Camera cam;

        public TextMeshProUGUI clipUI;
        public int totalBulletStock = 120;  // Total bullets you start with
        private int bulletsToAdd;  // Bullets to be added when reloading
        public GameObject reloadActive;
        public TextMeshProUGUI reloadUI;

        public GameObject refillAmmo;
        public bool ammoActive = false;


        void Start()
        {
            if(refillAmmo == null)
            {
                GameObject[] refillAmmoObject = GameObject.FindGameObjectsWithTag("Ammo");
                refillAmmo = refillAmmoObject[0];
            }
            StartCoroutine(Reload());
            cam = Camera.main;
            // GunfireController initialization
            if (source != null) source.clip = GunShotClip;
            timeLastFired = 0;
            lastScopeState = scopeActive;
            if (muzzlePosition == null) 
            {
                GameObject[] muzzleObjects = GameObject.FindGameObjectsWithTag("Muzzle");
                muzzlePosition = muzzleObjects[0];
            }

            // AttackController initialization
            currentMagazineSize = maxMagazineSize;
            UpdateClipUI();
        }

        void Update()
        {
        
            if (muzzlePosition == null) 
            {
                GameObject[] muzzleObjects = GameObject.FindGameObjectsWithTag("Muzzle");
                muzzlePosition = muzzleObjects[0];
            }

            if (refillAmmo == null)
            {
                GameObject[] refillAmmoObject = GameObject.FindGameObjectsWithTag("Ammo");

                if (refillAmmoObject.Length > 0)
                {
                    refillAmmo = refillAmmoObject[0];
                }
                else
                {
                    // Handle the case where no objects with the "Ammo" tag were found.
                    // You might want to log an error or take other appropriate action.
                }
            }
            reFill();

            // GunfireController logic
            if (rotate)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + rotationSpeed, transform.localEulerAngles.z);
            }

            if (autoFire && ((timeLastFired + shotDelay) <= Time.time))
            {
                if (!isReloading)  // Add this line
                {
                    FireWeapon();
                }
            }


            if (scope && lastScopeState != scopeActive)
            {
                lastScopeState = scopeActive;
                scope.SetActive(scopeActive);
            }

            if (Input.GetButton("Fire1") && Time.time - timeLastFired >= shootingRate)
            {
                if (!isReloading)  
                {
                    FireWeapon();
                }
            }

            if (Input.GetKeyDown("r") && !isReloading && totalBulletStock > 0)
            {
                StartCoroutine(Reload());
            }

            
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Ammo")
            {
                RefillAmmo(totalBulletStock); // Here you can specify how much ammo to refill
                Destroy(other.gameObject); // Remove the Ammo object from the scene
            }
        }

        public void RefillAmmo(int amount)
        {
            totalBulletStock += amount;
            UpdateClipUI();
        }

        public void FireWeapon()
        {
            if (currentMagazineSize > 0)
            {
                timeLastFired = Time.time;

                // GunfireController logic
                var flash = Instantiate(muzzlePrefab, muzzlePosition.transform);

                // Shooting logic
                Ray ray = cam.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
                RaycastHit hit;

                Vector3 shootingDirection;
                if (Physics.Raycast(ray, out hit))
                {
                    shootingDirection = (hit.point - muzzlePosition.transform.position).normalized;
                }
                else
                {
                    shootingDirection = ray.direction;  // fallback to shooting in the direction the camera is facing
                }

                GameObject bullet = Instantiate(projectilePrefab, muzzlePosition.transform.position, Quaternion.LookRotation(shootingDirection / 2));
                Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
                if (bulletRigidbody != null)
                {
                    bulletRigidbody.velocity = shootingDirection * bulletSpeed;
                }

                if (projectileToDisableOnFire != null)
                {
                    projectileToDisableOnFire.SetActive(false);
                    Invoke("ReEnableDisabledProjectile", 3);
                }

                // Audio
                HandleAudio();

                currentMagazineSize--;

                UpdateClipUI();
            }
        }

        private void HandleAudio()
        {
            if (source == null) return;

            if (source.transform.IsChildOf(transform))
            {
                source.Play();
            }
            else
            {
                AudioSource newAS = Instantiate(source);
                if (newAS?.outputAudioMixerGroup?.audioMixer != null)
                {
                    float pitchValue = Random.Range(audioPitch.x, audioPitch.y);
                    newAS.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", pitchValue);
                    newAS.pitch = pitchValue;
                    newAS.PlayOneShot(GunShotClip);
                    Destroy(newAS.gameObject, 4);
                }
            }
        }

        private void ReEnableDisabledProjectile()
        {
            projectileToDisableOnFire.SetActive(true);
        }

        IEnumerator Reload()
        {
            isReloading = true;
            yield return new WaitForSeconds(reloadTime);

            // Calculate the number of bullets to add during reload
            bulletsToAdd = Mathf.Min(maxMagazineSize - currentMagazineSize, totalBulletStock);

            // Update magazine and total bullet stock
            currentMagazineSize += bulletsToAdd;
            totalBulletStock -= bulletsToAdd;
           

            // Update the clipUI
            UpdateClipUI();

            isReloading = false;
        }
        private void UpdateClipUI()
        {
            if (clipUI != null)
            {
                clipUI.text = string.Format("{0:D2}/{1:D2}", currentMagazineSize, totalBulletStock);
            }

            if(currentMagazineSize < 5)
            {
                reloadActive.SetActive(true);
                reloadUI.text = "        Reload";
            }
            else if(currentMagazineSize == 0 && totalBulletStock == 0)
            {
                reloadActive.SetActive(true);
                reloadUI.text = "      NO AMMO";
            }
            else
            {
                reloadActive.SetActive(false);
            }
        }

        public void reFill()
        {
            if(refillAmmo != null)
            {
                ammoActive = true;
            }

            if(refillAmmo == null && ammoActive)
                {
                    totalBulletStock = 120;
                    ammoActive = false;
                    UpdateClipUI();
                }
            
        }
    }
}
