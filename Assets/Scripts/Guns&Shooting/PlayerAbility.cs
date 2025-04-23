using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float abilityCooldown = 5f;
    private float nextAbilityTime = 0f;
    public AbilityType currentAbility = AbilityType.None;
    

    public enum AbilityType
    {
        None,
        Invincibility,
        DoubleBulletSize,
        DoubleFireRate,
        SlowAllWallDots
    }
    

    void Start()
    {
        currentAbility = GameManager.Instance.selectedAbility;
        Debug.Log($"Current Selected Ability: {currentAbility}");
    }

    // Update is called once per frame
    void Update()
    {
        currentAbility = GameManager.Instance.selectedAbility;
        if ((Input.GetKeyDown(KeyCode.E) || OVRInput.GetDown(OVRInput.Button.One)) && Time.time >= nextAbilityTime)
        {
            UseAbility();
            nextAbilityTime = Time.time + abilityCooldown;
        }


    }



    void UseAbility()
    {
        Debug.Log("Ability used!");

        ScreenTintController tint = FindObjectOfType<ScreenTintController>();

        if (tint == null)
        {
            Debug.Log("Tint is not being found");
        }
        else
        {
            Debug.Log("Tint is found");
        }

        switch (currentAbility)
        {
            case AbilityType.Invincibility:
                ActivatInvincible(10f);
                tint?.ShowTint(Color.yellow, 10f);
                break;

            case AbilityType.DoubleBulletSize:
                DoubleBulletSize(2f, 5f);
                tint?.ShowTint(Color.cyan, 5f);
                break;

            case AbilityType.DoubleFireRate:
                DoubleFireRate(0.5f, 5f);
                tint?.ShowTint(Color.red, 5f);
                break;

            case AbilityType.SlowAllWallDots:
                SlowAllWallDots(0.2f, 3f);
                tint?.ShowTint(Color.blue, 3f);
                break;

            default:
                Debug.LogWarning("No ability assigned!");
                break;
        }
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
