using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using WarpDeck.Domain.Configuration;
using WarpDeck.Domain.Device;
using WarpDeck.Domain.Layer;
using WarpDeck.Domain.Monitor.Rules;
using WarpDeck.Domain.Property.Rules;

namespace WarpDeck.Adapter.Configuration
{
    public class FileDeviceReaderWriter : IDeviceReader, IDeviceWriter
    {
        private readonly string _configBaseDir;

        public FileDeviceReaderWriter(string configBaseDir)
        {
            _configBaseDir = configBaseDir;
        }

        public DeviceModelList ReadDevices()
        {
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
                    JsonSerializer.Deserialize<DeviceInfo>(
                        File.ReadAllText(Path.Join(deviceBaseDir, "device.wdspec.json")));
                //monitor rules
                device.MonitorRules =
                    JsonSerializer.Deserialize<MonitorRuleList>(
                        File.ReadAllText(Path.Join(deviceBaseDir, "monitorRules.json")));
                device.PropertyRules =
                    JsonSerializer.Deserialize<PropertyRuleModelList>(
                        File.ReadAllText(Path.Join(deviceBaseDir, "propertyRules.json")));


                string[] layerFiles = Directory.GetFiles(deviceBaseDir, "*.wdlayer.json");
                foreach (string layerFile in layerFiles)
                {
                    LayerModel layer = JsonSerializer.Deserialize<LayerModel>(File.ReadAllText(layerFile));
                    device.Layers.Add(layer.LayerId, layer);
                }

                devices.Add(device);
            }


            return devices;
        }

        public void WriteDeviceModel(DeviceModel deviceModel)
        {
            string deviceBaseDir = Path.Join(_configBaseDir, "devices", deviceModel.DeviceId);
            if (!Directory.Exists(_configBaseDir))
                Directory.CreateDirectory(_configBaseDir);
            if (!Directory.Exists(deviceBaseDir))
                Directory.CreateDirectory(deviceBaseDir);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            //Save specifications
            string specPath = Path.Join(deviceBaseDir, "device.wdspec.json");
            File.WriteAllText(specPath, JsonSerializer.Serialize(deviceModel.Info, options));

            //save monitors
            string monitorFilePath = Path.Join(deviceBaseDir, "monitorRules.json");
            File.WriteAllText(monitorFilePath, JsonSerializer.Serialize(deviceModel.MonitorRules, options)); 

            //save property rules
            string propertyRulesPath = Path.Join(deviceBaseDir, "propertyRules.json");
            File.WriteAllText(propertyRulesPath, JsonSerializer.Serialize(deviceModel.PropertyRules, options));

            foreach (var layerModel in deviceModel.Layers.Values)
            {
                string layerFilePath = Path.Join(deviceBaseDir, $"{layerModel.LayerId}.wdlayer.json");
                File.WriteAllText(layerFilePath, JsonSerializer.Serialize(layerModel, options));
            }
        }
    }
}