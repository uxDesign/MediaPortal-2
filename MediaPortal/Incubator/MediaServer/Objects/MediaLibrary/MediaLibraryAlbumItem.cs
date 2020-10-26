#region Copyright (C) 2007-2020 Team MediaPortal

/*
    Copyright (C) 2007-2020 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System.Collections.Generic;
using System.Linq;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Common.MediaManagement.MLQueries;
using MediaPortal.Extensions.MediaServer.Profiles;
using MediaPortal.Utilities;

namespace MediaPortal.Extensions.MediaServer.Objects.MediaLibrary
{
  public class MediaLibraryAlbumItem : MediaLibraryContainer, IDirectoryMusicAlbum
  {
    public MediaLibraryAlbumItem(MediaItem item, EndPointSettings client)
      : base(item, NECESSARY_MUSIC_MIA_TYPE_IDS, OPTIONAL_MUSIC_MIA_TYPE_IDS, 
          new RelationshipFilter(AudioAspect.ROLE_TRACK, AudioAlbumAspect.ROLE_ALBUM, item.MediaItemId), client)
    {
      Genre = new List<string>();
      Artist = new List<string>();
      Contributor = new List<string>();

      if (MediaItemAspect.TryGetAspect(item.Aspects, AudioAlbumAspect.Metadata, out SingleMediaItemAspect albumAspect))
      {
        //Title = albumAspect.GetAttributeValue<string>(AudioAlbumAspect.ATTR_ALBUM);
        if (Client.Profile.Settings.Metadata.Delivery == MetadataDelivery.All)
        {
          IList<MultipleMediaItemAspect> genreAspects;
          if (MediaItemAspect.TryGetAspects(item.Aspects, GenreAspect.Metadata, out genreAspects))
          {
            CollectionUtils.AddAll(Genre, genreAspects.Select(g => g.GetAttributeValue<string>(GenreAspect.ATTR_GENRE)));
          }

          var artistObj = albumAspect.GetCollectionAttribute<object>(AudioAlbumAspect.ATTR_ARTISTS);
          if (artistObj != null)
            CollectionUtils.AddAll(Artist, artistObj.Cast<string>());

          Description = albumAspect.GetAttributeValue<string>(AudioAlbumAspect.ATTR_DESCRIPTION);
          LongDescription = Description;
        }
      }

      //Support alternative ways to get album art
      var albumArt = new MediaLibraryAlbumArt(item, Client);
      if (albumArt != null)
      {
        albumArt.Initialise();
        if (Client.Profile.Settings.Thumbnails.Delivery == ThumbnailDelivery.All || Client.Profile.Settings.Thumbnails.Delivery == ThumbnailDelivery.Resource)
        {
          var albumResource = new MediaLibraryAlbumArtResource(albumArt);
          albumResource.Initialise();
          Resources.Add(albumResource);
        }
        if (Client.Profile.Settings.Thumbnails.Delivery == ThumbnailDelivery.All || Client.Profile.Settings.Thumbnails.Delivery == ThumbnailDelivery.AlbumArt)
        {
          AlbumArtUrl = albumArt.Uri;
        }
      }
    }

    public override string Class
    {
      get { return "object.container.album.musicAlbum"; }
    }

    public string StorageMedium { get; set; }
    public string LongDescription { get; set; }
    public string Description { get; set; }
    public IList<string> Publisher { get; set; }
    public IList<string> Contributor { get; set; }
    public string Date { get; set; }
    public string Relation { get; set; }
    public IList<string> Rights { get; set; }
    public IList<string> Artist { get; set; }
    public IList<string> Genre { get; set; }
    public IList<string> Producer { get; set; }
    public string AlbumArtUrl { get; set; }
    public string Toc { get; set; }
  }
}
