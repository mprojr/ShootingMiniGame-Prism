using UnityEngine;
using UnityEngine.SceneManagement;
using static PlayerAbility;

public class GameManagerButtonHelper : MonoBehaviour
{
    public void LoadNextScene()
    {
        GameManager.Instance.LoadScene();
    }

    public void AddAttackSpeed()
    {
        GameManager.Instance.AddAttackSpeed();
    }

    public void AddBulletSize()
    {
        GameManager.Instance.AddBulletSize();
    }

    public void LowerWallDotSpeed()
    {
        GameManager.Instance.LowerWallDotSpeed();
    }

    public void AddMaxHealth()
    {
        GameManager.Instance.AddMaxHealth();
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }

    public void SetAbilityToInvincibility()
    {
        GameManager.Instance.SetAbilityToInvincibility();
    }

    public void SetAbilityToDoubleFireRate()
    {
        GameManager.Instance.SetAbilityToDoubleFireRate();
    }

    public void SetAbilityToSlowAllWallDots()
    {
        GameManager.Instance.SetAbilityToSlowAllWallDots();
    }

    public void SetLeftHandSalt()
    {
        GameManager.Instance.SetLeftHandSalt();
    }

    public void SetLeftHandSpray()
    {
        GameManager.Instance.SetLeftHandSpray();
    }

    public void SetTwoHandSalt()
    {
        GameManager.Instance.SetTwoHandSalt();
    }

    public void SetTwoHandSpray()
    {
        GameManager.Instance.SetTwoHandSpray();
    }

    public void SetRightHandSalt()
    {
        GameManager.Instance.SetRightHandSalt();
    }

    public void SetRightHandSpray()
    {
        GameManager.Instance.SetRightHandSpray();
    }

}
