using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Autofac;
using COSMIC.Warpdeck.Adapter.Hardware;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Icon;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Layer;
using COSMIC.Warpdeck.Domain.Monitor;
using OpenMacroBoard.SDK;
using StreamDeckSharp;

namespace COSMIC.Warpdeck.Managers
{
    public class DeviceManager : IDisposable
    {
        private readonly ActionTimer _timer;
        private readonly IIconCache _cache;
        private readonly MonitorManager _monitorManager = new();
        private readonly Dictionary<string, PropertyRuleManager> _propertyRuleManagers = new();
        private Dictionary<string, DeviceModel> Devices { get; } = new();
        private Dictionary<string, IMacroBoard> Boards { get; } = new();

        public DeviceManager(ActionTimer timer, IIconCache cache)
        {
            _timer = timer;
            _cache = cache;
        }

        public void BindVirtualDevice(DeviceModel newDevice)
        {
            if (newDevice.Info.HardwareId == "virtual")
            {
                BindDevice(new VirtualBoard(), newDevice);
            }
            else
            {
                IStreamDeckRefHandle refHandle =
                    StreamDeck.EnumerateDevices().First(x => x.DevicePath == newDevice.Info.HardwareId);
                IStreamDeckBoard board = refHandle.Open();
                BindDevice(board, newDevice);
            }
        }

        public void BindDevice(IMacroBoard macroBoard, DeviceModel deviceModel)
        {
            macroBoard.KeyStateChanged += (_, args) => BoardOnKeyStateChanged(deviceModel, args.Key, args.IsDown);
            macroBoard.ConnectionStateChanged += (_, args) =>
            {
                if (args.NewConnectionState)
                {
                    macroBoard.SetBrightness(100);
                    RedrawDevice(deviceModel.DeviceId);
                }
            };
            macroBoard.SetBrightness(100);
            Devices.Add(deviceModel.DeviceId, deviceModel);
            Boards.Add(deviceModel.DeviceId, macroBoard);

            _propertyRuleManagers.Add(deviceModel.DeviceId, new PropertyRuleManager());
            deviceModel.MonitorRules.Rules.ForEach(x => _monitorManager.AddMonitorRule(x));
            deviceModel.PropertyRules.ForEach(x => _propertyRuleManagers[deviceModel.DeviceId].Rules.Add(x));
            ClearDevice(deviceModel.DeviceId);
        }


        public void UnbindDevice(string deviceId)
        {
            if (Devices.ContainsKey(deviceId))
                Devices.Remove(deviceId);
            if (Boards.ContainsKey(deviceId))
            {
                Boards[deviceId].Dispose();
                Boards.Remove(deviceId);
            }
        }


        public void ClearDevice(string deviceId)
        {
            Boards[deviceId].ClearKeys();
        }

        public void ClearDevices()
        {
            foreach (string devicesKey in Devices.Keys)
            {
                ClearDevice(devicesKey);
            }
        }

        public void FireActionOnActiveDevice(string deviceId, int keyId, string action)
        {
            var orderedLayers = Devices[deviceId]
                .ActiveLayers
                .OrderBy(x => x.Value.Level)
                .Select(x => x.Key);
            KeyBehavior behavior = WarpdeckApp.Container.Resolve<KeyBehavior>();

            behavior.FireEvent(Devices[deviceId].KeyStates[keyId].Behavior, action);
        }


        private void DrawLayerOnBoard(string layerId, string deviceId)
        {
            _timer.UnregisterAllRepeatable();
            DeviceModel device = Devices[deviceId];
            LayerModel layer = device.Layers[layerId];
            foreach (var (keyId, keyModel) in layer.Keys)
            {
                device.KeyStates.UpdateKeyState(keyId, keyModel);
                Boards[deviceId].SetKeyBitmap(keyId, KeyBitmap.Create.FromBitmap(GenerateKeyIcon(keyModel, deviceId)));
            }
        }

        private void BoardOnKeyStateChanged(DeviceModel device, int keyId, bool isDown)
        {
            try
            {
                var keyModels = device.ActiveLayers
                    .Where(x => x.Value.Keys.ContainsKey(keyId))
                    .OrderBy(x => x.Value.Level)
                    .Select(x => x.Value.Keys[keyId]);
                if (!keyModels.Any())
                    return;
                var key = keyModels.Last();

                if (isDown)
                    key.History.LastDown = DateTime.Now;
                else
                    key.History.LastUp = DateTime.Now;

                string behaviorTypeName = device.KeyStates[keyId].Behavior.Type;
                KeyBehavior behavior = WarpdeckApp.Container.Resolve<KeyBehavior>();

                if (isDown)
                    behavior.OnKeyDown(device, keyId, key.Behavior, key.History);
                else
                    behavior.OnKeyUp(device, keyId, key.Behavior, key.History);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Dispose()
        {
            foreach (var boardKvp in Boards)
            {
                boardKvp.Value.Dispose();
            }

            Boards.Clear();
            Devices.Clear();
            _propertyRuleManagers.Clear();
        }

        public void AddMonitor(IMonitor monitor)
        {
            _monitorManager.AddMonitor(monitor);
        }

        public void RedrawDevice(string deviceId)
        {
            ClearDevice(deviceId);
            var orderedLayers = Devices[deviceId]
                .ActiveLayers
                .OrderBy(x => x.Value.Level)
                .Select(x => x.Key);
            foreach (string activeLayerId in orderedLayers)
            {
                DrawLayerOnBoard(activeLayerId, deviceId);
            }
        }


        public Bitmap GenerateKeyIcon(KeyModel keyModel, string deviceId, bool skipCache = false)
        {
            if (keyModel == null)
                return IconHelpers.DrawBlankKeyIcon(144, 144);

            if (_cache.DoesCacheHaveIcon(keyModel) && !skipCache) return _cache.GetIcon(keyModel).ToBitmap();

            IconTemplate template = WarpdeckApp.Container.Resolve<IconTemplate>();
            template.PropertyRule = _propertyRuleManagers[deviceId];

            return _cache.SetIcon(keyModel, template.GenerateIcon(keyModel)).ToBitmap();
        }


        public DeviceModel GetDevice(string deviceId)
        {
            if (Devices.ContainsKey(deviceId))
                return Devices[deviceId];
            throw new DeviceNotFoundFoundException(deviceId);
        }

        public IEnumerable<DeviceModel> GetAllDevices() => Devices.Values;
        public void RedrawDevices() => Devices.Keys.ToList().ForEach(RedrawDevice);
    }
}