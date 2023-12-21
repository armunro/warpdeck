using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using COSMIC.Warpdeck.Domain.Configuration;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Layer;
using COSMIC.Warpdeck.Domain.Monitor.Rules;
using COSMIC.Warpdeck.Domain.Property.Rules;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace COSMIC.Warpdeck.Adapter.Configuration
{
    public class YamlFileDeviceReaderWriter : IDeviceReader, IDeviceWriter
    {
        private readonly string _configBaseDir;

        public YamlFileDeviceReaderWriter(string configBaseDir)
        {
            _configBaseDir = configBaseDir;
        }

        public DeviceModelList ReadDevices()
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) 
                .Build();
            
            string devicesDir = Path.Join(_configBaseDir, "devices");
            if (!Directory.Exists(devicesDir))
                return new DeviceModelList() { new DeviceModel() };
            string[] deviceBaseDirs = Directory.GetDirectories(devicesDir);

            DeviceModelList devices = new DeviceModelList();
            foreach (string deviceBaseDir in deviceBaseDirs)
            {
                DeviceModel device = new DeviceModel();
                device.DeviceId = Path.GetFileName(deviceBaseDir);

                //device Info
                device.Info =
                    deserializer.Deserialize<DeviceInfo>(
                        File.ReadAllText(Path.Join(deviceBaseDir, "device.yaml")));
                //monitor rules
                device.MonitorRules =
                    deserializer.Deserialize<MonitorRuleList>(
                        File.ReadAllText(Path.Join(deviceBaseDir, "monitorRules.yaml")));
                device.PropertyRules =
                    deserializer.Deserialize<List<PropertyRuleModel>>(
                        File.ReadAllText(Path.Join(deviceBaseDir, "propertyRules.yaml")));


                string[] layerFiles = Directory.GetFiles(deviceBaseDir, "*.layer.yaml");
                foreach (string layerFile in layerFiles)
                {
                    LayerModel layer = deserializer.Deserialize<LayerModel>(File.ReadAllText(layerFile));
                    device.Layers.Add(layer.LayerId, layer);
                }
                devices.Add(device);
            }
            return devices;
        }

        public void WriteDeviceModel(DeviceModel deviceModel)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            string deviceBaseDir = Path.Join(_configBaseDir, "devices", deviceModel.DeviceId);
            if (!Directory.Exists(_configBaseDir))
                Directory.CreateDirectory(_configBaseDir);
            if (!Directory.Exists(deviceBaseDir))
                Directory.CreateDirectory(deviceBaseDir);

            //Device
            string specPath = Path.Join(deviceBaseDir, "device.yaml");
            File.WriteAllText(specPath, serializer.Serialize(deviceModel.Info));

            //Monitors
            string monitorFilePath = Path.Join(deviceBaseDir, "monitorRules.yaml");
            File.WriteAllText(monitorFilePath, serializer.Serialize(deviceModel.MonitorRules));

            //Property Rules
            string propertyRulesPath = Path.Join(deviceBaseDir, "propertyRules.yaml");
            File.WriteAllText(propertyRulesPath, serializer.Serialize(deviceModel.PropertyRules));

            //Layers
            foreach (var layerModel in deviceModel.Layers.Values)
            {
                string layerFilePath = Path.Join(deviceBaseDir, $"{layerModel.LayerId}.layer.yaml");
                File.WriteAllText(layerFilePath, serializer.Serialize(layerModel));
            }
        }
    }
}