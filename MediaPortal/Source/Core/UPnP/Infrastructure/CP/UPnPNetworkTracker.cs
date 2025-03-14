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

using MediaPortal.Utilities.Exceptions;
using MediaPortal.Utilities.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.XPath;
using UPnP.Infrastructure.CP.Description;
using UPnP.Infrastructure.CP.SSDP;
using UPnP.Infrastructure.Http;

namespace UPnP.Infrastructure.CP
{
  /// <summary>
  /// Tracks UPnP devices which are available in the network. Provides materialized descriptions for each available
  /// device and service. Provides an event <see cref="RootDeviceAdded"/> which gets fired when all description documents
  /// were fetched from the device's server. Provides an event <see cref="RootDeviceRemoved"/> which gets fired when
  /// the device disappears. SSDP events for device configuration changes are completely hidden by this class and mapped
  /// to calls to <see cref="RootDeviceRemoved"/> and <see cref="RootDeviceAdded"/>.
  /// </summary>
  public class UPnPNetworkTracker : IDisposable
  {
    /// <summary>
    /// Delegate to be called when a network device was added and its given <paramref name="rootDescriptor"/> was filled
    /// completely.
    /// </summary>
    /// <param name="rootDescriptor">Descriptor containing all description documents of the new UPnP device.</param>
    public delegate void DeviceAddedDlgt(RootDescriptor rootDescriptor);

    /// <summary>
    /// Delegate to be called when a network device was removed.
    /// </summary>
    /// <param name="rootDescriptor">Descriptor of the UPnP device which was removed.</param>
    public delegate void DeviceRemovedDlgt(RootDescriptor rootDescriptor);

    /// <summary>
    /// Delegate to be called when a reboot of a network device was detected.
    /// </summary>
    /// <param name="rootDescriptor">Descriptor of the UPnP device which was rebooted.</param>
    public delegate void DeviceRebootedDlgt(RootDescriptor rootDescriptor);

    protected class DescriptionRequestState
    {
      protected RootDescriptor _rootDescriptor;
      protected HttpRequestMessage _httpWebRequest;
      protected ICollection<ServiceDescriptor> _pendingServiceDescriptions = new List<ServiceDescriptor>();
      protected ServiceDescriptor _currentServiceDescriptor = null;

      public DescriptionRequestState(RootDescriptor rootDescriptor, HttpRequestMessage httpWebRequest)
      {
        _rootDescriptor = rootDescriptor;
        _httpWebRequest = httpWebRequest;
      }

      public RootDescriptor RootDescriptor
      {
        get { return _rootDescriptor; }
      }

      public HttpRequestMessage Request
      {
        get { return _httpWebRequest; }
        set { _httpWebRequest = value; }
      }

      public ServiceDescriptor CurrentServiceDescriptor
      {
        get { return _currentServiceDescriptor; }
        set { _currentServiceDescriptor = value; }
      }

      public ICollection<ServiceDescriptor> PendingServiceDescriptions
      {
        get { return _pendingServiceDescriptions; }
      }
    }

    /// <summary>
    /// Timeout for a pending request for a description document in seconds.
    /// </summary>
    protected const int PENDING_REQUEST_TIMEOUT = 30;

    protected const string KEY_ROOT_DESCRIPTOR = "ROOT-DESCRIPTOR";

    protected ICollection<DescriptionRequestState> _pendingRequests = new List<DescriptionRequestState>();
    protected bool _active = false;
    protected CPData _cpData;
    protected LocalEndPointHttpClient _httpClient;

    #region Ctor

    /// <summary>
    /// Creates a new UPnP network tracker instance.
    /// </summary>
    /// <param name="cpData">Shared control point data instance.</param>
    public UPnPNetworkTracker(CPData cpData)
    {
      _cpData = cpData;
    }

    public void Dispose()
    {
      Close();
    }

    #endregion

    #region Public members

    /// <summary>
    /// Gets called when a new device appeared at the network. When this event is called, all description documents from the
    /// sub devices have already been loaded.
    /// </summary>
    public event DeviceAddedDlgt RootDeviceAdded;

    /// <summary>
    /// Gets called when a device disappeared from the network. Will be called when the device explicitly cancelled its
    /// advertisement as well as when its advertisements expired.
    /// </summary>
    public event DeviceRemovedDlgt RootDeviceRemoved;

    /// <summary>
    /// Gets called when the SSDP subsystem detects a reboot of one of our known devices.
    /// </summary>
    public event DeviceRebootedDlgt DeviceRebooted;

    /// <summary>
    /// Returns the information whether this UPnP network tracker is active, i.e. the network listener is active and this
    /// instance is tracking network devices.
    /// </summary>
    public bool IsActive
    {
      get { return _active; }
    }

    /// <summary>
    /// Returns a mapping of root device UUIDs to descriptors containing the information about that device for all known
    /// UPnP network devices. Returns <c>null</c> if this network tracker isn't active.
    /// </summary>
    public IDictionary<string, RootDescriptor> KnownRootDevices
    {
      get
      {
        using (_cpData.Lock.EnterRead())
        {
          if (!_active)
            return null;
          IDictionary<string, RootDescriptor> result = new Dictionary<string, RootDescriptor>();
          foreach (RootEntry entry in _cpData.SSDPController.RootEntries)
          {
            RootDescriptor rd = GetRootDescriptor(entry);
            if (rd == null)
              continue;
            result.Add(rd.SSDPRootEntry.RootDeviceUUID, rd);
          }
          return result;
        }
      }
    }

    /// <summary>
    /// Data which is shared among all components of the control point system.
    /// </summary>
    public CPData SharedControlPointData
    {
      get { return _cpData; }
    }

    /// <summary>
    /// Starts this UPnP network tracker. After the tracker is started, its <see cref="RootDeviceAdded"/> and
    /// <see cref="RootDeviceRemoved"/> events will begin to be raised when UPnP devices become available at the network of when
    /// UPnP devices disappear from the network.
    /// </summary>
    public void Start()
    {
      using (_cpData.Lock.EnterWrite())
      {
        if (_active)
          throw new IllegalCallException("UPnPNetworkTracker is already active");
        _active = true;
        _httpClient = CreateHttpClient();
        SSDPClientController ssdpController = new SSDPClientController(_cpData);
        ssdpController.RootDeviceAdded += OnSSDPRootDeviceAdded;
        ssdpController.RootDeviceRemoved += OnSSDPRootDeviceRemoved;
        ssdpController.DeviceRebooted += OnSSDPDeviceRebooted;
        ssdpController.DeviceConfigurationChanged += OnSSDPDeviceConfigurationChanged;
        _cpData.SSDPController = ssdpController;
        ssdpController.Start();
        ssdpController.SearchAll(null);
      }
    }

    /// <summary>
    /// Stops this UPnP network tracker's activity, i.e. closes the UPnP network listener and clears all
    /// <see cref="KnownRootDevices"/>.
    /// </summary>
    public void Close()
    {
      using (_cpData.Lock.EnterWrite())
      {
        if (!_active)
          return;
        _active = false;
        foreach (RootEntry entry in _cpData.SSDPController.RootEntries)
        {
          RootDescriptor rd = GetRootDescriptor(entry);
          if (rd == null)
            continue;
          InvalidateDescriptor(rd);
        }
        SSDPClientController ssdpController = _cpData.SSDPController;
        ssdpController.Close();
        ssdpController.RootDeviceAdded -= OnSSDPRootDeviceAdded;
        ssdpController.RootDeviceRemoved -= OnSSDPRootDeviceRemoved;
        ssdpController.DeviceRebooted -= OnSSDPDeviceRebooted;
        ssdpController.DeviceConfigurationChanged -= OnSSDPDeviceConfigurationChanged;
        _cpData.SSDPController = null;
        _pendingRequests.Clear();
        _httpClient.CancelPendingRequests();
        _httpClient.Dispose();
      }
    }

    #endregion

    #region Private/protected members

    protected void InvokeRootDeviceAdded(RootDescriptor rd)
    {
      try
      {
        DeviceAddedDlgt dlgt = RootDeviceAdded;
        if (dlgt != null)
          dlgt(rd);
      }
      catch (Exception e)
      {
        UPnPConfiguration.LOGGER.Warn("UPnPNetworkTracker: Error invoking RootDeviceAdded delegate", e);
      }
    }

    protected void InvokeRootDeviceRemoved(RootDescriptor rd)
    {
      try
      {
        DeviceRemovedDlgt dlgt = RootDeviceRemoved;
        if (dlgt != null)
          dlgt(rd);
      }
      catch (Exception e)
      {
        UPnPConfiguration.LOGGER.Warn("UPnPNetworkTracker: Error invoking RootDeviceRemoved delegate", e);
      }
    }

    protected void InvokeDeviceRebooted(RootDescriptor rd)
    {
      try
      {
        DeviceRebootedDlgt dlgt = DeviceRebooted;
        if (dlgt != null)
          dlgt(rd);
      }
      catch (Exception e)
      {
        UPnPConfiguration.LOGGER.Warn("UPnPNetworkTracker: Error invoking DeviceRebooted delegate", e);
      }
    }

    private void OnSSDPRootDeviceAdded(RootEntry rootEntry)
    {
      InitializeRootDescriptor(rootEntry);
    }

    protected void InitializeRootDescriptor(RootEntry rootEntry)
    {
      RootDescriptor rd = new RootDescriptor(rootEntry)
        {
            State = RootDescriptorState.AwaitingDeviceDescription
        };
      using (_cpData.Lock.EnterWrite())
        SetRootDescriptor(rootEntry, rd);

      try
      {
        LinkData preferredLink = rootEntry.PreferredLink;
        HttpRequestMessage request = CreateHttpGetRequest(new Uri(preferredLink.DescriptionLocation), preferredLink.Endpoint.EndPointIPAddress);
        DescriptionRequestState state = new DescriptionRequestState(rd, request);
        using (_cpData.Lock.EnterWrite())
          _pendingRequests.Add(state);

        _httpClient.SendAsync(request).ContinueWith(response => OnDeviceDescriptionReceived(response, state));
      }
      catch (Exception) // Don't log messages at this low protocol level
      {
        using (_cpData.Lock.EnterWrite())
          rd.State = RootDescriptorState.Erroneous;
      }
    }

    private void OnDeviceDescriptionReceived(Task<HttpResponseMessage> responseTask, DescriptionRequestState state)
    {
      RootDescriptor rd = state.RootDescriptor;
      HttpResponseMessage response = null;
      try
      {
        // GetAwaiter().GetResult() unwraps any aggregate exceptions to make exception handling easier
        response = responseTask.GetAwaiter().GetResult();
        if (rd.State != RootDescriptorState.AwaitingDeviceDescription)
          return;
        if (!response.IsSuccessStatusCode)
        {
          rd.State = RootDescriptorState.Erroneous;
          return;
        }
        using (Stream body = response.Content.ReadAsStream())
        {
          XPathDocument xmlDeviceDescription = new XPathDocument(body);
          using (_cpData.Lock.EnterWrite())
          {
            rd.DeviceDescription = xmlDeviceDescription;
            DeviceDescriptor rootDeviceDescriptor = DeviceDescriptor.CreateRootDeviceDescriptor(rd);
            if (rootDeviceDescriptor == null)
            { // No root device description available
              rd.State = RootDescriptorState.Erroneous;
              return;
            }

            ExtractServiceDescriptorsRecursive(rootDeviceDescriptor, rd.ServiceDescriptors, state.PendingServiceDescriptions);
            rd.State = RootDescriptorState.AwaitingServiceDescriptions;
          }
        }
        ContinueGetServiceDescription(state);
      }
      catch (Exception) // Don't log exceptions at this low protocol level
      {
        rd.State = RootDescriptorState.Erroneous;
      }
      finally
      {
        response?.Dispose();
      }
    }

    private void ContinueGetServiceDescription(DescriptionRequestState state)
    {
      RootDescriptor rootDescriptor = state.RootDescriptor;

      IEnumerator<ServiceDescriptor> enumer = state.PendingServiceDescriptions.GetEnumerator();
      if (!enumer.MoveNext())
      {
        using (_cpData.Lock.EnterWrite())
        {
          if (rootDescriptor.State != RootDescriptorState.AwaitingServiceDescriptions)
            return;
          _pendingRequests.Remove(state);
          rootDescriptor.State = RootDescriptorState.Ready;
        }
        // This event is needed for two cases: 1) "Normal" first device advertisement, 2) configuration change event (see comment in method HandleDeviceConfigurationChanged)
        InvokeRootDeviceAdded(rootDescriptor);
      }
      else
      {
        using (_cpData.Lock.EnterRead())
          if (rootDescriptor.State != RootDescriptorState.AwaitingServiceDescriptions)
            return;

        state.CurrentServiceDescriptor = enumer.Current;
        string url = state.CurrentServiceDescriptor.DescriptionURL;
        state.PendingServiceDescriptions.Remove(state.CurrentServiceDescriptor);
        state.CurrentServiceDescriptor.State = ServiceDescriptorState.AwaitingDescription;
        try
        {
          LinkData preferredLink = rootDescriptor.SSDPRootEntry.PreferredLink;
          HttpRequestMessage request = CreateHttpGetRequest(new Uri(new Uri(preferredLink.DescriptionLocation), url),
              preferredLink.Endpoint.EndPointIPAddress);
          state.Request = request;
          _httpClient.SendAsync(request).ContinueWith(response => OnServiceDescriptionReceived(response, state));
        }
        catch (Exception) // Don't log exceptions at this low protocol level
        {
          using (_cpData.Lock.EnterWrite())
            state.CurrentServiceDescriptor.State = ServiceDescriptorState.Erroneous;
        }
      }
    }

    private void OnServiceDescriptionReceived(Task<HttpResponseMessage> responseTask, DescriptionRequestState state)
    {
      RootDescriptor rd = state.RootDescriptor;
      HttpResponseMessage response = null;
      try
      {
        // GetAwaiter().GetResult() unwraps any aggregate exceptions to make exception handling easier
        response = responseTask.GetAwaiter().GetResult();
        if (response.IsSuccessStatusCode)
        {
          using (_cpData.Lock.EnterRead())
            if (rd.State != RootDescriptorState.AwaitingServiceDescriptions)
              return;

          using (Stream body = response.Content.ReadAsStream())
          {
            XPathDocument xmlServiceDescription = new XPathDocument(body);
            state.CurrentServiceDescriptor.ServiceDescription = xmlServiceDescription;
            state.CurrentServiceDescriptor.State = ServiceDescriptorState.Ready;
          }
        }
        else
        {
          using (_cpData.Lock.EnterWrite())
          {
            state.CurrentServiceDescriptor.State = ServiceDescriptorState.Erroneous;
            rd.State = RootDescriptorState.Erroneous;
          }
        }
      }
      catch (Exception)
      {
        using (_cpData.Lock.EnterWrite())
        {
          state.CurrentServiceDescriptor.State = ServiceDescriptorState.Erroneous;
          rd.State = RootDescriptorState.Erroneous;
        }
      }
      finally
      {
        response?.Dispose();
      }

      // Don't hold the lock while calling ContinueGetServiceDescription - that method is calling event handlers
      try
      {
        ContinueGetServiceDescription(state);
      }
      catch (Exception) // Don't log exceptions at this low protocol level
      {
        using (_cpData.Lock.EnterWrite())
          rd.State = RootDescriptorState.Erroneous;
      }
    }

    private void OnSSDPRootDeviceRemoved(RootEntry rootEntry)
    {
      RootDescriptor rd = GetRootDescriptor(rootEntry);
      if (rd == null)
        return;
      using (_cpData.Lock.EnterWrite())
        InvalidateDescriptor(rd);
      InvokeRootDeviceRemoved(rd);
    }

    private void OnSSDPDeviceRebooted(RootEntry rootEntry, bool configurationChanged)
    {
      RootDescriptor rd = GetRootDescriptor(rootEntry);
      if (rd == null)
        return;
      if (configurationChanged)
        HandleDeviceConfigurationChanged(rd);
      else
        InvokeDeviceRebooted(rd);
    }

    private void OnSSDPDeviceConfigurationChanged(RootEntry rootEntry)
    {
      RootDescriptor rd = GetRootDescriptor(rootEntry);
      if (rd == null)
        return;
      HandleDeviceConfigurationChanged(rd);
    }

    private void HandleDeviceConfigurationChanged(RootDescriptor rootDescriptor)
    {
      // Configuration changes cannot be given to our clients because they need a re-initialization of the
      // device and all service description documents. So configuration changes will be handled by invocing a
      // root device remove/add event combination. The add event will be fired when the initialization of the root descriptor has finished.
      InvokeRootDeviceRemoved(rootDescriptor);
      InitializeRootDescriptor(rootDescriptor.SSDPRootEntry);
    }

    /// <summary>
    /// Given a <paramref name="deviceDescriptor"/>, this method extracts two kinds of data:
    /// <list type="bullet">
    /// <item><see cref="ServiceDescriptor"/>s for services of the given device and all embedded devices (organized in a dictionary mapping
    /// device UUIDs to dictionaries mapping service type and version to service descriptors for all services in the device) in
    /// <paramref name="serviceDescriptors"/></item>
    /// <item>A collection of all service descriptors which are returned in <paramref name="serviceDescriptors"/>
    /// in <paramref name="pendingServiceDescriptions"/></item>
    /// </list>
    /// </summary>
    /// <param name="deviceDescriptor">Descriptor of the device to start extracting service descriptors.</param>
    /// <param name="serviceDescriptors">Dictionary of device UUIDs to dictionaries mapping service type and version to service descriptors of services
    /// which are contained in the device with the key UUID.</param>
    /// <param name="pendingServiceDescriptions">Dictionary of device service descriptors mapped to the SCPD url of the
    /// service.</param>
    private static void ExtractServiceDescriptorsRecursive(DeviceDescriptor deviceDescriptor,
        IDictionary<string, IDictionary<string, ServiceDescriptor>> serviceDescriptors,
        ICollection<ServiceDescriptor> pendingServiceDescriptions)
    {
      string deviceUuid = deviceDescriptor.DeviceUUID;
      ICollection<ServiceDescriptor> services = deviceDescriptor.CreateServiceDescriptors();
      if (services.Count > 0)
      {
        IDictionary<string, ServiceDescriptor> sds = serviceDescriptors[deviceUuid] = new Dictionary<string, ServiceDescriptor>();
        foreach (ServiceDescriptor service in services)
        {
          sds.Add(service.ServiceTypeVersion_URN, service);
          pendingServiceDescriptions.Add(service);
        }
      }
      foreach (DeviceDescriptor childDevice in deviceDescriptor.ChildDevices)
        ExtractServiceDescriptorsRecursive(childDevice, serviceDescriptors, pendingServiceDescriptions);
    }

    private static LocalEndPointHttpClient CreateHttpClient()
    {
      LocalEndPointHttpClient client = LocalEndPointHttpClient.Create(new LocalEndPointHttpClientOptions
      {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
      });
      client.Timeout = TimeSpan.FromSeconds(PENDING_REQUEST_TIMEOUT);
      client.DefaultRequestHeaders.UserAgent.TryParseAdd(UPnPConfiguration.UPnPMachineInfoHeader);
      return client;
    }

    private static HttpRequestMessage CreateHttpGetRequest(Uri uri, IPAddress localIpAddress)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
      if (NetworkUtils.LimitIPEndpoints)
        LocalEndPointHttpClient.SetLocalEndpoint(request, localIpAddress);
      return request;
    }

    protected void InvalidateDescriptor(RootDescriptor rd)
    {
      rd.State = RootDescriptorState.Invalid;
      foreach (IDictionary<string, ServiceDescriptor> sdDict in rd.ServiceDescriptors.Values)
        foreach (ServiceDescriptor sd in sdDict.Values)
          sd.State = ServiceDescriptorState.Invalid;
    }

    protected RootDescriptor GetRootDescriptor(RootEntry rootEntry)
    {
      object rdObj;
      if (!rootEntry.ClientProperties.TryGetValue(KEY_ROOT_DESCRIPTOR, out rdObj))
        return null;
      return (RootDescriptor) rdObj;
    }

    protected void SetRootDescriptor(RootEntry rootEntry, RootDescriptor rootDescriptor)
    {
      rootEntry.ClientProperties[KEY_ROOT_DESCRIPTOR] = rootDescriptor;
    }

    #endregion
  }
}
