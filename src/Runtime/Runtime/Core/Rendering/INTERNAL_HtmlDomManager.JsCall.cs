using OpenSilver;

namespace CSHTML5.Internal;

public static partial class INTERNAL_HtmlDomManager
{
    private enum JsCall
    {
        [JsCall("ref;document.getXamlRoot()")]
        GetXamlRoot,

        [JsCall(@"void;
let element = document.getElementById({htmlDomElRef.UniqueIdentifier});
if (element) element.parentNode.removeChild(element);
")]
        RemoveFromDom,

        [JsCall("{sDomElement}.childNodes.length")]
        GetChildDomElementAt1,

        [JsCall("let child = {sDomElement}.childNodes[{index,n}]; return child.id;")]
        GetChildDomElementAt2,

        [JsCall("ref;window")]
        HtmlWindow,








        [JsCall("void;{sElement}.focus({{ preventScroll: true }});")]
        SetFocusNative,

        [JsCall("void;document.setContentString({uniqueIdentifier},{content},{removeTextWrapping})")]
        SetContent,

        [JsCall(@"void;
let element = document.getElementById({uniqueIdentifier});
if (element)
{{
element.value = {newText};
element.style.visibility='collapse';
setTimeout(function() {{ let element2 = document.getElementById({uniqueIdentifier}); if (element2) {{ element2.style.visibility='visible'; }} }}, 0);
}}
")]
        SetUIElementContent,

        [JsCall("getTextAreaInnerText({sElement})")]
        GetTextBoxText,

        [JsCall(@"ref;
(function() {{
    let option = document.createElement('option');
    option.text = {elementToAdd};
    {sElement}.add(option, {index,n});
    return option;
}}())")]
        AddOptionToNativeComboBoxAtIndex,

        [JsCall(@"ref;
(function() {{
    let option = document.createElement('option');
    option.text = {elementToAdd};
    {sElement}.add(option);
    return option;
}}())")]
        AddOptionToNativeComboBox,

        [JsCall("void;{sElement}.removeChild({sToRemove})")]
        RemoveChild,

        [JsCall("void;{sElement}.remove({index,n})")]
        RemoveIndex,

        [JsCall("void;let element = document.getElementById({uniqueIdentifier});if (element) {{ element[{attributeName}] = {attributeValue} }};")]
        SetDomElementProperty,
        
        [JsCall("void;document.setDomAttribute({uid},{attributeName},{value})")]
        SetDomElementAttribute,

        [JsCall("void;document.setDomAttribute({uid},{attributeName},{value,d})")]
        SetDomElementAttributeDouble,

        [JsCall("void;let element = document.getElementById({uniqueIdentifier});if (element) {{ element.removeAttribute({attributeName}) }};")]
        RemoveDomElementAttribute,

        [JsCall("ref;{sElement}[{attributeName}]")]
        GetDomElementAttribute,

        [JsCall("void;document.createPopupRootElement('{uniqueIdentifier}', {sRootElement}, '{sPointerEvents}');")]
        CreatePopupRoot,

        [JsCall("void;document.createTextBlockElement({uniqueIdentifier}, {sParentRef}, {wrap})")]
        CreateTextBlock,

        [JsCall("void;document.createCanvasElement({uniqueIdentifier}, {sParentRef})")]
        CreateCanvas,

        [JsCall("void;document.createImageElement({uniqueIdentifier}, {sParentRef})")]
        CreateImage,
        
        [JsCall("void;document.createFrameworkElement({uniqueIdentifier}, {sParentRef}, {enablePointerEvents,b})")]
        CreateFE,

        
        [JsCall("void;document.createRunElement({uniqueIdentifier}, {sParentRef})")]
        CreateRun,

        
        [JsCall("void;document.createShapeOuterElement({uniqueIdentifier}, {sParentRef})")]
        CreateShapeOuter,

        
        [JsCall("void;document.createShapeInnerElement({uniqueIdentifier}, {sParentRef})")]
        CreateShapeInner,

        
        [JsCall("void;document.createElementSafe({domElementTag}, {uniqueIdentifier}, {sParentRef}, {index,n})")]
        CreateDomElement,

        [JsCall(@"void;
let newElement = document.createElement({domElementTag});
newElement.setAttribute('id', {uniqueIdentifier});
let parentElement = document.getElementByIdSafe({parentUniqueIdentifier});
parentElement.children[{insertionIndex,n}].insertAdjacentElement({relativePosition}, newElement);")]
        CreateDomElementAndInsert,

        [JsCall(@"void;
let tempDiv = document.createElement(div);
tempDiv.innerHTML = {domAsString};
let newElement = tempDiv.firstChild;
newElement.setAttribute('id', {uniqueIdentifier});
let parentElement = document.getElementByIdSafe({parentUniqueIdentifier});
parentElement.appendChild(newElement);")]
        CreateDomFromString,















        
        [JsCall(@"void;
let child = document.getElementByIdSafe({childUniqueIdentifier});
let parentElement = document.getElementByIdSafe({parentUniqueIdentifier});
parentElement.appendChild(child);
")]
        AppendChild,

        
        [JsCall(@"ref;
(function(){{
    let domElementAtCoordinates = document.elementFromPoint({x,n}, {y,n});
    if (!domElementAtCoordinates || domElementAtCoordinates === document.documentElement)
    {{
        return null;
    }}
    else
    {{
        return domElementAtCoordinates;
    }}
}}())")]
        FindElementSimulator,

        
        [JsCall("window.elementsFromPointOpensilver({intersectingPoint.X,d},{intersectingPoint.Y,d},{sDiv})")]
        FindElement,
        [JsCall("window.elementsFromPointOpensilver({intersectingPoint.X,d},{intersectingPoint.Y,d},null)")]
        FindElementNull,

        
        [JsCall("$0 == null")]
        GetUIElementFromDomElement0,

        
        [JsCall("{sElement}.id")]
        GetUIElementFromDomElementGetId,
        
        [JsCall("ref;{sElement}.parentNode")]
        GetParentNode,

        [JsCall("void;document.setVisualBounds({style.Uid},{left,d},{top,d},{width,d},{height,d},{bSetPositionAbsolute,n},{bSetZeroMargin,n},{bSetZeroPadding,n})")]
        SetVisualBounds,

        
        [JsCall("void;document.setPosition({style.Uid},{left,d},{top,d},{bSetPositionAbsolute,n},{bSetZeroMargin,n},{bSetZeroPadding,n})")]
        SetPosition,

        [JsCall("let element = document.getElementById({uniqueIdentifier});if (element) {{ $result = element[{methodName}](); }};")]
        CallDomMethod,

        [JsCall("let element = document.getElementById({uniqueIdentifier});if (element) {{ $result = element[{methodName}]({parameters}); }};")]
        CallDomMethodArg,

























        [JsCall(@"void;
let element = document.getElementById({uniqueIdentifier});
if (!element)
    return;

$temp0 = JSON.parse({values});
for (const v of $temp0)
    element.style[v.propertyName] = v.value;
")]
        SetDomElementStyleProperty,

        [JsCall("void;document.velocityHelpers.setDomStyle({sElement}, '{cssPropertyNames}', {sCssValue});")]
        SetDomElementStylePropertyUsingVelocity,










        


    }
}