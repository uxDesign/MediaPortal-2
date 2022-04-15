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

using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.Services.ResourceAccess;
using MediaPortal.Plugins.MP2Extended.Common;
using MediaPortal.Plugins.MP2Extended.Controllers.Interfaces;
using MediaPortal.Plugins.MP2Extended.MAS.General;
using MediaPortal.Plugins.MP2Extended.WSS;
using MediaPortal.Plugins.MP2Extended.WSS.General;
using MediaPortal.Plugins.MP2Extended.WSS.Profiles;
using MediaPortal.Plugins.MP2Extended.WSS.StreamInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaPortal.Plugins.MP2Extended.Controllers.json
{
  [ApiController]
  [Route("MPExtended/StreamingService/json/[action]")]
  [MediaPortalAuthorize]
  public class StreamingServiceController : Controller, IStreamingServiceController
  {
    #region General

    [HttpGet]
    [ApiExplorerSettings]
    [AllowAnonymous]
    public Task<WebStreamServiceDescription> GetServiceDescription()
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.General.GetServiceDescription.ProcessAsync(ControllerContext.HttpContext);
    }

    #endregion

    #region Profiles

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTranscoderProfile>> GetTranscoderProfiles()
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.Profiles.GetTranscoderProfiles.ProcessAsync(ControllerContext.HttpContext);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTranscoderProfile>> GetTranscoderProfilesForTarget(string target)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.Profiles.GetTranscoderProfilesForTarget.ProcessAsync(ControllerContext.HttpContext, target);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebTranscoderProfile> GetTranscoderProfileByName(string name)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.Profiles.GetTranscoderProfileByName.ProcessAsync(ControllerContext.HttpContext, name);
    }

    #endregion

    #region StreamInfo

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebMediaInfo> GetMediaInfo(string itemId, WebMediaType type)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.StreamInfo.GetMediaInfo.ProcessAsync(ControllerContext.HttpContext, itemId, type);
    }

    #endregion

    #region Control

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebTranscodingInfo> GetTranscodingInfo(string identifier, long? playerPosition)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.Control.GetTranscodingInfo.ProcessAsync(ControllerContext.HttpContext, identifier, playerPosition);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> InitStream(string itemId, string clientDescription, string identifier, WebMediaType type, int? idleTimeout)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.Control.InitStream.ProcessAsync(ControllerContext.HttpContext, itemId, clientDescription, identifier, type, idleTimeout);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebStringResult> StartStream(string identifier, string profileName, long startPosition)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.Control.StartStream.ProcessAsync(ControllerContext.HttpContext, identifier, profileName, startPosition);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebStringResult> StartStreamWithStreamSelection(string identifier, string profileName, long startPosition, int audioId, int subtitleId)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.Control.StartStreamWithStreamSelection.ProcessAsync(ControllerContext.HttpContext, identifier, profileName, startPosition, audioId, subtitleId);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> StopStream(string identifier)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.Control.StopStream.ProcessAsync(ControllerContext.HttpContext, identifier);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> FinishStream(string identifier)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.Control.FinishStream.ProcessAsync(ControllerContext.HttpContext, identifier);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebStreamingSession>> GetStreamingSessions(string filter = null)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.General.GetStreamingSessions.ProcessAsync(ControllerContext.HttpContext, filter);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebResolution> GetStreamSize(WebMediaType type, int? provider, string itemId, int? offset, string profile)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.General.GetStreamSize.ProcessAsync(ControllerContext.HttpContext, type, provider, itemId, offset, profile);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> AuthorizeStreaming()
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return Task.FromResult(new WebBoolResult { Result = true });
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> AuthorizeRemoteHostForStreaming(string host)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return Task.FromResult(new WebBoolResult { Result = true });
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebItemSupportStatus> GetItemSupportStatus(WebMediaType type, int? provider, string itemId, int? offset)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.General.GetItemSupportStatus.ProcessAsync(ControllerContext.HttpContext, type, provider, itemId, offset);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> RequestImageResize(WebMediaType mediatype, int? provider, string id, WebFileType imagetype, int offset, int maxWidth, int maxHeight, string borders = null, string format = null)
    {
      Logger.Debug("WSS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.WSS.json.General.RequestImageResize.ProcessAsync(ControllerContext.HttpContext, mediatype, provider, id, imagetype, offset, maxWidth, maxHeight, borders, format);
    }

    #endregion

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}
