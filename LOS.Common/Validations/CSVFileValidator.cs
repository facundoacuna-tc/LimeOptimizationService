using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Common.Validations
{
    public class CSVFileValidator
    {
        private bool result = false;


        /// <summary>
        /// Validates file location and if file exists
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="fileType">file type, can be Local or Remote</param>
        /// <returns></returns>
        public bool ValidatesFileExist(string filePath, string fileType)
        {
            if (fileType == "Local")
                result = LocalFileExists(filePath);

            if (fileType == "Remote")
                result = RemoteFileExists(filePath);

            return result;
        }



        /// <summary>
        /// Checks if file exists in the path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool LocalFileExists(string path)
        {
            result = File.Exists(path);
            return result;
        }




        /// <summary>
        /// Checks if file exists in the Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Returns TRUE if the Status code == 200</returns>
        private bool RemoteFileExists(string url)
        {
            try
            {
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";

                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                result = (response.StatusCode == HttpStatusCode.OK);
                response.Close();
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }


}
