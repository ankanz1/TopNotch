using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

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
            var songTitle = this.GetTemplateChild("SongTitle") as TextBlock;
            if (songTitle != null)
            {
                songTitle.SetValue(TextBlock.TextProperty, "");
            }
            var artistAlbum = this.GetTemplateChild("ArtistAlbum") as TextBlock;
            if (artistAlbum != null)
            {
                artistAlbum.SetValue(TextBlock.TextProperty, "");
            }
            HasArtwork = false;
            ArtworkPath = "";
            return;
        }

        var songTitle2 = this.GetTemplateChild("SongTitle") as TextBlock;
        if (songTitle2 != null)
        {
            songTitle2.SetValue(TextBlock.TextProperty, Media.Title);
        }
        var artistAlbum2 = this.GetTemplateChild("ArtistAlbum") as TextBlock;
        if (artistAlbum2 != null)
        {
            artistAlbum2.SetValue(TextBlock.TextProperty, Media.Artist ?? "");
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