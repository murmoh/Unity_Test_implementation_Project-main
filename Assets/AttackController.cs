using System.Collections;
using UnityEngine;

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

        void Start()
        {
            cam = Camera.main;
            // GunfireController initialization
            if (source != null) source.clip = GunShotClip;
            timeLastFired = 0;
            lastScopeState = scopeActive;

            // AttackController initialization
            currentMagazineSize = maxMagazineSize;
        }

        void Update()
        {
            // GunfireController logic
            if (rotate)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + rotationSpeed, transform.localEulerAngles.z);
            }

            if (autoFire && ((timeLastFired + shotDelay) <= Time.time))
            {
                FireWeapon();
            }

            if (scope && lastScopeState != scopeActive)
            {
                lastScopeState = scopeActive;
                scope.SetActive(scopeActive);
            }

            // AttackController logic
            if (Input.GetButton("Fire1") && Time.time - timeLastFired >= shootingRate)
            {
                FireWeapon();
            }

            if (currentMagazineSize == 0 && !isReloading)
            {
                StartCoroutine(Reload());
            }

            
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

                GameObject bullet = Instantiate(projectilePrefab, muzzlePosition.transform.position, Quaternion.LookRotation(shootingDirection));
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
            currentMagazineSize = maxMagazineSize;
            isReloading = false;
        }
    }
}
