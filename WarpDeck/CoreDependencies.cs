using System;
using Autofac;
using WarpDeck.Adapter.Configuration;
using WarpDeck.Adapter.PropertyRule;
using WarpDeck.Domain.Configuration;
using WarpDeck.Domain.Device;
using WarpDeck.Domain.Key;
using WarpDeck.Domain.Key.Action;
using WarpDeck.Domain.Key.Behavior;
using WarpDeck.Domain.Property;
using WarpDeck.Domain.Property.Rules;
using WarpDeck.UseCase.Device;
using WarpDeck.UseCase.DeviceLayer;
using WarpDeck.UseCase.Key;
using WarpDeck.UseCase.Layer;
using WarpDeck.UseCase.Property;

namespace WarpDeck
{
    public class CoreDependencies
    {
        public class BehaviorsModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<Plugins.Behaviors.Press>()
                    .Named<KeyBehavior>(nameof(Plugins.Behaviors.Press))
                    .As<KeyBehavior>()
                    .As<IHasProperties>()
                    .As<IHasActions>().InstancePerDependency();
                builder.RegisterType<Plugins.Behaviors.PressAndHold>()
                    .Named<KeyBehavior>(nameof(Plugins.Behaviors.PressAndHold))
                    .As<KeyBehavior>()
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
                builder.RegisterType<PropertyEqualsRule>().Named<IPropertyRule>("TagEquals");
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
                builder.RegisterType<KeyTimer>().SingleInstance();

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

                base.Load(builder);
            }
        }
    
    }
}