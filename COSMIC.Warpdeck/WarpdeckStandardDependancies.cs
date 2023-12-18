using System;
using Autofac;
using COSMIC.Warpdeck.Adapter;
using COSMIC.Warpdeck.Adapter.Configuration;
using COSMIC.Warpdeck.Adapter.PropertyRule;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Configuration;
using COSMIC.Warpdeck.Domain.Device;
using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Rules;
using COSMIC.Warpdeck.Managers;
using COSMIC.Warpdeck.UseCase.Device;
using COSMIC.Warpdeck.UseCase.DeviceLayer;
using COSMIC.Warpdeck.UseCase.Key;
using COSMIC.Warpdeck.UseCase.Layer;
using COSMIC.Warpdeck.UseCase.Property;

namespace COSMIC.Warpdeck
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WarpdeckStandardDependancies
    {
        public class BehaviorsModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<ButtonBehavior>()
                    .As<ButtonBehavior>()
                    .As<IHasProperties>()
                    .As<IHasActions>().InstancePerDependency();
                base.Load(builder);
            }
        }
        
            public class Property : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<PropertyRuleManager>().SingleInstance();
                builder.RegisterType<AlwaysRule>().Named<IPropertyRule>("Always");
                builder.RegisterType<PropertyEqualsRule>().Named<IPropertyRule>("PropertyEquals");
                builder.RegisterType<GetTypePropertyUseCase>().AsSelf();

                base.Load(builder);
            }
        }


        public class DevicesModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<DeviceManager>().SingleInstance();
                builder.RegisterType<ActivateDeviceLayerUseCase>().AsSelf();
                builder.RegisterType<DeactivateDeviceLayerUseCase>().AsSelf();
                builder.RegisterType<RedrawDeviceLayersUseCase>().AsSelf();
                builder.RegisterType<CreateDeviceLayerKeyUseCase>().AsSelf();
                builder.RegisterType<UpdateDeviceUseCase>().AsSelf();
                builder.RegisterType<MoveKeyUseCase>().AsSelf();
                builder.RegisterType<DuplicateKeyUseCase>().AsSelf();
                builder.RegisterType<ActionTimer>().SingleInstance();

                base.Load(builder);
            }
        }

        public class LayersModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<NewLayerUseCase>();
                base.Load(builder);
            }
        }


        public class ConfigModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<FileDeviceReaderWriter>()
                    .As<IDeviceReader>()
                    .As<IDeviceWriter>()
                    .WithParameter("configBaseDir", Environment.GetCommandLineArgs()[1])
                    .SingleInstance();
                builder.RegisterType<ClipPatternFileReaderWriter>()
                    .As<IClipPatternReader>()
                    .As<IClipPatternWriter>()
                    .WithParameter("configBaseDir", Environment.GetCommandLineArgs()[1])
                    .SingleInstance();

                base.Load(builder);
            }
        }
    
    }
}