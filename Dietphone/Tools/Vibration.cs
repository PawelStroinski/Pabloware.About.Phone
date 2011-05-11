using System;
using Microsoft.Devices;

namespace Dietphone.Tools
{
    public interface Vibration
    {
        void VibrateOnButtonPress();
    }

    public class VibrationImpl : Vibration
    {
        private readonly VibrateController vibration = VibrateController.Default;
        private const int BUTTON_PRESS_MILLISECONDS = 100;

        public void VibrateOnButtonPress()
        {
            VibrateMiliseconds(BUTTON_PRESS_MILLISECONDS);
        }

        private void VibrateMiliseconds(int miliseconds)
        {
            var duration = TimeSpan.FromMilliseconds(miliseconds);
            vibration.Start(duration);
        }
    }
}