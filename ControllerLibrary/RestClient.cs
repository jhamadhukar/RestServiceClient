using System;
using System.IO;
using System.Net;
using System.Text;

namespace ControllerLibrary
{
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class RestClient
    {
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }

        public RestClient()
        {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "application/json";//"text/xml";
            PostData = "";
        }
        public RestClient(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "application/json";//"text/xml";
            PostData = "";
        }
        public RestClient(string endpoint, HttpVerb method)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "text/xml";
            PostData = "";
        }

        public RestClient(string endpoint, HttpVerb method, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "text/xml";
            PostData = postData;
        }


        public string MakeRequest()
        {
            return MakeRequest("");
        }

        public string MakeRequest(string parameters, bool sendChunked=false)
        {
            try
            {
                //var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);
                Uri uri = new Uri(EndPoint + parameters);
                var request = (HttpWebRequest)WebRequest.Create(uri);

                request.Method = Method.ToString();
                request.ContentLength = 0;
                request.ContentType = ContentType;
                //request.Headers.Add("SOAPAction", EndPoint);
                request.Timeout = 30000000;
                if (sendChunked)
                    request.SendChunked = true;

                request.Headers.Add("ContentType", "text/xml");
                try
                {
                    if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
                    {
                        request.ContentLength = PostData.Length;

                        var streamWriter = new StreamWriter(request.GetRequestStream());

                            streamWriter.Write(PostData);
                            streamWriter.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var responseValue = string.Empty;

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }

                    return responseValue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
               
            }
        }
    }
}