

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TypeScriptDefToCSharp.Model;

namespace TypeScriptDefToCSharp
{
    // This class is used for functions that are useful but not specific to one element
    public static class Tool
    {
        // Used to get the Text attribute of the ident/dotident tag in the Elem
        static public string GetIdent(XElement elem)
        {
            var Ident = elem.Element("ident");
            var DotIdent = elem.Element("dotident");
            var DotIdentWithAdditionalCharsAllowed = elem.Element("dotident_with_additional_chars_allowed");
            string Res = null;

            if (Ident != null)
                Res = Ident.Attribute("text").Value;
            else if (DotIdent != null)
                Res = DotIdent.Attribute("text").Value;
            else if (DotIdentWithAdditionalCharsAllowed != null)
                Res = RemoveDotIdentAdditionalChars(DotIdentWithAdditionalCharsAllowed.Attribute("text").Value);

            return Res;
        }

        static public string RemoveDotIdentAdditionalChars(string nameThatCanContainAdditionalChars)
        {
            // We remove forward slashes and dashes because they can be supported in JavaScript but not in C#. For example in the following TypeScript code: 
            //  declare module 'test/clipboard-js' {
            //      export = clipboard;
            //  }

            return nameThatCanContainAdditionalChars.Replace("-", "_").Replace("/", ".");
        }

        static public TSType NewType(XElement elem, TypeScriptDefContext context)
        {
            if (elem == null)
                return null;

            TSType T = null;
            UnionTypeModel TypeList = new UnionTypeModel();
            HashSet<string> typesAlreadyInTheList = new HashSet<string>();

            foreach (XElement e in elem.Elements())
            {
                switch (e.Name.LocalName)
                {
                    case "dotident":
                        T = new BasicType(e);
                        if (!Tool.IsBasicType(T))
                            T = (TSType)TypeDeclaration.New(e, null, context) ?? T;
                        break;
                    case "anonymoustype":
                        T = new AnonymousType(e, context);
                        break;
                    case "functiontype":
                        T = new FunctionType(e, context);
                        break;
                    case "generic":
                        T = new Generic(e, context);
                        break;
                    case "arraylevel":
                        T = new Model.Array(T);
                        TypeList.Types.RemoveAt(TypeList.Types.Count - 1);
                        break;
                    default:
                        continue;
                }
                if (!typesAlreadyInTheList.Contains(T.ToString())) // Avoids adding the same type multiple times
                {
                    TypeList.Types.Add(T);
                    typesAlreadyInTheList.Add(T.ToString());
                }
            }

            if (TypeList.Count < 2)
                return T;
            else
                return TypeList;
        }

        // Return the index at which two indexable collections diverge, starting from the first element
        static public int FirstMismatchIndex<T>(this IReadOnlyList<T> l1, IReadOnlyList<T> l2)
        {
            int index = 0;
            int minCount = Math.Min(l1.Count, l2.Count);

            Func<bool> cont = () =>
            {
                return index < minCount
                && l1[index].Equals(l2[index]);
            };

            for (; cont(); ++index) { }

            return index;
        }

        // Return a string of t*4 spaces to indent
        static public string Tab(int t)
        {
            if (t >= 0)
                return new String(' ', 4 * t);
            return "";
        }

        // Indent a file (increase and decrease at every line beggining respectively with '{' and '}')
        static public string ReIndent(string Text)
        {
            var Lines = new StringReader(Text);
            string Line;
            var Res = new StringBuilder();
            int indent = 0;

            while ((Line = Lines.ReadLine()) != null)
            {
                if (Line.Length < 1)
                {
                    Res.AppendLine();
                    continue;
                }
                if (Line[0] == '{')
                {
                    Res.AppendLine(Tab(indent) + Line);
                    indent++;
                }
                else if (Line[0] == '}')
                {
                    indent--;
                    Res.AppendLine(Tab(indent) + Line);
                }
                else
                    Res.AppendLine(Tab(indent) + Line);
            }

            return Res.ToString();
        }

        static public bool IsPrimitiveType(BasicType type)
        {
            bool res;

            switch (type.Name)
            {
                case "bool":
                case "double":
                case "string":
                    res = true;
                    break;
                default:
                    res = false;
                    break;
            }

            return res;
        }

        static public bool IsDomType(BasicType type)
        {
            #region JavaScript Dom Types
            switch (type.Name)
            {
                case "PropertyDescriptor":
                case "PropertyDescriptorMap":
                case "Object":
                case "ObjectConstructor":
                //case "Function":
                case "FunctionConstructor":
                case "IArguments":
                //case "String":
                case "StringConstructor":
                //case "Boolean":
                case "BooleanConstructor":
                //case "Number":
                case "NumberConstructor":
                case "TemplateStringsArray":
                case "Math":
                case "Date":
                case "DateConstructor":
                case "RegExpMatchArray":
                case "RegExpExecArray":
                case "RegExp":
                case "RegExpConstructor":
                case "ErrorConstructor":
                case "EvalError":
                case "EvalErrorConstructor":
                case "RangeError":
                case "RangeErrorConstructor":
                case "ReferenceError":
                case "ReferenceErrorConstructor":
                case "SyntaxError":
                case "SyntaxErrorConstructor":
                case "TypeError":
                case "TypeErrorConstructor":
                case "URIError":
                case "URIErrorConstructor":
                case "JSON":
                case "ReadonlyArray<T>":
                case "Array<T>":
                case "ArrayConstructor":
                case "TypedPropertyDescriptor<T>":
                case "ClassDecorator":
                case "PropertyDecorator":
                case "MethodDecorator":
                case "ParameterDecorator":
                case "PromiseConstructorLike":
                case "PromiseLike<T>":
                case "ArrayLike<T>":
                case "ArrayBuffer":
                case "ArrayBufferConstructor":
                case "ArrayBufferView":
                case "DataView":
                case "DataViewConstructor":
                case "Int8Array":
                case "Int8ArrayConstructor":
                case "Uint8Array":
                case "Uint8ArrayConstructor":
                case "Uint8ClampedArray":
                case "Uint8ClampedArrayConstructor":
                case "Int16Array":
                case "Int16ArrayConstructor":
                case "Uint16Array":
                case "Uint16ArrayConstructor":
                case "Int32Array":
                case "Int32ArrayConstructor":
                case "Uint32Array":
                case "Uint32ArrayConstructor":
                case "Float32Array":
                case "Float32ArrayConstructor":
                case "Float64Array":
                case "Float64ArrayConstructor":
                case "Algorithm":
                case "AriaRequestEventInit":
                case "CommandEventInit":
                case "CompositionEventInit":
                case "ConfirmSiteSpecificExceptionsInformation":
                case "ConstrainBooleanParameters":
                case "ConstrainDOMStringParameters":
                case "ConstrainDoubleRange":
                case "ConstrainLongRange":
                case "ConstrainVideoFacingModeParameters":
                case "CustomEventInit":
                case "DeviceAccelerationDict":
                case "DeviceLightEventInit":
                case "DeviceRotationRateDict":
                case "DoubleRange":
                case "EventInit":
                case "EventModifierInit":
                case "ExceptionInformation":
                case "FocusEventInit":
                case "HashChangeEventInit":
                case "IDBIndexParameters":
                case "IDBObjectStoreParameters":
                case "KeyAlgorithm":
                case "KeyboardEventInit":
                case "LongRange":
                case "MSAccountInfo":
                case "MSAudioLocalClientEvent":
                case "MSAudioRecvPayload":
                case "MSAudioRecvSignal":
                case "MSAudioSendPayload":
                case "MSAudioSendSignal":
                case "MSConnectivity":
                case "MSCredentialFilter":
                case "MSCredentialParameters":
                case "MSCredentialSpec":
                case "MSDelay":
                case "MSDescription":
                case "MSFIDOCredentialParameters":
                case "MSIPAddressInfo":
                case "MSIceWarningFlags":
                case "MSJitter":
                case "MSLocalClientEventBase":
                case "MSNetwork":
                case "MSNetworkConnectivityInfo":
                case "MSNetworkInterfaceType":
                case "MSOutboundNetwork":
                case "MSPacketLoss":
                case "MSPayloadBase":
                case "MSRelayAddress":
                case "MSSignatureParameters":
                case "MSTransportDiagnosticsStats":
                case "MSUtilization":
                case "MSVideoPayload":
                case "MSVideoRecvPayload":
                case "MSVideoResolutionDistribution":
                case "MSVideoSendPayload":
                case "MediaEncryptedEventInit":
                case "MediaKeyMessageEventInit":
                case "MediaKeySystemConfiguration":
                case "MediaKeySystemMediaCapability":
                case "MediaStreamConstraints":
                case "MediaStreamErrorEventInit":
                case "MediaStreamTrackEventInit":
                case "MediaTrackCapabilities":
                case "MediaTrackConstraintSet":
                case "MediaTrackConstraints":
                case "MediaTrackSettings":
                case "MediaTrackSupportedConstraints":
                case "MouseEventInit":
                case "MsZoomToOptions":
                case "MutationObserverInit":
                case "ObjectURLOptions":
                case "PeriodicWaveConstraints":
                case "PointerEventInit":
                case "PositionOptions":
                case "RTCDTMFToneChangeEventInit":
                case "RTCDtlsFingerprint":
                case "RTCDtlsParameters":
                case "RTCIceCandidate":
                case "RTCIceCandidateAttributes":
                case "RTCIceCandidateComplete":
                case "RTCIceCandidatePair":
                case "RTCIceCandidatePairStats":
                case "RTCIceGatherOptions":
                case "RTCIceParameters":
                case "RTCIceServer":
                case "RTCInboundRTPStreamStats":
                case "RTCMediaStreamTrackStats":
                case "RTCOutboundRTPStreamStats":
                case "RTCRTPStreamStats":
                case "RTCRtcpFeedback":
                case "RTCRtcpParameters":
                case "RTCRtpCapabilities":
                case "RTCRtpCodecCapability":
                case "RTCRtpCodecParameters":
                case "RTCRtpContributingSource":
                case "RTCRtpEncodingParameters":
                case "RTCRtpFecParameters":
                case "RTCRtpHeaderExtension":
                case "RTCRtpHeaderExtensionParameters":
                case "RTCRtpParameters":
                case "RTCRtpRtxParameters":
                case "RTCRtpUnhandled":
                case "RTCSrtpKeyParam":
                case "RTCSrtpSdesParameters":
                case "RTCSsrcRange":
                case "RTCStats":
                case "RTCStatsReport":
                case "RTCTransportStats":
                case "StoreExceptionsInformation":
                case "StoreSiteSpecificExceptionsInformation":
                case "UIEventInit":
                case "WebGLContextAttributes":
                case "WebGLContextEventInit":
                case "WheelEventInit":
                case "EventListener":
                case "ANGLE_instanced_arrays":
                case "AnalyserNode":
                case "AnimationEvent":
                case "ApplicationCache":
                case "AriaRequestEvent":
                case "Attr":
                case "AudioBuffer":
                case "AudioBufferSourceNode":
                case "AudioContext":
                case "AudioDestinationNode":
                case "AudioListener":
                case "AudioNode":
                case "AudioParam":
                case "AudioProcessingEvent":
                case "AudioTrack":
                case "AudioTrackList":
                case "BarProp":
                case "BeforeUnloadEvent":
                case "BiquadFilterNode":
                case "Blob":
                case "CDATASection":
                case "CSS":
                case "CSSConditionRule":
                case "CSSFontFaceRule":
                case "CSSGroupingRule":
                case "CSSImportRule":
                case "CSSKeyframeRule":
                case "CSSKeyframesRule":
                case "CSSMediaRule":
                case "CSSNamespaceRule":
                case "CSSPageRule":
                case "CSSRule":
                case "CSSRuleList":
                case "CSSStyleDeclaration":
                case "CSSStyleRule":
                case "CSSStyleSheet":
                case "CSSSupportsRule":
                case "CanvasGradient":
                case "CanvasPattern":
                case "CanvasRenderingContext2D":
                case "ChannelMergerNode":
                case "ChannelSplitterNode":
                case "CharacterData":
                case "ClientRect":
                case "ClientRectList":
                case "ClipboardEvent":
                case "CloseEvent":
                case "CommandEvent":
                case "Comment":
                case "CompositionEvent":
                case "Console":
                case "ConvolverNode":
                case "Coordinates":
                case "Crypto":
                case "CryptoKey":
                case "CryptoKeyPair":
                case "CustomEvent":
                case "DOMError":
                case "DOMException":
                case "DOMImplementation":
                case "DOMParser":
                case "DOMSettableTokenList":
                case "DOMStringList":
                case "DOMStringMap":
                case "DOMTokenList":
                case "DataCue":
                case "DataTransfer":
                case "DataTransferItem":
                case "DataTransferItemList":
                case "DeferredPermissionRequest":
                case "DelayNode":
                case "DeviceAcceleration":
                case "DeviceLightEvent":
                case "DeviceMotionEvent":
                case "DeviceOrientationEvent":
                case "DeviceRotationRate":
                case "Document":
                case "DocumentFragment":
                case "DocumentType":
                case "DragEvent":
                case "DynamicsCompressorNode":
                case "EXT_frag_depth":
                case "EXT_texture_filter_anisotropic":
                case "Element":
                case "ErrorEvent":
                case "Event":
                case "EventTarget":
                case "External":
                case "File":
                case "FileList":
                case "FileReader":
                case "FocusEvent":
                case "FormData":
                case "GainNode":
                case "Gamepad":
                case "GamepadButton":
                case "GamepadEvent":
                case "Geolocation":
                case "HTMLAllCollection":
                case "HTMLAnchorElement":
                case "HTMLAppletElement":
                case "HTMLAreaElement":
                case "HTMLAreasCollection":
                case "HTMLAudioElement":
                case "HTMLBRElement":
                case "HTMLBaseElement":
                case "HTMLBaseFontElement":
                case "HTMLBodyElement":
                case "HTMLButtonElement":
                case "HTMLCanvasElement":
                case "HTMLCollection":
                case "HTMLDListElement":
                case "HTMLDataListElement":
                case "HTMLDirectoryElement":
                case "HTMLDivElement":
                case "HTMLDocument":
                case "HTMLElement":
                case "HTMLEmbedElement":
                case "HTMLFieldSetElement":
                case "HTMLFontElement":
                case "HTMLFormElement":
                case "HTMLFrameElement":
                case "HTMLFrameSetElement":
                case "HTMLHRElement":
                case "HTMLHeadElement":
                case "HTMLHeadingElement":
                case "HTMLHtmlElement":
                case "HTMLIFrameElement":
                case "HTMLImageElement":
                case "HTMLInputElement":
                case "HTMLLIElement":
                case "HTMLLabelElement":
                case "HTMLLegendElement":
                case "HTMLLinkElement":
                case "HTMLMapElement":
                case "HTMLMarqueeElement":
                case "HTMLMediaElement":
                case "HTMLMenuElement":
                case "HTMLMetaElement":
                case "HTMLMeterElement":
                case "HTMLModElement":
                case "HTMLOListElement":
                case "HTMLObjectElement":
                case "HTMLOptGroupElement":
                case "HTMLOptionElement":
                case "HTMLOptionsCollection":
                case "HTMLParagraphElement":
                case "HTMLParamElement":
                case "HTMLPictureElement":
                case "HTMLPreElement":
                case "HTMLProgressElement":
                case "HTMLQuoteElement":
                case "HTMLScriptElement":
                case "HTMLSelectElement":
                case "HTMLSourceElement":
                case "HTMLSpanElement":
                case "HTMLStyleElement":
                case "HTMLTableCaptionElement":
                case "HTMLTableCellElement":
                case "HTMLTableColElement":
                case "HTMLTableDataCellElement":
                case "HTMLTableElement":
                case "HTMLTableHeaderCellElement":
                case "HTMLTableRowElement":
                case "HTMLTableSectionElement":
                case "HTMLTemplateElement":
                case "HTMLTextAreaElement":
                case "HTMLTitleElement":
                case "HTMLTrackElement":
                case "HTMLUListElement":
                case "HTMLUnknownElement":
                case "HTMLVideoElement":
                case "HashChangeEvent":
                case "History":
                case "IDBCursor":
                case "IDBCursorWithValue":
                case "IDBDatabase":
                case "IDBFactory":
                case "IDBIndex":
                case "IDBKeyRange":
                case "IDBObjectStore":
                case "IDBOpenDBRequest":
                case "IDBRequest":
                case "IDBTransaction":
                case "IDBVersionChangeEvent":
                case "ImageData":
                case "KeyboardEvent":
                case "ListeningStateChangedEvent":
                case "Location":
                case "LongRunningScriptDetectedEvent":
                case "MSApp":
                case "MSAppAsyncOperation":
                case "MSAssertion":
                case "MSBlobBuilder":
                case "MSCredentials":
                case "MSFIDOCredentialAssertion":
                case "MSFIDOSignature":
                case "MSFIDOSignatureAssertion":
                case "MSGesture":
                case "MSGestureEvent":
                case "MSGraphicsTrust":
                case "MSHTMLWebViewElement":
                case "MSInputMethodContext":
                case "MSManipulationEvent":
                case "MSMediaKeyError":
                case "MSMediaKeyMessageEvent":
                case "MSMediaKeyNeededEvent":
                case "MSMediaKeySession":
                case "MSMediaKeys":
                case "MSPointerEvent":
                case "MSRangeCollection":
                case "MSSiteModeEvent":
                case "MSStream":
                case "MSStreamReader":
                case "MSWebViewAsyncOperation":
                case "MSWebViewSettings":
                case "MediaDeviceInfo":
                case "MediaDevices":
                case "MediaElementAudioSourceNode":
                case "MediaEncryptedEvent":
                case "MediaError":
                case "MediaKeyMessageEvent":
                case "MediaKeySession":
                case "MediaKeyStatusMap":
                case "MediaKeySystemAccess":
                case "MediaKeys":
                case "MediaList":
                case "MediaQueryList":
                case "MediaSource":
                case "MediaStream":
                case "MediaStreamAudioSourceNode":
                case "MediaStreamError":
                case "MediaStreamErrorEvent":
                case "MediaStreamTrack":
                case "MediaStreamTrackEvent":
                case "MessageChannel":
                case "MessageEvent":
                case "MessagePort":
                case "MimeType":
                case "MimeTypeArray":
                case "MouseEvent":
                case "MutationEvent":
                case "MutationObserver":
                case "MutationRecord":
                case "NamedNodeMap":
                case "NavigationCompletedEvent":
                case "NavigationEvent":
                case "NavigationEventWithReferrer":
                case "Navigator":
                case "Node":
                case "NodeFilter":
                case "NodeIterator":
                case "NodeList":
                case "OES_element_index_uint":
                case "OES_standard_derivatives":
                case "OES_texture_float":
                case "OES_texture_float_linear":
                case "OfflineAudioCompletionEvent":
                case "OfflineAudioContext":
                case "OscillatorNode":
                case "OverflowEvent":
                case "PageTransitionEvent":
                case "PannerNode":
                case "PerfWidgetExternal":
                case "Performance":
                case "PerformanceEntry":
                case "PerformanceMark":
                case "PerformanceMeasure":
                case "PerformanceNavigation":
                case "PerformanceNavigationTiming":
                case "PerformanceResourceTiming":
                case "PerformanceTiming":
                case "PeriodicWave":
                case "PermissionRequest":
                case "PermissionRequestedEvent":
                case "Plugin":
                case "PluginArray":
                case "PointerEvent":
                case "PopStateEvent":
                case "Position":
                case "PositionError":
                case "ProcessingInstruction":
                case "ProgressEvent":
                case "RTCDTMFToneChangeEvent":
                case "RTCDtlsTransport":
                case "RTCDtlsTransportStateChangedEvent":
                case "RTCDtmfSender":
                case "RTCIceCandidatePairChangedEvent":
                case "RTCIceGatherer":
                case "RTCIceGathererEvent":
                case "RTCIceTransport":
                case "RTCIceTransportStateChangedEvent":
                case "RTCRtpReceiver":
                case "RTCRtpSender":
                case "RTCSrtpSdesTransport":
                case "RTCSsrcConflictEvent":
                case "RTCStatsProvider":
                case "Range":
                case "SVGAElement":
                case "SVGAngle":
                case "SVGAnimatedAngle":
                case "SVGAnimatedBoolean":
                case "SVGAnimatedEnumeration":
                case "SVGAnimatedInteger":
                case "SVGAnimatedLength":
                case "SVGAnimatedLengthList":
                case "SVGAnimatedNumber":
                case "SVGAnimatedNumberList":
                case "SVGAnimatedPreserveAspectRatio":
                case "SVGAnimatedRect":
                case "SVGAnimatedString":
                case "SVGAnimatedTransformList":
                case "SVGCircleElement":
                case "SVGClipPathElement":
                case "SVGComponentTransferFunctionElement":
                case "SVGDefsElement":
                case "SVGDescElement":
                case "SVGElement":
                case "SVGElementInstance":
                case "SVGElementInstanceList":
                case "SVGEllipseElement":
                case "SVGFEBlendElement":
                case "SVGFEColorMatrixElement":
                case "SVGFEComponentTransferElement":
                case "SVGFECompositeElement":
                case "SVGFEConvolveMatrixElement":
                case "SVGFEDiffuseLightingElement":
                case "SVGFEDisplacementMapElement":
                case "SVGFEDistantLightElement":
                case "SVGFEFloodElement":
                case "SVGFEFuncAElement":
                case "SVGFEFuncBElement":
                case "SVGFEFuncGElement":
                case "SVGFEFuncRElement":
                case "SVGFEGaussianBlurElement":
                case "SVGFEImageElement":
                case "SVGFEMergeElement":
                case "SVGFEMergeNodeElement":
                case "SVGFEMorphologyElement":
                case "SVGFEOffsetElement":
                case "SVGFEPointLightElement":
                case "SVGFESpecularLightingElement":
                case "SVGFESpotLightElement":
                case "SVGFETileElement":
                case "SVGFETurbulenceElement":
                case "SVGFilterElement":
                case "SVGForeignObjectElement":
                case "SVGGElement":
                case "SVGGradientElement":
                case "SVGImageElement":
                case "SVGLength":
                case "SVGLengthList":
                case "SVGLineElement":
                case "SVGLinearGradientElement":
                case "SVGMarkerElement":
                case "SVGMaskElement":
                case "SVGMatrix":
                case "SVGMetadataElement":
                case "SVGNumber":
                case "SVGNumberList":
                case "SVGPathElement":
                case "SVGPathSeg":
                case "SVGPathSegArcAbs":
                case "SVGPathSegArcRel":
                case "SVGPathSegClosePath":
                case "SVGPathSegCurvetoCubicAbs":
                case "SVGPathSegCurvetoCubicRel":
                case "SVGPathSegCurvetoCubicSmoothAbs":
                case "SVGPathSegCurvetoCubicSmoothRel":
                case "SVGPathSegCurvetoQuadraticAbs":
                case "SVGPathSegCurvetoQuadraticRel":
                case "SVGPathSegCurvetoQuadraticSmoothAbs":
                case "SVGPathSegCurvetoQuadraticSmoothRel":
                case "SVGPathSegLinetoAbs":
                case "SVGPathSegLinetoHorizontalAbs":
                case "SVGPathSegLinetoHorizontalRel":
                case "SVGPathSegLinetoRel":
                case "SVGPathSegLinetoVerticalAbs":
                case "SVGPathSegLinetoVerticalRel":
                case "SVGPathSegList":
                case "SVGPathSegMovetoAbs":
                case "SVGPathSegMovetoRel":
                case "SVGPatternElement":
                case "SVGPoint":
                case "SVGPointList":
                case "SVGPolygonElement":
                case "SVGPolylineElement":
                case "SVGPreserveAspectRatio":
                case "SVGRadialGradientElement":
                case "SVGRect":
                case "SVGRectElement":
                case "SVGSVGElement":
                case "SVGScriptElement":
                case "SVGStopElement":
                case "SVGStringList":
                case "SVGStyleElement":
                case "SVGSwitchElement":
                case "SVGSymbolElement":
                case "SVGTSpanElement":
                case "SVGTextContentElement":
                case "SVGTextElement":
                case "SVGTextPathElement":
                case "SVGTextPositioningElement":
                case "SVGTitleElement":
                case "SVGTransform":
                case "SVGTransformList":
                case "SVGUnitTypes":
                case "SVGUseElement":
                case "SVGViewElement":
                case "SVGZoomAndPan":
                case "SVGZoomEvent":
                case "Screen":
                case "ScriptNotifyEvent":
                case "ScriptProcessorNode":
                case "Selection":
                case "SourceBuffer":
                case "SourceBufferList":
                case "StereoPannerNode":
                case "Storage":
                case "StorageEvent":
                case "StyleMedia":
                case "StyleSheet":
                case "StyleSheetList":
                case "StyleSheetPageList":
                case "SubtleCrypto":
                case "Text":
                case "TextEvent":
                case "TextMetrics":
                case "TextTrack":
                case "TextTrackCue":
                case "TextTrackCueList":
                case "TextTrackList":
                case "TimeRanges":
                case "Touch":
                case "TouchEvent":
                case "TouchList":
                case "TrackEvent":
                case "TransitionEvent":
                case "TreeWalker":
                case "UIEvent":
                case "URL":
                case "UnviewableContentIdentifiedEvent":
                case "ValidityState":
                case "VideoPlaybackQuality":
                case "VideoTrack":
                case "VideoTrackList":
                case "WEBGL_compressed_texture_s3tc":
                case "WEBGL_debug_renderer_info":
                case "WEBGL_depth_texture":
                case "WaveShaperNode":
                case "WebGLActiveInfo":
                case "WebGLBuffer":
                case "WebGLContextEvent":
                case "WebGLFramebuffer":
                case "WebGLObject":
                case "WebGLProgram":
                case "WebGLRenderbuffer":
                case "WebGLRenderingContext":
                case "WebGLShader":
                case "WebGLShaderPrecisionFormat":
                case "WebGLTexture":
                case "WebGLUniformLocation":
                case "WebKitCSSMatrix":
                case "WebKitPoint":
                case "WebSocket":
                case "WheelEvent":
                case "Window":
                case "Worker":
                case "XMLDocument":
                case "XMLHttpRequest":
                case "XMLHttpRequestUpload":
                case "XMLSerializer":
                case "XPathEvaluator":
                case "XPathExpression":
                case "XPathNSResolver":
                case "XPathResult":
                case "XSLTProcessor":
                case "AbstractWorker":
                case "CanvasPathMethods":
                case "ChildNode":
                case "DOML2DeprecatedColorProperty":
                case "DOML2DeprecatedSizeProperty":
                case "DocumentEvent":
                case "ElementTraversal":
                case "GetSVGDocument":
                case "GlobalEventHandlers":
                case "HTMLTableAlignment":
                case "IDBEnvironment":
                case "LinkStyle":
                case "MSBaseReader":
                case "MSFileSaver":
                case "MSNavigatorDoNotTrack":
                case "NavigatorContentUtils":
                case "NavigatorGeolocation":
                case "NavigatorID":
                case "NavigatorOnLine":
                case "NavigatorStorageUtils":
                case "NavigatorUserMedia":
                case "NodeSelector":
                case "RandomSource":
                case "SVGAnimatedPathData":
                case "SVGAnimatedPoints":
                case "SVGExternalResourcesRequired":
                case "SVGFilterPrimitiveStandardAttributes":
                case "SVGFitToViewBox":
                case "SVGLangSpace":
                case "SVGLocatable":
                case "SVGStylable":
                case "SVGTests":
                case "SVGTransformable":
                case "SVGURIReference":
                case "WindowBase64":
                case "WindowConsole":
                case "WindowLocalStorage":
                case "WindowSessionStorage":
                case "WindowTimers":
                case "WindowTimersExtension":
                case "XMLHttpRequestEventTarget":
                case "StorageEventInit":
                case "Canvas2DContextAttributes":
                case "BlobPropertyBag":
                case "FilePropertyBag":
                case "EventListenerObject":
                case "MessageEventInit":
                case "ProgressEventInit":
                case "ScrollOptions":
                case "ScrollToOptions":
                case "ScrollIntoViewOptions":
                case "ClipboardEventInit":
                case "IDBArrayKey":
                case "RsaKeyGenParams":
                case "RsaHashedKeyGenParams":
                case "RsaKeyAlgorithm":
                case "RsaHashedKeyAlgorithm":
                case "RsaHashedImportParams":
                case "RsaPssParams":
                case "RsaOaepParams":
                case "EcdsaParams":
                case "EcKeyGenParams":
                case "EcKeyAlgorithm":
                case "EcKeyImportParams":
                case "EcdhKeyDeriveParams":
                case "AesCtrParams":
                case "AesKeyAlgorithm":
                case "AesKeyGenParams":
                case "AesDerivedKeyParams":
                case "AesCbcParams":
                case "AesCmacParams":
                case "AesGcmParams":
                case "AesCfbParams":
                case "HmacImportParams":
                case "HmacKeyAlgorithm":
                case "HmacKeyGenParams":
                case "DhKeyGenParams":
                case "DhKeyAlgorithm":
                case "DhKeyDeriveParams":
                case "DhImportKeyParams":
                case "ConcatParams":
                case "HkdfCtrParams":
                case "Pbkdf2Params":
                case "RsaOtherPrimesInfo":
                case "JsonWebKey":
                case "ParentNode":
                case "ErrorEventHandler":
                case "PositionCallback":
                case "PositionErrorCallback":
                case "MediaQueryListListener":
                case "MSLaunchUriCallback":
                case "FrameRequestCallback":
                case "MSUnsafeFunctionCallback":
                case "MSExecAtPriorityFunctionCallback":
                case "MutationCallback":
                case "DecodeSuccessCallback":
                case "DecodeErrorCallback":
                case "FunctionStringCallback":
                case "NavigatorUserMediaSuccessCallback":
                case "NavigatorUserMediaErrorCallback":
                case "ForEachCallback":
                case "ActiveXObject":
                case "ITextWriter":
                case "TextStreamBase":
                case "TextStreamWriter":
                case "TextStreamReader":
                case "Enumerator<T>":
                case "EnumeratorConstructor":
                case "VBArray<T>":
                case "VBArrayConstructor":
                case "VarDate":
                    return true;
                default:
                    return false;
            }
            #endregion
        }

        static public bool IsBasicType(TSType type)
        {
            bool res = false;

            if (type is BasicType)
            {
                var t = (BasicType)type;
                res = Tool.IsPrimitiveType(t);
                switch (t.Name)
                {
                    case "Action":
                    case "IJSObject":
                    case "object":
                        res = true;
                        break;
                }
            }

            return res;
        }


        // Is the word a C# Keyword?
        static public bool IsKeyWord(string Word)
        {
            switch (Word)
            {
                #region C# keywords
                case "abstract":
                case "as":
                case "base":
                case "bool":
                case "break":
                case "byte":
                case "case":
                case "catch":
                case "char":
                case "checked":
                case "class":
                case "const":
                case "continue":
                case "decimal":
                case "default":
                case "delegate":
                case "do":
                case "double":
                case "else":
                case "enum":
                case "event":
                case "explicit":
                case "extern":
                case "false":
                case "finally":
                case "fixed":
                case "float":
                case "for":
                case "foreach":
                case "goto":
                case "if":
                case "implicit":
                case "in":
                case "int":
                case "interface":
                case "internal":
                case "is":
                case "lock":
                case "long":
                case "namespace":
                case "new":
                case "null":
                case "object":
                case "operator":
                case "out":
                case "override":
                case "params":
                case "private":
                case "protected":
                case "public":
                case "readonly":
                case "ref":
                case "return":
                case "sbyte":
                case "sealed":
                case "short":
                case "sizeof":
                case "stackalloc":
                case "static":
                case "string":
                case "struct":
                case "switch":
                case "this":
                case "throw":
                case "true":
                case "try":
                case "typeof":
                case "uint":
                case "ulong":
                case "unchecked":
                case "unsafe":
                case "ushort":
                case "using":
                case "virtual":
                case "void":
                case "volatile":
                case "while":
                #endregion
                    return true;
                default:
                    return false;
            }
        }

        // Add a '@' in front of the word if that word is a C# Keyword
        static public string ClearKeyWord(string Name)
        {
            if (Tool.IsKeyWord(Name))
                return "@" + Name;

            Name = Name.Replace("$", "_S");

            return Name;
        }

        // Not used right now, but useful to generate some UnionType classes
        static public string GenerateUnionType(int n)
        {
            string res = @"
using CSHTML5;
using System;

namespace TypeScriptDefinitionsSupport
{
    public abstract class UnionType : JSObject
    {
        protected Type _Type { get; set; }

        public bool instanceof(string type)
        {
            return Convert.ToBoolean(Interop.ExecuteJavaScript(""$0 instanceof $1"", this.UnderlyingJSInstance, type));
        }

        public T CreateInstance<T>()
        {
            if (typeof(T) == typeof(double))
                return (T)(object)Convert.ToDouble(this.UnderlyingJSInstance);
            else if (typeof(T) == typeof(string))
                return (T)(object)Convert.ToString(this.UnderlyingJSInstance);
            else if (typeof(T) == typeof(bool))
                return (T)(object)Convert.ToBoolean(this.UnderlyingJSInstance);
            else if (typeof(T) == typeof(object))
                return (T)this.UnderlyingJSInstance;
            else
                return (T)Activator.CreateInstance(typeof(T), this.UnderlyingJSInstance);
        }
    }

";
            for (int i = 2; i < n; ++i)
                res += _GenerateUnionType(i) + Environment.NewLine;

            res += "}" + Environment.NewLine;

            return res;
        }

        // Not used right now, but useful to generate some UnionType classes
        static private string _GenerateUnionType(int n)
        {
            // Create the class signature
            string res = "public class ";
            string type = "UnionType<T0";

            for (int i = 1; i < n; ++i)
            {
                type += ", T" + i;
            }
            type += ">";

            res += type + " : UnionType" + Environment.NewLine + "{";

            // Global constructor when type is unknown (property getter)
            res += @"
private UnionType(object jsObj)
{
    this.UnderlyingJSInstance = jsObj;
}";

            for (int i = 0; i < n; ++i)
            {
                // {0} is i
                // {1} is type
                // Only used for readability (normal '{' and '}' are doubled)

                string tmp = "";

                // Private value
                tmp += @"
private T{0} _t{0} {{ get; set; }}
";

                // Specific constructor
                tmp += @"
public UnionType(T{0} t)
{{
this._t{0} = t;
this._Type = typeof(T{0});
JSObject o = t as JSObject;
if (o == null)
    throw new Exception(""The type '"" + this._Type.Name + ""' in not a JSObject or a C# basic data type"");
this.UnderlyingJSInstance = o.ToJavaScriptObject();
}}
";

                // Cast operators
                tmp += @"
static public implicit operator {1}(T{0} t)
{{
return new {1}(t);
}}

static public implicit operator T{0}({1} value)
{{
if (value._Type == null && value.instanceof(typeof(T{0}).Name))
{{
    value._t{0} = value.CreateInstance<T{0}>();
    value._Type = typeof(T{0});
    return value._t{0};
}}
if (value._Type == typeof(T{0}))
    return value._t{0};
else
    throw new Exception(""Unable to cast this UnionType to the specified type because this UnionType contains a value that is of another type."");
}}";

                // Replace {0} and {1} by the value of i and type
                res += string.Format(tmp, i, type);
            }

            res += Environment.NewLine + "}" + Environment.NewLine;
            return res;
        }

        public static T Deserialize<T>(string toDeserialize)
        {
            // Note: we use the DataContractSerializer instead of the XmlSerializer because the latter resulted in a PathTooLong exception when deserializing the cache file.

            using (XmlReader reader = XmlReader.Create(new StringReader(toDeserialize)))
            {
                DataContractSerializer deserializer = new DataContractSerializer(typeof(T));
                return (T)deserializer.ReadObject(reader);
            }

            //cf. https://stackoverflow.com/questions/5010191/using-datacontractserializer-to-serialize-but-cant-deserialize-back
            //using (Stream stream = new MemoryStream())
            //{
            //    byte[] data = System.Text.Encoding.UTF8.GetBytes(toDeserialize);
            //    stream.Write(data, 0, data.Length);
            //    stream.Position = 0;
            //    DataContractSerializer deserializer = new DataContractSerializer(typeof(T));
            //    return (T)deserializer.ReadObject(stream);
            //}
        }

        public static string Serialize<T>(T toSerialize)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(memoryStream, toSerialize);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static string JSArrayDimensionName(string name, int arrayLevel)
        {
            string res = "";

            for (int i = 0; i < arrayLevel; i++)
                res += "JSArray<";
            res += name;
            for (int i = 0; i < arrayLevel; i++)
                res += ">";

            return res;
        }
    }
}
