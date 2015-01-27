using System;
using System.Collections;
using System.IO;
using System.Windows;
using Multitouch.Framework.Input;
using System.Windows.Forms;

namespace Pen.Service
{
    class HidContactInfo : IEquatable<HidContactInfo>
    {
        public HidContactState State { get; private set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }        
        public ushort Id { get; set; }        
        public readonly ushort Width = 10;
        public readonly ushort Height = 10;
        public readonly ushort Pressure = 100;
        public DateTime Timestamp { get; set; }
        private const ushort MaxSize = 32767;
        private static readonly double XRatio = ((double)SystemInformation.VirtualScreen.Width) / MaxSize;
        private static readonly double YRatio = ((double)SystemInformation.VirtualScreen.Height) / MaxSize;

        /// <summary>
        /// Size of one contact in bytes
        /// </summary>
        public const byte HidContactInfoSize = 14;       
        
        public bool TipSwitch
        {
            get
            {
                switch (State)
                {
                    case HidContactState.Adding:
                    case HidContactState.Removing:
                    case HidContactState.Removed:
                        return false;
                    case HidContactState.Updated:
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool InRange
        {
            get
            {
                switch (State)
                {
                    case HidContactState.Adding:
                    case HidContactState.Removing:
                    case HidContactState.Updated:
                        return true;
                    case HidContactState.Removed:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Used to create a new HidContactInfo from another HidContactInfo object.
        /// The boolean null value is used to defferibtiate between the two overloads of this constructor.
        /// </summary>        
        internal HidContactInfo(HidContactState state, ushort x, ushort y, ushort id, bool nullValue)
        {
            State = state;
            X = x;
            Y = y;
            Id = id;            
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Used to create a new HidContactInfo object using basic values as: id, x, y and state
        /// </summary>
        public HidContactInfo(HidContactState state, int x, int y, int id)
        {
            State = state;
            Point point = GetPoint(new Point(x, y));
            X = Convert.ToUInt16(point.X);
            Y = Convert.ToUInt16(point.Y);            
            Id = (ushort)id;
            Timestamp = DateTime.Now;
        }        

        internal static Point GetPoint(Point position)
        {
            return new Point(Math.Max(0, position.X / XRatio), Math.Max(0, position.Y / YRatio));
        }

        public bool Equals(HidContactInfo obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.Id == Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(HidContactInfo)) return false;
            return Equals((HidContactInfo)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        internal byte[] ToBytes()
        {
            byte[] buffer = new byte[HidContactInfoSize];
            BitArray bits = new BitArray(new[] { TipSwitch, InRange });
            bits.CopyTo(buffer, 0);
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream(buffer)))
            {
                writer.Seek(2, SeekOrigin.Begin);
                writer.Write(X);
                writer.Write(Y);
                writer.Write(Pressure);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(Id);
            }
            return buffer;
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, State: {1}, TipSwitch: {2}, InRange: {3}, X,Y: {4},{5}, W,H: {6},{7}, Pressure: {8}, TimeStamp: {9}",
                Id, State, TipSwitch, InRange, X, Y, Width, Height, Pressure, Timestamp.Ticks);
        }
    }
}
