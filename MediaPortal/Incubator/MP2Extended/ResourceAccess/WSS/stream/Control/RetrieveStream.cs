﻿#region Copyright (C) 2007-2020 Team MediaPortal

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
using MediaPortal.Extensions.TranscodingService.Interfaces;
using MediaPortal.Extensions.TranscodingService.Interfaces.Helpers;
using MediaPortal.Extensions.TranscodingService.Interfaces.Transcoding;
using MediaPortal.Plugins.MP2Extended.Attributes;
using MediaPortal.Plugins.MP2Extended.Controllers.Contexts;
using MediaPortal.Plugins.MP2Extended.Exceptions;
using MediaPortal.Plugins.MP2Extended.ResourceAccess.WSS.Profiles;
using MediaPortal.Plugins.MP2Extended.ResourceAccess.WSS.stream.BaseClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.WSS.stream.Control
{
  [ApiFunctionDescription(Type = ApiFunctionDescription.FunctionType.Stream, Summary = "")]
  [ApiFunctionParam(Name = "identifier", Type = typeof(string), Nullable = false)]
  [ApiFunctionParam(Name = "file", Type = typeof(string), Nullable = true)]
  [ApiFunctionParam(Name = "hls", Type = typeof(string), Nullable = true)]
  internal class RetrieveStream : BaseSendData
  {
    public static async Task<bool> ProcessAsync(RequestContext context, string identifier, string file, string hls)
    {
      Stream resourceStream = null;
      bool onlyHeaders = false;

      if (identifier == null)
        throw new BadRequestException("RetrieveStream: Identifier is null");

      StreamItem streamItem = await StreamControl.GetStreamItemAsync(identifier);
      if (streamItem == null)
        throw new BadRequestException("RetrieveStream: Identifier is not valid");

      if (!streamItem.IsActive)
      {
        Logger.Debug("RetrieveStream: Stream for {0} is no longer active", identifier);
        SetErrorStatus(context, "Stream is no longer active");
        return true;
      }

      long startPosition = streamItem.StartPosition;
      if (hls != null)
      {
        #region Handle segment/playlist request

        if (await SendSegmentAsync(hls, context, streamItem))
          return true;

        if (streamItem.ItemType != Common.WebMediaType.TV && streamItem.ItemType != Common.WebMediaType.Radio &&
          MediaConverter.GetSegmentSequence(hls) >= 0)
        {
          long segmentRequest = MediaConverter.GetSegmentSequence(hls);
          if (await streamItem.RequestSegmentAsync(segmentRequest) == false)
          {
            Logger.Error("RetrieveStream: Request for segment file {0} canceled", hls);
            SetErrorStatus(context, "Request for segment file canceled");
            return true;
          }
          startPosition = segmentRequest * MediaConverter.HLSSegmentTimeInSeconds;
        }
        else
        {
          Logger.Error("RetrieveStream: Unable to find segment file {0}", hls);
          SetErrorStatus(context, "Unable to find segment file");
          return true;
        }

        #endregion
      }

      #region Init response

      // Grab the mimetype from the media item and set the Content Type header.
      if (streamItem.TranscoderObject.Mime == null)
        throw new InternalServerException("RetrieveStream: Media item has bad mime type, re-import media item");
      context.Response.ContentType = streamItem.TranscoderObject.Mime;

      TransferMode mediaTransferMode = TransferMode.Interactive;
      if (streamItem.TranscoderObject.IsVideo || streamItem.TranscoderObject.IsAudio)
        mediaTransferMode = TransferMode.Streaming;

      StreamMode requestedStreamingMode = StreamMode.Normal;
      string byteRangesSpecifier = context.Request.Headers["Range"];
      if (byteRangesSpecifier != null)
      {
        Logger.Debug("RetrieveStream: Requesting range {1} for mediaitem {0}", streamItem.RequestedMediaItem.MediaItemId, byteRangesSpecifier);
        requestedStreamingMode = StreamMode.ByteRange;
      }

      #endregion

      #region Process range request

      if (streamItem.StreamContext is TranscodeContext tc && (streamItem.TranscoderObject.IsTranscoding == false ||
        (tc.Partial == false && tc.TargetFileSize > 0 && tc.TargetFileSize > streamItem.TranscoderObject.Metadata.Size)))
      {
        streamItem.TranscoderObject.Metadata.Size = tc.TargetFileSize;
      }

      IList<Range> ranges = null;
      Range timeRange = new Range(startPosition, 0);
      Range byteRange = null;
      if (requestedStreamingMode == StreamMode.ByteRange)
      {
        long lSize = GetStreamSize(streamItem.TranscoderObject);
        ranges = ParseByteRanges(byteRangesSpecifier, lSize);
        if (ranges == null || ranges.Count == 0)
        {
          //At least 1 range is needed
          context.Response.StatusCode = (int)HttpStatusCode.RequestedRangeNotSatisfiable;
          context.Response.ContentLength = 0;
          context.Response.ContentType = null;
          Logger.Debug("RetrieveStream: Sending headers: " + string.Join(";", context.Response.Headers.Select(x => x.Key + "=" + x.Value).ToArray()));
          return true;
        }
      }

      if (streamItem.TranscoderObject.IsSegmented == false && streamItem.TranscoderObject.IsTranscoding == true && mediaTransferMode == TransferMode.Streaming)
      {
        if ((requestedStreamingMode == StreamMode.ByteRange) && (ranges == null || ranges.Count == 0))
        {
          //At least 1 range is needed
          context.Response.StatusCode = (int)HttpStatusCode.RequestedRangeNotSatisfiable;
          context.Response.ContentLength = 0;
          context.Response.ContentType = null;
          Logger.Debug("RetrieveStream: Sending headers: " + string.Join(";", context.Response.Headers.Select(x => x.Key + "=" + x.Value).ToArray()));
          return true;
        }
      }

      if (ranges != null && ranges.Count > 0)
      {
        //Use only last range
        if (requestedStreamingMode == StreamMode.ByteRange)
        {
          byteRange = ranges[ranges.Count - 1];
          timeRange = ConvertToTimeRange(byteRange, streamItem.TranscoderObject);
        }
      }

      #endregion

      #region Handle ready file request

      // The initial request?
      if (resourceStream == null && streamItem.StreamContext != null && (streamItem.StartPosition == timeRange.From || file != null))
        resourceStream = streamItem.StreamContext.Stream;

      if (resourceStream == null && streamItem.TranscoderObject.IsTranscoded == false)
      {
        var streamContext = await StreamControl.StartOriginalFileStreamingAsync(identifier);
        resourceStream = streamContext?.Stream;
      }

      #endregion

      #region Handle transcode

      bool partialResource = false;
      if (resourceStream == null)
      {
        Logger.Debug("RetrieveStream: Attempting to start streaming for mediaitem {0} in mode {1}", streamItem.RequestedMediaItem.MediaItemId, requestedStreamingMode.ToString());
        var transcode = await StreamControl.StartStreamingAsync(identifier, timeRange.From);
        partialResource = transcode?.Partial ?? false;
        resourceStream = transcode?.Stream;

        //Send any HLS file originally requested
        if (hls != null && await SendSegmentAsync(hls, context, streamItem))
          return true;
      }

      if (!streamItem.TranscoderObject.IsStreamable)
        Logger.Debug("RetrieveStream: Live transcoding of mediaitem {0} is not possible because of media container", streamItem.RequestedMediaItem.MediaItemId);

      #endregion

      #region Finish and send response

      // HTTP/1.1 RFC2616 section 14.25 'If-Modified-Since'
      if (!string.IsNullOrEmpty(context.Request.Headers["If-Modified-Since"]))
      {
        DateTime lastRequest = DateTime.Parse(context.Request.Headers["If-Modified-Since"]);
        if (lastRequest.CompareTo(streamItem.TranscoderObject.LastUpdated) <= 0)
          context.Response.StatusCode = (int)HttpStatusCode.NotModified;
      }

      // HTTP/1.1 RFC2616 section 14.29 'Last-Modified'
      context.Response.Headers["Last-Modified"] = streamItem.TranscoderObject.LastUpdated.ToUniversalTime().ToString("r");

      if (resourceStream == null)
      {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.SetReasonPhrase("No resource stream found");
        context.Response.ContentLength = 0;
        context.Response.ContentType = null;

        return true;
      }

      using (await streamItem.RequestBusyLockAsync(SendDataCancellation.Token))
      {
        // TODO: fix method
        onlyHeaders = context.Request.Method == "HEAD" || context.Response.StatusCode == (int)HttpStatusCode.NotModified;
        if (requestedStreamingMode == StreamMode.ByteRange)
        {
          if (ranges != null && ranges.Count > 0)
          {
            // We only support last range
            await SendByteRangeAsync(context, resourceStream, streamItem.TranscoderObject, streamItem.Profile, ranges[ranges.Count - 1], onlyHeaders, partialResource, mediaTransferMode);
            return true;
          }
        }
        Logger.Debug("RetrieveStream: Sending file header only: {0}", onlyHeaders.ToString());
        await SendWholeFileAsync(context, resourceStream, streamItem.TranscoderObject, streamItem.Profile, onlyHeaders, partialResource, mediaTransferMode);
      }

      #endregion

      return true;
    }

    private static async Task<bool> SendSegmentAsync(string fileName, RequestContext context, StreamItem streamItem)
    {
      if (!string.IsNullOrEmpty(fileName) && streamItem.StreamContext is TranscodeContext tc)
      {
        using (await streamItem.RequestBusyLockAsync(SendDataCancellation.Token))
        {
          var segment = await MediaConverter.GetSegmentFileAsync((VideoTranscoding)streamItem.TranscoderObject.TranscodingParameter, tc, fileName);
          if (segment != null)
          {
            if (segment.Value.ContainerEnum is VideoContainer)
            {
              VideoTranscoding video = (VideoTranscoding)streamItem.TranscoderObject.TranscodingParameter;
              List<string> profiles = ProfileMime.ResolveVideoProfile((VideoContainer)segment.Value.ContainerEnum, video.TargetVideoCodec, video.TargetAudioCodec, EncodingProfile.Unknown, 0, 0, 0, 0, 0, 0, Timestamp.None);
              string mime = "video/unknown";
              ProfileMime.FindCompatibleMime(streamItem.Profile, profiles, ref mime);
              context.Response.ContentType = mime;
            }
            else if (segment.Value.ContainerEnum is SubtitleCodec)
            {
              context.Response.ContentType = SubtitleHelper.GetSubtitleMime((SubtitleCodec)segment.Value.ContainerEnum);
            }
            bool onlyHeaders = context.Request.Method == "HEAD" || context.Response.StatusCode == (int)HttpStatusCode.NotModified;
            Logger.Debug("RetrieveStream: Sending file header only: {0}", onlyHeaders.ToString());

            await SendWholeFileAsync(context, segment.Value.FileData, onlyHeaders);
            // Close the Stream so that FFMpeg can replace the playlist file
            segment.Value.FileData.Dispose();
            return true;
          }
        }
      }
      return false;
    }

    protected static void SetErrorStatus(RequestContext context, string message)
    {
      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      context.Response.SetReasonPhrase(message);
      context.Response.ContentLength = 0;
      context.Response.ContentType = null;
    }

    internal static IMediaConverter MediaConverter
    {
      get { return ServiceRegistration.Get<IMediaConverter>(); }
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}
