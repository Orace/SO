using System;
using System.Windows;
using LibVLCSharp.Shared;

namespace SO_71919433;

public partial class MainWindow
{
    private readonly LibVLC _libVlc;
    private readonly MediaPlayer _mediaPlayer;

    public MainWindow()
    {
        _libVlc = new LibVLC();
        _mediaPlayer = new MediaPlayer(_libVlc);
        InitializeComponent();

        VideoView.Loaded += VideoViewOnLoaded;
    }

    private void VideoViewOnLoaded(object sender, RoutedEventArgs e)
    {
        VideoView.MediaPlayer = _mediaPlayer;
    }

    private void PlayButtonOnClick(object sender, RoutedEventArgs e)
    {
        using var media = new Media(_libVlc, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"));
        VideoView.MediaPlayer?.Play(media);
    }
}