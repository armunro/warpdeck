using Autofac;
using COSMIC.Warpdeck.Adapter.Hardware;
using COSMIC.Warpdeck.Adapter.Icon;
using COSMIC.Warpdeck.Adapter.Monitor;
using COSMIC.Warpdeck.Adapter.Monitor.Criteria;
using COSMIC.Warpdeck.Domain.Action;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Clipboard;
using COSMIC.Warpdeck.Domain.Device.Hardware;
using COSMIC.Warpdeck.Domain.DeviceHost;
using COSMIC.Warpdeck.Domain.Icon;
using COSMIC.Warpdeck.Domain.Monitor;
using COSMIC.Warpdeck.Domain.Monitor.Rules;
using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Managers;
using COSMIC.Warpdeck.UseCase.Device;
using COSMIC.Warpdeck.UseCase.Hardware;
using COSMIC.Warpdeck.Windows.Adapter;
using COSMIC.Warpdeck.Windows.Adapter.Monitor;
using COSMIC.Warpdeck.Windows.Plugins.Actions.Launcher;
using COSMIC.Warpdeck.Windows.Plugins.Actions.Macro;
using COSMIC.Warpdeck.Windows.Plugins.Actions.Window;


namespace COSMIC.Warpdeck.Windows
{
    public class WarpdeckWindowsDependencies
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
                    .WithParameter(new NamedParameter("filePath", "PressAndHold.svg"));
                builder.RegisterType<PressAndHold>().As<IconTemplate>().As<IHasProperties>();
                builder.RegisterType<InMemoryIconCache>().As<IIconCache>();

                base.Load(builder);
            }
        }

        public class ActionsModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<KeyMacro>().Named<ButtonAction>(nameof(KeyMacro))
                    .As<IHasActionParameters>();
                builder.RegisterType<LauncherAction>().Named<ButtonAction>(nameof(LauncherAction))
                    .As<IHasActionParameters>();
                builder.RegisterType<ManageWindowAction>().Named<ButtonAction>(nameof(ManageWindowAction))
                    .As<IHasActionParameters>();
                base.Load(builder);
            }
        }

        public class MonitorsModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<MonitorManager>().SingleInstance();
                builder.RegisterType<ActiveWindowMonitor>().Named<IMonitor>(nameof(ActiveWindowMonitor))
                    .SingleInstance();
                builder.RegisterType<ActivateLayerMonitorAction>()
                    .Named<IMonitorRuleAction>(nameof(ActivateLayerMonitorAction));
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
                builder.RegisterType<Web.WarpDeckFrontend>();
                base.Load(builder);
            }
        }

        public class DeviceHostModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<DeviceHostManager>().AsSelf().SingleInstance();
                base.Load(builder);
            }
        }

        public class ClipboardModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<WindowsClipboardManager>().SingleInstance().As<IClipboardManager>();
                builder.RegisterType<ClipListManager>().SingleInstance().AsSelf();
                
                base.Load(builder);
            }
        }
    }
}