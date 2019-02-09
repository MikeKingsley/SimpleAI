using UnityEngine;

public class AIGun : MonoBehaviour {

    public Transform firePoint;
    public GameObject projectile;
    public BulletStats stats;
    public Light muzzleLight;
    public ParticleSystem muzzleEffect;

    public SoundClip gunShot;

    AudioSource source;
    float timeElapsed;
    bool fire;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (CheckWaitTimer(1f) && fire)
        {
            fire = false;
            timeElapsed = 0;
            StopMuzzleEffects();
        }
    }

    bool CheckWaitTimer(float time)
    {
        timeElapsed += Time.deltaTime;
        return (timeElapsed >= time);
    }

    public void Shoot(AIController controller)
    {
        fire = true;
        GameObject bullet = Instantiate(projectile);
        bullet.transform.position = firePoint.transform.position;
        bullet.transform.rotation = firePoint.transform.rotation;

        Projectile_3 projectile_3 = bullet.GetComponent<Projectile_3>();
        projectile_3.controller = controller;
        projectile_3.SetStats(stats.speed, stats.damage, stats.force, stats.lifetime, stats.defaultImpactEffect);

        PlayMuzzleEffects();
        source.PlayOneShot(gunShot.sound, gunShot.volumeMax);
    }

    void PlayMuzzleEffects()
    {
        if (muzzleLight != null)
        {
            muzzleLight.enabled = true;
        }
        if (muzzleEffect != null)
        {
            muzzleEffect.Play();
        }
    }

    void StopMuzzleEffects()
    {
        if (muzzleLight != null)
        {
            muzzleLight.enabled = false;
        }
        if (muzzleEffect != null)
        {
            muzzleEffect.Stop();
        }
    }
}
