using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Autofac;
using COSMIC.Warpdeck.Adapter.Hardware;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Icon;
using COSMIC.Warpdeck.Domain.Layer;
using COSMIC.Warpdeck.Domain.Monitor;
using COSMIC.Warpdeck.Domain.Monitor.Rules;
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
            BindDevice(new VirtualBoard(), newDevice);
        }

        public void BindDevice(IMacroBoard macroBoard, DeviceModel deviceModel)
        {
            macroBoard.KeyStateChanged +=
                (_, args) => BoardOnKeyStateChanged(deviceModel, args.Key.ToString(), args.IsDown);
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
            CombineDeviceActions(deviceModel);
        }

        private void CombineDeviceActions(DeviceModel deviceModel)
        {
            foreach (LayerModel layerModel in deviceModel.Layers.Values.ToList())
            {
                foreach (var button in layerModel.Buttons)
                {
                    foreach (string actionName in button.Value.Actions.Keys)
                    {
                        string key = $"{layerModel.LayerId}.{button.Key}.{actionName}";
                        deviceModel.ActionsCombined.Add(key, button.Value.Actions[actionName]);
                    }
                }
            }
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

        public void TriggerButtonAction(string deviceId, string buttonId, string action)
        {
            ButtonBehavior behavior = WarpdeckAppContext.Container.Resolve<ButtonBehavior>();
            behavior.TriggerButtonAction(Devices[deviceId].ButtonStates[buttonId], action);
        }

        public void TriggerAction(ActionModel action)
        {
            ButtonBehavior behavior = WarpdeckAppContext.Container.Resolve<ButtonBehavior>();
            behavior.TriggerAction(action);
        }


        private void DrawLayerOnBoard(string layerId, string deviceId)
        {
            _timer.UnregisterAllRepeatable();
            DeviceModel device = Devices[deviceId];
            LayerModel layer = device.Layers[layerId];
            foreach (var (keyId, keyModel) in layer.Buttons)
            {
                device.ButtonStates.UpdateKeyState(keyId, keyModel);
                Boards[deviceId].SetKeyBitmap(Int32.Parse(keyId),
                    KeyBitmap.Create.FromBitmap(GenerateKeyIcon(keyModel, deviceId)));
            }
        }

        private void BoardOnKeyStateChanged(DeviceModel device, string keyId, bool isDown)
        {
            try
            {
                var keyModels = device.ActiveLayers
                    .Where(x => x.Value.Buttons.ContainsKey(keyId))
                    .OrderBy(x => x.Value.Level)
                    .Select(x => x.Value.Buttons[keyId]);
                if (!keyModels.Any())
                    return;
                var key = keyModels.Last();

                if (isDown)
                    key.History.LastDown = DateTime.Now;
                else
                    key.History.LastUp = DateTime.Now;


                //TODO: This needs to be removed
                ButtonBehavior behavior = new ButtonBehavior();

                if (isDown)
                    behavior.OnKeyDown(key, key.History);
                else
                    behavior.OnKeyUp(key, key.History);
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


        public Bitmap GenerateKeyIcon(ButtonModel buttonModel, string deviceId, bool skipCache = false)
        {
            if (buttonModel == null)
                return IconHelpers.DrawBlankKeyIcon(144, 144);

            if (_cache.DoesCacheHaveIcon(buttonModel) && !skipCache) return _cache.GetIcon(buttonModel).ToBitmap();

            IconTemplate template = WarpdeckAppContext.Container.Resolve<IconTemplate>();
            template.PropertyRule = _propertyRuleManagers[deviceId];

            return _cache.SetIcon(buttonModel, template.GenerateIcon(buttonModel)).ToBitmap();
        }


        public DeviceModel GetDevice(string deviceId)
        {
            if (Devices.ContainsKey(deviceId))
                return Devices[deviceId];
            throw new DeviceNotFoundFoundException(deviceId);
        }

        public IEnumerable<DeviceModel> GetAllDevices() => Devices.Values;
        public void RedrawDevices() => Devices.Keys.ToList().ForEach(RedrawDevice);

        public void AddMonitorRules(MonitorRuleList monitorRules)
        {
            monitorRules.Rules.ForEach(x=> this._monitorManager.AddMonitorRule(x)); 
        }
    }
}