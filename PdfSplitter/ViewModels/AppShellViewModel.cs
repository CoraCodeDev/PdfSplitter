using PdfSplitter.Services.Interfaces;
using PdfSplitter.Views;
using System.Windows.Input;

namespace PdfSplitter.ViewModels;

public class AppShellViewModel
{
	private IPdfService _pdfService;

    public ICommand OnMenuItemClickedCommand { get; private set; }
    public ICommand OnMenuItemClosePdfClickedCommand { get; private set; }

    public ICommand OnMenuItemAboutClickedCommand { get; private set; }

    private bool PdfOpen = false;

    public AppShellViewModel(IPdfService pdfService)
	{
		_pdfService= pdfService;

		OnMenuItemClickedCommand = new Command<string>(async (s) => await OnMenuItemClicked(s));
        OnMenuItemClosePdfClickedCommand = new Command<string>(async (s) => await OnMenuItemClicked(s), (s) => PdfOpen);

        OnMenuItemAboutClickedCommand = new Command(DisplayAbout);
    }

	public async Task OnMenuItemClicked(string item)
	{
		switch (item) {
			case "Exit":
				Application.Current.Quit();
				break;
			case "Open":
				await SelectFile();
				break;
            case "Close":
				await CloseFile();
                break;
            default:
				break;
		}
	}

	public async Task SelectFile()
	{        
		try
		{
			var options = new PickOptions()
			{
				FileTypes = FilePickerFileType.Pdf
			};

			var fileResult = await FilePicker.PickAsync(options);

			if (fileResult != null)
			{
				await _pdfService.LoadPdf(fileResult.FullPath);

				PdfOpen = true;
				(OnMenuItemClosePdfClickedCommand as Command).ChangeCanExecute();
				await Shell.Current.GoToAsync(nameof(PdfViewPage));
			}
		}
		catch (Exception ex)
		{
			await Shell.Current.DisplayAlertAsync("Error", $"Failed to open PDF file.\n\n{ex.Message}", "OK");
		}
	}

    public async Task CloseFile()
    {
		_pdfService.UnloadPdf();
        PdfOpen = false;
        (OnMenuItemClosePdfClickedCommand as Command).ChangeCanExecute();
        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }

	public void DisplayAbout()
	{
        var about = new Window(new About(new AboutViewModel()));
		about.Width = 650;
		about.Height = 300;
        Application.Current.OpenWindow(about);
    }
}
