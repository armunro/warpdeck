using System.IO;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Icon;
using COSMIC.Warpdeck.Domain.Property.Descriptors;
using COSMIC.Warpdeck.Managers;
// ReSharper disable MemberCanBePrivate.Global

namespace COSMIC.Warpdeck
{
    public class PressAndHold : IconTemplate
    {
        public static readonly PropertyDescriptor BackgroundColor = PropertyDescriptor.Color("BackgroundColor")
            .Named("Background Color")
            .Described("The background color for the icon.")
            .WithDefault("#000");

        public static readonly PropertyDescriptor IconColor = PropertyDescriptor.Color("IconColor")
            .Named("Graphic Color")
            .Described("The fill color of the primary graphic.")
            .WithDefault("#FFF");
        
        public static readonly PropertyDescriptor AccentColor = PropertyDescriptor.Color("AccentColor")
            .Named("Accent Color")
            .Described("The accent color.")
            .WithDefault("#FFF");

        public static readonly PropertyDescriptor Text = PropertyDescriptor.Text("Text")
            .Named("Text")
            .Described("The text that will display on the icon.")
            .WithDefault("#FFF");

        public static readonly PropertyDescriptor GraphicPath = PropertyDescriptor.Path("Icon")
            .Named("Icon")
            .Described("Relative path to the icon svg.")
            .WithDefault(string.Empty);

        public override PropertyDescriptorSet SpecifyProperties() => PropertyDescriptorSet.New().Has(
            Text,
            BackgroundColor,
            IconColor,
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
            if (button.Properties.HasProperty(IconColor.Key))
            {
                graphicColorCalc = PropertyRule.GetProperty(button, IconColor);
            }


            AddGraphic("__wd_pb_glyph", svgPath,IconHelpers.GetColorFromHex(graphicColorCalc));
            SetFill("__wd_pb_accent_line", IconHelpers.GetColorFromHex(PropertyRule.GetProperty(button, AccentColor)));
            SetElementText("__wd_pb_text", PropertyRule.GetProperty(button, Text),IconHelpers.GetColorFromHex("#FFFFFF"));
        }
    }
}