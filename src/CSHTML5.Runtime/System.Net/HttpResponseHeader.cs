
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    // Summary:
    //     The HTTP headers that can be specified in a server response.
    public enum HttpResponseHeader
    {
        /// <summary>
        /// The Cache-Control header, which specifies caching directives that must be
        /// obeyed by all caching mechanisms along the request/response chain.
        /// </summary>
        CacheControl = 0,
        /// <summary>
        /// The Connection header, which specifies options that are desired for a particular
        /// connection.
        /// </summary>
        Connection = 1,
        /// <summary>
        /// The Date header, which specifies the date and time at which the response
        /// originated.
        /// </summary>
        Date = 2,
        /// <summary>
        /// The Keep-Alive header, which specifies a parameter to be used to maintain
        /// a persistent connection.
        /// </summary>
        KeepAlive = 3,
        /// <summary>
        /// The Pragma header, which specifies implementation-specific directives that
        /// might apply to any agent along the request/response chain.
        /// </summary>
        Pragma = 4,
        /// <summary>
        /// The Trailer header, which specifies that the indicated header fields are
        /// present in the trailer of a message that is encoded with chunked transfer-coding.
        /// </summary>
        Trailer = 5,
        /// <summary>
        /// The Transfer-Encoding header, which specifies what (if any) type of transformation
        /// has been applied to the message body.
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
        /// The Allow header, which specifies the set of HTTP methods that are supported.
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
        /// The Content-Langauge header, which specifies the natural language or languages
        /// of the accompanying body data.
        /// </summary>
        ContentLanguage = 14,
        /// <summary>
        /// The Content-Location header, which specifies a URI from which the accompanying
        /// body can be obtained.
        /// </summary>
        ContentLocation = 15,
        /// <summary>
        /// The Content-MD5 header, which specifies the MD5 digest of the accompanying
        /// body data, for the purpose of providing an end-to-end message integrity check.
        /// </summary>
        ContentMd5 = 16,
        /// <summary>
        /// The Range header, which specifies the subrange or subranges of the response
        /// that the client requests be returned in lieu of the entire response.
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
        /// The Accept-Ranges header, which specifies the range that is accepted by the
        /// server.
        /// </summary>
        AcceptRanges = 20,
        /// <summary>
        /// The Age header, which specifies the time, in seconds, since the response
        /// was generated by the originating server.
        /// </summary>
        Age = 21,
        /// <summary>
        /// The Etag header, which specifies the current value for the requested variant.
        /// </summary>
        ETag = 22,
        /// <summary>
        /// The Location header, which specifies a URI to which the client is redirected
        /// to obtain the requested resource.
        /// </summary>
        Location = 23,
        /// <summary>
        /// The Proxy-Authenticate header, which specifies that the client must authenticate
        /// itself to a proxy.
        /// </summary>
        ProxyAuthenticate = 24,
        /// <summary>
        /// The Retry-After header, which specifies a time (in seconds), or a date and
        /// time, after which the client can retry its request.
        /// </summary>
        RetryAfter = 25,
        /// <summary>
        /// The Server header, which specifies information about the originating server
        /// agent.
        /// </summary>
        Server = 26,
        /// <summary>
        /// The Set-Cookie header, which specifies cookie data that is presented to the
        /// client.
        /// </summary>
        SetCookie = 27,
        /// <summary>
        /// The Vary header, which specifies the request headers that are used to determine
        /// whether a cached response is fresh.
        /// </summary>
        Vary = 28,
        /// <summary>
        /// The WWW-Authenticate header, which specifies that the client must authenticate
        /// itself to the server.
        /// </summary>
        WwwAuthenticate = 29,
    }
}
