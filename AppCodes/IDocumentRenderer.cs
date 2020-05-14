using System;
using System.Windows.Documents;
using Newtonsoft.Json.Linq;

namespace Office.Work.Platform.AppCodes
{
    public interface IDocumentRenderer
    {
        void Render(FlowDocument doc, JArray data, DateTime DataDateTime);
    }
}
