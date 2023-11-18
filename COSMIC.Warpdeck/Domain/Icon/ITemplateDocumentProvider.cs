using Svg;

namespace COSMIC.Warpdeck.Domain.Icon
{
    public interface ITemplateDocumentProvider
    {
        SvgDocument ProvideTemplateDocument();
    }
}