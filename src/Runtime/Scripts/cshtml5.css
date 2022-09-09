

/*===================================================================================
*
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*
*   This file is part of both the OpenSilver Runtime (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Runtime (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/




* {
    -webkit-tap-highlight-color: rgba(0,0,0,0);
}

html {
    height: 100%;
    width: 100%;
    margin: 0px;
}

body {
    background-color: white;
    margin: 0px;
    padding: 0px;
    height: 100%;
    width: 100%;
    font-size: 11px;
    overflow-x: hidden;
    overflow-y: hidden;
    cursor: default;
    font-family: 'Segoe UI', Verdana, 'DejaVu Sans', Lucida, 'MS Sans Serif', sans-serif;
    -webkit-touch-callout: none; /* prevents callout to copy image, etc when tap to hold */
    -webkit-text-size-adjust: none; /* prevents webkit from resizing text to fit */
    -webkit-user-select: text; /* 'none' prevents copy paste. 'text' allows it. */
}

.ie_set_p_margins_to_zero p {
    margin-top: 0px; /* This prevents weird additional spaces between lines in multiline TextBoxes in IE. */
    margin-bottom: 0px; /* This prevents weird additional spaces between lines in multiline TextBoxes in IE. */
}

#log {
    position: fixed;
    left: 0;
    bottom: 0;
    pointer-events: none;
    color: #808080;
    word-wrap: break-word;
    max-width: 100%; /* for better word-wrap */
}

#loadingProgress {
    background-color: #DDDDDD;
    z-index: 9999;
    position: relative;
    margin-left: auto;
    margin-right: auto;
    width: 200px;
    height: 3px;
}

#progressBar {
    background-color: #555555;
    margin: 0px;
    padding: 0px;
    height: 3px;
}

#splashScreen {
    margin-left: auto;
    margin-right: auto;
    position: relative;
}

#splashScreenContainer {
    display: table-cell;
    margin-left: auto;
    margin-right: auto;
    position: relative;
    vertical-align: middle;
}

#splashScreenContainerContainer {
    display: table;
    top: 0;
    width: 100%;
    height: 100%;
    position: fixed;
    vertical-align: middle;
}

#progressText {
    display: none;
}

/* Media query used for printing (cf. "CSHTML5.Native.Html.Printing.PrintManager") */
@media print {
    body {
        -webkit-print-color-adjust: exact;
    }

        body * {
            visibility: hidden;
        }

    .section-to-print, .section-to-print * {
        visibility: visible;
    }

    .section-to-print {
        position: fixed;
        left: 0;
        top: 0;
        height: 100%;
        width: 100%;
    }
}
