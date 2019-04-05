
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    /// <summary>
    /// The HTTP headers that may be specified in a client request.
    /// </summary>
    public enum HttpRequestHeader
    {
        /// <summary>
        /// The Cache-Control header, which specifies directives that must be obeyed
        /// by all cache control mechanisms along the request/response chain.
        /// </summary>
        CacheControl = 0,
        /// <summary>
        /// The Connection header, which specifies options that are desired for a particular
        /// connection.
        /// </summary>
        Connection = 1,
        /// <summary>
        /// The Date header, which specifies the date and time at which the request originated.
        /// </summary>
        Date = 2,
        /// <summary>
        /// The Keep-Alive header, which specifies a parameter used into order to maintain
        /// a persistent connection.
        /// </summary>
        KeepAlive = 3,
        /// <summary>
        /// The Pragma header, which specifies implementation-specific directives that
        /// might apply to any agent along the request/response chain.
        /// </summary>
        Pragma = 4,
        /// <summary>
        /// The Trailer header, which specifies the header fields present in the trailer
        /// of a message encoded with chunked transfer-coding.
        /// </summary>
        Trailer = 5,
        /// <summary>
        /// The Transfer-Encoding header, which specifies what (if any) type of transformation
        /// that has been applied to the message body.
        /// </summary>
        TransferEncoding = 6,
        /// <summary>
        /// The Upgrade header, which specifies additional communications protocols that
        /// the client supports.
        /// </summary>
        Upgrade = 7,
        /// <summary>
        /// The Via header, which specifies intermediate protocols to be used by gateway
        /// and proxy agents.
        /// </summary>
        Via = 8,
        /// <summary>
        /// The Warning header, which specifies additional information about that status
        /// or transformation of a message that might not be reflected in the message.
        /// </summary>
        Warning = 9,
        /// <summary>
        /// The Allow header, which specifies the set of HTTP methods supported.
        /// </summary>
        Allow = 10,
        /// <summary>
        /// The Content-Length header, which specifies the length, in bytes, of the accompanying
        /// body data.
        /// </summary>
        ContentLength = 11,
        /// <summary>
        /// The Content-Type header, which specifies the MIME type of the accompanying
        /// body data.
        /// </summary>
        ContentType = 12,
        /// <summary>
        /// The Content-Encoding header, which specifies the encodings that have been
        /// applied to the accompanying body data.
        /// </summary>
        ContentEncoding = 13,
        /// <summary>
        /// The Content-Langauge header, which specifies the natural language(s) of the
        /// accompanying body data.
        /// </summary>
        ContentLanguage = 14,
        /// <summary>
        /// The Content-Location header, which specifies a URI from which the accompanying
        /// body may be obtained.
        /// </summary>
        ContentLocation = 15,
        /// <summary>
        /// The Content-MD5 header, which specifies the MD5 digest of the accompanying
        /// body data, for the purpose of providing an end-to-end message integrity check.
        /// </summary>
        ContentMd5 = 16,
        /// <summary>
        /// The Content-Range header, which specifies where in the full body the accompanying
        /// partial body data should be applied.
        /// </summary>
        ContentRange = 17,
        /// <summary>
        /// The Expires header, which specifies the date and time after which the accompanying
        /// body data should be considered stale.
        /// </summary>
        Expires = 18,
        /// <summary>
        /// The Last-Modified header, which specifies the date and time at which the
        /// accompanying body data was last modified.
        /// </summary>
        LastModified = 19,
        /// <summary>
        /// The Accept header, which specifies the MIME types that are acceptable for
        /// the response.
        /// </summary>
        Accept = 20,
        /// <summary>
        /// The Accept-Charset header, which specifies the character sets that are acceptable
        /// for the response.
        /// </summary>
        AcceptCharset = 21,
        /// <summary>
        /// The Accept-Encoding header, which specifies the content encodings that are
        /// acceptable for the response.
        /// </summary>
        AcceptEncoding = 22,
        /// <summary>
        /// The Accept-Langauge header, which specifies that natural languages that are
        /// preferred for the response.
        /// </summary>
        AcceptLanguage = 23,
        /// <summary>
        /// The Authorization header, which specifies the credentials that the client
        /// presents in order to authenticate itself to the server.
        /// </summary>
        Authorization = 24,
        /// <summary>
        /// The Cookie header, which specifies cookie data presented to the server.
        /// </summary>
        Cookie = 25,
        /// <summary>
        /// The Expect header, which specifies particular server behaviors that are required
        /// by the client.
        /// </summary>
        Expect = 26,
        /// <summary>
        /// The From header, which specifies an Internet E-mail address for the human
        /// user who controls the requesting user agent.
        /// </summary>
        From = 27,
        /// <summary>
        /// The Host header, which specifies the host name and port number of the resource
        /// being requested.
        /// </summary>
        Host = 28,
        /// <summary>
        /// The If-Match header, which specifies that the requested operation should
        /// be performed only if the client's cached copy of the indicated resource is
        /// current.
        /// </summary>
        IfMatch = 29,
        /// <summary>
        /// The If-Modified-Since header, which specifies that the requested operation
        /// should be performed only if the requested resource has been modified since
        /// the indicated data and time.
        /// </summary>
        IfModifiedSince = 30,
        /// <summary>
        /// The If-None-Match header, which specifies that the requested operation should
        /// be performed only if none of client's cached copies of the indicated resources
        /// are current.
        /// </summary>
        IfNoneMatch = 31,
        /// <summary>
        /// The If-Range header, which specifies that only the specified range of the
        /// requested resource should be sent, if the client's cached copy is current.
        /// </summary>
        IfRange = 32,
        /// <summary>
        /// The If-Unmodified-Since header, which specifies that the requested operation
        /// should be performed only if the requested resource has not been modified
        /// since the indicated date and time.
        /// </summary>
        IfUnmodifiedSince = 33,
        /// <summary>
        /// The Max-Forwards header, which specifies an integer indicating the remaining
        /// number of times that this request may be forwarded.
        /// </summary>
        MaxForwards = 34,
        /// <summary>
        /// The Proxy-Authorization header, which specifies the credentials that the
        /// client presents in order to authenticate itself to a proxy.
        /// </summary>
        ProxyAuthorization = 35,
        /// <summary>
        /// The Referer header, which specifies the URI of the resource from which the
        /// request URI was obtained.
        /// </summary>
        Referer = 36,
        /// <summary>
        /// The Range header, which specifies the the sub-range(s) of the response that
        /// the client requests be returned in lieu of the entire response.
        /// </summary>
        Range = 37,
        /// <summary>
        /// The TE header, which specifies the transfer encodings that are acceptable
        /// for the response.
        /// </summary>
        Te = 38,
        /// <summary>
        /// The Translate header, a Microsoft extension to the HTTP specification used
        /// in conjunction with WebDAV functionality.
        /// </summary>
        Translate = 39,
        /// <summary>
        /// The User-Agent header, which specifies information about the client agent.
        /// </summary>
        UserAgent = 40,
    }
}
