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
    public class AutoSnapshotUnitTests
    {
        private readonly ITestOutputHelper output;
        private readonly Camera _camera;

        public AutoSnapshotUnitTests(ITestOutputHelper output)
        {
            this.output = output;
            _camera = new Camera();

        }

        [Fact]
        public void should_create_a_snapshot()
        {
            var human = new Superhero("Clark").TakeSnapshot<Superhero>();
            human.Name = "Daniel";
            human.Name = "Morgan";
            var snapshots = _camera.GetAllSnapshots(human);
            Assert.True(snapshots.Count >= 1);
        }

        [Fact]
        public void should_take_snapshots_of_multiple_references()
        {
            var human = new Superhero("Peter Park").TakeTypeSnapshot<Superhero>();
            var h2 = new Superhero("Steve Rogers").TakeSnapshot<Superhero>();
            human.Name = "Spider-Man";
            h2.Name = "Captain America";

            var typeSnapshots = _camera.GetSnapShotTypeCollection(human);
            var h2Snapshots = _camera.GetAllSnapshots(h2);

            Assert.True(typeSnapshots.Count >= 4);
            Assert.True(h2Snapshots.Count == 2);

        }

        [Fact]
        public void should_retrieve_the_first_instaniation_from_the_collection()
        {
            var human = new Superhero("Clark Kent").TakeSnapshot<Superhero>();
            human.Name = "Kal-El";
            human.Name = "Superman";
            var snapshot = _camera.GetFirstSnapshot(human);
            Assert.True(snapshot.ObjImage.Name == "Clark Kent");
        }

        [Fact]
        public void should_retrieve_the_lastest_instantiation_from_the_collection()
        {
            var human = new Superhero("Clark Kent").TakeSnapshot<Superhero>();
            human.Name = "Kal-El";
            human.Name = "Superman";
            var snapshot = _camera.GetLatestSnapShot(human);
            Assert.True(snapshot.ObjImage.Name == "Superman");
        }

        [Fact]
        private void private_snaphsot_should_not_be_in_type_collection()
        {
            var human = new Superhero("Peter Parker").TakeTypeSnapshot<Superhero>();
            var h2 = new Superhero("Barry Allen").TakePrivateSnapshot<Superhero>();
            human.Name = "Spiderman";
            h2.Name = "Flash";

            var typeSnapshots = _camera.GetSnapShotTypeCollection(human);
            var h2Snapshots = _camera.GetAllSnapshots(h2);

            Assert.True(typeSnapshots.Count == 2);
            Assert.True(h2Snapshots.Count == 2);
        }
    }






    public class Superhero : IAutoSnapshot
    {
        public Superhero(string name)
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
