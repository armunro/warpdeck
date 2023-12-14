using System.IO;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Icon;
using COSMIC.Warpdeck.Domain.Property.Descriptors;
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
            .Named("Button Text")
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


        protected override void DrawIcon(ButtonModel button)
        {
            string svgPath = Path.Combine(@"C:\Atom\Resources\Icons", PropertyRule.GetProperty(button, GraphicPath));

            string graphicColorCalc = PropertyRule.GetProperty(button, AccentColor);
            if (button.Properties.HasProperty(GraphicColor.Key))
            {
                graphicColorCalc = PropertyRule.GetProperty(button, GraphicColor);
            }


            AddGraphic("__wd_pb_glyph", svgPath,IconHelpers.GetColorFromHex(graphicColorCalc));
            SetFill("__wd_pb_accent_line", IconHelpers.GetColorFromHex(PropertyRule.GetProperty(button, AccentColor)));
            SetElementText("__wd_pb_text", PropertyRule.GetProperty(button, Text),IconHelpers.GetColorFromHex("#FFFFFF"));
        }
    }
}