using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float abilityCooldown = 5f;
    private float nextAbilityTime = 0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E) || OVRInput.GetDown(OVRInput.Button.One)) && Time.time >= nextAbilityTime)
        {
            UseAbility();
            nextAbilityTime = Time.time + abilityCooldown;
        }


    }

    void UseAbility()
    {
        Debug.Log("Ability used!");

        ActivatInvincible(10f);
        // DoubleBulletSize(2f, 5f);
        // DoubleFireRate(0.5f, 5f);
        // SlowAllWallDots(0.20f, 3f);


    }


    public void SlowAllWallDots(float multiplier, float duration)
    {
        WallDotMovement[] allDots = GameObject.FindObjectsOfType<WallDotMovement>();

        foreach (WallDotMovement dot in allDots)
        {
            dot.ApplySpeedMultiplier(multiplier, duration);
        }
    }

    public void DoubleFireRate(float multiplier, float duration)
    {
        LeftOneHandGun leftGun = GameObject.FindObjectOfType<LeftOneHandGun>();
        if (leftGun != null)
        {
            leftGun.FireRateAbility(multiplier, duration);
        }

        RightOneHandGun rightGun = GameObject.FindObjectOfType<RightOneHandGun>();
        if (rightGun != null)
        {
            rightGun.FireRateAbility(multiplier, duration);
        }

        LeftTwoHandGun twoLeftGun = GameObject.FindObjectOfType<LeftTwoHandGun>();
        if (twoLeftGun != null)
        {
            twoLeftGun.FireRateAbility(multiplier, duration);
        }
    }

    public void DoubleBulletSize(float multiplier, float duration)
    {
        GameManager.Instance.BulletSizeAbility(multiplier, duration);
    }

    public void ActivatInvincible(float duration)
    {
        GameManager.Instance.ActivateInvincibility(duration);
    }



}
