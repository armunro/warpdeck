using System.Drawing;
using System.IO;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Icon;
using COSMIC.Warpdeck.Domain.Property;
using COSMIC.Warpdeck.Domain.Property.Descriptors;
using COSMIC.Warpdeck.Managers;
using Svg;
using Svg.Transforms;

namespace COSMIC.Warpdeck
{
    public abstract class IconTemplate : IHasProperties
    {
        private readonly ITemplateDocumentProvider _templateDocumentProvider;
        private SvgDocument Document { get; set; }

        //TODO: Decouple this from the property rule manager so it can isolated to the domain
        public PropertyRuleManager PropertyRule { get; set; }
        protected abstract void DrawIcon(ButtonModel button);
        public abstract PropertyDescriptorSet SpecifyProperties();

        public IconTemplate(ITemplateDocumentProvider templateDocumentProvider, PropertyRuleManager propertyRuleManager)
        {
            _templateDocumentProvider = templateDocumentProvider;
            PropertyRule = propertyRuleManager;
        }

        public KeyIcon GenerateIcon(ButtonModel buttonModel)
        {
            Document = _templateDocumentProvider.ProvideTemplateDocument();
            DrawIcon(buttonModel);
            Bitmap export = new Bitmap(244, 244);
            Graphics graphics = Graphics.FromImage(export);
            Document.Draw(graphics);
            KeyIcon newIcon = new KeyIcon(export);
            return newIcon;
        }


        protected void SetElementText(string elementId, string text, Color fill)
        {
            Document.GetElementById<SvgText>(elementId).Fill = new SvgColourServer(fill);
            Document.GetElementById<SvgText>(elementId).Text = text;
        }

        protected void SetFill(string elementId, Color fill)
        {
            Document.GetElementById(elementId).Fill = new SvgColourServer(fill);
        }

        protected void AddGraphic(string elementId, string graphicPath, Color fill)
        {
            if (File.Exists(graphicPath))
            {
                SvgDocument glyphDoc = SvgDocument.Open(graphicPath);
                var glyph = (SvgPath)glyphDoc.Children[0];
                glyphDoc.Transforms = new SvgTransformCollection()
                {
                    new SvgScale(.35f),
                    new SvgTranslate(230, 360)
                };
                glyph.Fill = new SvgColourServer(fill);
                Document.GetElementById(elementId).Children.Add(glyphDoc);
            }
        }
    }
}