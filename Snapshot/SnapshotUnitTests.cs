using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit.Abstractions;
using Xunit.Runner.Reporters;

namespace Snapshot
{
    public class UnitTests
    {
        // what about private ctors?
        // what about a base class?
        // need to get all snap shots of the same type -- done

        private readonly ITestOutputHelper output;
        private readonly Camera _camera;

        public UnitTests(ITestOutputHelper output)
        {
            this.output = output;
            _camera = new Camera();

        }

        [Fact]
        public async Task should_create_a_snapshot()
        {
            var human = new Human("Clark").TakeSnapshot<Human>();
            human.Name = "Daniel";
            human.Name = "Morgan";
            var snapshots = await _camera.GetAllSnapshots(human);
            Assert.True(snapshots.Count >= 1);
        }

        [Fact]
        public async Task should_take_snapshots_of_multiple_references()
        {
            var human = new Human("Clark Kent").TakeTypeInstanceSnapshot<Human>();
            var h2 = new Human("Bruce Wayne").TakeSnapshot<Human>();
            human.Name = "Superman";
            h2.Name = "Batman";

            var typeSnapshots = await _camera.GetSnapShotTypeCollection(human);
            var h2Snapshots = await _camera.GetAllSnapshots(h2);

            Assert.True(typeSnapshots.Count == 4);
            Assert.True(h2Snapshots.Count == 2);

        }

        [Fact]
        public async Task should_retrieve_the_first_instaniation_from_the_collection()
        {
            var human = new Human("Clark Kent").TakeSnapshot<Human>();
            human.Name = "Kal-El";
            human.Name = "Superman";
            var snapshot = await _camera.GetFirstSnapshot(human);
            Assert.True(snapshot.ObjImage.Name == "Clark Kent");
        }

        [Fact]
        public async Task should_retrieve_the_lastest_instantiation_from_the_collection()
        {
            var human = new Human("Clark Kent").TakeSnapshot<Human>();
            human.Name = "Kal-El";
            human.Name = "Superman";
            var snapshot = await _camera.GetLatestSnapShot(human);
            Assert.True(snapshot.ObjImage.Name == "Superman");
        }
    }



   


    public class Human : IAutoSnapshot
    {
        public Human(string name)
        {
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }


    }

   

    


}
