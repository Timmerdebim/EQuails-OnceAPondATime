using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(Animator))]
public class TransitionScreenController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    void OnValidate()
    {
        _animator = GetComponent<Animator>();
    }

    [SerializeField] private float _enterDuration = 1f;
    [SerializeField] private float _exitDuration = 1f;

    void Start()
    {
        _animator.SetTrigger("Exit");
    }

    public async Task EnterTransitionScreen()
    {
        _animator.SetTrigger("Enter");
        await Task.Delay(Mathf.CeilToInt(_enterDuration * 100));
    }

    public async Task ExitTransitionScreen()
    {
        _animator.SetTrigger("Exit");
        await Task.Delay(Mathf.CeilToInt(_exitDuration * 100));
    }
}