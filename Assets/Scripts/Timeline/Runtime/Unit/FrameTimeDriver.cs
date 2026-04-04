using System;

namespace PJR.Timeline
{
    public class FrameTimeDriver : ITimeDriver
    {
        private float _remainDeltaTime;
        private float _secondPerFrame;

        public FrameTimeDriver(float secondPerFrame)
        {
            _secondPerFrame = secondPerFrame;
        }

        public void Drive(float deltaTime, TimeDriverContext shared, Action<UpdateContext> onUpdate)
        {
            _remainDeltaTime += deltaTime * (float)shared.timeScale;
            while (_remainDeltaTime >= _secondPerFrame)
            {
                _remainDeltaTime -= _secondPerFrame;
                var ctx = new UpdateContext
                {
                    timeScale = shared.timeScale,
                    currentTime = shared.currentTime,
                    currentFrame = shared.currentFrame,
                    deltaTime = _secondPerFrame,
                    frameCount = UnityEngine.Time.frameCount,
                    updateIntervalType = IntervalType.Frame,
                    gameObject = shared.gameObject,
                };
                onUpdate(ctx);
            }
        }

        public void Reset()
        {
            _remainDeltaTime = 0;
        }
    }
}
