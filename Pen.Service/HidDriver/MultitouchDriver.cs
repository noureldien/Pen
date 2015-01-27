using System;
using Multitouch.Framework.Input;
using System.Collections.Generic;
using HIDLibrary;
using System.Threading;

namespace Pen.Service
{
    class MultitouchDriver
    {               
        private List<DriverCommunicator> communicators;
        private List<HidDevice> devices;
        private int numOfDevices = 2;

        public void Start()
        {
            devices = new List<HidDevice>(numOfDevices); // id of contacts = [10,130,386,258] respectively
            devices.Add(HidDevices.GetDevice("\\\\?\\hid#unisofthiddevice&col01#1&2d595ca7&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"));
            devices.Add(HidDevices.GetDevice("\\\\?\\hid#unisofthiddevice&col01#1&4784345&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"));
            //devices.Add(HidDevices.GetDevice("\\\\?\\hid#unisofthiddevice&col01#1&29EBA48F&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"));
            //devices.Add(HidDevices.GetDevice("\\\\?\\hid#unisofthiddevice&col01#1&1731F3EA&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"));

            communicators = new List<DriverCommunicator>(numOfDevices);

            for (int i = 0; i < numOfDevices; i++)
                communicators.Add(new DriverCommunicator(devices[i]));
        }

        /// <summary>
        /// Add/ update/ remove a contact point. This is to send touch message to Windows platform
        /// </summary>
        /// <param name="id">User id. Starts from 1.</param>
        /// <param name="x">X-axis value of the position of user contact.</param>
        /// <param name="y">Y-axis value of the position of user contact.</param>
        /// <param name="state">Contact state.</param>
        public void SendContact(int id, int x, int y, HidContactState state)
        {
            communicators[id -1].Enqueue(new HidContactInfo(state, x, y, id));            
        }
        
        /// <summary>
        /// Safely diallocate (free) all resources.
        /// </summary>
        public void Dispose()
        {
            communicators = null;
            devices = null;            
        }
    }
}
