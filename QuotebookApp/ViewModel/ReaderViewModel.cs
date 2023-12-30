namespace QuotebookApp.ViewModel;

public partial class ReaderViewModel : BaseViewModel
{
    private const int filenameLabelHeight = 35;

    BaseDriveService driveService;
    ReaderService readerService;

    [ObservableProperty]
    double downloadProgress;

    [ObservableProperty]
    bool downloadingFile;

    [ObservableProperty]
    List<ReaderFileInfo> files;

    [ObservableProperty]
    ReaderFileInfo selectedFile;

    [ObservableProperty]
    string selectedFilename;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DownloadButtonText))]
    bool fileAvailableBtn;

    [ObservableProperty]
    bool fileSelectedBtn;

    [ObservableProperty]
    int topButtonHeight;

    [ObservableProperty]
    int fileListHeight;

    [ObservableProperty]
    LayoutOptions fileListAlignment;

    public string DownloadButtonText => FileAvailableBtn ? "Update" : "Download";


    public ReaderViewModel(BaseDriveService driveService, ReaderService readerService)
    {
        Title = "Reader";
        IsBusy = false;
        FileAvailableBtn = false;
        FileSelectedBtn = false;
        Files = new List<ReaderFileInfo>();
        SelectedFile = null;
        SelectedFilename = "Selected: None";
        this.driveService = driveService;
        this.readerService = readerService;

        setCollectionListProperties();
    }

    private void setCollectionListProperties()
    {
        TopButtonHeight = 45;
        /* this sucks to do, but it's the only way since there is a bug with .NET MAUI CollectionView scrolling
         * TODO: fix this when the CollectionView is scrolling properly automatically */
#if ANDROID || IOS
        FileListHeight = GlobalData.ClientHeight - (TopButtonHeight + 15) - filenameLabelHeight;
        FileListAlignment = LayoutOptions.Center;
#else
        FileListHeight = GlobalData.ClientHeight - (TopButtonHeight + 15) - filenameLabelHeight;
        FileListAlignment = LayoutOptions.Fill;
#endif
    }

    [RelayCommand]
    async Task GetAvailableFilesAsync()
    {
        try
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Files = await readerService.GetAvailableFiles();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to retrieve list of available files: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task DownloadFileAsync()
    {
        try
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (SelectedFile is null)
                return;

            /* Set up progress bar */
            DownloadProgress = 0.0;
            DownloadingFile = true;

            string filename = SelectedFile.Name;
            string fileId = SelectedFile.ID;
            int filesize = SelectedFile.Size;

            string directory = GlobalData.PdfDownloadDirectory;
            string target = Path.Combine(directory, $"{filename}.pdf");
            if (File.Exists(target))
                File.Delete(target);

            /* Start a thread to asynchronously run the pdf downloader until the file
             * is downloaded or there is an error while downloading */
            _ = Task.Run(() => driveService.DriveDownloadPdf(filename, fileId, filesize));

            /* Progress is as follows:
             * -1 indicates successful download
             * -2 indicates failure during download
             * 0-100 indicates the percentage of the file that has been downloaded 
             * while download is in progress */
            int progress = 0;
            while (true)
            {
                progress = driveService.DownloadProgress;

                if (progress < 0)
                    break; // download complete or failed

                /* Download still in progress */
                DownloadProgress = (double)(progress/100.0);
            }

            if (progress == -1)
            {
                DownloadProgress = 1.0;
                await Shell.Current.DisplayAlert("Complete!", $"The requested file has finished downloading.", "OK");
            }
            else if (progress == -2)
            {
                Debug.WriteLine(driveService.DownloadException.ToString());
                await Shell.Current.DisplayAlert("Error!", $"An error occurred while downloading the requested file: {driveService.DownloadException.Message}", "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to download requested file: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            DownloadingFile = false;

            /* Update button states appropriately based on outcome of download */
            FileSelected(SelectedFile);
        }
    }

    [RelayCommand]
    void FileSelected(ReaderFileInfo file)
    {
        if (IsBusy)
            return;

        if (file is null)
            return;

        FileAvailableBtn = false;
        FileSelectedBtn = true;

        /* Check if the file is already downloaded and set buttons accordingly */
        string directory = GlobalData.PdfDownloadDirectory;
        string target = Path.Combine(directory, $"{file.Name}.pdf");

        if (File.Exists(target))
            FileAvailableBtn = true;

        SelectedFilename = $"Selected: {file.FriendlyName}";
        SelectedFile = file;
    }

    [RelayCommand]
    async Task DeleteFileAsync()
    {
        try
        {
            if (IsBusy)
                return;

            if (SelectedFile is null)
                return;

            IsBusy = true;

            string directory = GlobalData.PdfDownloadDirectory;
            string target = Path.Combine(directory, $"{SelectedFile.Name}.pdf");

            if (File.Exists(target))
                File.Delete(target);

            await Shell.Current.DisplayAlert("Success!", $"File deleted successfully.", "OK");

            /* Update button states appropriately.
             * There seems to be an issue with setting the button state
             * via data binding of the button calling the command while
             * the command is being executed. To fix this I just ran on a
             * separate thread code to disable the delete button after a slight
             * delay to ensure this command has completed running by the time the
             * button is disabled. Really annoying, hopefully it gets fixed soon.
             * TODO: change this line to FileAvailableBtn = false when community
             * toolkit bug is fixed.
             */
            _ = Task.Run(() => { Thread.Sleep(1); FileAvailableBtn = false; });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            await Shell.Current.DisplayAlert("Error!", $"Unable to delete requested file: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task OpenPdfFileAsync()
    {
        if (SelectedFile is null)
            return;

        string directory = GlobalData.PdfDownloadDirectory;
        string target = Path.Combine(directory, $"{SelectedFile.Name}.pdf");
        if (!File.Exists(target))
            return;


        /* I hate having to store this in global memory, but the .NET MAUI QueryProperty
         * is not working for some reason. I've tried about everything I can find for getting
         * this to work. Whenever this .NET MAUI bug is resolved, I can switch to using a
         * QueryProperty to pass this data directly to the PdfReaderViewModel through the Shell
         * instead of storing it in Global memory. When I do this, I will have to move the code
         * to load the PDF out of the constructor and into a setter for some property that will
         * receive the PDF filepath from the shell. TODO: fix this when QueryProperty bug is fixed */
        GlobalData.TargetPdfFile = target;
        await Shell.Current.GoToAsync(nameof(PdfReaderPage));
    }
}
