using UnityEngine;

namespace Baracuda.Utilities.Types
{
    public readonly struct ScaledTimer
    {
        private readonly float _targetTime;
        private readonly float _startTime;

        private ScaledTimer(float delayInSeconds)
        {
            _startTime = Time.time;
            _targetTime = _startTime + delayInSeconds;
        }

        public bool IsRunning => _targetTime > Time.time;

        public bool IsNotRunning => IsRunning is false;

        public bool Expired => 0 < _targetTime && _targetTime <= Time.time;

        public bool ExpiredOrNotRunning => _targetTime <= Time.time;

        public float RemainingTime => IsRunning ? _targetTime - Time.time : 0;

        public float Delta(float fallback = 0)
        {
            if (IsRunning)
            {
                var totalDuration = _targetTime - _startTime;
                var passedTime = totalDuration - RemainingTime;
                return passedTime / totalDuration;
            }

            return fallback;
        }

        public override string ToString()
        {
            return $"{nameof(ScaledTimer)}: {RemainingTime:0.00} | IsRunning: {IsRunning.ToColorBool()}";
        }

        public string ToString(string format)
        {
            return $"{nameof(ScaledTimer)}: {RemainingTime.ToString(format)}";
        }

        public static ScaledTimer None => new();

        public static ScaledTimer FromSeconds(float durationInSeconds)
        {
            return new ScaledTimer(durationInSeconds);
        }
    }
}