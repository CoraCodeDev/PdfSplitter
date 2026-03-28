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

    public void SelectAll()
    {
        var allItems = Items.ToList();
        foreach (var item in allItems)
        {
            SelectedItems.Add(item);
        }
        Items.Clear();
        SelectedItems = new ObservableCollection<PdfPageItem>(SelectedItems.OrderBy(x => x.PageNumber));
    }

    public void DeselectAll()
    {
        var allItems = SelectedItems.ToList();
        foreach (var item in allItems)
        {
            Items.Add(item);
        }
        SelectedItems.Clear();
        Items = new ObservableCollection<PdfPageItem>(Items.OrderBy(x => x.PageNumber));
    }

    public void SelectPages(IEnumerable<int> pageNumbers)
    {
        var pagesToSelect = Items.Where(x => pageNumbers.Contains(x.PageNumber)).ToList();
        foreach (var item in pagesToSelect)
        {
            SelectedItems.Add(item);
            Items.Remove(item);
        }
        SelectedItems = new ObservableCollection<PdfPageItem>(SelectedItems.OrderBy(x => x.PageNumber));
    }

    public ObservableCollection<PdfPageItem> SelectedItems { get; private set; }

    public ObservableCollection<PdfPageItem> Items { get; private set; }

    public string PdfFilePath => _filePath;
}
