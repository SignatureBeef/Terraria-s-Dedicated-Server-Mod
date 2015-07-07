using System.Net;
using tdsm.api.Logging;

namespace tdsm.api.Misc
{
    public class ProgressWebClient : WebClient
    {
        ProgressLogger _logger;

        public ProgressWebClient(string message)
        {
            _logger = new ProgressLogger(1, message);
        }

        protected override void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            base.OnDownloadProgressChanged(e);

            lock (_logger)
            {
                if (_logger.Max != e.TotalBytesToReceive)
                {
                    _logger.Max = (int)e.TotalBytesToReceive;
                }

                _logger.Value = (int)e.BytesReceived;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_logger != null)
                    _logger.Dispose();
                _logger = null;
            }
        }
    }
}
