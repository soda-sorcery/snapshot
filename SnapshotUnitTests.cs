using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics;

namespace Snapshot
{
    public class UnitTests
    {
        // what about private ctors?
        // what about a base class?
        // need to get all snap shots of the same type -- done


        [Fact]
        public async Task ChangingPropertyShouldTakeASnapshot()
        {
            var p = new Person("Clark Kent").TakeSnapshot<Person>();
            p.Name = "Superman";

            var camera = new Camera();
            var snapshots = await camera.GetAllSnapshots(p);
            var snap = await camera.GetFirstSnapshot(p);

            Assert.True(snapshots.Count == 2);
        }

        [Fact]
        public async Task Doesnt_Include_Opted_Out_Types_In_SnapshotTypeCollection()
        {
            var superHero = new SuperHuman("Superman", "Clark Kent").TakeTypeInstanceSnapshot<SuperHuman>();
            superHero.Strength = 200;
            Debug.WriteLine(superHero.GetType().GetHashCode());
            superHero = superHero.ChangeSuperHeroName("Superdan").TakeSnapshot<SuperHuman>();
            var camera = new Camera();

            var villian = new SuperHuman("Lex Luthor", "Alexis").TakePrivateSnapshot<SuperHuman>();

            var superHeroSnapshots = await camera.GetAllSnapshots(superHero);
            var villianSnapShots = await camera.GetAllSnapshots(villian);
            var superHeroTypeCollection = camera.GetSnapShotTypeCollection(superHero);

            Assert.True(superHeroSnapshots.Count == 1);
            Assert.True(villianSnapShots.Count == 1);
            Assert.True(superHeroTypeCollection.Count == 3);
        }
    }




    internal class Human : ISnapshot
    {
        public string Name { get; set; }

        public int ReturnTwo => 2;
    }

    internal class SuperHuman : Person, ISnapshot
    {
        public SuperHuman(string superHeroName, string name)
            : base(name)
        {
            SuperHeroName = superHeroName;
        }

        private SuperHuman(SuperHuman s)
            : base(s.Name)
        {
            SuperHeroName = s.SuperHeroName;
            Strength = s.Strength;
        }

        public string SuperHeroName { get; private set; }
        public int Strength { get; set; } = 10;

        public SuperHuman ChangeSuperHeroName(string name)
        {
            SuperHeroName = name;
            return new SuperHuman(this);
        }

    }

    internal class SpaceShip : ISnapshot
    {
        public SpaceShip(string shipName)
        {
            Name = shipName;
            Type = "Fighter";
        }

        public SpaceShip(SpaceShip ship)
        {
            Name = ship.Name;
            Type = ship.Type;
        }

        public string Name { get; private set; }

        public string Type { get; private set; }
    }

    internal class Animal : ISnapshot
    {
        public string Type { get; set; }
    }


}
