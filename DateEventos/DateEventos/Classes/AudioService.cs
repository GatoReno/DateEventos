using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DateEventos.Classes
{
    public class AudioService
    {
        public void Play() {
            DependencyService.Get<IAudio>().PlayAudioFile("softAlarm.mp3");
        }
       
    }
}
