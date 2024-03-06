using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Sources.Scripts.UI.BuildingControlPanel.Part
{
    public class MoveFromStorageTimerUI : IBcpMoveFromStorageTimerUI
    {
        private readonly VisualElement _timerProgress;
        private readonly Label _timerLabel;
        private const int Width = 130;
        private readonly VisualElement _timerContainer;

        public MoveFromStorageTimerUI(VisualElement panel)
        {
            _timerContainer = panel.Q<VisualElement>("timer-cont");
            _timerProgress = _timerContainer.Q<VisualElement>("pb-bar");
            _timerLabel = _timerContainer.Q<Label>("text-label");
            _timerLabel.text = "";
            _timerProgress.style.width = 0;
        }

        public async void RunMoveFromStorageTimerAsync(float duration)
        {
            float previousWidth = 0;
            var tempDuration = duration;

            while (tempDuration >= 0)
            {
                var newWidth = Width - ((Width / duration) * tempDuration);

                _timerProgress.style.width = newWidth;

                _timerProgress.experimental.animation.Start(
                    new StyleValues { width = previousWidth },
                    new StyleValues { width = newWidth },
                    500).Start();

                _timerLabel.text = tempDuration + "s";

                previousWidth = newWidth;
                tempDuration--;
                await Task.Delay(1000);

                if (tempDuration != 0) continue;

                _timerLabel.text = "Delivered!";
                
                var scaleAnimation = _timerContainer.experimental.animation.Scale(1.1f, 200);
                scaleAnimation.onAnimationCompleted +=
                    () => _timerContainer.experimental.animation.Scale(1f, 200).Start();
                scaleAnimation.Start();
            }
            
            await Task.Delay(1000);
            _timerLabel.text = "";
        }
    }

    public interface IBcpMoveFromStorageTimerUI
    {
        public void RunMoveFromStorageTimerAsync(float duration);
    }
}