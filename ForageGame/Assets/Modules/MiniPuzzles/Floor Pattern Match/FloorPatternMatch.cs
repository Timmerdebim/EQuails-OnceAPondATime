using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FloorPatternMatch : MonoBehaviour
{
    [SerializeField] private TileButton[] buttons;
    [SerializeField] private GameObject reward;

    public void UpdatePuzzle()
    {
        // Check if solved
        foreach (TileButton button in buttons)
        {
            if (!button.isCorrect)
                return;
        }
        PuzzelSolved();
    }


    private void PuzzelSolved()
    {
        // Disable puzzle
        foreach (TileButton button in buttons)
            button._isUseable = false;

        // Give reward

        // Spawn slightly below ground
        Vector3 spawnPosition = transform.position + Vector3.down * 1.5f;
        GameObject item = Instantiate(reward, spawnPosition, Quaternion.identity);

        // Jump settings
        float jumpPower = 2f;      // Height of jump
        int numJumps = 1;
        float duration = 0.6f;

        // Tween jump upward
        item.transform
            .DOJump(transform.position + Vector3.up, jumpPower, numJumps, duration)
            .SetEase(Ease.OutQuad);

        // Optional: little spin effect
        item.transform
            .DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic);
    }


}
