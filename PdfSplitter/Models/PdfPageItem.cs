using Windows.Data.Pdf;
using Windows.Storage.Streams;

namespace PdfSplitter.Models
{
    public class PdfPageItem : IDisposable
    {
        private bool _disposed;

        public PdfPage Page {get;set;}

        public int PageNumber { get; set; }

        public string PageName { get { return $"Page {PageNumber}"; } }

        public bool Selected { get; set; }

        public ImageSource Preview => ImageSource.FromStream(async (token) => (await GeneratePreview()).AsStream());

        public ImageSource PageView => ImageSource.FromStream(async (token) => (await GenerateView()).AsStream());

        public async Task<InMemoryRandomAccessStream> GeneratePreview()
        {
            double resizeFactor = Page.Size.Height / Page.Size.Width;
            double destinationHeight = 120;
            double destinationWidth = destinationHeight / resizeFactor;

            await Page.PreparePageAsync();
            var stream = new InMemoryRandomAccessStream();
            PdfPageRenderOptions pdfPageRenderOptions = new PdfPageRenderOptions();
            pdfPageRenderOptions.DestinationHeight = (uint)destinationHeight;
            pdfPageRenderOptions.DestinationWidth= (uint)destinationWidth;

            await Page.RenderToStreamAsync(stream, pdfPageRenderOptions);
            stream.Seek(0);

            return stream;
        }

        public async Task<InMemoryRandomAccessStream> GenerateView()
        {
            await Page.PreparePageAsync();
            var stream = new InMemoryRandomAccessStream();
            await Page.RenderToStreamAsync(stream);
            stream.Seek(0);

            return stream;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Page?.Dispose();
                Page = null;
                _disposed = true;
            }
        }
    }
}
