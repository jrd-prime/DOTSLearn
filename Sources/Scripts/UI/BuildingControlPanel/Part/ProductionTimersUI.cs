﻿using System.Threading.Tasks;
using Sources.Scripts.CommonData;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Sources.Scripts.UI.BuildingControlPanel.Part
{
    public class ProductionTimersUI
    {
        #region Const

        private const string ProducedText = "Produced!";
        private const string EmptyText = "";
        private const string SecondsText = " sec.";

        private const float StartWidth = 0f;
        private const float Width = 130f;

        private const int AnimationDuration = 500;

        #endregion

        #region Visual

        // Visual element
        private readonly VisualElement _timerProgress;
        private readonly VisualElement _fullProgressBar;

        private readonly VisualElement _oneProgressBar;

        // Label
        private readonly Label _timerLabel;
        private readonly Label _fullTimerLabel;
        private readonly Label _oneTimerLabel;
        private readonly Label _cycleLabel;

        #endregion

        private bool _isTimerRunning;

        public ProductionTimersUI(VisualElement panel)
        {
            var fullTimerContainer = panel.Q<VisualElement>(Names.ProductionFullTimerContainerName);
            _fullProgressBar = fullTimerContainer.Q<VisualElement>(Names.ProductionTimersBothProgressBarName);
            _fullTimerLabel = fullTimerContainer.Q<Label>(Names.ProductionTimersBothLabelsName);

            var oneTimerContainer = panel.Q<VisualElement>(Names.ProductionOneTimerContainerName);
            _oneProgressBar = oneTimerContainer.Q<VisualElement>(Names.ProductionTimersBothProgressBarName);
            _oneTimerLabel = oneTimerContainer.Q<Label>(Names.ProductionTimersBothLabelsName);

            var cycleCont = panel.Q<VisualElement>(Names.ProductionTimersCycleContainerName);
            _cycleLabel = cycleCont.Q<Label>();

            _fullProgressBar.style.width = StartWidth;
            _oneProgressBar.style.width = StartWidth;
            _fullTimerLabel.text = EmptyText;
            _oneTimerLabel.text = EmptyText;
        }

        public void UpdateTimers(int currentCycle, int maxLoads, int fullCycle)
        {
            _cycleLabel.text = currentCycle <= maxLoads ? $"{currentCycle} / {maxLoads}" : ProducedText.ToUpper();

            Task timerTask = null;

            if (!_isTimerRunning)
            {
                timerTask = RunProductionTimersUpdaterAsync(fullCycle, maxLoads);
                _isTimerRunning = true;
            }
        }

        // TODO refact
        // Not working in closed app
        private async Task RunProductionTimersUpdaterAsync(int fullCycleDuration, int maxLoadsCount)
        {
            int oneLoadDuration = fullCycleDuration / maxLoadsCount;
            int tempFullCycleDuration = fullCycleDuration;

            bool isFirstCycle = true;

            float fullLoadPreviousWidth = StartWidth;
            float oneLoadPreviousWidth = StartWidth;

            while (fullCycleDuration >= 0)
            {
                float oneNewWidth;
                int divisionResidue = fullCycleDuration % oneLoadDuration;

                if (divisionResidue == 0)
                {
                    if (!isFirstCycle)
                    {
                        SetNameAndAnimate(_oneProgressBar, oneLoadPreviousWidth, oneNewWidth = Width, _oneTimerLabel,
                            ProducedText);
                    }
                    else
                    {
                        isFirstCycle = false;
                        SetNameAndAnimate(_oneProgressBar, oneLoadPreviousWidth, oneNewWidth = StartWidth,
                            _oneTimerLabel,
                            oneLoadDuration + SecondsText);
                    }
                }
                else
                {
                    SetNameAndAnimate(_oneProgressBar, oneLoadPreviousWidth,
                        oneNewWidth = Mathf.Abs((Width / oneLoadDuration * divisionResidue) - Width), _oneTimerLabel,
                        divisionResidue + SecondsText);
                }

                float fullNewWidth;
                if (fullCycleDuration != 0)
                {
                    SetNameAndAnimate(_fullProgressBar, fullLoadPreviousWidth,
                        fullNewWidth = Mathf.Abs((Width / tempFullCycleDuration * fullCycleDuration) - Width),
                        _fullTimerLabel,
                        fullCycleDuration + SecondsText);
                }
                else
                {
                    SetNameAndAnimate(_fullProgressBar, fullLoadPreviousWidth, fullNewWidth = Width,
                        _fullTimerLabel, ProducedText);
                }

                Debug.LogWarning("im running = " + oneNewWidth + " / " + fullNewWidth);

                oneLoadPreviousWidth = oneNewWidth;
                fullLoadPreviousWidth = fullNewWidth;

                fullCycleDuration -= 1;
                await Task.Delay(1000);
            }

            await Task.Delay(500);

            ClearNameAndAnimate(_fullProgressBar, Width, StartWidth, _fullTimerLabel);
            ClearNameAndAnimate(_oneProgressBar, Width, StartWidth, _oneTimerLabel);

            _isTimerRunning = false;
        }

        private static void ClearNameAndAnimate(VisualElement progressBar, float from, float to, Label label)
        {
            progressBar.experimental.animation.Start(
                new StyleValues { width = from },
                new StyleValues { width = to },
                AnimationDuration);

            label.text = EmptyText;
        }


        private static void SetNameAndAnimate(VisualElement progressBar, float fromWidth, float toWidth, Label label,
            string text)
        {
            progressBar.experimental.animation.Start(
                new StyleValues { width = fromWidth },
                new StyleValues { width = toWidth },
                AnimationDuration);

            label.text = text;
        }
    }
}