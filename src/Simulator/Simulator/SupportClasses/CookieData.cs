using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    public struct CookieData
    {
        public string url;
        public string domain;
        public string name;
        public string path;
        public bool secure;
        public bool httpOnly;
        public bool session;
        public string value;
        public long expirationTime;

        public CookieData(DotNetBrowser.Cookie cookie, string url)
        {
            this.url = url;
            this.name = cookie.Name;
            this.path = cookie.Path;
            this.secure = cookie.Secure;
            this.httpOnly = cookie.HttpOnly;
            this.value = cookie.Value;
            this.domain = cookie.Domain;
            this.expirationTime = cookie.ExpirationTime;
            this.session = cookie.Session;
        }
    }
}
