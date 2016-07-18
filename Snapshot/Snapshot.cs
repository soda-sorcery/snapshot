using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Threading;
using Xunit;


namespace Snapshot
{
    // a snapshot is an object's picture at a moment in time. any object that implements
    // ISnapshot can be photographed. The class gives you the object, with all its relevant
    // data from an exact moment in the app's lifecycle. 

    public class Snapshot<T> : ISnapshot where T : ISnapshot
    {
        internal Snapshot(T obj, bool isPrivate = false)
        {
            var serializedObj = JsonConvert.SerializeObject(obj);
            Key = obj.GetHashCode();
            _reference = obj;
            _isPrivate = isPrivate;
            CreateDate = DateTime.Now;
            Id = CreateId();
            ObjImage = GetImage(serializedObj);
            
            
        }

        // public properties
        public DateTime CreateDate { get; private set; }

        // the picture of the object
        //  the object from the exact moment in time
        public T ObjImage { get; private set; }
        public string Id { get; private set; }

        // private fields
        private readonly T _reference;
        private readonly bool _isPrivate;
        internal readonly int Key;

        internal void TryCreateSubscription()
        {

            var implementedInterfaces = _reference.GetType().GetInterfaces().FirstOrDefault(r => r == typeof(IAutoSnapshot));
            if (implementedInterfaces == null)
            {
                return;
            }
            var publisher = _reference as INotifyPropertyChanged;

            if (publisher != null)
            {
                publisher.PropertyChanged += OnPropChanged;
            }
        }

        public void OnPropChanged(object sender, PropertyChangedEventArgs e)
        {
            var obj = (T)sender;
            Camera.CreateSnapshot(obj, Key, _isPrivate);
        }


        private T GetImage(string serializedObj)
        {
            var img = JsonConvert.DeserializeObject<T>(serializedObj);
            return img;
        }

        private string CreateId()
        {
            var guid = Guid.NewGuid();
            return guid.ToString();
        }
    }
}
