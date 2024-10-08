using System;
using System.Linq;
using COSMIC.Warpdeck.Domain.Monitor.Rules;
using COSMIC.Warpdeck.UseCase.DeviceLayer;

namespace COSMIC.Warpdeck.Adapter.Monitor
{
    public class ActivateLayerMonitorAction : IMonitorRuleAction
    {
        private readonly string _deviceId;
        private readonly string _layerIds;
        private readonly ActivateDeviceLayerUseCase _activateLayer;
        private readonly DeactivateDeviceLayerUseCase _deactivateLayer;
        private readonly RedrawDeviceLayersUseCase _redrawDeviceLayersUseCase;

        public ActivateLayerMonitorAction(string deviceId, string layerIds,
            ActivateDeviceLayerUseCase activateLayer,
            DeactivateDeviceLayerUseCase deactivateLayer,
            RedrawDeviceLayersUseCase redrawDeviceLayersUseCase)
        {
            _deviceId = deviceId;
            _layerIds = layerIds;
            _activateLayer = activateLayer;
            _deactivateLayer = deactivateLayer;
            _redrawDeviceLayersUseCase = redrawDeviceLayersUseCase;
        }

        public void ActionWhenTrue()
        {
            var layers = _layerIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            foreach (string layer in layers)
            {
                _activateLayer.Invoke(_deviceId, layer);
            }
        }

        public void ActionWhenFalse()
        {
            var layers = _layerIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            foreach (string layer in layers)
            {
                _deactivateLayer.Invoke(_deviceId, layer);
            }
            
        }
    }
}