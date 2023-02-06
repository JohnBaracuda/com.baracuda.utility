using System;
using System.Runtime.CompilerServices;

namespace Baracuda.Gameloop.Jobs
{
    public class FrameJob : JobBase
    {
        /*
         * Fields
         */

        private float _currentFrame;
        private float _targetFrames;
        private Action _action;

        /*
         * Factory
         */

        public static FrameJobHandle Run(Action action, int frames)
        {
            var frameJob = FrameJobPool.Get();
            frameJob._currentFrame = 0;
            frameJob._action = action;
            frameJob._targetFrames = frames;
            frameJob.IsValid = true;
            JobSystem.RegisterJob(frameJob);
            return new FrameJobHandle(frameJob);
        }

        /*
         * Public
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Cancel()
        {
            Reset();
        }

        //--------------------------------------------------------------------------------------------------------------

        /*
         * Internal
         */

        internal override void Update(float delta)
        {
            _currentFrame++;
            _action();
            if (_currentFrame > _targetFrames)
            {
                Reset();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Reset()
        {
            if (IsValid)
            {
                _action = null;
                _targetFrames = 0;
                _currentFrame = 0;
                JobSystem.UnregisterJob(this);
                FrameJobPool.Release(this);
                IsValid = false;
            }
        }
    }
}