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

using MediaPortal.Backend.MediaLibrary;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Plugins.MP2Extended.Attributes;
using MediaPortal.Plugins.MP2Extended.Common;
using System;
using System.Threading.Tasks;
using MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.Music.BaseClasses;
using MediaPortal.Plugins.MP2Extended.Exceptions;
using MediaPortal.Plugins.MP2Extended.Controllers.Contexts;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.Playlist
{
  [ApiFunctionDescription(Type = ApiFunctionDescription.FunctionType.Json, Summary = "")]
  [ApiFunctionParam(Name = "playlistId", Type = typeof(string), Nullable = false)]
  internal class RemovePlaylistItems : BaseMusicTrackBasic
  {
    public static Task<WebBoolResult> ProcessAsync(RequestContext context, string playlistId, string positions)
    {
      // get the playlist
      PlaylistRawData playlistRawData = ServiceRegistration.Get<IMediaLibrary>().ExportPlaylist(Guid.Parse(playlistId));

      string[] splitIds = positions.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string p in splitIds)
      {
        int position = int.Parse(p);

        if (position < 0 || position >= playlistRawData.MediaItemIds.Count)
          throw new BadRequestException(string.Format("Index out of bound for removing playlist item: {0}", position));

        playlistRawData.MediaItemIds.RemoveAt(position);
      }

      // save playlist
      ServiceRegistration.Get<IMediaLibrary>().SavePlaylist(playlistRawData);

      return System.Threading.Tasks.Task.FromResult(new WebBoolResult { Result = true });
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}
