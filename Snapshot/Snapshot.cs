using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Diagnostics;
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
        internal Snapshot(T obj, AutoCaptureControls acc, bool isPrivate = false)
        {
            _serializedObj = JsonConvert.SerializeObject(obj);
            Id = CreateHash(obj);
            _key = obj.GetHashCode();
            CreateDate = DateTime.Now;
            ObjImage = GetImage(_serializedObj);
            _reference = obj;
            _isPrivate = isPrivate;           
            TryCreateSubscription();
        }

        // public properties
        public DateTime CreateDate { get; private set; }

        // the picture of the object
        //  the object from the exact moment in time
        public T ObjImage { get; private set; }
        public string Id { get; private set; }

        // private fields
        private readonly string _serializedObj;
        private readonly T _reference;
        private readonly bool _isPrivate;
        private readonly int _key;

        private void TryCreateSubscription()
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
           var updatedObj = (T) sender;
           Camera.CreateSnapshot(updatedObj, _key, _isPrivate);
        }


        private static T GetImage(string _serializedObj)
        {
            var img = JsonConvert.DeserializeObject<T>(_serializedObj);
            return img;
        }


        public static string CreateHash(ISnapshot obj)
        {
            var snapshotHashProvider = new SnapshotHashProvider();
            var serializedObj = JsonConvert.SerializeObject(obj);
            var hash = new StringBuilder();
            var byteArray = Encoding.UTF8.GetBytes(serializedObj + serializedObj.GetHashCode());
            using (var stream = new MemoryStream(byteArray))
            {
                snapshotHashProvider.Key = byteArray;
                snapshotHashProvider.ComputeHash(stream);
                hash.Append(Convert.ToBase64String(snapshotHashProvider.GetFinalizedHashCode()));
            }

            return hash.ToString();
        }


    }




  





}
