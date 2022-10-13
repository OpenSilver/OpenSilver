

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OpenSilver.Simulator
{
    public class WpfMediaElementFactory
    {
        internal static Grid _gridWhereToPlaceMediaElements;
        Dictionary<MediaElement, ActionsContainer> _mediaElementToActions = new Dictionary<MediaElement, ActionsContainer>();
        public MediaElement Create(Action mediaElement_Loaded, Action mediaElement_MediaEnded)
        {
            if (_gridWhereToPlaceMediaElements != null)
            {
                MediaElement mediaElement;
                mediaElement = new MediaElement();
                _mediaElementToActions.Add(mediaElement, new ActionsContainer(mediaElement_Loaded, mediaElement_MediaEnded));
                mediaElement.LoadedBehavior = MediaState.Manual; //Wpf forces us to do that so that we can use Play(), Pause(), Stop().
                mediaElement.Loaded += LocalMediaElement_Loaded;
                mediaElement.MediaEnded += LocalMediaElement_MediaEnded;
                _gridWhereToPlaceMediaElements.Children.Add(mediaElement);
                return mediaElement; //todo: check if there is a MediaElement that was already created, that we can reuse (see todo in Remove)
            }
            throw new Exception("Error in the emulator: a mediaElement could not be added. please report this issue via email at support@cshtml5.com");
        }

        void LocalMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement mediaElement = (MediaElement)sender;
            _mediaElementToActions[mediaElement].MediaEndedAction();
        }

        void LocalMediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            MediaElement mediaElement = (MediaElement)sender;
            _mediaElementToActions[mediaElement].LoadedAction();
        }


        public void Remove(MediaElement mediaElement)
        {
            if (_gridWhereToPlaceMediaElements != null)
            {
                _gridWhereToPlaceMediaElements.Children.Remove(mediaElement);
            }
            if (_mediaElementToActions.ContainsKey(mediaElement))
            {
                _mediaElementToActions.Remove(mediaElement);
            }
        }
    }
    internal class ActionsContainer
    {
        internal ActionsContainer(Action loadedAction, Action mediaEndedAction)
        {
            LoadedAction = loadedAction;
            MediaEndedAction = mediaEndedAction;
        }
        internal Action MediaEndedAction;
        internal Action LoadedAction;

    }
}
