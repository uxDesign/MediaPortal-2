﻿using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Plugins.MP2Extended.Attributes;
using MediaPortal.Plugins.MP2Extended.Exceptions;
using MediaPortal.Plugins.MP2Extended.MAS.Music;
using MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.Music.BaseClasses;
using System;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.Music
{
  [ApiFunctionDescription(Type = ApiFunctionDescription.FunctionType.Json, Summary = "")]
  [ApiFunctionParam(Name = "id", Type = typeof(string), Nullable = false)]
  internal class GetMusicTrackDetailedById : BaseMusicTrackDetailed
  {
    public WebMusicTrackDetailed Process(Guid id)
    {
      MediaItem item = GetMediaItems.GetMediaItemById(id, BasicNecessaryMIATypeIds, BasicOptionalMIATypeIds);

      if (item == null)
        throw new BadRequestException($"No track found with id {id}");

      return MusicTrackDetailed(item);
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}
