using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Media_Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MediaPlayerContext dbContext = new MediaPlayerContext();

            Query query = new Query(dbContext);

            listPlaylist.ItemsSource = query.GetAllPlaylist();

            DispatcherTimer timer = new();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (myMediaElement.Source != null && myMediaElement.NaturalDuration.HasTimeSpan)
            {
                var playTime = myMediaElement.NaturalDuration.TimeSpan;
                var timeElapsed = myMediaElement.Position;
                mediaTime.Content = timeElapsed.ToString(@"hh\:mm\:ss");
                mediaDuration.Content = playTime.ToString(@"hh\:mm\:ss");
                lectureSlider.Value = timeElapsed.TotalSeconds;
            }
        }

        private void btnCreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            String playlistName = Interaction.InputBox("Playlist", "nom de la playlist");

            if ( !string.IsNullOrEmpty(playlistName)) 
            {
                MediaPlayerContext dbContext = new MediaPlayerContext();

                Query query = new Query(dbContext);

                query.CreatePlaylist(playlistName);

                listPlaylist.ItemsSource = query.GetAllPlaylist();

            }


        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Playlist playlist = (Playlist)listPlaylist.SelectedItem;

            if( playlist != null )
            {
                MediaPlayerContext dbContext = new MediaPlayerContext();
                Query query = new Query(dbContext);

                listMedia.ItemsSource = playlist.Medias;

            }
        }

        private void BtnAddMedia_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichiers audio et vidéo|*.mp3;*.wav;*.wma;*.aac;*.flac;*.m4a;*.ogg;*.mp4;*.avi;*.mkv;*.mov;*.wmv";
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {

                Playlist playlist = (Playlist)listPlaylist.SelectedItem;
                if (playlist == null)
                {
                    MessageBox.Show("Creer ou selectionner une playlist", "Erreur");
                    return;
                }

                string selectedFile = openFileDialog.FileName;
                TagLib.File tagFile = TagLib.File.Create(selectedFile);

                string artist = tagFile.Tag.FirstAlbumArtist;
                string title = tagFile.Tag.Title;
                if (string.IsNullOrEmpty(artist))
                {
                    artist = "inconnue";
                }
                if (string.IsNullOrEmpty(title))
                {
                    title = "Inconnue";
                }

                MediaPlayerContext dbContext = new MediaPlayerContext();
                Query query = new Query(dbContext);

                Media media = new Media{Artiste = artist, Path = openFileDialog.FileName, Title = title};

                query.AddMedia(media, playlist.PlaylistId);

                var p = query.GetPlaylist(playlist.PlaylistId);

                listMedia.ItemsSource = p.Medias;

            }
            

        }

        private void listMedia_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Media media = (Media)listMedia.SelectedItem;

            if(media != null)
            {
                myMediaElement.Source = new Uri(media.Path, UriKind.Absolute);

                myMediaElement.Play();

                Title = media.Title;

            }
        }


        // Change the volume of the media.
        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            myMediaElement.Volume = (double)volumeSlider.Value;
        }

        // Change the speed of the media.
        private void ChangeMediaSpeedRatio(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            myMediaElement.SpeedRatio = (double)speedRatioSlider.Value;
        }

        // When the media opens, initialize the "Seek To" slider maximum value
        // to the total number of miliseconds in the length of the media clip.
        private void Element_MediaOpened(object sender, EventArgs e)
        {
            timelineSlider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;

            lectureSlider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
        }

        // When the media playback is finished. Stop() the media to seek to media start.
        private void Element_MediaEnded(object sender, EventArgs e)
        {
            myMediaElement.Stop();
        }

        // Jump to different parts of the media (seek to).
        private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            int SliderValue = (int)timelineSlider.Value;

            // Overloaded constructor takes the arguments days, hours, minutes, seconds, milliseconds.
            // Create a TimeSpan with miliseconds equal to the slider value.
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            myMediaElement.Position = ts;
        }

        void InitializePropertyValues()
        {
            // Set the media's starting Volume and SpeedRatio to the current value of the
            // their respective slider controls.
            myMediaElement.Volume = (double)volumeSlider.Value;
            myMediaElement.SpeedRatio = (double)speedRatioSlider.Value;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            myMediaElement.Play();
            InitializePropertyValues();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            myMediaElement.Stop();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            myMediaElement.Pause();
        }

        private void BtnDeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            Playlist playlist = (Playlist)listPlaylist.SelectedItem;
            if (playlist == null)
            {
                MessageBox.Show("Sélectionner une playlist à supprimer", "Erreur");
                return;
            }

            MediaPlayerContext dbContext = new MediaPlayerContext();
            Query query = new Query(dbContext);

            query.DeletePlaylist(playlist.PlaylistId);

            listPlaylist.ItemsSource = query.GetAllPlaylist();

        }
    }
}