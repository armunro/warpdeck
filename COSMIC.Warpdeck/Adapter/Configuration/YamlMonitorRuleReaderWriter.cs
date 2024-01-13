using System.Collections.Generic;
using System.IO;
using COSMIC.Warpdeck.Domain.Configuration;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Layer;
using COSMIC.Warpdeck.Domain.Monitor.Rules;
using COSMIC.Warpdeck.Domain.Property.Rules;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace COSMIC.Warpdeck.Adapter.Configuration
{
    
    
    public class YamlMonitorRulesReaderWriter : IMonitorRuleReader
    {
        
        private readonly string _configBaseDir;

        public YamlMonitorRulesReaderWriter(string configBaseDir)
        {
            _configBaseDir = configBaseDir;
        }

        // public DeviceModelList ReadDevices()
        // {
        //     var deserializer = new DeserializerBuilder()
        //         .WithNamingConvention(PascalCaseNamingConvention.Instance) 
        //         .Build();
        //     
        //     string devicesDir = Path.Join(_configBaseDir, "devices");
        //     if (!Directory.Exists(devicesDir))
        //         return new DeviceModelList() { };
        //     string[] deviceBaseDirs = Directory.GetDirectories(devicesDir);
        //
        //     DeviceModelList devices = new DeviceModelList();
        //     foreach (string deviceBaseDir in deviceBaseDirs)
        //     {
        //         DeviceModel device = new DeviceModel();
        //         device.DeviceId = Path.GetFileName(deviceBaseDir);
        //
        //         //device Info
        //         device.Info =
        //             deserializer.Deserialize<DeviceInfo>(
        //                 File.ReadAllText(Path.Join(deviceBaseDir, "device.yaml")));
        //         //monitor rules
        //         device.MonitorRules =
        //             deserializer.Deserialize<MonitorRuleList>(
        //                 File.ReadAllText(Path.Join(deviceBaseDir, "monitorRules.yaml")));
        //         device.PropertyRules =
        //             deserializer.Deserialize<List<PropertyRuleModel>>(
        //                 File.ReadAllText(Path.Join(deviceBaseDir, "propertyRules.yaml")));
        //
        //
        //         string[] layerFiles = Directory.GetFiles(deviceBaseDir, "*.layer.yaml");
        //         foreach (string layerFile in layerFiles)
        //         {
        //             LayerModel layer = deserializer.Deserialize<LayerModel>(File.ReadAllText(layerFile));
        //             device.Layers.Add(layer.LayerId, layer);
        //         }
        //         devices.Add(device);
        //     }
        //     return devices;
        // }


        public MonitorRuleList ReadMonitorRules()
        {
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance) 
                .Build();
            
            return deserializer.Deserialize<MonitorRuleList>(
                    File.ReadAllText(Path.Join(_configBaseDir, "monitorRules.yaml")));
            
        }
    }
}