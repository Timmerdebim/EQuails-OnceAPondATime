using UnityEngine;
using UnityEngine.Events;

namespace TDK.GadgetSystem.Runtime.Gadgets
{
    /// <summary>
    /// A Generator is a power-source Gadget. When running it continuously sends
    /// an ON signal to all connected Gadgets. It can be started/stopped externally.
    /// </summary>
    public class GeneratorGadget : Gadget
    {
        [Header("Generator Settings")]
        [SerializeField] private float startDelay = 0f;     // seconds before it powers up on Start()
        [SerializeField] private bool autoStartOnPlay = true;

        [Header("Events")]
        public UnityEvent OnGeneratorStarted;
        public UnityEvent OnGeneratorStopped;

        protected override void Start()
        {
            base.Start();

            if (autoStartOnPlay)
            {
                if (startDelay > 0f)
                    Invoke(nameof(StartGenerator), startDelay);
                else
                    StartGenerator();
            }
        }

        /// <summary>Start the generator (turns ON and propagates power).</summary>
        public void StartGenerator() => SetActive(true);

        /// <summary>Stop the generator (turns OFF, connected gadgets lose power).</summary>
        public void StopGenerator() => SetActive(false);

        protected override void OnStateChanged(bool isActive)
        {
            if (isActive) OnGeneratorStarted?.Invoke();
            else OnGeneratorStopped?.Invoke();

            Debug.Log($"[Generator] {Label} → {(isActive ? "RUNNING" : "STOPPED")}");
        }

#if UNITY_EDITOR
        private new void OnDrawGizmosSelected()
        {
            // Draw spinning-coil icon suggestion: concentric circles
            Gizmos.color = IsActive ? new Color(1f, 0.85f, 0f) : new Color(0.5f, 0.5f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawWireSphere(transform.position, 0.35f);
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }
#endif
    }
}