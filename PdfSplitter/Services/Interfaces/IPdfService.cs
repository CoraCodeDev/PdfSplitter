using PdfSplitter.Models;
using System.Collections.ObjectModel;

namespace PdfSplitter.Services.Interfaces
{
    public interface IPdfService
    {
        Task LoadPdf(string path);

        void UnloadPdf();

        ObservableCollection<PdfPageItem> Items { get; }
        ObservableCollection<PdfPageItem> SelectedItems { get; }

        void SelectItem(PdfPageItem item);

        void UnselectItem(PdfPageItem item);

        void SelectAll();

        void DeselectAll();

        void SelectPages(IEnumerable<int> pageNumbers);

        string PdfFilePath { get; }
    }
}
