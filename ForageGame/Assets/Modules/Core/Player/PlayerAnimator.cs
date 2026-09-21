using System.Linq;
using NUnit.Framework.Constraints;
using TDK.Physics3DSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TDK.PlayerSystem
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        private static readonly int IsDeadHash = Animator.StringToHash("isDead");
        private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
        private static readonly int JumpHash = Animator.StringToHash("jump");
        private static readonly int FlyHash = Animator.StringToHash("fly");
        private static readonly int AttackHash = Animator.StringToHash("attack");
        private static readonly int RunHash = Animator.StringToHash("run");
        private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
        private static readonly int IsSwimmingHash = Animator.StringToHash("isSwimming");
        public Animator _animator { get; private set; }

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void IsSwimming(bool isSwimming) => _animator.SetBool(IsSwimmingHash, isSwimming);
        public void IsMoving(bool isMoving) => _animator.SetBool(IsMovingHash, isMoving);
        public void IsSprinting(bool isSprinting) => _animator.SetBool(RunHash, isSprinting);
        public void IsAttacking(bool isAttacking) => _animator.SetBool(AttackHash, isAttacking);
        public void IsJumping(bool isJumping) => _animator.SetBool(JumpHash, isJumping);
        public void IsFlying(bool isFlying) => _animator.SetBool(FlyHash, isFlying);
        public void IsGrounded(bool isGrounded) => _animator.SetBool(IsGroundedHash, isGrounded);
        public void IsDead(bool isDead) => _animator.SetBool(IsDeadHash, isDead);

        public void Reset()
        {
            IsSwimming(false);
            IsMoving(false);
            IsSprinting(false);
            IsAttacking(false);
            IsJumping(false);
            IsFlying(false);
            IsGrounded(true);
            IsDead(false);
        }
    }
}