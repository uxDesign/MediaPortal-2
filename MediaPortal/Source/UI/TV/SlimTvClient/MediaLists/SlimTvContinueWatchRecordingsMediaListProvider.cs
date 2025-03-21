#region Copyright (C) 2007-2021 Team MediaPortal

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

using MediaPortal.Common.Commands;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Plugins.SlimTv.Client.Models.Navigation;
using MediaPortal.Plugins.SlimTv.Client.TvHandler;
using MediaPortal.Plugins.SlimTv.Interfaces.Aspects;
using MediaPortal.UiComponents.Media.MediaLists;
using MediaPortal.UiComponents.Media.Models;
using System.Linq;

namespace MediaPortal.Plugins.SlimTv.Client.MediaLists
{
  public class SlimTvContinueWatchRecordingsMediaListProvider : BaseContinueWatchMediaListProvider
  {
    public SlimTvContinueWatchRecordingsMediaListProvider()
    {
      _changeAspectId = RecordingAspect.ASPECT_ID;
      _necessaryMias = SlimTvConsts.NECESSARY_RECORDING_MIAS;
      _optionalMias = SlimTvConsts.OPTIONAL_RECORDING_MIAS;
      _playableConverterAction = mi => new RecordingItem(mi) { Command = new MethodDelegateCommand(() => PlayItemsModel.CheckQueryPlayAction(mi)) };
    }
  }

  public class SlimTvContinueWatchTVRecordingsMediaListProvider : SlimTvContinueWatchRecordingsMediaListProvider
  {
    public SlimTvContinueWatchTVRecordingsMediaListProvider()
    {
      _necessaryMias = _necessaryMias.Union(new[] { VideoAspect.ASPECT_ID });
    }
  }

  public class SlimTvContinueWatchRadioRecordingsMediaListProvider : SlimTvContinueWatchRecordingsMediaListProvider
  {
    public SlimTvContinueWatchRadioRecordingsMediaListProvider()
    {
      _necessaryMias = _necessaryMias.Union(new[] { AudioAspect.ASPECT_ID });
    }
  }
}
