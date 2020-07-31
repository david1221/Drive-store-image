using Google.Apis.Auth.OAuth2;
using System;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Google.Apis.Util.Store;
using WebDriveImageApp.Models;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Google.Apis.Drive.v2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;

namespace WebDriveImageApp.ServiseRepozitory
{

    public class DriveMethod
    {
        string info = "";
        private readonly IWebHostEnvironment _env;
        public DriveMethod(IWebHostEnvironment env, string info)
        {
            _env = env;
            this.info = info;
        }
        private static string FName;
        private static string LName;
        public static string[] Scopes = { "https://www.googleapis.com/auth/drive.file","https://www.googleapis.com/auth/userinfo.profile",
 };
        static string ApplicationName = "Drive API Web";

        /// <summary>
        /// get Service
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Google.Apis.Drive.v3.DriveService GetService(string info)
        {
            UserCredential credential;

            using (var stream = new FileStream("client_secret_9.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = $"token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                                Scopes,
                                "user-" + info,
                                CancellationToken.None,
                                new FileDataStore(credPath, true)).Result;
            }
            Google.Apis.Drive.v3.DriveService service = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,

            });
            try
            {
                var oauthSerivce = new Oauth2Service(new BaseClientService.Initializer { HttpClientInitializer = credential });
                var UserInfo = oauthSerivce.Userinfo.Get().Execute();

                FName = UserInfo.FamilyName;
                LName = UserInfo.GivenName;
            }
            catch (Exception)
            {
                ;
            }

            return service;
        }

        public static UserInformation Getuser(string info)
        {
            GetService(info);

            return new UserInformation { LName = LName, FName = FName };
        }

        public static List<Image> GetDriveFiles(string info)
        {
            Google.Apis.Drive.v3.DriveService service = GetService(info);


            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();

            FileListRequest.Fields = "nextPageToken, files(*)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<Image> FileList = new List<Image>();


            // For getting only folders
            files = files.Where(x => x.MimeType == "image/png" || x.MimeType == "image/jpeg" || x.MimeType == "image/gif").Where(s => s.Size > 0).ToList();



            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Image File = new Image
                    {
                        Id = file.Id,
                        Name = file.Name,
                        MimeType = file.MimeType,
                        ImageLink = file.ThumbnailLink,
                    };
                    FileList.Add(File);
                }
            }
            return FileList;
        }

        public void FileUpload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                Google.Apis.Drive.v3.DriveService service = GetService(info);
                string path = Path.Combine(_env.WebRootPath, "image", "GoogleDriveFiles");
                var filePath = Path.Combine(path, file.FileName);
                Path.GetFileName(file.FileName);
                if (file.Length > 0)
                {

                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                }
                string contentType = null;
                new FileExtensionContentTypeProvider().TryGetContentType(filePath, out contentType);

                var FileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(file.FileName),
                    MimeType = contentType ?? "application/octet-stream",
                };
                Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();
                }
            }
        }
    }
}
