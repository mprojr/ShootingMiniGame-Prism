using UnityEngine;
using UnityEngine.SceneManagement;

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


}
