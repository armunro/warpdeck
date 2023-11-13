using System;
using Autofac;
using WarpDeck.Adapter.Configuration;
using WarpDeck.Adapter.Hardware;
using WarpDeck.Adapter.Icon;
using WarpDeck.Adapter.Monitor;
using WarpDeck.Adapter.PropertyRule;
using WarpDeck.Domain.Configuration;
using WarpDeck.Domain.Hardware;
using WarpDeck.Domain.Icon;
using WarpDeck.Domain.Key;
using WarpDeck.Domain.Key.Action;
using WarpDeck.Domain.Key.Behavior;
using WarpDeck.Domain.Monitor;
using WarpDeck.Domain.Monitor.Rules;
using WarpDeck.Domain.Property;
using WarpDeck.Domain.Property.Rules;
using WarpDeck.Plugins.Icon;
using WarpDeck.Plugins.Monitor.Action;
using WarpDeck.Plugins.Monitor.Criteria;
using WarpDeck.UseCase.Device;
using WarpDeck.UseCase.DeviceLayer;
using WarpDeck.UseCase.Hardware;
using WarpDeck.UseCase.Key;
using WarpDeck.UseCase.Layer;
using WarpDeck.UseCase.Property;
using WarpDeck.Windows.Plugins.Actions.Launcher;
using WarpDeck.Windows.Plugins.Actions.Macro;
using WarpDeck.Windows.Plugins.Actions.Window;


namespace WarpDeck.Windows
{
    public class Dependencies
    {
        public class BoardModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                base.Load(builder);
                builder.RegisterType<AttachedHardwareProvider>().As<IHardwareProvider>().SingleInstance();
                builder.RegisterType<CreateDeviceUseCase>().SingleInstance();
                builder.RegisterType<GetHardwareUseCase>().SingleInstance();
            }
        }


        public class IconsModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<TemplateDocumentFileProvider>()
                    .As<ITemplateDocumentProvider>()
                    .WithParameter(new NamedParameter("filePath", "Untitled.svg"));
                builder.RegisterType<PressAndHold>()
                    .Named<IconTemplate>(nameof(WarpDeck.Plugins.Behaviors.PressAndHold))
                    .As<IHasProperties>();
                builder.RegisterType<Press>()
                    .Named<IconTemplate>(nameof(WarpDeck.Plugins.Behaviors.Press))
                    .As<IHasProperties>();
                builder.RegisterType<InMemoryIconCache>().As<IIconCache>();

                base.Load(builder);
            }
        }

        public class ActionsModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<KeyMacro>()
                    .Named<KeyAction>(nameof(KeyMacro))
                    .As<IHasActionParameters>();
                builder.RegisterType<LauncherAction>()
                    .Named<KeyAction>(nameof(LauncherAction))
                    .As<IHasActionParameters>();
                builder.RegisterType<ManageWindowAction>()
                    .Named<KeyAction>(nameof(ManageWindowAction))
                    .As<IHasActionParameters>();

                base.Load(builder);
            }
        }

        public class MonitorsModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<MonitorManager>().SingleInstance();
                builder.RegisterType<ActiveWindowMonitor>().Named<IMonitor>(nameof(ActiveWindowMonitor)).SingleInstance();
                builder.RegisterType<ActivateLayer>().Named<IMonitorRuleAction>(nameof(ActivateLayer));

                //criteria
                builder.RegisterType<Always>().Named<MonitorCondition>(nameof(Always));
                builder.RegisterType<AppPathMatches>().Named<MonitorCondition>(nameof(AppPathMatches));
                builder.RegisterType<WindowTitleMatches>().Named<MonitorCondition>(nameof(WindowTitleMatches));


                base.Load(builder);
            }
        }

        public class PresentationModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<Presentation.WarpDeckFrontend>();
                base.Load(builder);
            }
        }

    
    }
}