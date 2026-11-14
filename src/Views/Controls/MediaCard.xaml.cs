using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Globalization;

namespace TopNotch.Views.Controls;

/// <summary>
/// Media card control for displaying now playing information
/// </summary>
public partial class MediaCard : UserControl
{
    public MediaCard()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static readonly StyledProperty<Models.MediaInfo?> MediaProperty =
        AvaloniaProperty.Register<MediaCard, Models.MediaInfo?>(nameof(Media), null, OnMediaChanged);

    public Models.MediaInfo? Media
    {
        get => GetValue(MediaProperty);
        set => SetValue(MediaProperty, value);
    }

    private static void OnMediaChanged(Avalonia.AvaloniaObject d, AvaloniaProperty e, object? old, object? newvalue)
    {
        var control = (MediaCard)d;
        control.UpdateContent();
    }

    private void UpdateContent()
    {
        if (Media == null)
        {
            if (FindName("SongTitle") is TextBlock songTitle)
            {
                songTitle.SetValue(TextBlock.TextProperty, "");
            }
            if (FindName("ArtistAlbum") is TextBlock artistAlbum)
            {
                artistAlbum.SetValue(TextBlock.TextProperty, "");
            }
            HasArtwork = false;
            ArtworkPath = "";
            return;
        }

        if (FindName("SongTitle") is TextBlock songTitle)
        {
            songTitle.SetValue(TextBlock.TextProperty, Media.Title);
        }
        if (FindName("ArtistAlbum") is TextBlock artistAlbum)
        {
            artistAlbum.SetValue(TextBlock.TextProperty, Media.Artist ?? "");
        }
        HasArtwork = Media.HasArtwork;
        ArtworkPath = Media.ArtworkPath;
    }

    public static readonly StyledProperty<bool> HasArtworkProperty =
        AvaloniaProperty.Register<MediaCard, bool>(nameof(HasArtwork), false);

    public bool HasArtwork
    {
        get => GetValue(HasArtworkProperty);
        set => SetValue(HasArtworkProperty, value);
    }

    public static readonly StyledProperty<string> ArtworkPathProperty =
        AvaloniaProperty.Register<MediaCard, string>(nameof(ArtworkPath), string.Empty);

    public string ArtworkPath
    {
        get => GetValue(ArtworkPathProperty);
        set => SetValue(ArtworkPathProperty, value);
    }
}