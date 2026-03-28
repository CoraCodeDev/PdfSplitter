using PdfSplitter.Models;
using PdfSplitter.Services.Interfaces;
using System.Collections.ObjectModel;
using Windows.Data.Pdf;
using Windows.Storage;

namespace PdfSplitter.Services;

public class PdfService : IPdfService
{
    private PdfDocument _document;
    private string _filePath;

    public PdfService()
    {
        Items = new ObservableCollection<PdfPageItem>();
        SelectedItems = new ObservableCollection<PdfPageItem>(); 
    }   
    
    public async Task LoadPdf(string path)
    {
        foreach (var item in Items)
            item.Dispose();
        foreach (var item in SelectedItems)
            item.Dispose();

        SelectedItems.Clear();
        Items.Clear();

        _filePath = path;

        var file = await StorageFile.GetFileFromPathAsync(path);
        _document = await PdfDocument.LoadFromFileAsync(file);        

        for(int i = 1; i <= _document.PageCount; i++)
        {
            Items.Add(new PdfPageItem
            {
                Page = _document.GetPage((uint)i-1),
                PageNumber = i                

            }); ; 
        }
    }

    public void UnloadPdf()
    {
        foreach (var item in Items)
            item.Dispose();
        foreach (var item in SelectedItems)
            item.Dispose();

        Items.Clear();
        SelectedItems.Clear();
        _document = null;
    }

    public void SelectItem(PdfPageItem item)
    {
        SelectedItems.Add(item);
        Items.Remove(item);
    }

    public void UnselectItem(PdfPageItem item)
    {
        SelectedItems.Remove(item);
        Items.Add(item);
        Items = new ObservableCollection<PdfPageItem>(Items.OrderBy(x => x.PageNumber));
    }

    public ObservableCollection<PdfPageItem> SelectedItems { get; private set; }

    public ObservableCollection<PdfPageItem> Items { get; private set; }

    public string PdfFilePath => _filePath;
}
