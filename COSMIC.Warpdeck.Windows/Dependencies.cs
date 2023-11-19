using System;
using Autofac;
using COSMIC.Warpdeck.Adapter.Configuration;
using COSMIC.Warpdeck.Adapter.Hardware;
using COSMIC.Warpdeck.Adapter.Icon;
using COSMIC.Warpdeck.Adapter.Monitor;
using COSMIC.Warpdeck.Adapter.PropertyRule;
using COSMIC.Warpdeck.Domain.Configuration;
using COSMIC.Warpdeck.Domain.Hardware;
using COSMIC.Warpdeck.Domain.Icon;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Key.Action;
using COSMIC.Warpdeck.Domain.Key.Behavior;
using COSMIC.Warpdeck.Domain.Monitor;
using COSMIC.Warpdeck.Domain.Monitor.Rules;
using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Rules;
using COSMIC.Warpdeck.Monitor.Action;
using COSMIC.Warpdeck.Monitor.Criteria;
using COSMIC.Warpdeck.UseCase.Device;
using COSMIC.Warpdeck.UseCase.DeviceLayer;
using COSMIC.Warpdeck.UseCase.Hardware;
using COSMIC.Warpdeck.UseCase.Key;
using COSMIC.Warpdeck.UseCase.Layer;
using COSMIC.Warpdeck.UseCase.Property;
using COSMIC.Warpdeck.Windows.Plugins.Actions.Launcher;
using COSMIC.Warpdeck.Windows.Plugins.Actions.Macro;
using COSMIC.Warpdeck.Windows.Plugins.Actions.Window;


namespace COSMIC.Warpdeck.Windows
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
                    .WithParameter(new NamedParameter("filePath", "DefaultIconTemplate.svg"));
                builder.RegisterType<DefaultIconTemplate>().As<IconTemplate>().As<IHasProperties>();
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
                builder.RegisterType<COSMIC.Warpdeck.Web.WarpDeckFrontend>();
                base.Load(builder);
            }
        }

    
    }
}