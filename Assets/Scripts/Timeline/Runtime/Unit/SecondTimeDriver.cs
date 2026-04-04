using System;

namespace PJR.Timeline
{
    public class SecondTimeDriver : ITimeDriver
    {
        public void Drive(float deltaTime, TimeDriverContext shared, Action<UpdateContext> onUpdate)
        {
            float scaled = deltaTime * (float)shared.timeScale;
            var ctx = new UpdateContext
            {
                timeScale = shared.timeScale,
                currentTime = shared.currentTime,
                currentFrame = shared.currentFrame,
                deltaTime = scaled,
                unscaledDeltaTime = deltaTime,
                updateIntervalType = IntervalType.Second,
                gameObject = shared.gameObject,
            };
            onUpdate(ctx);
        }

        public void Reset() { }
    }
}
