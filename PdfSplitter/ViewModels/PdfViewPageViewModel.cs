using PdfSplitter.Models;
using PdfSplitter.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Windows.Storage.Pickers;

namespace PdfSplitter.ViewModels
{
    public class PdfViewPageViewModel : BindableModelBase
    {
        private IPdfService _pdfService;
        private ISavePdfService _savePdfService;
        private PdfPageItem _selectedPageItem;
        private string _pageRangeText;
        public ICommand OnItemSelectedCommand { get; private set; }
        public ICommand OnItemRemovedCommand { get; private set; }
        public ICommand OnSaveFileCommand { get; private set; }
        public ICommand OnSelectAllCommand { get; private set; }
        public ICommand OnDeselectAllCommand { get; private set; }
        public ICommand OnApplyPageRangeCommand { get; private set; }

        public PdfViewPageViewModel(IPdfService pdfService, ISavePdfService savePdfService)
        {
            _pdfService = pdfService;
            _savePdfService = savePdfService;
            OnItemSelectedCommand = new Command(SelectPage);
            OnItemRemovedCommand = new Command<string>(RemovePage);
            OnSaveFileCommand =  new Command<string>(async (s) => await OnSaveClicked(s));
            OnSelectAllCommand = new Command(SelectAll);
            OnDeselectAllCommand = new Command(DeselectAll);
            OnApplyPageRangeCommand = new Command(async () => await ApplyPageRange());
        }

        private async Task OnSaveClicked(string s)
        {
            try
            {
                var window = new Microsoft.UI.Xaml.Window();
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("PDF Document", new List<string>() { ".pdf" });
                savePicker.SuggestedFileName = $"New_Document_{DateTime.Now:ddMMyyyy_HHmmss}";

                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);
                var file = await savePicker.PickSaveFileAsync();

                if (file == null)
                    return;

                await _savePdfService.SavePdf(_pdfService.PdfFilePath, file, _pdfService.SelectedItems.Select(x => x.PageNumber).ToList());

                await App.Current.Windows[0].Page.DisplayAlertAsync("Success", "File has been saved", "OK");
                _pdfService.SelectedItems.Clear();
                NotifyPropertyChanged(() => DisplaySaveButton);
            }
            catch (Exception ex)
            {
                await App.Current.Windows[0].Page.DisplayAlertAsync("Error", $"Failed to save PDF file.\n\n{ex.Message}", "OK");
            }
        }

        public ObservableCollection<PdfPageItem> Items => _pdfService.Items;

        public ObservableCollection<PdfPageItem> SelectedItems => _pdfService.SelectedItems;

        public PdfPageItem SelectedPageItem 
        {
            get 
            {     
                if(_selectedPageItem == null && _pdfService.Items.Count > 0)
                {
                    _selectedPageItem = _pdfService.Items[0];
                }
                return _selectedPageItem;
            }
            set 
            { 
                _selectedPageItem = value;
                NotifyPropertyChanged(() => SelectedPageItem);
            }
        }

        public string PageRangeText
        {
            get => _pageRangeText;
            set
            {
                _pageRangeText = value;
                NotifyPropertyChanged(() => PageRangeText);
            }
        }

        public bool PdfOpen { get; private set; }

        public void SelectPage()
        {
            if(_selectedPageItem != null)
            {
                _pdfService.SelectItem(_selectedPageItem);

                int selectedItemPageNo = _selectedPageItem.PageNumber;

                SelectedPageItem = _pdfService.Items.FirstOrDefault(x => x.PageNumber > selectedItemPageNo);

                NotifyPropertyChanged(() => DisplaySaveButton);
            }
        }

        public void RemovePage(string pageName)
        {
            var pageToRemove = SelectedItems.FirstOrDefault(x => x.PageName == pageName);

            if (pageToRemove != null)
            {
                _pdfService.UnselectItem(pageToRemove);
                NotifyPropertyChanged(() => Items);
                NotifyPropertyChanged(() => DisplaySaveButton);
            }
        }

        public void SelectAll()
        {
            _pdfService.SelectAll();
            SelectedPageItem = _pdfService.Items.FirstOrDefault();
            NotifyPropertyChanged(() => DisplaySaveButton);
        }

        public void DeselectAll()
        {
            _pdfService.DeselectAll();
            SelectedPageItem = _pdfService.Items.FirstOrDefault();
            NotifyPropertyChanged(() => DisplaySaveButton);
        }

        public async Task ApplyPageRange()
        {
            if (string.IsNullOrWhiteSpace(PageRangeText))
            {
                await App.Current.Windows[0].Page.DisplayAlertAsync("Error", "Please enter a page range (e.g. 1-5, 10, 15-20)", "OK");
                return;
            }

            var pageNumbers = ParsePageRange(PageRangeText);

            if (pageNumbers == null)
            {
                await App.Current.Windows[0].Page.DisplayAlertAsync("Error", "Invalid page range format. Use: 1-5, 10, 15-20", "OK");
                return;
            }

            int maxPage = _pdfService.Items.Concat(_pdfService.SelectedItems).Max(x => x.PageNumber);
            var validPages = pageNumbers.Where(p => p >= 1 && p <= maxPage).ToList();

            if (!validPages.Any())
            {
                await App.Current.Windows[0].Page.DisplayAlertAsync("Error", $"No valid pages in range. Document has pages 1-{maxPage}", "OK");
                return;
            }

            _pdfService.SelectPages(validPages);
            SelectedPageItem = _pdfService.Items.FirstOrDefault();
            NotifyPropertyChanged(() => DisplaySaveButton);

            await App.Current.Windows[0].Page.DisplayAlertAsync("Success", $"Selected {validPages.Count} page(s)", "OK");
        }

        private static List<int> ParsePageRange(string input)
        {
            var result = new List<int>();
            var pattern = @"^\s*(\d+(-\d+)?)\s*(,\s*(\d+(-\d+)?))*\s*$";

            if (!Regex.IsMatch(input, pattern))
                return null;

            var parts = input.Split(',');

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (trimmed.Contains('-'))
                {
                    var range = trimmed.Split('-');
                    if (range.Length == 2 && int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
                    {
                        if (start > end)
                            return null;
                        for (int i = start; i <= end; i++)
                            result.Add(i);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (int.TryParse(trimmed, out int page))
                        result.Add(page);
                    else
                        return null;
                }
            }

            return result.Distinct().OrderBy(x => x).ToList();
        }

        public bool DisplaySaveButton => _pdfService.SelectedItems.Any();
    }
}
