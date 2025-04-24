using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    private float abilityCooldown = 15f;
    private float nextAbilityTime = 0f;
    private bool isOnCooldown = false;
    private float cooldownStartTime;
    private float currentEffectDuration;

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

    void Update()
    {
        if (isOnCooldown)
        {
            if (Time.time >= cooldownStartTime + abilityCooldown)
            {
                isOnCooldown = false;
            }
        }

        GameManager.Instance.isAbilityReady = !isOnCooldown;

        currentAbility = GameManager.Instance.selectedAbility;

        if ((Input.GetKeyDown(KeyCode.E) || OVRInput.GetDown(OVRInput.Button.One)) && !isOnCooldown)
        {
            UseAbility();
            // cooldownStartTime will be set AFTER the effect ends (in coroutine)
        }
    }

    void UseAbility()
    {
        Debug.Log("Ability used!");

        ScreenTintController tint = FindObjectOfType<ScreenTintController>();
        if (tint == null)
            Debug.Log("Tint is not being found");
        else
            Debug.Log("Tint is found");

        switch (currentAbility)
        {
            case AbilityType.Invincibility:
                currentEffectDuration = 5f;
                ActivatInvincible(currentEffectDuration);
                tint?.ShowTint(Color.yellow, currentEffectDuration);
                break;

            case AbilityType.DoubleBulletSize:
                currentEffectDuration = 7.5f;
                DoubleBulletSize(2f, currentEffectDuration);
                tint?.ShowTint(Color.cyan, currentEffectDuration);
                break;

            case AbilityType.DoubleFireRate:
                currentEffectDuration = 5f;
                DoubleFireRate(0.5f, currentEffectDuration);
                tint?.ShowTint(Color.red, currentEffectDuration);
                break;

            case AbilityType.SlowAllWallDots:
                currentEffectDuration = 3f;
                SlowAllWallDots(0.2f, currentEffectDuration);
                tint?.ShowTint(Color.blue, currentEffectDuration);
                break;

            default:
                Debug.LogWarning("No ability assigned!");
                return;
        }

        // Start cooldown AFTER the effect duration
        StartCoroutine(StartCooldownAfterEffect(currentEffectDuration));
    }

    System.Collections.IEnumerator StartCooldownAfterEffect(float effectDuration)
    {
        yield return new WaitForSeconds(effectDuration);
        cooldownStartTime = Time.time;
        isOnCooldown = true;
    }

    public void SlowAllWallDots(float multiplier, float duration)
    {
        foreach (WallDotMovement dot in GameObject.FindObjectsOfType<WallDotMovement>())
        {
            dot.ApplySpeedMultiplier(multiplier, duration);
        }
    }

    public void DoubleFireRate(float multiplier, float duration)
    {
        LeftOneHandGun leftGun = GameObject.FindObjectOfType<LeftOneHandGun>();
        if (leftGun != null) leftGun.FireRateAbility(multiplier, duration);

        RightOneHandGun rightGun = GameObject.FindObjectOfType<RightOneHandGun>();
        if (rightGun != null) rightGun.FireRateAbility(multiplier, duration);

        LeftTwoHandGun twoLeftGun = GameObject.FindObjectOfType<LeftTwoHandGun>();
        if (twoLeftGun != null) twoLeftGun.FireRateAbility(multiplier, duration);
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