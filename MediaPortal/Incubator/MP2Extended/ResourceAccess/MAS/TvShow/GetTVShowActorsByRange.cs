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

using MediaPortal.Plugins.MP2Extended.Attributes;
using MediaPortal.Plugins.MP2Extended.Common;
using MediaPortal.Plugins.MP2Extended.Extensions;
using MediaPortal.Plugins.MP2Extended.MAS.General;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaPortal.Plugins.MP2Extended.Controllers.Contexts;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.MAS.TvShow
{
  [ApiFunctionDescription(Type = ApiFunctionDescription.FunctionType.Json, Summary = "")]
  [ApiFunctionParam(Name = "start", Type = typeof(string), Nullable = false)]
  [ApiFunctionParam(Name = "end", Type = typeof(string), Nullable = false)]
  [ApiFunctionParam(Name = "filter", Type = typeof(string), Nullable = true)]
  [ApiFunctionParam(Name = "sort", Type = typeof(WebSortField), Nullable = true)]
  [ApiFunctionParam(Name = "order", Type = typeof(WebSortOrder), Nullable = true)]
  internal class GetTVShowActorsByRange
  {
    public static async Task<IList<WebActor>> ProcessAsync(RequestContext context, int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      IEnumerable<WebActor> output = await GetTVShowActors.ProcessAsync(context, filter, sort, order);

      output.TakeRange(start, end);
      return output.ToList();
    }
  }
}
