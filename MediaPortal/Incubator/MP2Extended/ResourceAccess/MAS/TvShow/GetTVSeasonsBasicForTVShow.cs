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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaPortal.Backend.MediaLibrary;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Common.MediaManagement.MLQueries;
using MediaPortal.Plugins.MP2Extended.Attributes;
using MediaPortal.Plugins.MP2Extended.Common;
using MediaPortal.Plugins.MP2Extended.Exceptions;
using MediaPortal.Plugins.MP2Extended.MAS.TvShow;
using MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.TvShow.BaseClasses;
using MediaPortal.Plugins.MP2Extended.Controllers.Contexts;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.TvShow
{
  // TODO: Add more details
  [ApiFunctionDescription(Type = ApiFunctionDescription.FunctionType.Json, Summary = "")]
  [ApiFunctionParam(Name = "id", Type = typeof(Guid), Nullable = false)]
  [ApiFunctionParam(Name = "sort", Type = typeof(WebSortField), Nullable = true)]
  [ApiFunctionParam(Name = "order", Type = typeof(WebSortOrder), Nullable = true)]
  internal class GetTVSeasonsBasicForTVShow : BaseTvSeasonBasic
  {
    public static Task<IList<WebTVSeasonBasic>> ProcessAsync(RequestContext context, string id, string filter, WebSortField? sort, WebSortOrder? order)
    {
      // Get all seasons for this series
      IList<MediaItem> seasons = MediaLibraryAccess.GetMediaItemsByGroup(context, SeasonAspect.ROLE_SEASON, SeriesAspect.ROLE_SERIES, Guid.Parse(id), BasicNecessaryMIATypeIds, BasicOptionalMIATypeIds);

      if (seasons.Count == 0)
        return Task.FromResult<IList<WebTVSeasonBasic>>(new List<WebTVSeasonBasic>());

      var output = new List<WebTVSeasonBasic>();

      foreach (var season in seasons)
        output.Add(TVSeasonBasic(context, season, Guid.Parse(id)));

      // sort
      if (sort != null && order != null)
        output = output.SortWebTVSeasonBasic(sort, order).ToList();

      return Task.FromResult<IList<WebTVSeasonBasic>>(output);
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}
