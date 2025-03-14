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

using System;
using System.Threading.Tasks;
using MediaPortal.Common;
using MediaPortal.Common.Commands;
using MediaPortal.Common.General;
using MediaPortal.Common.Localization;
using MediaPortal.Common.Messaging;
using MediaPortal.Plugins.SlimTv.Client.Helpers;
using MediaPortal.Plugins.SlimTv.Client.Messaging;
using MediaPortal.Plugins.SlimTv.Interfaces;
using MediaPortal.Plugins.SlimTv.Interfaces.Items;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Workflow;
using MediaPortal.UiComponents.Media.General;
using MediaPortal.UI.SkinEngine.MpfElements;

namespace MediaPortal.Plugins.SlimTv.Client.Models
{
  /// <summary>
  /// <see cref="SlimTvGuideModelBase"/> acts as base class for all TvGuide models (single channel, multi channel).
  /// </summary>
  public abstract class SlimTvGuideModelBase : SlimTvModelBase
  {
    #region Protected fields

    protected AbstractProperty _groupNameProperty = null;
    protected AbstractProperty _currentProgramProperty = null;

    protected ListItem _selectedItem;

    #endregion

    #region GUI properties and methods

    /// <summary>
    /// Exposes the current group name to the skin.
    /// </summary>
    public string GroupName
    {
      get { return (string)_groupNameProperty.GetValue(); }
      set { _groupNameProperty.SetValue(value); }
    }

    /// <summary>
    /// Exposes the current group name to the skin.
    /// </summary>
    public AbstractProperty GroupNameProperty
    {
      get { return _groupNameProperty; }
    }

    /// <summary>
    /// Exposes the current program of tuned channel to the skin.
    /// </summary>
    public ProgramProperties CurrentProgram
    {
      get { return (ProgramProperties)_currentProgramProperty.GetValue(); }
      set { _currentProgramProperty.SetValue(value); }
    }

    /// <summary>
    /// Exposes the current program of tuned channel to the skin.
    /// </summary>
    public AbstractProperty CurrentProgramProperty
    {
      get { return _currentProgramProperty; }
    }

    public ListItem SelectedItem
    {
      get { return _selectedItem; }
    }

    // this overload is used by MultiChannelGuide in got focus trigger
    public void UpdateProgram(ListItem selectedItem)
    {
      _selectedItem = selectedItem;
      if (selectedItem != null)
      {
        IProgram program = (IProgram)selectedItem.AdditionalProperties["PROGRAM"];
        UpdateProgramStatus(program);
      }
    }

    // this overload is used by events
    public void UpdateProgram(object sender, SelectionChangedEventArgs e)
    {
      UpdateProgram(e.FirstAddedItem as ListItem);
    }

    // Lost focus trigger
    public void ClearItem()
    {
      _selectedItem = null;
    }

    protected virtual void UpdateGuiProperties()
    {
      GroupName = CurrentChannelGroup != null ? CurrentChannelGroup.Name : string.Empty;
    }

    protected virtual void ShowProgramActions(IProgram program)
    {
      if (program == null)
        return;

      ILocalization loc = ServiceRegistration.Get<ILocalization>();

      ItemsList actions = new ItemsList();
      bool isRunning = DateTime.Now >= program.StartTime && DateTime.Now <= program.EndTime;
      // if program is over already, there is nothing to do.
      if (program.EndTime < DateTime.Now)
      {
        actions.Add(new ListItem(Consts.KEY_NAME, loc.ToString("[SlimTvClient.ProgramOver]")));
      }
      else
      {
        // Check if program is currently running.
        if (isRunning)
        {
          actions.Add(new ListItem(Consts.KEY_NAME, loc.ToString("[SlimTvClient.WatchNow]"))
          {
            Command = new AsyncMethodDelegateCommand(() => TuneChannelByProgram(program))
          });
        }
      }
      if (_tvHandler.ScheduleControl != null)
      {
        var result = _tvHandler.ScheduleControl.GetRecordingStatusAsync(program).Result;
        if (result.Success)
        {
          if (isRunning && result.Result != RecordingStatus.None)
            actions.Add(
              new ListItem(Consts.KEY_NAME, loc.ToString("[SlimTvClient.WatchFromBeginning]"))
              {
                Command = new AsyncMethodDelegateCommand(() => _tvHandler.WatchRecordingFromBeginningAsync(program))
              });

          AddRecordingOptions(actions, program, result.Result);
        }
      }
      SlimTvExtScheduleModel.ShowDialog("[SlimTvClient.ChooseProgramAction]", actions);
    }

    protected async Task TuneChannelByProgram(IProgram program)
    {
      var result = await _tvHandler.ProgramInfo.GetChannelAsync(program);
      if (result.Success)
      {
        IWorkflowManager workflowManager = ServiceRegistration.Get<IWorkflowManager>();
        SlimTvClientModel model = workflowManager.GetModel(SlimTvClientModel.MODEL_ID) as SlimTvClientModel;
        if (model != null)
        {
          await model.Tune(result.Result);
          // Always switch to fullscreen
          workflowManager.NavigatePush(result.Result.MediaType == MediaType.Radio ? Consts.WF_STATE_ID_FULLSCREEN_AUDIO : Consts.WF_STATE_ID_FULLSCREEN_VIDEO);
        }
      }
    }

    protected virtual bool UpdateRecordingStatus(IProgram program)
    {
      return true;
    }

    protected virtual bool UpdateRecordingStatus(IProgram program, RecordingStatus newStatus)
    {
      IProgramRecordingStatus status = program as IProgramRecordingStatus;
      if (status == null)
        return false;

      status.RecordingStatus = newStatus;
      SlimTvClientMessaging.SendSlimTvProgramChangedMessage(program);
      return true;
    }

    #endregion

    #region Members

    #region Inits and Updates

    protected override void InitModel()
    {
      if (!_isInitialized)
      {
        _groupNameProperty = new WProperty(typeof(string), String.Empty);
        _currentProgramProperty = new WProperty(typeof(ProgramProperties), new ProgramProperties());

        _isInitialized = true;
      }
      SubscribeToMessages();
      base.InitModel();
    }

    void SubscribeToMessages()
    {
      _messageQueue.SubscribeToMessageChannel(SlimTvClientMessaging.CHANNEL);
      _messageQueue.MessageReceived += OnMessageReceived;
    }

    protected void OnMessageReceived(AsynchronousMessageQueue queue, SystemMessage message)
    {
      if (message.ChannelName == SlimTvClientMessaging.CHANNEL)
      {
        SlimTvClientMessaging.MessageType messageType = (SlimTvClientMessaging.MessageType)message.MessageType;
        switch (messageType)
        {
          case SlimTvClientMessaging.MessageType.ProgramStatusChanged:
            IProgram program = (IProgram)message.MessageData[SlimTvClientMessaging.KEY_PROGRAM];
            UpdateRecordingStatus(program);
            break;
        }
      }
    }

    #endregion

    #region Channel, groups and programs

    protected virtual void UpdateProgramStatus(IProgram program)
    {
      CurrentProgram.SetProgram(program);
    }

    #endregion

    #endregion

  }
}
