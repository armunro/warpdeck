using Svg;

namespace WarpDeck.Domain.Icon
{
    public interface ITemplateDocumentProvider
    {
        SvgDocument ProvideTemplateDocument();
    }
}