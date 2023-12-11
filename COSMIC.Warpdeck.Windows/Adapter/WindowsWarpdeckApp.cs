using Autofac;

namespace COSMIC.Warpdeck.Windows.Adapter
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WindowsWarpdeckApp
    {
        private readonly string[] _commandLineArgs;
        public static IContainer Container;
        

        public WindowsWarpdeckApp(string[] commandLineArgs)
        {
            _commandLineArgs = commandLineArgs;
        }


        public void RegisterDependencies()
        {
            ContainerBuilder builder = new ContainerBuilder();

            //Register Core dependencies 
            builder.RegisterModule<CoreDependencies.LayersModule>();
            builder.RegisterModule<CoreDependencies.ConfigModule>();
            builder.RegisterModule<CoreDependencies.BehaviorsModule>();
            builder.RegisterModule<CoreDependencies.Property>();
            builder.RegisterModule<CoreDependencies.DevicesModule>();
           
            //Register Windows dependencies
            builder.RegisterModule<Dependencies.BoardModule>();
            builder.RegisterModule<Dependencies.IconsModule>();
            builder.RegisterModule<Dependencies.ActionsModule>();
            builder.RegisterModule<Dependencies.PresentationModule>();
            builder.RegisterModule<Dependencies.MonitorsModule>();
            builder.RegisterModule<Dependencies.ClipboardModule>();

            Container = builder.Build();
            WarpdeckApp.Container = Container;
            Web.WarpDeckFrontend.Container = Container;
        }


        public void StartPresentation()
        {
            Web.WarpDeckFrontend.StartAsync(_commandLineArgs);
        }
    }
}