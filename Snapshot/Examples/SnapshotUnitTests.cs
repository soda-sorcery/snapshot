using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Snapshot.Examples;
using Xunit;
using Xunit.Abstractions;

namespace Snapshot.Examples
{
    public class SnapshotUnitTests
    {

        // in a real-world app, ICamera would be wired into an IoC container at App Start and implemented
        // as a singleton. For the purposes of the example, will have a single reference to Camera in class
        private readonly ITestOutputHelper output;
        private readonly Camera _camera;

        public SnapshotUnitTests(ITestOutputHelper output)
        {
            this.output = output;
            _camera = new Camera();

        }

        [Fact]
        public void Simple_Example_With_Auto_Snapshot()
        {
            // a new superhero is created with a snapshot. This tells the Camera to add
            // a new snapshot for this instance
            var superman = new Superhero("Clark Kent").TakeSnapshot<Superhero>();

            superman.Name = "Superman";

            superman.Name = "Clark";

            superman.Name = "Superman";

            superman.Name = "Clark";

            superman.Name = "Superman";


            // now we want to see all the moments of superman changing from Clark to Superman.
            var snapshots = _camera.GetAllSnapshots(superman);

            // we can see that evertime Clark became Superman, and vice-versa, that we captured
            // the superhero object and the moment in time when it changed.            
            Assert.True(snapshots.Count >= 6);
        }

        [Fact]
        public void Simple_Example_With_Type_Snapshot()
        {
            // we create a superhero instance and simultaneously invoke the Type Snapshot
            var superman = new Superhero("Clark Kent").TakeTypeSnapshot<Superhero>();

            // we create a superhero instance and simultaneously invoke the regular snapshot
            var batman = new Superhero("Bruce").TakeSnapshot<Superhero>();


            batman.Name = "Bruce Wayne";
            superman.Name = "Kal-El";
            batman.Name = "Batman";
            superman.Name = "Superman";

            // we get all the type snapshots
            var typeSnapshots = _camera.GetSnapShotTypeCollection<Superhero>();

            // we get specific snapshots for an instance
            var batmanShots = _camera.GetAllSnapshots(batman);

            // we can also get superman only snapshots
            var supermanShots = _camera.GetAllSnapshots(superman);

            // the type snapshot contains all of the snapshots taken for a type
            Assert.True(typeSnapshots.Count >= 6);

            // only the specific instance snapshots are returned
            Assert.True(batmanShots.Count == 3);

            // we also have all the Superman ones
            Assert.True(supermanShots.Count == 3);

        }

        [Fact]
        public void Simple_Example_With_Private_Snapshot()
        {
            // we create a superhero instance and simultaneously invoke the Type Snapshot
            var superman = new Superhero("Clark Kent").TakeTypeSnapshot<Superhero>();

            // we create a superhero instance and simultaneously invoke the regular snapshot
            var batman = new Superhero("Bruce").TakeSnapshot<Superhero>();

            // we create a superhero instance and hide it from type capturing
            var catwoman = new Superhero("Selena Kyle").TakePrivateSnapshot<Superhero>();


            batman.Name = "Bruce Wayne";
            superman.Name = "Kal-El";
            batman.Name = "Batman";
            superman.Name = "Superman";

            catwoman.Name = "Catwoman";

            // get all snapshots for a type
            var typeSnapshots = _camera.GetSnapShotTypeCollection<Superhero>();

            // get the instance specific 
            var catwomanShots = _camera.GetAllSnapshots(catwoman);

            // the type snapshots will only contain six snapshots instead of 8, because the catwoman
            // instance is a private snapshot
            Assert.True(typeSnapshots.Count >= 6);

            // the catwoman shot will contain all snapshots of the catwoman instance.
            Assert.True(catwomanShots.Count >= 2);
        }

        [Fact]
        public void Simple_Example_Of_Manual_Snapshot()
        {
            // we can take a snapshot on any object that implements ISnapshot. Since SuperVillian doesn't
            // implement IAutoSnapshot, snapshots are not taken automatically
            var lexLuthor = new SuperVillian("Lex Luther").TakeSnapshot<SuperVillian>();
            lexLuthor.HasEvilPlan = true;

            var snapshots = _camera.GetAllSnapshots(lexLuthor);
            // the property change we made wasn't detected
            Assert.True(snapshots.Count == 1);

            // we manually take another snapshot
            lexLuthor.TakeSnapshot<SuperVillian>();
            snapshots = _camera.GetAllSnapshots(lexLuthor);

            // we now have two snaphots, the initial initialization and one after the property changed
            Assert.True(snapshots.Count == 2);
        }

    }

}
