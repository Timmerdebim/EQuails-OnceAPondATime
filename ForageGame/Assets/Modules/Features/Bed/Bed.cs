using System.Threading.Tasks;
using TDK.PlayerSystem;
using UnityEngine;
using FMODUnity;

public class Bed : MonoBehaviour
{
    [SerializeField] private float _animationLength;
    [SerializeField] private EventReference gettingInBedSfx;
    public void Interact()
    {
        if (!AppController.Instance.IsInputsActive) return;
        AppController.Instance.SetInputsActive(false);
        RuntimeManager.PlayOneShot(gettingInBedSfx);
        _ = Sleep();
    }

    public async Task Sleep()
    {
        Player.Instance.playerController.IsSleeping(true);
        await Task.Delay(Mathf.CeilToInt(_animationLength * 1000));
        await GameplayController.Instance.Sleep(); // honk-shew-mi-mi-mi
    }
}
