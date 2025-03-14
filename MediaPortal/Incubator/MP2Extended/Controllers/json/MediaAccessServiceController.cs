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
using MediaPortal.Plugins.MP2Extended.Controllers.Contexts;
using MediaPortal.Plugins.MP2Extended.Controllers.Interfaces;
using MediaPortal.Plugins.MP2Extended.MAS;
using MediaPortal.Plugins.MP2Extended.MAS.FileSystem;
using MediaPortal.Plugins.MP2Extended.MAS.General;
using MediaPortal.Plugins.MP2Extended.MAS.Movie;
using MediaPortal.Plugins.MP2Extended.MAS.Music;
using MediaPortal.Plugins.MP2Extended.MAS.OnlineVideos;
using MediaPortal.Plugins.MP2Extended.MAS.Picture;
using MediaPortal.Plugins.MP2Extended.MAS.Playlist;
using MediaPortal.Plugins.MP2Extended.MAS.TvShow;
using MediaPortal.Plugins.MP2Extended.TAS.Tv;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
using System.Web.Http.Description;
#endif

namespace MediaPortal.Plugins.MP2Extended.Controllers.json
{
#if NET5_0_OR_GREATER
  [ApiController]
  [Route("MPExtended/MediaAccessService/json/[action]")]
  [MediaPortalAuthorize]
  public class MediaAccessServiceController : ControllerBase, IMediaAccessServiceController
  {
    protected RequestContext Context => ControllerContext.HttpContext.ToRequestContext();
#else
  [RoutePrefix("MPExtended/MediaAccessService/json")]
  [Route("{action}")]
  [MediaPortalAuthorize]
  public class MediaAccessServiceController : ApiController, IMediaAccessServiceController
  {
    protected RequestContext Context => Request.GetOwinContext().ToRequestContext();
#endif
    #region General

    [HttpGet]
    [ApiExplorerSettings]
    [AllowAnonymous]
    public Task<WebMediaServiceDescription> GetServiceDescription()
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.General.GetServiceDescription.ProcessAsync(Context);
    }

    [HttpGet]
    [ApiExplorerSettings]
    [AllowAnonymous]
    public Task<WebBoolResult> TestConnection()
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return Task.FromResult(new WebBoolResult { Result = true });
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebMediaItem> GetMediaItem(WebMediaType type, string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.General.GetMediaItem.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebDictionary<string>> GetExternalMediaInfo(WebMediaType type, string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.General.GetExternalMediaInfo.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebDiskSpaceInformation>> GetLocalDiskInformation(string filter)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.General.GetLocalDiskInformation.ProcessAsync(Context);
    }

    #endregion

    #region Movies

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetMovieCount()
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMovieCount.ProcessAsync(Context);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMovieBasic>> GetMoviesBasic(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMoviesBasic.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMovieDetailed>> GetMoviesDetailed(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMoviesDetailed.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMovieBasic>> GetMoviesBasicByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMoviesBasicByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMovieDetailed>> GetMoviesDetailedByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMoviesDetailedByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebMovieBasic> GetMovieBasicById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMovieBasicById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebMovieDetailed> GetMovieDetailedById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMovieDetailedById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebGenre>> GetMovieGenres(WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMovieGenres.ProcessAsync(Context, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebGenre>> GetMovieGenresByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMovieGenresByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetMovieGenresCount(string filter)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMovieGenresCount.ProcessAsync(Context, filter);
    }

    //[HttpGet]
    //[ApiExplorerSettings]
    //public Task<IList<WebCategory>> GetMovieCategories(string filter, WebSortField? sort, WebSortOrder? order)
    //{
    //  Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());

    //}

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebActor>> GetMovieActors(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMovieActors.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebActor>> GetMovieActorsByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMovieActorsByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetMovieActorCount(string filter)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Movie.GetMovieActorCount.ProcessAsync(Context, filter);
    }

    #endregion

    #region Music

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetMusicAlbumCount(string filter)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicAlbumCount.ProcessAsync(Context, filter);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicAlbumBasic>> GetMusicAlbumsBasic(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicAlbumsBasic.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicAlbumBasic>> GetMusicAlbumsBasicByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicAlbumsBasicByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicAlbumBasic>> GetMusicAlbumsBasicForArtist(string id, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicAlbumsBasicForArtist.ProcessAsync(Context, id, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebMusicAlbumBasic> GetMusicAlbumBasicById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicAlbumBasicById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetMusicArtistCount(string filter)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicArtistCount.ProcessAsync(Context, filter);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicArtistBasic>> GetMusicArtistsBasic(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicArtistsBasic.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicArtistBasic>> GetMusicArtistsBasicByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicArtistsBasicByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebMusicArtistBasic> GetMusicArtistBasicById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicArtistBasicById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicArtistDetailed>> GetMusicArtistsDetailed(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicArtistsDetailed.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicArtistDetailed>> GetMusicArtistsDetailedByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicArtistsDetailedByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebMusicArtistDetailed> GetMusicArtistDetailedById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicArtistDetailedById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetMusicTrackCount(string filter)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicTrackCount.ProcessAsync(Context, filter);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicTrackBasic>> GetMusicTracksBasic(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicTracksBasic.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicTrackDetailed>> GetMusicTracksDetailed(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicTracksDetailed.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicTrackBasic>> GetMusicTracksBasicByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicTracksBasicByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicTrackDetailed>> GetMusicTracksDetailedByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicTracksDetailedByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicTrackBasic>> GetMusicTracksBasicForAlbum(string id, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicTracksBasicForAlbum.ProcessAsync(Context, id, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicTrackBasic>> GetMusicTracksBasicForArtist(string id, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicTracksBasicForArtist.ProcessAsync(Context, id, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicTrackDetailed>> GetMusicTracksDetailedForAlbum(string id, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicTracksDetailedForAlbum.ProcessAsync(Context, id, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebMusicTrackDetailed>> GetMusicTracksDetailedForArtist(string id, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicTracksDetailedForArtist.ProcessAsync(Context, id, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebMusicTrackBasic> GetMusicTrackBasicById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicTrackBasicById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebMusicTrackDetailed> GetMusicTrackDetailedById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicTrackDetailedById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebGenre>> GetMusicGenres(WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicGenres.ProcessAsync(Context, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebGenre>> GetMusicGenresByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicGenresByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetMusicGenresCount(string filter)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Music.GetMusicGenresCount.ProcessAsync(Context, filter);
    }

    //[HttpGet]
    //[ApiExplorerSettings]
    //public Task<IList<WebCategory>> GetMusicCategories(string filter)
    //{
    //  Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      
    //}

    #endregion

    #region OnlineVideos

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebOnlineVideosVideo>> GetOnlineVideosCategoryVideos(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.OnlineVideos.GetOnlineVideosCategoryVideos.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebOnlineVideosGlobalSite>> GetOnlineVideosGlobalSites(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.OnlineVideos.GetOnlineVideosGlobalSites.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebOnlineVideosSiteCategory>> GetOnlineVideosSiteCategories(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.OnlineVideos.GetOnlineVideosSiteCategories.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebOnlineVideosSite>> GetOnlineVideosSites(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.OnlineVideos.GetOnlineVideosSites.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebOnlineVideosSiteSetting>> GetOnlineVideosSiteSettings(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.OnlineVideos.GetOnlineVideosSiteSettings.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebOnlineVideosSiteCategory>> GetOnlineVideosSubCategories(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.OnlineVideos.GetOnlineVideosSubCategories.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<string>> GetOnlineVideosVideoUrls(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.OnlineVideos.GetOnlineVideosVideoUrls.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> SetOnlineVideosSiteSetting(string siteId, string property, string value)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.OnlineVideos.SetOnlineVideosSiteSetting.ProcessAsync(Context, siteId, property, value);
    }

    #endregion

    #region Pictures

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetPictureCount()
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Picture.GetPictureCount.ProcessAsync(Context);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebPictureBasic>> GetPicturesBasic(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Picture.GetPicturesBasic.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebPictureBasic>> GetPicturesBasicByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Picture.GetPicturesBasicByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebPictureDetailed>> GetPicturesDetailed(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Picture.GetPicturesDetailed.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebPictureDetailed>> GetPicturesDetailedByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Picture.GetPicturesDetailedByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebPictureBasic> GetPictureBasicById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Picture.GetPictureBasicById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebPictureDetailed> GetPictureDetailedById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Picture.GetPictureDetailedById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebCategory>> GetPictureCategories()
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Picture.GetPictureCategories.ProcessAsync(Context);
    }

    //[HttpGet]
    //[ApiExplorerSettings]
    //public Task<IList<WebCategory>> GetPictureSubCategories(string id, string filter)
    //{
    //  Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      
    //}

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebPictureBasic>> GetPicturesBasicByCategory(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Picture.GetPicturesBasicByCategory.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebPictureDetailed>> GetPicturesDetailedByCategory(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Picture.GetPicturesDetailedByCategory.ProcessAsync(Context, id);
    }

    #endregion

    #region TVShows

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetTVEpisodeCount()
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodeCount.ProcessAsync(Context);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetTVEpisodeCountForTVShow(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodeCountForTVShow.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetTVEpisodeCountForSeason(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodeCountForSeason.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetTVShowCount(string filter)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowCount.ProcessAsync(Context, filter);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetTVSeasonCountForTVShow(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVSeasonCountForTVShow.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVShowBasic>> GetTVShowsBasic(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowsBasic.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVShowDetailed>> GetTVShowsDetailed(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowsDetailed.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVShowBasic>> GetTVShowsBasicByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowsBasicByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVShowDetailed>> GetTVShowsDetailedByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowsDetailedRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebTVShowBasic> GetTVShowBasicById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowBasicById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebTVShowDetailed> GetTVShowDetailedById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowDetailedById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVSeasonBasic>> GetTVSeasonsBasicForTVShow(string id, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVSeasonsBasicForTVShow.ProcessAsync(Context, id, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVSeasonDetailed>> GetTVSeasonsDetailedForTVShow(string id, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVSeasonsDetailedForTVShow.ProcessAsync(Context, id, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebTVSeasonBasic> GetTVSeasonBasicById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVSeasonBasicById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebTVSeasonDetailed> GetTVSeasonDetailedById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVSeasonDetailedById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVEpisodeBasic>> GetTVEpisodesBasic(WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodesBasic.ProcessAsync(Context, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVEpisodeDetailed>> GetTVEpisodesDetailed(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodesDetailed.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVEpisodeBasic>> GetTVEpisodesBasicByRange(int start, int end, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodesBasicByRange.ProcessAsync(Context, start, end, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVEpisodeDetailed>> GetTVEpisodesDetailedByRange(int start, int end, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodesDetailedByRange.ProcessAsync(Context, start, end, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVEpisodeBasic>> GetTVEpisodesBasicForTVShow(string id, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodesBasicForTVShow.ProcessAsync(Context, id, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVEpisodeDetailed>> GetTVEpisodesDetailedForTVShow(string id, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodesDetailedForTVShow.ProcessAsync(Context, id, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVEpisodeBasic>> GetTVEpisodesBasicForTVShowByRange(string id, int start, int end, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodesBasicForTVShowByRange.ProcessAsync(Context, id, start, end, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVEpisodeDetailed>> GetTVEpisodesDetailedForTVShowByRange(string id, int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodesDetailedForTVShowByRange.ProcessAsync(Context, id, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVEpisodeBasic>> GetTVEpisodesBasicForSeason(string id, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodesBasicForSeason.ProcessAsync(Context, id, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebTVEpisodeDetailed>> GetTVEpisodesDetailedForSeason(string id, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodesDetailedForSeason.ProcessAsync(Context, id, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebTVEpisodeBasic> GetTVEpisodeBasicById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodeBasicById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebTVEpisodeDetailed> GetTVEpisodeDetailedById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVEpisodeDetailedById.ProcessAsync(Context, id);
    }

    //[HttpGet]
    //[ApiExplorerSettings]
    //public Task<IList<WebCategory>> GetTVShowCategories(string filter)
    //{
    //  Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      
    //}

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebGenre>> GetTVShowGenres(WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowGenres.ProcessAsync(Context, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebGenre>> GetTVShowGenresByRange(int start, int end, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowGenresByRange.ProcessAsync(Context, start, end, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetTVShowGenresCount()
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowGenresCount.ProcessAsync(Context);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebActor>> GetTVShowActors(string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowActors.ProcessAsync(Context, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebActor>> GetTVShowActorsByRange(int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowActorsByRange.ProcessAsync(Context, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetTVShowActorCount(string filter)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.TvShow.GetTVShowActorCount.ProcessAsync(Context, filter);
    }

    #endregion

    #region Filesystem

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetFileSystemDriveCount()
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemDriveCount.ProcessAsync(Context);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebDriveBasic>> GetFileSystemDrives(WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemDrives.ProcessAsync(Context, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebDriveBasic>> GetFileSystemDrivesByRange(int start, int end, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemDrivesByRange.ProcessAsync(Context, start, end, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebFolderBasic>> GetFileSystemFolders(string id, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemFolders.ProcessAsync(Context, id, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebFolderBasic>> GetFileSystemFoldersByRange(string id, int start, int end, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemFoldersByRange.ProcessAsync(Context, id, start, end, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebFileBasic>> GetFileSystemFiles(string id, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemFiles.ProcessAsync(Context, id, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebFileBasic>> GetFileSystemFilesByRange(string id, int start, int end, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemFilesByRange.ProcessAsync(Context, id, start, end, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebFilesystemItem>> GetFileSystemFilesAndFolders(string id, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemFilesAndFolders.ProcessAsync(Context, id, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebFilesystemItem>> GetFileSystemFilesAndFoldersByRange(string id, int start, int end, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemFilesAndFoldersByRange.ProcessAsync(Context, id, start, end, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetFileSystemFilesAndFoldersCount(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemFilesAndFoldersCount.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetFileSystemFilesCount(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemFilesCount.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetFileSystemFoldersCount(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemFoldersCount.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebDriveBasic> GetFileSystemDriveBasicById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemDriveBasicById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebFolderBasic> GetFileSystemFolderBasicById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemFolderBasicById.ProcessAsync(Context, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebFileBasic> GetFileSystemFileBasicById(string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.FileSystem.GetFileSystemFileBasicById.ProcessAsync(Context, id);
    }

    #endregion

    #region Files

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebArtwork>> GetArtwork(WebMediaType type, string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.General.GetArtwork.ProcessAsync(Context, type, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<string>> GetPathList(WebMediaType mediatype, WebFileType filetype, string id)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.General.GetPathList.ProcessAsync(Context, mediatype, filetype, id);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebFileInfo> GetFileInfo(WebMediaType mediatype, WebFileType filetype, string id, int offset)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.General.GetFileInfo.ProcessAsync(Context, mediatype, filetype, id, offset);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> IsLocalFile(WebMediaType mediatype, WebFileType filetype, string id, int offset)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.General.IsLocalFile.ProcessAsync(Context, mediatype, filetype, id, offset);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<Stream> RetrieveFile(WebMediaType mediatype, WebFileType filetype, string id, int offset)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.General.RetrieveFile.ProcessAsync(Context, mediatype, filetype, id, offset);
    }

    #endregion

    #region Playlist

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebPlaylist>> GetPlaylists()
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.GetPlaylists.ProcessAsync(Context);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebPlaylistItem>> GetPlaylistItems(string playlistId, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.GetPlaylistItems.ProcessAsync(Context, playlistId, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebPlaylistItem>> GetPlaylistItemsByRange(string playlistId, int start, int end, string filter, WebSortField? sort, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.GetPlaylistItemsByRange.ProcessAsync(Context, playlistId, start, end, filter, sort, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetPlaylistItemsCount(string playlistId, string filter)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.GetPlaylistItemsCount.ProcessAsync(Context, playlistId, filter);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> AddPlaylistItem(string playlistId, WebMediaType type, string id, int? position)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.AddPlaylistItem.ProcessAsync(Context, playlistId, type, id, position);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> ClearAndAddPlaylistItems(string playlistId, WebMediaType type, int? position, List<string> ids)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.ClearAndAddPlaylistItems.ProcessAsync(Context, playlistId, type, position, ids);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> AddPlaylistItems(string playlistId, WebMediaType type, int? position, List<string> ids)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.AddPlaylistItems.ProcessAsync(Context, playlistId, type, position, ids);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> RemovePlaylistItem(string playlistId, int position)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.RemovePlaylistItem.ProcessAsync(Context, playlistId, position);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> RemovePlaylistItems(string playlistId, string positions)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.RemovePlaylistItems.ProcessAsync(Context, playlistId, positions);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> MovePlaylistItem(string playlistId, int oldPosition, int newPosition)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.MovePlaylistItem.ProcessAsync(Context, playlistId, oldPosition, newPosition);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebStringResult> CreatePlaylist(string playlistName)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.CreatePlaylist.ProcessAsync(Context, playlistName);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebBoolResult> DeletePlaylist(string playlistId)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Playlist.DeletePlaylist.ProcessAsync(Context, playlistId);
    }

    #endregion

    #region Filters

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebIntResult> GetFilterValuesCount(WebMediaType mediaType, string filterField, string op, int? limit)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Filter.GetFilterValuesCount.ProcessAsync(Context, mediaType, filterField, op, limit);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<string>> GetFilterValues(WebMediaType mediaType, string filterField, string op, int? limit, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Filter.GetFilterValues.ProcessAsync(Context, mediaType, filterField, op, limit, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<string>> GetFilterValuesByRange(int start, int end, WebMediaType mediaType, string filterField, string op, int? limit, WebSortOrder? order)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Filter.GetFilterValuesByRange.ProcessAsync(Context, start, end, mediaType, filterField, op, limit, order);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<WebStringResult> CreateFilterString(string field, string op, string value, string conjunction)
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Filter.CreateFilterString.ProcessAsync(Context, field, op, value, conjunction);
    }

    [HttpGet]
    [ApiExplorerSettings]
    public Task<IList<WebFilterOperator>> GetFilterOperators()
    {
      Logger.Debug("MAS Request: {0}", Request.GetDisplayUrl());
      return ResourceAccess.MAS.Filter.GetFilterOperators.ProcessAsync(Context);
    }

    #endregion

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}
