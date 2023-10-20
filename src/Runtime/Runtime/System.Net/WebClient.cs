
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace OpenSilver.Compatibility
{
    /// <summary>
    /// Provides common methods for sending data to and receiving data from a resource
    /// identified by a URI.
    /// </summary>
    /// <example>
    /// Here is an example of a use of the WebClient to receive data:
    /// <code lang="C#">
    /// //We create the WebClient with the right encoding and headers:
    /// var webClient = new WebClient();
    /// webClient.Encoding = Encoding.UTF8;
    /// webClient.Headers[HttpRequestHeader.Accept] = "application/xml";
    /// 
    /// //We submit the request to the server and wait for its response:
    /// string response = await webClient.DownloadStringTaskAsync("http://someAddress.com");
    /// 
    /// //We modify the response so that it can be deserialized (deserialization is not perfect yet):
    /// response = response.Replace(@"xmlns=""http://NameSpaceOfTheDeserialization""", "");
    /// response = "&lt;ToDoItemsWrapper&gt;" + response.Replace("ArrayOfToDoItem", "ToDoItems") + "&lt;/ToDoItemsWrapper&gt;"; // Workaround for the fact that "ArrayOf" types cannot be directly deserialized by the XmlSerializer in this Beta version.
    /// 
    /// //We create the Deserializer:
    /// var deserializer = new XmlSerializer(typeof(ToDoItemsWrapper));
    /// var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(response));
    /// var xmlReader = XmlReader.Create(memoryStream);
    /// 
    /// //We deserialize:
    /// ToDoItemsWrapper items = (ToDoItemsWrapper)deserializer.Deserialize(xmlReader);
    /// </code>
    /// Here is what the ToDoItemsWrapper class looks like (with ToDoItem a Serializable class) :
    /// <code lang="C#">
    /// // Workaround for the fact that "ArrayOf" types cannot directly be deserialized by the XmlSerializer in this Beta version:
    /// [DataContract]
    /// public partial class ToDoItemsWrapper
    /// {
    ///     public List&lt;ToDoItem&gt; ToDoItems { get; set; }
    /// }
    /// </code>
    /// Here is another example that shows how you can use the WebClient to send data:
    /// <code lang="C#">
    /// //We parse the data in a string:
    /// string data = string.Format(@"{{""Id"": ""{0}"",""Description"": ""{1}""}}"Guid.NewGuid(), MyTextBox.Text.Replace("\"", "'"));
    /// //We create the WebClient:
    /// var webClient = new WebClient();
    /// We set the encoding and Headers (note: our data is formatted in json so we set the HttpRequestHeader.ContentType header accordingly) 
    /// webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
    /// webClient.Encoding = Encoding.UTF8;
    /// string response = await webClient.UploadStringTaskAsync("http://cshtml5-rest-sample.azurewebsites.net/api/Todo/", "POST", data);
    /// </code>
    /// </example>
    public partial class WebClient // : Component
    {
        //todo: handle the DownloadStringCompletedEventArgs properly when in javascript (otherwise, this whole thing is pretty much useless : people want to know when an error has occured while downloading the resource or when the downloading has been cancelled)



        // Returns:
        //     A System.Text.Encoding that is used to encode strings. The default value
        //     of this property is the encoding returned by System.Text.Encoding.Default.
        /// <summary>
        /// Gets or sets the System.Text.Encoding used to upload and download strings.
        /// </summary>
        public Encoding Encoding { get; set; }

        private WebHeaderCollection _headers = new WebHeaderCollection();
        // Returns:
        //     A System.Net.WebHeaderCollection containing header name/value pairs associated
        //     with this request.
        /// <summary>
        /// Gets or sets a collection of header name/value pairs associated with the
        /// request.
        /// </summary>
        public WebHeaderCollection Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }



        /// <summary>
        /// Initializes a new instance of the System.Net.WebClient class.
        /// </summary>
        public WebClient()
        {
        }

        /// <summary>
        /// Occurs when an asynchronous resource-download operation completes.
        /// </summary>
        public event DownloadStringCompletedEventHandler DownloadStringCompleted;

        //todo: replace that with something in javascript (and see todo on INTERNAL_TestIfCompletedStatus even though this will not be used that way)
        void OnDownloadStringCompleted(object sender, INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e)
        {
            if (DownloadStringCompleted != null)
            {
                DownloadStringCompleted(sender, new DownloadStringCompletedEventArgs(e));
            }
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The address parameter is null.
        //
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- An error occurred while downloading the resource.
        //
        //   System.NotSupportedException:
        //     The method has been called simultaneously on multiple threads.
        /// <summary>
        /// Downloads the requested resource as a System.String. The resource to download
        /// is specified as a System.String containing the URI.
        /// </summary>
        /// <param name="address">A System.String containing the URI to download.</param>
        /// <returns>A System.String containing the requested resource.</returns>
        public string DownloadString(string address)
        {
            return DownloadString(new Uri(address));
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The address parameter is null.
        //
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- An error occurred while downloading the resource.
        //
        //   System.NotSupportedException:
        //     The method has been called simultaneously on multiple threads.
        /// <summary>
        /// Downloads the requested resource as a System.String. The resource to download
        /// is specified as a System.Uri.
        /// </summary>
        /// <param name="address">A System.Uri object containing the URI to download.</param>
        /// <returns>A System.String containing the requested resource.</returns>
        public string DownloadString(Uri address)
        {
            if (Interop.IsRunningInTheSimulator_WorkAround)
            {
                // When running in the Simulator, we use the WebClient implementation provided by WPF:
                var netStandardWebClient = new System.Net.WebClient();
                return netStandardWebClient.DownloadString(address);
            }
            
            if (address == null)
            {
                throw new ArgumentNullException("The address parameter in DownloadString cannot be null");
            }
            Guid guid = Guid.NewGuid();

            INTERNAL_WebRequestHelper_JSOnly webRequestHelper = new INTERNAL_WebRequestHelper_JSOnly();

            webRequestHelper.DownloadStringCompleted -= OnDownloadStringCompleted;
            webRequestHelper.DownloadStringCompleted += OnDownloadStringCompleted;

            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (string key in _headers.AllKeys)
            {
                headers.Add(key, _headers.Get(key)); //todo-perf: improve performance?
            }
            return webRequestHelper.MakeRequest(address, "GET", this, headers, null, OnDownloadStringCompleted, false, GetCredentialsMode());
        }

        // Exceptions:
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- An error occurred while downloading the resource.
        /// <summary>
        /// Downloads the resource specified as a System.Uri. This method does not block
        /// the calling thread.
        /// </summary>
        /// <param name="address">A System.Uri containing the URI to download.</param>
        public void DownloadStringAsync(Uri address)
        {
            DownloadStringAsync(address, null);
        }

        // Exceptions:
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- An error occurred while downloading the resource.
        /// <summary>
        /// Downloads the specified string to the specified resource. This method does
        /// not block the calling thread.
        /// </summary>
        /// <param name="address">A System.Uri containing the URI to download.</param>
        /// <param name="userToken">
        /// A user-defined object that is passed to the method invoked when the asynchronous
        /// operation completes.
        /// </param>
        private void DownloadStringAsync(Uri address, object userToken)
        {
            if (Interop.IsRunningInTheSimulator_WorkAround)
            {
                // When running in the Simulator, we use the WebClient implementation provided by WPF:
                var netStandardWebClient = new System.Net.WebClient();
                netStandardWebClient.DownloadStringCompleted += (s, e) =>
                 {
                     OnDownloadStringCompleted(this, new INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs()
                     {
                         Result = e.Result,
                         Cancelled = e.Cancelled,
                         Error = e.Error,
                         UserState = e.UserState
                     });
                 };
                netStandardWebClient.DownloadStringAsync(address);
                return;
            }

            if (address == null)
            {
                throw new ArgumentNullException("The address parameter in DownloadStringAsync cannot be null");
            }
            INTERNAL_WebRequestHelper_JSOnly webRequestHelper = new INTERNAL_WebRequestHelper_JSOnly();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (string key in _headers.AllKeys)
            {
                headers.Add(key, _headers.Get(key)); //todo-perf: improve performance?
            }

            webRequestHelper.MakeRequest(address, "GET", this, headers, null, OnDownloadStringCompleted, true, GetCredentialsMode());


            //define the XMLHttpRequest:
            //_xmlHttpRequest = GetWebRequest();

            ////define the action when the xmlhttp has finished the request:
            //INTERNAL_SetCallbackMethod((object)_xmlHttpRequest, OnDownloadStringCompleted);

            ////create the request:
            //INTERNAL_CreateRequest((object)_xmlHttpRequest, address.OriginalString, true);

            ////send the request:
            //INTERNAL_SendRequestAsync((object)_xmlHttpRequest, address, userToken);
        }


        /// <summary>
        /// Downloads the resource as a String from the URI specified as an asynchronous operation using a task object.
        /// </summary>
        /// <param name="address">A System.Uri containing the URI to download.</param>
        /// <returns>Returns the resource as <see cref="System.Threading.Tasks.Task{TResult}"/>.</returns>
        public Task<string> DownloadStringTaskAsync(string address)
        {
            return DownloadStringTaskAsync(new Uri(address));
        }



        /// <summary>
        /// Downloads the resource as a String from the URI specified as an asynchronous operation using a task object.
        /// </summary>
        /// <param name="address">A System.Uri containing the URI to download.</param>
        /// <returns>Returns the resource as <see cref="System.Threading.Tasks.Task{TResult}"/>.</returns>
        public Task<string> DownloadStringTaskAsync(Uri address)
        {
            if (Interop.IsRunningInTheSimulator_WorkAround)
            {
                // When running in the Simulator, we use the WebClient implementation provided by WPF:
                var netStandardWebClient = new System.Net.WebClient();
                return netStandardWebClient.DownloadStringTaskAsync(address);
            }

            INTERNAL_WebRequestHelper_JSOnly webRequestHelper = new INTERNAL_WebRequestHelper_JSOnly();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (string key in _headers.AllKeys)
            {
                headers.Add(key, _headers.Get(key)); //todo-perf: improve performance?
            }

            var taskCompletionSource = new TaskCompletionSource<string>();
            webRequestHelper.MakeRequest(address, "GET", this, headers, null, (sender, args) => TriggerDownloadStringTaskCompleted(taskCompletionSource, args), true, GetCredentialsMode());

            return taskCompletionSource.Task;
        }

        private void TriggerDownloadStringTaskCompleted(TaskCompletionSource<string> taskCompletionSource, INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                taskCompletionSource.SetResult(e.Result);
            }
            else
            {
                taskCompletionSource.TrySetException(e.Error);
            }
        }

        // for compilation only, because this file is not included in simulator
        public WebHeaderCollection ResponseHeaders
        {
            get;
            private set;
        }
        private CredentialsMode GetCredentialsMode()
        {

            System.Reflection.PropertyInfo credentialMode = this.GetType().GetProperty("CredentialsMode");

            if (credentialMode != null)
            {
                object mode = credentialMode.GetValue(this);

                return (CredentialsMode)mode;
            }

            return CredentialsMode.Disabled;
        }



        // Exceptions:
        //   System.ArgumentNullException:
        //     The address parameter is null.-or-The data parameter is null.
        //
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- There was no response from the server hosting the resource.
        /// <summary>
        /// Uploads the specified string to the specified resource, using the POST method.
        /// </summary>
        /// <param name="address">
        /// The URI of the resource to receive the string. For Http resources, this URI
        /// must identify a resource that can accept a request sent with the POST method,
        /// such as a script or ASP page.
        /// </param>
        /// <param name="data">The string to be uploaded.</param>
        /// <returns>A System.String containing the response sent by the server.</returns>
        public string UploadString(string address, string data)
        {
            return UploadString(new Uri(address), "POST", data);

        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The address parameter is null.-or-The data parameter is null.
        //
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- There was no response from the server hosting the resource.
        /// <summary>
        /// Uploads the specified string to the specified resource, using the POST method.
        /// </summary>
        /// <param name="address">
        /// The URI of the resource to receive the string. For Http resources, this URI
        /// must identify a resource that can accept a request sent with the POST method,
        /// such as a script or ASP page.
        /// </param>
        /// <param name="data">The string to be uploaded.</param>
        /// <returns>A System.String containing the response sent by the server.</returns>
        public string UploadString(Uri address, string data)
        {
            return UploadString(address, "POST", data);
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The address parameter is null.-or-The data parameter is null.
        //
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- There was no response from the server hosting the resource.-or-method
        //     cannot be used to send content.
        /// <summary>
        /// Uploads the specified string to the specified resource, using the specified
        /// method.
        /// </summary>
        /// <param name="address">
        /// The URI of the resource to receive the file. This URI must identify a resource
        /// that can accept a request sent with the method method.
        /// </param>
        /// <param name="method">
        /// The HTTP method used to send the string to the resource. If null, the default
        /// is POST for http and STOR for ftp.
        /// </param>
        /// <param name="data">The string to be uploaded.</param>
        /// <returns>A System.String containing the response sent by the server.</returns>
        public string UploadString(string address, string method, string data)
        {
            return UploadString(new Uri(address), method, data);
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The address parameter is null.-or-The data parameter is null.
        //
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- There was no response from the server hosting the resource.-or-method
        //     cannot be used to send content.
        /// <summary>
        /// Uploads the specified string to the specified resource, using the specified
        /// method.
        /// </summary>
        /// <param name="address">
        /// The URI of the resource to receive the file. This URI must identify a resource
        /// that can accept a request sent with the method method.
        /// </param>
        /// <param name="method">
        /// The HTTP method used to send the string to the resource. If null, the default
        /// is POST for http and STOR for ftp.
        /// </param>
        /// <param name="data">The string to be uploaded.</param>
        /// <returns>A System.String containing the response sent by the server.</returns>
        public string UploadString(Uri address, string method, string data)
        {
            return UploadString(address, method, data, null, false);
        }

        private string UploadString(Uri address, string method, string data, INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventHandler onCompleted, bool isAsync) //todo: see if we should use UploadStringCompletedEventHandler instead
        {
            if (Interop.IsRunningInTheSimulator_WorkAround)
            {
                var netStandardWebClient = new System.Net.WebClient();
                if (isAsync)
                {
                    netStandardWebClient.UploadStringCompleted += (s, e) =>
                    {
                        onCompleted(this, new INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs()
                        {
                            Result = e.Result,
                            Cancelled = e.Cancelled,
                            Error = e.Error,
                            UserState = e.UserState
                        });
                    };
                    netStandardWebClient.UploadStringAsync(address, method, data);
                    return "";
                }
                else
                {
                    return netStandardWebClient.UploadString(address, method, data);
                }
                
            }

            INTERNAL_WebRequestHelper_JSOnly webRequestHelper = new INTERNAL_WebRequestHelper_JSOnly();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            // Don't add the content-length we get the error :Refused to set unsafe header "Content-Length"
            //if (data != null && data.Length > 0)
            //{
            //    headers.Add("Content-Length", data.Length.ToString());
            //    //headers["Content-Type"] = ContentType; //todo: determine content type
            //}
            foreach (string key in _headers.AllKeys)
            {
                headers.Add(key, _headers.Get(key)); //todo-perf: improve performance?
            }
            return webRequestHelper.MakeRequest(address, method, this, headers, data, onCompleted, isAsync, GetCredentialsMode());
        }


        // Exceptions:
        //   System.ArgumentNullException:
        //     data is null.
        //
        //   System.ArgumentNullException:
        //     The address parameter is null.-or-The data parameter is null.
        //
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- There was no response from the server hosting the resource.
        /// <summary>
        /// Uploads the specified string to the specified resource. This method does
        /// not block the calling thread.
        /// </summary>
        /// <param name="address">
        /// The URI of the resource to receive the file. For HTTP resources, this URI
        /// must identify a resource that can accept a request sent with the POST method,
        /// such as a script or ASP page.
        /// </param>
        /// <param name="data">The string to be uploaded.</param>
        public void UploadStringAsync(Uri address, string data)
        {
            UploadStringAsync(address, "POST", data);
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The address parameter is null.-or-The data parameter is null.
        //
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- There was no response from the server hosting the resource.
        /// <summary>
        /// Uploads the specified string to the specified resource. This method does
        /// not block the calling thread.
        /// </summary>
        /// <param name="address">
        /// The URI of the resource to receive the file. For HTTP resources, this URI
        /// must identify a resource that can accept a request sent with the POST method,
        /// such as a script or ASP page.
        /// </param>
        /// <param name="method">
        /// The HTTP method used to send the file to the resource. If null, the default
        /// is POST for http and STOR for ftp.
        /// </param>
        /// <param name="data">The string to be uploaded.</param>
        public void UploadStringAsync(Uri address, string method, string data)
        {
            UploadString(address, method, data, INTERNAL_OnUploadStringCompleted, true);
        }

        private void INTERNAL_OnUploadStringCompleted(object sender, INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e)
        {
            OnUploadStringCompleted(sender, new UploadStringCompletedEventArgs(e));
        }

        /// <summary>
        /// Uploads the specified string to the specified resource as an asynchronous operation using a task object.
        /// </summary>
        /// <param name="address">
        /// The URI of the resource to receive the string. For HTTP resources, this URI
        /// must identify a resource that can accept a request sent with the POST method,
        /// such as a script or ASP page.
        /// </param>
        /// <param name="data">The string to be uploaded.</param>
        /// <returns>
        /// Returns <see cref="System.Threading.Tasks.Task{TResult}"/>.
        /// The task object representing the asynchronous operation. The Result property
        /// on the task object returns a String containing the response sent by the server.
        /// </returns>
        public Task<string> UploadStringTaskAsync(string address, string data)
        {
            return UploadStringTaskAsync(new Uri(address), "POST", data);
        }

        /// <summary>
        /// Uploads the specified string to the specified resource as an asynchronous operation using a task object.
        /// </summary>
        /// <param name="address">
        /// The URI of the resource to receive the string. For HTTP resources, this URI
        /// must identify a resource that can accept a request sent with the POST method,
        /// such as a script or ASP page.
        /// </param>
        /// <param name="data">The string to be uploaded.</param>
        /// <returns>
        /// Returns <see cref="System.Threading.Tasks.Task{TResult}"/>.
        /// The task object representing the asynchronous operation. The Result property
        /// on the task object returns a String containing the response sent by the server.
        /// </returns>
        public Task<string> UploadStringTaskAsync(Uri address, string data)
        {
            return UploadStringTaskAsync(address, "POST", data);
        }

        /// <summary>
        /// Uploads the specified string to the specified resource as an asynchronous operation using a task object.
        /// </summary>
        /// <param name="address">
        /// The URI of the resource to receive the string. For HTTP resources, this URI
        /// must identify a resource that can accept a request sent with the POST method,
        /// such as a script or ASP page.
        /// </param>
        /// <param name="method">
        /// The HTTP method used to send the file to the resource. If null, the default
        /// is POST for http and STOR for ftp.
        /// </param>
        /// <param name="data">The string to be uploaded.</param>
        /// <returns>
        /// Returns <see cref="System.Threading.Tasks.Task{TResult}"/>.
        /// The task object representing the asynchronous operation. The Result property
        /// on the task object returns a String containing the response sent by the server.
        /// </returns>
        public Task<string> UploadStringTaskAsync(string address, string method, string data)
        {
            return UploadStringTaskAsync(new Uri(address), method, data);
        }

        /// <summary>
        /// Uploads the specified string to the specified resource as an asynchronous operation using a task object.
        /// </summary>
        /// <param name="address">
        /// The URI of the resource to receive the string. For HTTP resources, this URI
        /// must identify a resource that can accept a request sent with the POST method,
        /// such as a script or ASP page.
        /// </param>
        /// <param name="method">
        /// The HTTP method used to send the file to the resource. If null, the default
        /// is POST for http and STOR for ftp.
        /// </param>
        /// <param name="data">The string to be uploaded.</param>
        /// <returns>
        /// Returns <see cref="System.Threading.Tasks.Task{TResult}"/>.
        /// The task object representing the asynchronous operation. The Result property
        /// on the task object returns a String containing the response sent by the server.
        /// </returns>
        public Task<string> UploadStringTaskAsync(Uri address, string method, string data)
        {
            INTERNAL_WebRequestHelper_JSOnly webRequestHelper = new INTERNAL_WebRequestHelper_JSOnly();

            Dictionary<string, string> headers = new Dictionary<string, string>();
            // Don't add the content-length we get the error :Refused to set unsafe header "Content-Length"
            //if (data != null && data.Length > 0)
            //{
            // headers.Add("Content-Length", data.Length.ToString());
            //headers["Content-Type"] = ContentType; //todo: determine content type
            //}
            foreach (string key in _headers.AllKeys)
            {
                headers.Add(key, _headers.Get(key)); //todo-perf: improve performance?
            }

            var taskCompletionSource = new TaskCompletionSource<string>();
            webRequestHelper.MakeRequest(address, method, this, headers, data, (sender, args) => TriggerUploadStringTaskCompleted(taskCompletionSource, args), true, GetCredentialsMode());

            return taskCompletionSource.Task;
        }

        private void TriggerUploadStringTaskCompleted(TaskCompletionSource<string> taskCompletionSource, INTERNAL_WebRequestHelper_JSOnly_RequestCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                taskCompletionSource.SetResult(e.Result);
            }
            else
            {
                taskCompletionSource.TrySetException(e.Error);
            }
        }



        // Exceptions:
        //   System.ArgumentNullException:
        //     The address parameter is null.-or-The data parameter is null.
        //
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- There was no response from the server hosting the resource.
        //// <summary>
        //// Uploads the specified string to the specified resource. This method does
        //// not block the calling thread.
        //// </summary>
        //// <param name="address">
        //// The URI of the resource to receive the file. For HTTP resources, this URI
        //// must identify a resource that can accept a request sent with the POST method,
        //// such as a script or ASP page.
        //// </param>
        //// <param name="method">
        //// The HTTP method used to send the file to the resource. If null, the default
        //// is POST for http and STOR for ftp.
        //// </param>
        //// <param name="data">The string to be uploaded.</param>
        //// <param name="userToken">
        //// A user-defined object that is passed to the method invoked when the asynchronous
        //// operation completes.
        //// </param>
        //public void UploadStringAsync(Uri address, string method, string data, object userToken);

        /// <summary>
        /// Occurs when an asynchronous string-upload operation completes.
        /// </summary>
        public event UploadStringCompletedEventHandler UploadStringCompleted;
        void OnUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (UploadStringCompleted != null)
            {
                UploadStringCompleted(sender, e);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to buffer the data read from the
        /// Internet resource for a <see cref="WebClient"/> instance.
        /// </summary>
        /// <returns>
        /// true to enable buffering of the data received from the Internet resource; false
        /// to disable buffering. The default is true.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool AllowReadStreamBuffering { get; set; }

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        //public bool AllowWriteStreamBuffering { get; set; }








        //// Returns:
        ////     A System.String containing the base URI for requests made by a System.Net.WebClient
        ////     or System.String.Empty if no base address has been specified.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     System.Net.WebClient.BaseAddress is set to an invalid URI. The inner exception
        ////     may contain information that will help you locate the error.
        ///// <summary>
        ///// Gets or sets the base URI for requests made by a System.Net.WebClient.
        ///// </summary>
        //public string BaseAddress { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the application's cache policy for any resources obtained by
        ////     this WebClient instance using System.Net.WebRequest objects.
        ////
        //// Returns:
        ////     A System.Net.Cache.RequestCachePolicy object that represents the application's
        ////     caching requirements.
        //public RequestCachePolicy CachePolicy { get; set; }
        ////
        //// Summary:
        ////     Gets or sets the network credentials that are sent to the host and used to
        ////     authenticate the request.
        ////
        //// Returns:
        ////     An System.Net.ICredentials containing the authentication credentials for
        ////     the request. The default is null.
        //public ICredentials Credentials { get; set; }

        //// Summary:
        ////     Gets whether a Web request is in progress.
        ////
        //// Returns:
        ////     true if the Web request is still in progress; otherwise false.
        [OpenSilver.NotImplemented]
        public bool IsBusy { get; }

        //// Summary:
        ////     Gets or sets the proxy used by this System.Net.WebClient object.
        ////
        //// Returns:
        ////     An System.Net.IWebProxy instance used to send requests.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     System.Net.WebClient.Proxy is set to null.
        //public IWebProxy Proxy { get; set; }
        ////
        //// Summary:
        ////     Gets or sets a collection of query name/value pairs associated with the request.
        ////
        //// Returns:
        ////     A System.Collections.Specialized.NameValueCollection that contains query
        ////     name/value pairs associated with the request. If no pairs are associated
        ////     with the request, the value is an empty System.Collections.Specialized.NameValueCollection.
        //public NameValueCollection QueryString { get; set; }
        ////
        //// Summary:
        ////     Gets a collection of header name/value pairs associated with the response.
        ////
        //// Returns:
        ////     A System.Net.WebHeaderCollection containing header name/value pairs associated
        ////     with the response, or null if no response has been received.
        //public WebHeaderCollection ResponseHeaders { get; }
        ////
        //// Summary:
        ////     Gets or sets a System.Boolean value that controls whether the System.Net.CredentialCache.DefaultCredentials
        ////     are sent with requests.
        ////
        //// Returns:
        ////     true if the default credentials are used; otherwise false. The default value
        ////     is false.
        //public bool UseDefaultCredentials { get; set; }

        //// Summary:
        ////     Occurs when an asynchronous data download operation completes.
        //public event DownloadDataCompletedEventHandler DownloadDataCompleted;
        ////
        //// Summary:
        ////     Occurs when an asynchronous file download operation completes.
        //public event AsyncCompletedEventHandler DownloadFileCompleted;
        ////
        //// Summary:
        ////     Occurs when an asynchronous download operation successfully transfers some
        ////     or all of the data.
        //public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        //
        // Summary:
        //     Occurs when an asynchronous operation to open a stream containing a resource
        //     completes.
		[OpenSilver.NotImplemented]
        public event OpenReadCompletedEventHandler OpenReadCompleted;


        // Summary:
        //     Occurs when an asynchronous operation to open a stream to write data to a
        //     resource completes.
		[OpenSilver.NotImplemented]
        public event OpenWriteCompletedEventHandler OpenWriteCompleted;
        ////
        //// Summary:
        ////     Occurs when an asynchronous data-upload operation completes.
        //public event UploadDataCompletedEventHandler UploadDataCompleted;
        ////
        //// Summary:
        ////     Occurs when an asynchronous file-upload operation completes.
        //public event UploadFileCompletedEventHandler UploadFileCompleted;
        ////
        //// Summary:
        ////     Occurs when an asynchronous upload operation successfully transfers some
        ////     or all of the data.
        //public event UploadProgressChangedEventHandler UploadProgressChanged;

        ////
        //// Summary:
        ////     Occurs when an asynchronous upload of a name/value collection completes.
        //public event UploadValuesCompletedEventHandler UploadValuesCompleted;
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        //public event WriteStreamClosedEventHandler WriteStreamClosed;

        //// Summary:
        ////     Cancels a pending asynchronous operation.
        [OpenSilver.NotImplemented]
        public void CancelAsync() { }
        
        //// Summary:
        ////     Downloads the resource with the specified URI as a System.Byte array.
        ////
        //// Parameters:
        ////   address:
        ////     The URI from which to download data.
        ////
        //// Returns:
        ////     A System.Byte array containing the downloaded resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- An error occurred while downloading data.
        ////
        ////   System.NotSupportedException:
        ////     The method has been called simultaneously on multiple threads.
        //public byte[] DownloadData(string address);
        ////
        //// Summary:
        ////     Downloads the resource with the specified URI as a System.Byte array.
        ////
        //// Parameters:
        ////   address:
        ////     The URI represented by the System.Uri object, from which to download data.
        ////
        //// Returns:
        ////     A System.Byte array containing the downloaded resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        //public byte[] DownloadData(Uri address);
        ////
        //// Summary:
        ////     Downloads the specified resource as a System.Byte array. This method does
        ////     not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     A System.Uri containing the URI to download.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- An error occurred while downloading the resource.
        //public void DownloadDataAsync(Uri address);
        ////
        //// Summary:
        ////     Downloads the specified resource as a System.Byte array. This method does
        ////     not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     A System.Uri containing the URI to download.
        ////
        ////   userToken:
        ////     A user-defined object that is passed to the method invoked when the asynchronous
        ////     operation completes.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- An error occurred while downloading the resource.
        //public void DownloadDataAsync(Uri address, object userToken);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<byte[]> DownloadDataTaskAsync(string address);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<byte[]> DownloadDataTaskAsync(Uri address);
        ////
        //// Summary:
        ////     Downloads the resource with the specified URI to a local file.
        ////
        //// Parameters:
        ////   address:
        ////     The URI from which to download data.
        ////
        ////   fileName:
        ////     The name of the local file that is to receive the data.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- filename is null or System.String.Empty.-or-The file does
        ////     not exist.-or- An error occurred while downloading data.
        ////
        ////   System.NotSupportedException:
        ////     The method has been called simultaneously on multiple threads.
        //public void DownloadFile(string address, string fileName);
        ////
        //// Summary:
        ////     Downloads the resource with the specified URI to a local file.
        ////
        //// Parameters:
        ////   address:
        ////     The URI specified as a System.String, from which to download data.
        ////
        ////   fileName:
        ////     The name of the local file that is to receive the data.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null. -or-The fileName parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- filename is null or System.String.Empty.-or- The file does
        ////     not exist. -or- An error occurred while downloading data.
        ////
        ////   System.NotSupportedException:
        ////     The method has been called simultaneously on multiple threads.
        //public void DownloadFile(Uri address, string fileName);
        ////
        //// Summary:
        ////     Downloads, to a local file, the resource with the specified URI. This method
        ////     does not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to download.
        ////
        ////   fileName:
        ////     The name of the file to be placed on the local computer.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null. -or-The fileName parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- An error occurred while downloading the resource.
        ////
        ////   System.InvalidOperationException:
        ////     The local file specified by fileName is in use by another thread.
        //public void DownloadFileAsync(Uri address, string fileName);
        ////
        //// Summary:
        ////     Downloads, to a local file, the resource with the specified URI. This method
        ////     does not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to download.
        ////
        ////   fileName:
        ////     The name of the file to be placed on the local computer.
        ////
        ////   userToken:
        ////     A user-defined object that is passed to the method invoked when the asynchronous
        ////     operation completes.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null. -or-The fileName parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- An error occurred while downloading the resource.
        ////
        ////   System.InvalidOperationException:
        ////     The local file specified by fileName is in use by another thread.
        //public void DownloadFileAsync(Uri address, string fileName, object userToken);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task.
        //public Task DownloadFileTaskAsync(string address, string fileName);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task.
        //public Task DownloadFileTaskAsync(Uri address, string fileName);












        ////
        //// Summary:
        ////     Returns a System.Net.WebRequest object for the specified resource.
        ////
        //// Parameters:
        ////   address:
        ////     A System.Uri that identifies the resource to request.
        ////
        //// Returns:
        ////     A new System.Net.WebRequest object for the specified resource.
        //protected virtual WebRequest GetWebRequest(Uri address);
        ////
        //// Summary:
        ////     Returns the System.Net.WebResponse for the specified System.Net.WebRequest.
        ////
        //// Parameters:
        ////   request:
        ////     A System.Net.WebRequest that is used to obtain the response.
        ////
        //// Returns:
        ////     A System.Net.WebResponse containing the response for the specified System.Net.WebRequest.
        //protected virtual WebResponse GetWebResponse(WebRequest request);
        ////
        //// Summary:
        ////     Returns the System.Net.WebResponse for the specified System.Net.WebRequest
        ////     using the specified System.IAsyncResult.
        ////
        //// Parameters:
        ////   request:
        ////     A System.Net.WebRequest that is used to obtain the response.
        ////
        ////   result:
        ////     An System.IAsyncResult object obtained from a previous call to System.Net.WebRequest.BeginGetResponse(System.AsyncCallback,System.Object)
        ////     .
        ////
        //// Returns:
        ////     A System.Net.WebResponse containing the response for the specified System.Net.WebRequest.
        //protected virtual WebResponse GetWebResponse(WebRequest request, IAsyncResult result);
        ////
        //// Summary:
        ////     Raises the System.Net.WebClient.DownloadDataCompleted event.
        ////
        //// Parameters:
        ////   e:
        ////     A System.Net.DownloadDataCompletedEventArgs object that contains event data.
        //protected virtual void OnDownloadDataCompleted(DownloadDataCompletedEventArgs e);
        ////
        //// Summary:
        ////     Raises the System.Net.WebClient.DownloadFileCompleted event.
        ////
        //// Parameters:
        ////   e:
        ////     An System.ComponentModel.AsyncCompletedEventArgs object containing event
        ////     data.
        //protected virtual void OnDownloadFileCompleted(AsyncCompletedEventArgs e);
        ////
        //// Summary:
        ////     Raises the System.Net.WebClient.DownloadProgressChanged event.
        ////
        //// Parameters:
        ////   e:
        ////     A System.Net.DownloadProgressChangedEventArgs object containing event data.
        //protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e);
        ////
        //// Summary:
        ////     Raises the System.Net.WebClient.DownloadStringCompleted event.
        ////
        //// Parameters:
        ////   e:
        ////     A System.Net.DownloadStringCompletedEventArgs object containing event data.
        //protected virtual void OnDownloadStringCompleted(DownloadStringCompletedEventArgs e);
        ////
        //// Summary:
        ////     Raises the System.Net.WebClient.OpenReadCompleted event.
        ////
        //// Parameters:
        ////   e:
        ////     A System.Net.OpenReadCompletedEventArgs object containing event data.
        //protected virtual void OnOpenReadCompleted(OpenReadCompletedEventArgs e);
        ////
        //// Summary:
        ////     Raises the System.Net.WebClient.OpenWriteCompleted event.
        ////
        //// Parameters:
        ////   e:
        ////     A System.Net.OpenWriteCompletedEventArgs object containing event data.
        //protected virtual void OnOpenWriteCompleted(OpenWriteCompletedEventArgs e);
        ////
        //// Summary:
        ////     Raises the System.Net.WebClient.UploadDataCompleted event.
        ////
        //// Parameters:
        ////   e:
        ////     A System.Net.UploadDataCompletedEventArgs object containing event data.
        //protected virtual void OnUploadDataCompleted(UploadDataCompletedEventArgs e);
        ////
        //// Summary:
        ////     Raises the System.Net.WebClient.UploadFileCompleted event.
        ////
        //// Parameters:
        ////   e:
        ////     An System.Net.UploadFileCompletedEventArgs object containing event data.
        //protected virtual void OnUploadFileCompleted(UploadFileCompletedEventArgs e);
        ////
        //// Summary:
        ////     Raises the System.Net.WebClient.UploadProgressChanged event.
        ////
        //// Parameters:
        ////   e:
        ////     An System.Net.UploadProgressChangedEventArgs object containing event data.
        //protected virtual void OnUploadProgressChanged(UploadProgressChangedEventArgs e);
        ////
        //// Summary:
        ////     Raises the System.Net.WebClient.UploadStringCompleted event.
        ////
        //// Parameters:
        ////   e:
        ////     An System.Net.UploadStringCompletedEventArgs object containing event data.
        //protected virtual void OnUploadStringCompleted(UploadStringCompletedEventArgs e);
        ////
        //// Summary:
        ////     Raises the System.Net.WebClient.UploadValuesCompleted event.
        ////
        //// Parameters:
        ////   e:
        ////     A System.Net.UploadValuesCompletedEventArgs object containing event data.
        //protected virtual void OnUploadValuesCompleted(UploadValuesCompletedEventArgs e);
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        //protected virtual void OnWriteStreamClosed(WriteStreamClosedEventArgs e);
        ////
        //// Summary:
        ////     Opens a readable stream for the data downloaded from a resource with the
        ////     URI specified as a System.String.
        ////
        //// Parameters:
        ////   address:
        ////     The URI specified as a System.String from which to download data.
        ////
        //// Returns:
        ////     A System.IO.Stream used to read data from a resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, address is
        ////     invalid.-or- An error occurred while downloading data.
        //public Stream OpenRead(string address);
        ////
        //// Summary:
        ////     Opens a readable stream for the data downloaded from a resource with the
        ////     URI specified as a System.Uri
        ////
        //// Parameters:
        ////   address:
        ////     The URI specified as a System.Uri from which to download data.
        ////
        //// Returns:
        ////     A System.IO.Stream used to read data from a resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, address is
        ////     invalid.-or- An error occurred while downloading data.
        //public Stream OpenRead(Uri address);
        ////
        
        //// Summary:
        ////     Opens a readable stream containing the specified resource. This method does
        ////     not block the calling thread.
        ////
        //// Parameters:
        ////   address:OpenReadAsync
        ////     The URI of the resource to retrieve.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- An error occurred while downloading the resource. -or- An
        ////     error occurred while opening the stream.
        [OpenSilver.NotImplemented]
        public void OpenReadAsync(Uri address) { }

        // Summary:
        //     Opens a readable stream containing the specified resource. This method does
        //     not block the calling thread.
        //
        // Parameters:
        //   address:
        //     The URI of the resource to retrieve.
        //
        //   userToken:
        //     A user-defined object that is passed to the method invoked when the asynchronous
        //     operation completes.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The address parameter is null.
        //
        //   System.Net.WebException:
        //     The URI formed by combining System.Net.WebClient.BaseAddress and address
        //     is invalid.-or- An error occurred while downloading the resource. -or- An
        //     error occurred while opening the stream.
		[OpenSilver.NotImplemented]
        public void OpenReadAsync(Uri address, object userToken)
        {

        }
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<Stream> OpenReadTaskAsync(string address);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<Stream> OpenReadTaskAsync(Uri address);
        ////
        //// Summary:
        ////     Opens a stream for writing data to the specified resource.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        //// Returns:
        ////     A System.IO.Stream used to write data to the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- An error occurred while opening the stream.
        //public Stream OpenWrite(string address);
        ////
        //// Summary:
        ////     Opens a stream for writing data to the specified resource.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        //// Returns:
        ////     A System.IO.Stream used to write data to the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- An error occurred while opening the stream.
        //public Stream OpenWrite(Uri address);
        ////
        //// Summary:
        ////     Opens a stream for writing data to the specified resource, using the specified
        ////     method.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        ////   method:
        ////     The method used to send the data to the resource. If null, the default is
        ////     POST for http and STOR for ftp.
        ////
        //// Returns:
        ////     A System.IO.Stream used to write data to the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- An error occurred while opening the stream.
        //public Stream OpenWrite(string address, string method);
        ////
        //// Summary:
        ////     Opens a stream for writing data to the specified resource, by using the specified
        ////     method.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        ////   method:
        ////     The method used to send the data to the resource. If null, the default is
        ////     POST for http and STOR for ftp.
        ////
        //// Returns:
        ////     A System.IO.Stream used to write data to the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- An error occurred while opening the stream.
        //public Stream OpenWrite(Uri address, string method);
        ////
        // Summary:
        //     Opens a stream for writing data to the specified resource. This method does
        //     not block the calling thread.
        //
        // Parameters:
        //   address:
        //     The URI of the resource to receive the data.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The address parameter is null.
		[OpenSilver.NotImplemented]
        public void OpenWriteAsync(Uri address)
        {

        }
        ////
        //// Summary:
        ////     Opens a stream for writing data to the specified resource. This method does
        ////     not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        ////   method:
        ////     The method used to send the data to the resource. If null, the default is
        ////     POST for http and STOR for ftp.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        //public void OpenWriteAsync(Uri address, string method);
        ////
        //// Summary:
        ////     Opens a stream for writing data to the specified resource, using the specified
        ////     method. This method does not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        ////   method:
        ////     The method used to send the data to the resource. If null, the default is
        ////     POST for http and STOR for ftp.
        ////
        ////   userToken:
        ////     A user-defined object that is passed to the method invoked when the asynchronous
        ////     operation completes
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- An error occurred while opening the stream.
        //public void OpenWriteAsync(Uri address, string method, object userToken);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<Stream> OpenWriteTaskAsync(string address);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<Stream> OpenWriteTaskAsync(Uri address);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<Stream> OpenWriteTaskAsync(string address, string method);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<Stream> OpenWriteTaskAsync(Uri address, string method);
        ////
        //// Summary:
        ////     Uploads a data buffer to a resource identified by a URI.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        ////   data:
        ////     The data buffer to send to the resource.
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- data is null. -or-An error occurred while sending the data.-or-
        ////     There was no response from the server hosting the resource.
        //public byte[] UploadData(string address, byte[] data);
        ////
        //// Summary:
        ////     Uploads a data buffer to a resource identified by a URI.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        ////   data:
        ////     The data buffer to send to the resource.
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- data is null. -or-An error occurred while sending the data.-or-
        ////     There was no response from the server hosting the resource.
        //public byte[] UploadData(Uri address, byte[] data);
        ////
        //// Summary:
        ////     Uploads a data buffer to the specified resource, using the specified method.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        ////   method:
        ////     The HTTP method used to send the data to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   data:
        ////     The data buffer to send to the resource.
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- data is null.-or- An error occurred while uploading the data.-or-
        ////     There was no response from the server hosting the resource.
        //public byte[] UploadData(string address, string method, byte[] data);
        ////
        //// Summary:
        ////     Uploads a data buffer to the specified resource, using the specified method.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        ////   method:
        ////     The HTTP method used to send the data to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   data:
        ////     The data buffer to send to the resource.
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- data is null.-or- An error occurred while uploading the data.-or-
        ////     There was no response from the server hosting the resource.
        //public byte[] UploadData(Uri address, string method, byte[] data);
        ////
        //// Summary:
        ////     Uploads a data buffer to a resource identified by a URI, using the POST method.
        ////     This method does not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        ////   data:
        ////     The data buffer to send to the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- An error occurred while opening the stream.-or- There was
        ////     no response from the server hosting the resource.
        //public void UploadDataAsync(Uri address, byte[] data);
        ////
        //// Summary:
        ////     Uploads a data buffer to a resource identified by a URI, using the specified
        ////     method. This method does not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        ////   method:
        ////     The HTTP method used to send the file to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   data:
        ////     The data buffer to send to the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- An error occurred while opening the stream.-or- There was
        ////     no response from the server hosting the resource.
        //public void UploadDataAsync(Uri address, string method, byte[] data);
        ////
        //// Summary:
        ////     Uploads a data buffer to a resource identified by a URI, using the specified
        ////     method and identifying token.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the data.
        ////
        ////   method:
        ////     The HTTP method used to send the file to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   data:
        ////     The data buffer to send to the resource.
        ////
        ////   userToken:
        ////     A user-defined object that is passed to the method invoked when the asynchronous
        ////     operation completes.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- An error occurred while opening the stream.-or- There was
        ////     no response from the server hosting the resource.
        //public void UploadDataAsync(Uri address, string method, byte[] data, object userToken);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<byte[]> UploadDataTaskAsync(string address, byte[] data);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<byte[]> UploadDataTaskAsync(Uri address, byte[] data);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<byte[]> UploadDataTaskAsync(string address, string method, byte[] data);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<byte[]> UploadDataTaskAsync(Uri address, string method, byte[] data);
        ////
        //// Summary:
        ////     Uploads the specified local file to a resource with the specified URI.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the file. For example, ftp://localhost/samplefile.txt.
        ////
        ////   fileName:
        ////     The file to send to the resource. For example, "samplefile.txt".
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null. -or-The fileName parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- fileName is null, is System.String.Empty, contains invalid
        ////     characters, or does not exist.-or- An error occurred while uploading the
        ////     file.-or- There was no response from the server hosting the resource.-or-
        ////     The Content-type header begins with multipart.
        //public byte[] UploadFile(string address, string fileName);
        ////
        //// Summary:
        ////     Uploads the specified local file to a resource with the specified URI.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the file. For example, ftp://localhost/samplefile.txt.
        ////
        ////   fileName:
        ////     The file to send to the resource. For example, "samplefile.txt".
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null. -or-The fileName parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- fileName is null, is System.String.Empty, contains invalid
        ////     characters, or does not exist.-or- An error occurred while uploading the
        ////     file.-or- There was no response from the server hosting the resource.-or-
        ////     The Content-type header begins with multipart.
        //public byte[] UploadFile(Uri address, string fileName);
        ////
        //// Summary:
        ////     Uploads the specified local file to the specified resource, using the specified
        ////     method.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the file.
        ////
        ////   method:
        ////     The HTTP method used to send the file to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   fileName:
        ////     The file to send to the resource.
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null. -or-The fileName parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- fileName is null, is System.String.Empty, contains invalid
        ////     characters, or does not exist.-or- An error occurred while uploading the
        ////     file.-or- There was no response from the server hosting the resource.-or-
        ////     The Content-type header begins with multipart.
        //public byte[] UploadFile(string address, string method, string fileName);
        ////
        //// Summary:
        ////     Uploads the specified local file to the specified resource, using the specified
        ////     method.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the file.
        ////
        ////   method:
        ////     The HTTP method used to send the file to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   fileName:
        ////     The file to send to the resource.
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null. -or-The fileName parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- fileName is null, is System.String.Empty, contains invalid
        ////     characters, or does not exist.-or- An error occurred while uploading the
        ////     file.-or- There was no response from the server hosting the resource.-or-
        ////     The Content-type header begins with multipart.
        //public byte[] UploadFile(Uri address, string method, string fileName);
        ////
        //// Summary:
        ////     Uploads the specified local file to the specified resource, using the POST
        ////     method. This method does not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the file. For HTTP resources, this URI
        ////     must identify a resource that can accept a request sent with the POST method,
        ////     such as a script or ASP page.
        ////
        ////   fileName:
        ////     The file to send to the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null. -or-The fileName parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- fileName is null, is System.String.Empty, contains invalid
        ////     character, or the specified path to the file does not exist.-or- An error
        ////     occurred while opening the stream.-or- There was no response from the server
        ////     hosting the resource.-or- The Content-type header begins with multipart.
        //public void UploadFileAsync(Uri address, string fileName);
        ////
        //// Summary:
        ////     Uploads the specified local file to the specified resource, using the POST
        ////     method. This method does not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the file. For HTTP resources, this URI
        ////     must identify a resource that can accept a request sent with the POST method,
        ////     such as a script or ASP page.
        ////
        ////   method:
        ////     The HTTP method used to send the data to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   fileName:
        ////     The file to send to the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null. -or-The fileName parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- fileName is null, is System.String.Empty, contains invalid
        ////     character, or the specified path to the file does not exist.-or- An error
        ////     occurred while opening the stream.-or- There was no response from the server
        ////     hosting the resource.-or- The Content-type header begins with multipart.
        //public void UploadFileAsync(Uri address, string method, string fileName);
        ////
        //// Summary:
        ////     Uploads the specified local file to the specified resource, using the POST
        ////     method. This method does not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the file. For HTTP resources, this URI
        ////     must identify a resource that can accept a request sent with the POST method,
        ////     such as a script or ASP page.
        ////
        ////   method:
        ////     The HTTP method used to send the data to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   fileName:
        ////     The file to send to the resource.
        ////
        ////   userToken:
        ////     A user-defined object that is passed to the method invoked when the asynchronous
        ////     operation completes.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null. -or-The fileName parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- fileName is null, is System.String.Empty, contains invalid
        ////     character, or the specified path to the file does not exist.-or- An error
        ////     occurred while opening the stream.-or- There was no response from the server
        ////     hosting the resource.-or- The Content-type header begins with multipart.
        //public void UploadFileAsync(Uri address, string method, string fileName, object userToken);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<byte[]> UploadFileTaskAsync(string address, string fileName);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<byte[]> UploadFileTaskAsync(Uri address, string fileName);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<byte[]> UploadFileTaskAsync(string address, string method, string fileName);
        ////
        ////
        //// Returns:
        ////     Returns System.Threading.Tasks.Task<TResult>.
        //public Task<byte[]> UploadFileTaskAsync(Uri address, string method, string fileName);



        ////
        //// Summary:
        ////     Uploads the specified name/value collection to the resource identified by
        ////     the specified URI.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the collection.
        ////
        ////   data:
        ////     The System.Collections.Specialized.NameValueCollection to send to the resource.
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.-or-The data parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- data is null.-or- There was no response from the server hosting
        ////     the resource.-or- An error occurred while opening the stream.-or- The Content-type
        ////     header is not null or "application/x-www-form-urlencoded".
        //public byte[] UploadValues(string address, NameValueCollection data);
        ////
        //// Summary:
        ////     Uploads the specified name/value collection to the resource identified by
        ////     the specified URI.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the collection.
        ////
        ////   data:
        ////     The System.Collections.Specialized.NameValueCollection to send to the resource.
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.-or-The data parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- data is null.-or- There was no response from the server hosting
        ////     the resource.-or- An error occurred while opening the stream.-or- The Content-type
        ////     header is not null or "application/x-www-form-urlencoded".
        //public byte[] UploadValues(Uri address, NameValueCollection data);
        ////
        //// Summary:
        ////     Uploads the specified name/value collection to the resource identified by
        ////     the specified URI, using the specified method.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the collection.
        ////
        ////   method:
        ////     The HTTP method used to send the file to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   data:
        ////     The System.Collections.Specialized.NameValueCollection to send to the resource.
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.-or-The data parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- data is null.-or- An error occurred while opening the stream.-or-
        ////     There was no response from the server hosting the resource.-or- The Content-type
        ////     header value is not null and is not application/x-www-form-urlencoded.
        //public byte[] UploadValues(string address, string method, NameValueCollection data);
        ////
        //// Summary:
        ////     Uploads the specified name/value collection to the resource identified by
        ////     the specified URI, using the specified method.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the collection.
        ////
        ////   method:
        ////     The HTTP method used to send the file to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   data:
        ////     The System.Collections.Specialized.NameValueCollection to send to the resource.
        ////
        //// Returns:
        ////     A System.Byte array containing the body of the response from the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.-or-The data parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress, and address
        ////     is invalid.-or- data is null.-or- An error occurred while opening the stream.-or-
        ////     There was no response from the server hosting the resource.-or- The Content-type
        ////     header value is not null and is not application/x-www-form-urlencoded.
        //public byte[] UploadValues(Uri address, string method, NameValueCollection data);
        ////
        //// Summary:
        ////     Uploads the data in the specified name/value collection to the resource identified
        ////     by the specified URI. This method does not block the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the collection. This URI must identify
        ////     a resource that can accept a request sent with the default method. See remarks.
        ////
        ////   data:
        ////     The System.Collections.Specialized.NameValueCollection to send to the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.-or-The data parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- There was no response from the server hosting the resource.
        //public void UploadValuesAsync(Uri address, NameValueCollection data);
        ////
        //// Summary:
        ////     Uploads the data in the specified name/value collection to the resource identified
        ////     by the specified URI, using the specified method. This method does not block
        ////     the calling thread.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the collection. This URI must identify
        ////     a resource that can accept a request sent with the method method.
        ////
        ////   method:
        ////     The method used to send the string to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   data:
        ////     The System.Collections.Specialized.NameValueCollection to send to the resource.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.-or-The data parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- There was no response from the server hosting the resource.-or-method
        ////     cannot be used to send content.
        //public void UploadValuesAsync(Uri address, string method, NameValueCollection data);
        ////
        //// Summary:
        ////     Uploads the data in the specified name/value collection to the resource identified
        ////     by the specified URI, using the specified method. This method does not block
        ////     the calling thread, and allows the caller to pass an object to the method
        ////     that is invoked when the operation completes.
        ////
        //// Parameters:
        ////   address:
        ////     The URI of the resource to receive the collection. This URI must identify
        ////     a resource that can accept a request sent with the method method.
        ////
        ////   method:
        ////     The HTTP method used to send the string to the resource. If null, the default
        ////     is POST for http and STOR for ftp.
        ////
        ////   data:
        ////     The System.Collections.Specialized.NameValueCollection to send to the resource.
        ////
        ////   userToken:
        ////     A user-defined object that is passed to the method invoked when the asynchronous
        ////     operation completes.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The address parameter is null.-or-The data parameter is null.
        ////
        ////   System.Net.WebException:
        ////     The URI formed by combining System.Net.WebClient.BaseAddress and address
        ////     is invalid.-or- There was no response from the server hosting the resource.-or-method
        ////     cannot be used to send content.
        //public void UploadValuesAsync(Uri address, string method, NameValueCollection data, object userToken);
        //public Task<byte[]> UploadValuesTaskAsync(string address, NameValueCollection data);
        //public Task<byte[]> UploadValuesTaskAsync(Uri address, NameValueCollection data);
        //public Task<byte[]> UploadValuesTaskAsync(string address, string method, NameValueCollection data);
        //public Task<byte[]> UploadValuesTaskAsync(Uri address, string method, NameValueCollection data);
    }
}

