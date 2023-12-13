using System.IO;
using COSMIC.Warpdeck.Domain.Icon;
using COSMIC.Warpdeck.Domain.Key;
using COSMIC.Warpdeck.Domain.Property.Descriptors;
using COSMIC.Warpdeck.Icon;
using COSMIC.Warpdeck.Managers;

namespace COSMIC.Warpdeck
{
    public class PressAndHold : IconTemplate
    {
        public static PropertyDescriptor BackgroundColor = PropertyDescriptor.Color("background.fillColor")
            .Named("Background Color")
            .Described("The background color for the icon")
            .WithDefault("#000");

        public static PropertyDescriptor GraphicColor = PropertyDescriptor.Color("graphic.fillColor")
            .Named("Graphic Color")
            .Described("The fill color of the primary grahpic.")
            .WithDefault("#FFF");
        
        public static PropertyDescriptor AccentColor = PropertyDescriptor.Color("accent.fillColor")
            .Named("Accent Color")
            .Described("The accent color.")
            .WithDefault("#FFF");

        public static PropertyDescriptor Text = PropertyDescriptor.Text("text")
            .Named("Key Text")
            .Described("The text that will display on the icon")
            .WithDefault("#FFF");

        public static PropertyDescriptor GraphicPath = PropertyDescriptor.Path("graphic.path")
            .Named("Text")
            .Described("The text that will display on the icon")
            .WithDefault("#FFF");

        public override PropertyDescriptorSet SpecifyProperties() => PropertyDescriptorSet.New().Has(
            Text,
            BackgroundColor,
            GraphicColor,
            GraphicPath,
            AccentColor
        );

        public PressAndHold(ITemplateDocumentProvider templateDocumentProvider,
            PropertyRuleManager propertyRuleManager)
            : base(templateDocumentProvider, propertyRuleManager)
        {
        }


        protected override void DrawIcon(KeyModel key)
        {
            string svgPath = Path.Combine(@"C:\Atom\Resources\Icons", PropertyRule.GetProperty(key, GraphicPath));

            string graphicColorCalc = PropertyRule.GetProperty(key, AccentColor);
            if (key.Properties.HasProperty(GraphicColor.Key))
            {
                graphicColorCalc = PropertyRule.GetProperty(key, GraphicColor);
            }


            AddGraphic("__wd_pb_glyph", svgPath,IconHelpers.GetColorFromHex(graphicColorCalc));
            SetFill("__wd_pb_accent_line", IconHelpers.GetColorFromHex(PropertyRule.GetProperty(key, AccentColor)));
            SetElementText("__wd_pb_text", PropertyRule.GetProperty(key, Text),IconHelpers.GetColorFromHex("#FFFFFF"));
        }
    }
}