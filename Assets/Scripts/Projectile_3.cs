using UnityEngine;

public class Projectile_3 : MonoBehaviour
{
    public float maxRayDistance = 1;
    public float randomDirOffset = 0.2f;

    public Light projectileLight;
    public float fadeSpeed = 1;

    public BulletStats stats;

    [HideInInspector] public float damage;
    [HideInInspector] public float force;
    [HideInInspector] public float speed;
    [HideInInspector] public float lifetime;
    [HideInInspector] public ParticleEffect defaultImpactEffect;
    [HideInInspector] public Vector3 shotOrigin;
    [HideInInspector] public RaycastHit hitData;
    [HideInInspector] public AIController controller;

    Vector3 direction;
    Vector3 position;


    private void Start()
    {
        if (stats != null)
        {
            SetStats(stats.speed, stats.damage, stats.force, stats.lifetime, stats.defaultImpactEffect);
        }

        direction = transform.forward;
        position = transform.position ;//+ transform.forward * 0.75f
        shotOrigin = transform.position;

        Destroy(gameObject, lifetime);

    }

    public void SetStats(float s_speed, float s_damage, float s_force, float s_lifetime, ParticleEffect s_effect)
    {
        speed = s_speed;
        damage = s_damage;
        force = s_force;
        lifetime = s_lifetime;
        defaultImpactEffect = s_effect;
    }

    private void SetNextPoint(Vector3 pos, Vector3 dir)
    {
        //Vector3 startingPosition = pos;

        //direction += Physics.gravity * speed * Time.deltaTime;

        Ray ray = new Ray(pos, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            direction = Vector3.Reflect(dir, hit.normal);

            direction = new Vector3(
                Random.Range(direction.x - randomDirOffset, direction.x + randomDirOffset), 
                Random.Range(direction.y - randomDirOffset, direction.y + randomDirOffset), 
                Random.Range(direction.z - randomDirOffset, direction.z + randomDirOffset));
            position = hit.point;

            transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction);

            ObjectHit(hit, dir);
        }
        else
        {
            position += direction * maxRayDistance;
        }
    }

    private void Update()
    {
        if (transform.position == position)
        {
            SetNextPoint(position, direction);
        }

        transform.position = Vector3.MoveTowards(transform.position, position, maxRayDistance);

        if (projectileLight.intensity > 0)
        {
            FadeLightOverTime();
        }
    }

    void ObjectHit(RaycastHit hit, Vector3 dir)
    {
        GameObject obj = hit.transform.gameObject;

        hitData = hit;

        AIController aiController = obj.transform.root.GetComponent<AIController>();
        if (aiController != null)
        {
            HitImpact(aiController.enemyStats.impactEffect.effect, hit.point, hit.normal, aiController.enemyStats.impactEffect.lifetime);
            GameObject impact = Instantiate(aiController.enemyStats.impactEffect.effect, hit.point, Quaternion.LookRotation(hit.normal), aiController.transform) as GameObject;

            float damageCal = aiController.gun.stats.damage * aiController.gun.stats.force;
            aiController.TakeDamage(damageCal, controller);

            Destroy(impact, aiController.enemyStats.impactEffect.lifetime);
            DestoryProjectile();
        }

        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForceAtPosition(dir * force, hit.point, ForceMode.Impulse);
        }

    }

    public void HitImpact(GameObject effect, Vector3 pos, Vector3 rot, float lifetime)
    {
        GameObject impact = Instantiate(effect, pos, Quaternion.LookRotation(rot)) as GameObject;
        Destroy(impact, lifetime);
    }

    public void DestoryProjectile()
    {
        Destroy(gameObject);
    }

    void FadeLightOverTime()
    {
        if (projectileLight != null)
        {
            projectileLight.intensity = projectileLight.intensity - (Time.deltaTime * fadeSpeed);
        }
    }
}
