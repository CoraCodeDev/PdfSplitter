using PdfSplitter.ViewModels;
using PdfSplitter.Views;
using System.Reflection;

namespace PdfSplitter;

public partial class App : Application
{
	private readonly AppShellViewModel _viewModel;

	public App(AppShellViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
	}

	protected override Window CreateWindow(IActivationState activationState)
	{
		var shell = new AppShell(_viewModel);
		shell.Title = $"PDF Splitter v{ ((AssemblyInformationalVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).FirstOrDefault()).InformationalVersion }";
		return new Window(shell);
	}
}
