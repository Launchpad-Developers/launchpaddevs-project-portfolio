using Android.Media;
using System.Threading.Tasks;
using Xamarin.Forms;
using ASCommonServices.Interfaces;
using ASWorkoutTracker.Android.Services;

[assembly: Dependency(typeof(AudioService_Droid))]
namespace ASWorkoutTracker.Android.Services
{
    public class AudioService_Droid : IAudioService
    {
        private MediaPlayer _mediaPlayer;
        public async Task PlayAudioFile(string fileName, int? repeat)
        {
            _mediaPlayer = new MediaPlayer();
            var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);

            if (fd == null)
                return;

            _mediaPlayer.Prepared += (s, e) =>
            {
                _mediaPlayer.Start();
            };
            await _mediaPlayer.SetDataSourceAsync(fd.FileDescriptor, fd.StartOffset, fd.Length);
            _mediaPlayer.Prepare();
        }
    }
}
