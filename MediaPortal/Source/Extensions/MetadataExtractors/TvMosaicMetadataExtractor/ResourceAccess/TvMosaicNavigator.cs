﻿#region Copyright (C) 2007-2021 Team MediaPortal

/*
    Copyright (C) 2007-2021 Team MediaPortal
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

using MediaPortal.Common;
using MediaPortal.Common.Settings;
using MediaPortal.Plugins.ServerSettings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvMosaic.API;
using TvMosaic.Shared;

namespace TvMosaicMetadataExtractor.ResourceAccess
{
  /// <summary>
  /// Default implementation of <see cref="ITvMosaicNavigator"/> for navigating the TvMosaic API.
  /// </summary>
  public class TvMosaicNavigator : ITvMosaicNavigator
  {
    /// <summary>
    /// Id of the container containing recorded TV items sorted alphabetically
    /// </summary>
    public const string RECORDED_TV_OBJECT_ID = "8F94B459-EFC0-4D91-9B29-EC3D72E92677:E44367A7-6293-4492-8C07-0E551195B99F";

    protected HttpDataProvider _httpDataProvider;

    public ICollection<string> GetRootContainerIds()
    {
      return new List<string>
      {
        // Currently only the recorded TV container seems logical to include
        RECORDED_TV_OBJECT_ID,
      };
    }

    public async Task<IList<RecordedTV>> GetChildItemsAsync(string containerId)
    {
      return (await GetObjectResponseAsync(containerId, true).ConfigureAwait(false))?.Items;
    }

    public async Task<RecordedTV> GetItemAsync(string itemId)
    {
      return (await GetObjectResponseAsync(itemId, false).ConfigureAwait(false))?.Items?.FirstOrDefault();
    }

    public async Task<bool> ObjectExistsAsync(string objectId)
    {
      return (await GetObjectResponseAsync(objectId, false).ConfigureAwait(false)) != null;
    }

    public async Task<bool> RemoveObject(string objectId)
    {
      HttpDataProvider httpDataProvider = GetHttpDataProvider();
      var response = await httpDataProvider.RemoveObject(new ObjectRemover(objectId)).ConfigureAwait(false);
      return response.Status == StatusCode.STATUS_OK;
    }

    /// <summary>
    /// Asynchronously retrieves the object with the specified id, or it's child items.
    /// </summary>
    /// <param name="objectId">The id of the object to retrieve.</param>
    /// <param name="childrenRequest">Whether to request the object's child items.</param>
    /// <returns>The object or it's child items.</returns>
    protected async Task<ObjectResponse> GetObjectResponseAsync(string objectId, bool childrenRequest)
    {
      ObjectRequester request = new ObjectRequester
      {
        ObjectID = objectId,
        ChildrenRequest = childrenRequest
      };

      HttpDataProvider httpDataProvider = GetHttpDataProvider();

      var response = await httpDataProvider.GetObject(request);
      if (response.Status != StatusCode.STATUS_OK)
        return null;
      return response.Result;
    }

    public Task<string> GetObjectFriendlyNameAsync(string objectId)
    {
      if (objectId == RECORDED_TV_OBJECT_ID)
        return Task.FromResult("TvMosaic Recorded TV");
      //ToDo: we could retrieve the actual name of the object from the API, but for now we avoid an additional network request.
      return Task.FromResult(objectId);
    }

    HttpDataProvider GetHttpDataProvider()
    {
      if (_httpDataProvider != null)
        return _httpDataProvider;

      // There's a potential race condition when checking whether to create a new instance if the class
      // reference was null, but it will just cause another instance to be constructed unnecessarily and
      // won't effect usage so just allow it and avoid a lock.
      TvMosaicProviderSettings settings = GetSettings();
      _httpDataProvider = new HttpDataProvider(settings.Host, settings.Port, settings.Username ?? string.Empty, settings.Password ?? string.Empty);
      return _httpDataProvider;
    }

    protected TvMosaicProviderSettings GetSettings()
    {
      // If running on the client we need to load the settings from the server
      IServerSettingsClient serverSettings = ServiceRegistration.Get<IServerSettingsClient>(false);
      if (serverSettings != null)
        return serverSettings.Load<TvMosaicProviderSettings>();
      // We are running on the server so can load the settings locally
      return ServiceRegistration.Get<ISettingsManager>().Load<TvMosaicProviderSettings>();
    }
  }
}
