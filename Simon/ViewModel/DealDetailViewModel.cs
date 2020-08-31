using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Input;
using Plugin.Media.Abstractions;
using Simon.Helpers;
using Simon.Interfaces;
using Xamarin.Forms;

namespace Simon.ViewModel
{
    public class DealDetailViewModel : BaseViewModel
    {
        public string imagePath;
        string FilePath;

        public DealDetailViewModel()
        {
            HeaderTitle = Constants.DealsScreenTitle;
            HeaderLeftImage = "back_arrow.png";
        }

        private MediaFile _imageUrlMediaFile;
        public MediaFile imageUrlMediaFile
        {
            get { return _imageUrlMediaFile; }
            set
            {
                if (value != null)
                {
                    SetProperty(ref _imageUrlMediaFile, value);
                }
            }
        }

        public string _imageUrlfile;
        public string imageUrlfile
        {
            get { return _imageUrlfile; }
            set { SetProperty(ref _imageUrlfile, value); }
        }

        private ImageSource _imageUrl = "image_placeholder.png";
        public ImageSource ImageUrl
        {
            get { return _imageUrl; }
            set { SetProperty(ref _imageUrl, value); }
        }

        public string _FileName;
        public string FileName
        {
            get { return _FileName; }
            set { SetProperty(ref _FileName, value); }
        }

        public ICommand uploadImageCommand { get { return new Command(uploadImageCommandExecute); } }
        private async void uploadImageCommandExecute()
        {
            try
            {
                ImagePicker((string file, MediaFile mediafile) =>
                {
                    if (string.IsNullOrEmpty(file))
                    {
                        return;
                    }
                    else
                    {
                        imageUrlMediaFile = mediafile;
                        imageUrlfile = file;
                        ImageUrl = ImageSource.FromFile(file);
                    }
                });
            }
            catch (Exception ex)
            {
                await ClosePopup();
                Debug.WriteLine(ex.Message);
            }
        }

        public ICommand uploadDocumentCommand { get { return new Command(uploadDocumentCommandExecute); } }
        private void uploadDocumentCommandExecute()
        {
            try
            {
                DocumentPicker(((string name, string FileBase64String, string FileType, string FileName) obj) =>
                {
                    if (obj.name != null && obj.FileName != null && obj.FileBase64String != null && obj.FileType != null)
                    {
                        //AddDocument(obj.FileName, obj.FileBase64String, obj.FileType);
                        FileName = obj.FileName;
                    }
                    else
                    {
                        return;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: { ex.Message}");
            }
        }

        public ICommand ByteConverterCommand { get { return new Command(ByteConverterCommandExecute); } }
        private void ByteConverterCommandExecute()
        {
            ConvertIntoByte(FilePath);
        }

        public void ConvertIntoByte(string image)
        {
            try
            {
                byte[] imageData = File.ReadAllBytes(image);

                //string url = "";

                //url = Constants.BaseServiceURL + Constants.AddExericseImageURL;

                //create new HttpClient and MultipartFormDataContent and add our file
                string boundary = "---8d0f01e6b3b5dafaaadaad";
                HttpClient client = new HttpClient();
                //client.DefaultRequestHeaders.Add("AccessToken", Settings.AuthorizationToken);
                //client.DefaultRequestHeaders.Add("IdentityTokenKey", Settings.IdentityToken);
                client.Timeout = new TimeSpan(0, 10, 0);
                MultipartFormDataContent multipartContent = new MultipartFormDataContent(boundary);
                ByteArrayContent baContent = new ByteArrayContent(imageData);

                baContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                baContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "Upload",
                    FileName = Path.GetFileName(image)
                };

                multipartContent.Add(baContent);

                //upload MultipartFormDataContent content async and store response in response var
                //var response = await client.PostAsync(url + "/" + ExerciseId, multipartContent);

                ////read response result as a string async into json var
                //var result = response.Content.ReadAsStringAsync().Result;
                //Debug.WriteLine("\nResult : " + result);
                //var resultobject = JsonConvert.DeserializeObject<AddExericseImageResponseModel>(result);
                //return resultobject;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void OpenPDFOrDocs(string NameOfFile)
        {
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, NameOfFile);

                DependencyService.Get<IOpenFiles>().OpenAppFiles(NameOfFile, filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}