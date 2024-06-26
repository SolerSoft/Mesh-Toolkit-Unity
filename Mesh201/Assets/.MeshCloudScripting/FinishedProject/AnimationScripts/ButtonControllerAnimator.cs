//
// This file was auto-generated from Animator assets in Unity Project Mesh201.
//

// <auto-generated />

namespace MeshApp.Animations
{
    using System;
    using Microsoft.Mesh.CloudScripting;
    using Microsoft.Mesh.CloudScripting.Declarations;

    [UserCreatable(false)]
    public class ButtonControllerAnimator : AnimationNode
    {
        private readonly float[] _clickSpeeds = { 1F, 5F, };

        protected ButtonControllerAnimator(in Guid ahandle, bool transfer)
        : base(ahandle, transfer)
        { }

        public enum ClickState
        {
            OnClicked,
            Idle,
        }

        [Replication(ReplicationKind.Full)]
        public ClickState CurrentClickState
        {
            get => (ClickState)((int)this[nameof(CurrentClickState)]);
            set
            {
                this[nameof(CurrentClickState)].SetValue((int)value);
                SystemTimeOfClickUpdated = Application.ToServerTime(DateTime.UtcNow).Ticks;
            }
        }

        public float ClickSpeed
            => _clickSpeeds[(int)this[nameof(CurrentClickState)]];

        [Replication(ReplicationKind.Full)]
        [Serialized(false)]
        internal long SystemTimeOfClickUpdated
        {
            get => (long)GetPropertyAccessor(nameof(SystemTimeOfClickUpdated));
            set => GetPropertyAccessor(nameof(SystemTimeOfClickUpdated)).SetValue(value);
        }

        internal static ButtonControllerAnimator GetOrCreateInstance(in Guid cookie, bool transfer)
        {
            var result = GetOrCreateCloudObject(
                cookie,
                transfer,
                (handle, t) => new ButtonControllerAnimator(handle, transfer: t));

            return result as ButtonControllerAnimator;
        }
    }
}