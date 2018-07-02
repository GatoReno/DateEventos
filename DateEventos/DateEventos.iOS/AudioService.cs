using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AVFoundation;
using DateEventos.iOS;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(AudioService))]

namespace DateEventos.iOS
{
    public class AudioService : IAudio
    {
        public AudioService()
        {
        }

        public void PlayAudioFile(string fileName)
        {
            string sFilePath = NSBundle.MainBundle.PathForResource
            (Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
            var url = NSUrl.FromString(sFilePath);
            var _player = AVAudioPlayer.FromUrl(url);
            _player.FinishedPlaying += (object sender, AVStatusEventArgs e) => {
                _player = null;
            };
            _player.Play();
        }
    }
}