using System;
using System.Linq;
using WarpDeck.Domain.Monitor.Rules;
using WarpDeck.UseCase.DeviceLayer;

namespace WarpDeck.Plugins.Monitor.Action
{
    public class ActivateLayer : IMonitorRuleAction
    {
        private readonly string _deviceId;
        private readonly string _layerIds;
        private readonly ActivateDeviceLayerUseCase _activateLayer;
        private readonly DeactivateDeviceLayerUseCase _deactivateLayer;
        private readonly RedrawDeviceLayersUseCase _redrawDeviceLayersUseCase;

        public ActivateLayer(string deviceId, string layerIds,
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