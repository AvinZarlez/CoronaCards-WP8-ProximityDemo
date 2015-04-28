using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CoronaCardsProximityDemo.Resources;


namespace CoronaCardsProximityDemo
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            // Initialize this page's components that were set up via the UI designer.
            InitializeComponent();

            // Set up Corona to automatically start up when the control's Loaded event has been raised.
            // Note: By default, Corona will run the "main.lua" file in the "Assets\Corona" directory.
            //       You can change the defaults via the CoronaPanel's AutoLaunchSettings property.
            fCoronaPanel.AutoLaunchEnabled = true;

            // Set up the CoronaPanel control to render fullscreen via the DrawingSurfaceBackgroundGrid control.
            // This significantly improves the framerate and is the only means of achieving 60 FPS.
            fCoronaPanel.BackgroundRenderingEnabled = true;
            fDrawingSurfaceBackgroundGrid.SetBackgroundContentProvider(fCoronaPanel.BackgroundContentProvider);
            fDrawingSurfaceBackgroundGrid.SetBackgroundManipulationHandler(fCoronaPanel.BackgroundManipulationHandler);

            #region ProximityDevice example initialization
            // Add a Corona event handler which detects when the Corona project has been loaded, but not started yet.
            fCoronaPanel.Runtime.Loaded += OnCoronaRuntimeLoaded;


            // Add a Corona event handler which detects when the Corona project has been started.
            fCoronaPanel.Runtime.Started += OnCoronaRuntimeStarted;

            //For sending and receiving messages
            _proximityDevice = ProximityDevice.GetDefault();
            if (_proximityDevice == null)
            {
                // Device failed to load.
                // TO DO: Add error message
            }
            #endregion
        }

        #region ProximityDevice example
        /// <summary>Called when the Corona runtime has started and finished loaded the "main.lua" file.</summary>
        /// <param name="sender">The CoronaRuntime object that raised this event.</param>
        /// <param name="e">Event arguments providing the CoronaRuntimeEnvironment that has been started.</param>
        private void OnCoronaRuntimeStarted(object sender, CoronaLabs.Corona.WinRT.CoronaRuntimeEventArgs e)
        {

            coronaEventArgs = e;
        }

        /// <summary>
        ///  Called when a new CoronaRuntimeEnvironment has been created/loaded,
        ///  but before the "main.lua" has been executed.
        /// </summary>
        /// <param name="sender">The CoronaRuntime object that raised this event.</param>
        /// <param name="e">Event arguments providing the CoronaRuntimeEnvironment that has been created/loaded.</param>
        private void OnCoronaRuntimeLoaded(object sender, CoronaLabs.Corona.WinRT.CoronaRuntimeEventArgs e)
        {
            e.CoronaRuntimeEnvironment.AddEventListener("requestingMessageBox", OnRequestingMessageBox);

            e.CoronaRuntimeEnvironment.AddEventListener("startSubscribeAndPublish", OnStartSubscribeAndPublish);

            e.CoronaRuntimeEnvironment.AddEventListener("stopSubscribeAndPublish", OnStopSubscribeAndPublish);

        }

        /// <summary>Called when Corona runtime event "requestingMessageBox" has been dispatched.</summary>
        /// <param name="sender">The CoronaRuntimeEnvironment that dispatched the event.</param>
        /// <param name="e">Provides the Lua event table's fields/properties.</param>
        private CoronaLabs.Corona.WinRT.ICoronaBoxedData OnRequestingMessageBox(
            CoronaLabs.Corona.WinRT.CoronaRuntimeEnvironment sender,
            CoronaLabs.Corona.WinRT.CoronaLuaEventArgs e)
        {
            // Fetch the "event.message" property.
            var boxedMessage = e.Properties.Get("message") as CoronaLabs.Corona.WinRT.CoronaBoxedString;
            if (boxedMessage == null)
            {
                // A "message" property was not provided or it was not of type string.
                // Return an error message to Lua describing what went wrong.
                return CoronaLabs.Corona.WinRT.CoronaBoxedString.From("'event.message' is a required field.");
            }

            // Display a native message box with the given string.
            System.Windows.MessageBox.Show(boxedMessage.ToString());

            // Return a success message to Lua.
            return CoronaLabs.Corona.WinRT.CoronaBoxedString.From("Message box was displayed successfully!");
        }

        // Proximity Device
        private ProximityDevice _proximityDevice;
        // Published Message ID
        private long _publishedMessageID = -1;
        // Subscribed Message ID
        private long _subscribedMessageID = -1;

        CoronaLabs.Corona.WinRT.CoronaRuntimeEventArgs coronaEventArgs;

        /// <summary>
        /// Invoked when a message is received
        /// </summary>
        /// <param name="device">The ProximityDevice object that received the message.</param>
        /// <param name="message">The message that was received.</param>
        private void messageReceived(ProximityDevice device, ProximityMessage message)
        {
            //System.Windows.MessageBox.Show("Message receieved: " );

            // This will be converted into a Lua "event" table once dispatched by Corona.
            var eventProperties = CoronaLabs.Corona.WinRT.CoronaLuaEventProperties.CreateWithName("messageReceived");
            eventProperties.Set("message", message.DataAsString);

            // Dispatch the event to Lua.
            var eventArgs = new CoronaLabs.Corona.WinRT.CoronaLuaEventArgs(eventProperties);
            var result = coronaEventArgs.CoronaRuntimeEnvironment.DispatchEvent(eventArgs);
        }

        /// <summary>
        /// When the Subscribe button is pressed, start listening for other devices.
        /// </summary>
        /// <param name="sender">The CoronaRuntimeEnvironment that dispatched the event.</param>
        /// <param name="e">Provides the Lua event table's fields/properties.</param>
        private CoronaLabs.Corona.WinRT.ICoronaBoxedData OnStartSubscribeAndPublish(
            CoronaLabs.Corona.WinRT.CoronaRuntimeEnvironment sender,
            CoronaLabs.Corona.WinRT.CoronaLuaEventArgs e)
        {
            string str = "";

            // Fetch the "event.message" property.
            var boxedMessage = e.Properties.Get("message") as CoronaLabs.Corona.WinRT.CoronaBoxedString;
            if (boxedMessage == null)
            {
                // A "message" property was not provided or it was not of type string.
                // Return an error message to Lua describing what went wrong.
                return CoronaLabs.Corona.WinRT.CoronaBoxedString.From("'event.message' is a required field.");
            }

            // Display a native message box with the given string.
            //System.Windows.MessageBox.Show();

            if (_subscribedMessageID == -1)
            {
                _subscribedMessageID = _proximityDevice.SubscribeForMessage("Windows.ProximityDemo", messageReceived);
                str += "Subscribed";
            }
            else
            {
                str += "Already Subscribed";
            }


            //Stop Publishing the current message.
            if (_publishedMessageID != -1)
                _proximityDevice.StopPublishingMessage(_publishedMessageID);
            string msg = boxedMessage.ToString();
            if (msg.Length > 0)
            {
                _publishedMessageID = _proximityDevice.PublishMessage("Windows.ProximityDemo", msg);
                str += " - Published";
            }
            else
            {
                str += " - Error Length 0";
            }


            // Return a success message to Lua.
            return CoronaLabs.Corona.WinRT.CoronaBoxedString.From("Start pressed: " + str);
        }

        /// <summary>
        /// When the stop subscribing button is pressed, stop listening for other devices.
        /// </summary>
        /// <param name="sender">The CoronaRuntimeEnvironment that dispatched the event.</param>
        /// <param name="e">Provides the Lua event table's fields/properties.</param>
        private CoronaLabs.Corona.WinRT.ICoronaBoxedData OnStopSubscribeAndPublish(
            CoronaLabs.Corona.WinRT.CoronaRuntimeEnvironment sender,
            CoronaLabs.Corona.WinRT.CoronaLuaEventArgs e)
        {

            if (_subscribedMessageID != -1)
            {
                _proximityDevice.StopSubscribingForMessage(_subscribedMessageID);
                _subscribedMessageID = -1;
            }

            if (_publishedMessageID != -1)
            {
                _proximityDevice.StopPublishingMessage(_publishedMessageID);
                _publishedMessageID = -1;
            }
            // Display a native message box with the given string.
            System.Windows.MessageBox.Show("Stopped subscribing and publishing");

            // Return a success message to Lua.
            return CoronaLabs.Corona.WinRT.CoronaBoxedString.From("Stopped subscribing and publishing");
        }
        #endregion
    }
}
