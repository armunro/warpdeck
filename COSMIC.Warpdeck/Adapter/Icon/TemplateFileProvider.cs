using COSMIC.Warpdeck.Domain.Icon;
using COSMIC.Warpdeck.Icon;
using Svg;

namespace COSMIC.Warpdeck.Adapter.Icon
{
    public class TemplateDocumentFileProvider : ITemplateDocumentProvider
    {
   
        public string FilePath { get; set; }

        public TemplateDocumentFileProvider(string filePath  )
        {
            FilePath = filePath;
            
        }
        
        public SvgDocument ProvideTemplateDocument()
        {
            var document = SvgDocument.Open(FilePath);
            
            return document;
        }
    }
}