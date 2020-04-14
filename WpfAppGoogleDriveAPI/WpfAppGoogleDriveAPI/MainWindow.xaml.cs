using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppGoogleDriveAPI
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        DriveService driveService;
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "Drive API .NET Quickstart";

        public MainWindow()
        {
            InitializeComponent();

            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets { ClientId = @"769245077513-fn4c9ljpm4uk1i4ci1f5t753lleh56i5.apps.googleusercontent.com", ClientSecret = @"qrSaS6yiglJNLd8SBm8z5pRe" },
                new[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile },
                "WpfAppGoogleDriveAPI",
                CancellationToken.None,
                new FileDataStore("Drive.Auth.Store")).Result;

            driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "WpfAppGoogleDriveAPI",
            });


            //File body = updateFile(service, @"photo.jpg", "id2");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fileMetadata = new File()
            {
                Name = "photo.jpg"
            };
            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream("photo.jpg", System.IO.FileMode.Open))
            {
                request = driveService.Files.Create(fileMetadata, stream, "image/jpeg");
                request.Fields = "id";
                var result = request.Upload();
            }
            var file = request.ResponseBody;
            Console.WriteLine("File ID: " + file.Id);

            var test = driveService.BaseUri;

            var fileMetadata2 = new File()
            {
                Name = "photo.zip"
            };
            using (var stream = new System.IO.FileStream("photo.zip", System.IO.FileMode.Open))
            {
                request = driveService.Files.Create(fileMetadata2, stream, "application/zip");
                request.Fields = "id";
                var result = request.Upload();
            }
            var file2 = request.ResponseBody;

            Console.WriteLine("File ID: " + file2.Id);




            //UserCredential credential;

            //using (var stream =
            //    new System.IO.FileStream("credentials.json", System.IO.FileMode.Open, System.IO.FileAccess.Read))
            //{
            //    // The file token.json stores the user's access and refresh tokens, and is created
            //    // automatically when the authorization flow completes for the first time.
            //    string credPath = "token.json";
            //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        GoogleClientSecrets.Load(stream).Secrets,
            //        Scopes,
            //        "user",
            //        CancellationToken.None,
            //        new FileDataStore(credPath, true)).Result;
            //    Console.WriteLine("Credential file saved to: " + credPath);
            //}

            //// Create Drive API service.
            //var service = new DriveService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = ApplicationName,
            //});

            //// Define parameters of request.
            //FilesResource.ListRequest listRequest = service.Files.List();
            //listRequest.PageSize = 10;
            //listRequest.Fields = "nextPageToken, files(id, name)";

            //// List files.
            //IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
            //    .Files;
            //Console.WriteLine("Files:");
            //if (files != null && files.Count > 0)
            //{
            //    foreach (var item in files)
            //    {
            //        Console.WriteLine("{0} ({1})", item.Name, item  .Id);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No files found.");
            //}
            //Console.Read();















        }

        public static File updateFile(DriveService _service, string _uploadFile, string _fileId)
        {

            if (System.IO.File.Exists(_uploadFile))
            {
                File body = new File();
                body.Name = System.IO.Path.GetFileName(_uploadFile);
                body.Description = "File updated by Diamto Drive Sample";
                body.MimeType = GetMimeType(_uploadFile);

                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.UpdateMediaUpload request = _service.Files.Update(body, _fileId, stream, GetMimeType(_uploadFile));
                    var result = request.Upload();
                    if (result.Status == Google.Apis.Upload.UploadStatus.Failed)
                        throw new Exception(result.Exception.Message);

                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    MessageBox.Show("An error occurred: " + e.Message);
                    return null;
                }
            }
            else
            {
                MessageBox.Show("File does not exist: " + _uploadFile);
                return null;
            }

        }

        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

    }
}
