using PdfSplitter.Models;
using PdfSplitter.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Storage.Pickers;

namespace PdfSplitter.ViewModels
{
    public class PdfViewPageViewModel : BindableModelBase
    {
        private IPdfService _pdfService;
        private ISavePdfService _savePdfService;
        private PdfPageItem _selectedPageItem;
        public ICommand OnItemSelectedCommand { get; private set; }
        public ICommand OnItemRemovedCommand { get; private set; }
        public ICommand OnSaveFileCommand { get; private set; }

        public PdfViewPageViewModel(IPdfService pdfService, ISavePdfService savePdfService)
        {
            _pdfService = pdfService;
            _savePdfService = savePdfService;
            OnItemSelectedCommand = new Command(SelectPage);
            OnItemRemovedCommand = new Command<string>(RemovePage);
            OnSaveFileCommand =  new Command<string>(async (s) => await OnSaveClicked(s));
        }

        private async Task OnSaveClicked(string s)
        {
            var window = new Microsoft.UI.Xaml.Window();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("PDF Document", new List<string>() { ".pdf" });
            savePicker.SuggestedFileName = $"New_Document_{DateTime.Now:ddMMyyyy_HHmmss}";

            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);
            var file = await savePicker.PickSaveFileAsync();

            await _savePdfService.SavePdf(_pdfService.PdfFilePath, file, _pdfService.SelectedItems.Select(x => x.PageNumber).ToList());

            await App.Current.Windows[0].Page.DisplayAlertAsync("Success", "File has been saved", "OK");
            _pdfService.SelectedItems.Clear();
            NotifyPropertyChanged(() => DisplaySaveButton);
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

        public bool DisplaySaveButton => _pdfService.SelectedItems.Any();
    }
}
