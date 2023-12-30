using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace QuotebookApp.Services;


public class BaseDriveService
{

    public static DriveService service;

    /* Leave this settable so the lambda being passed to ProgressChanged
     * has the ability to change this variable */
    public int DownloadProgress { get; set; }
    public Exception DownloadException { get; private set; }

    public async static void InitializeClient()
    {
        string[] scopes = { DriveService.Scope.Drive };
        service = await BaseService.InitializeService<DriveService>(scopes);
    }

    /* This method must be run asynchronously on a separate thread
     * so it can correctly update any progress bars on the UI. Call
     * this method with the Task.Run function */
    public void DriveDownloadPdf(string filename, string fileId, int filesize)
    {
        try
        {
            var request = service.Files.Get(fileId);
            var stream = new MemoryStream();

            /* Add a handler which will be notified on progress changes.
             * It will notify on each chunk download and when the
             * download is completed or failed. */
            request.MediaDownloader.ProgressChanged +=
                progress =>
                {
                    int status = 0;
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                        {
                            status = Convert.ToInt32(progress.BytesDownloaded / filesize * 100);
                            Debug.WriteLine(status);
                            if (status < 0)
                                status = 0;
                            else if (status > 100)
                                status = 100;
                            break;
                        }
                        case DownloadStatus.Completed:
                        {
                            status = -1;
                            break;
                        }
                        case DownloadStatus.Failed:
                        {
                            throw new Exception("Google Drive API failed to download the file");
                        }
                    }

                    DownloadProgress = status;
                };

            request.Download(stream);

            string directory = GlobalData.PdfDownloadDirectory;
            string target = Path.Combine(directory, $"{filename}.pdf");

            if (File.Exists(target))
                File.Delete(target);

            FileStream filestream = File.OpenWrite(target);
            stream.WriteTo(filestream);
            filestream?.Close();
            stream?.Close();
        }
        catch (Exception ex)
        {
            DownloadProgress = -2; // send indication to ReaderViewModel that an error occurred
            DownloadException = ex; // store the exception so ReaderViewModel can access it
        }
    }
}

