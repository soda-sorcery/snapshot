# Snapshot
Snapshot is an object history library. It allows a user to get the changes of an object throughout the app’s lifecycle. 

## What’s a Snapshot?
Think of it as a picture of an object at a moment in time. The `Snapshot` class has 3 public properties
* `ObjImage` - This is the actual object the snapshot was taken of. It has all the values assigned to its properties from when the object was taken
* `Id` - The unique Id for a snapshot
* `CreateDate` - The date the the Snapshot was created

## What’s the Camera?
The library revolves around a single instance of `Camera`. The `Camera` can be thought of as a container. It creates snapshots of objects and has the ability to retrieve them. The `ICamera` interface needs to be wired up through your IoC Container as a Singleton when your App starts. Then inject it wherever you need to retrieve the snapshots for a particular instance or type.
The `Camera` class comes with a few helpful methods
* `GetAllSnapshots<T>(T obj)` - gets all the snapshots for a specified object
* `GetSnapShotTypeCollection<T>(T obj)` - returns all snapshots for a specific type
* ` GetFirstSnapshot<T>(T obj)` - retrieves the original snapshot from the collection
* `GetLatestSnapShot<T>(T obj)` - gets the most recent snapshot added to the collection

## How Does it All Fit Together?
Taking a Snapshot is easy, instantiate an object and then chain one of the three Snapshot options to your object, like this:

`var superman = new Superhero(“Superman”).TakeSnapshot<Superhero>();`

Now, what’s going on here? This part should be familiar: `var superman = new Superhero(“Superman”)` we’re simply instantiating the `Superhero` class. 
Here’s the fun part: `.TakeSnapshot<Superhero>();` with that, a snapshot has been created. 

The Snapshot library extends the `Object` class to provide this functionality. There are three extension methods:
* `TakeSnapshot<T>`
* `TakeTypeSnapshot<T>`
* `TakePrivateSnapshot<T>`

To use the Snapshot library you must implement one of the following interfaces:
* `IAutoSnapshot` - Automatic
* `ISnapshot` - Manual

## Automatic Snapshots - Using `IAutoSnapshot`
Creates a snapshot when a change happens on the object. This is done by implementing `IAutoSnapshot`, which implements `INotifyPropertyChanged`. Simply add the events to the properties you expect to change.

It’s easier to learn by doing, so let’s do a few examples. The `Superhero` class will be what we use for the `IAutoSnapshot` examples.
```
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
```

You can see in our `Superhero` class that we implement `IAutoSnapshot`. The property we expect to change is the name property, so we raise an event whenever that property changes.


## `TakeSnapshot<T>` Example
Our sample app wants to know every time a superhero changes from their alter-ego to their superhero.


```
public void Simple_Example_With_Auto_Snapshot()
{
    // a new superhero is created with a snapshot. This tells the `Camera` to add
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
    Assert.True(snapshots.Count == 6);
}
```

So what’s going on here? First,  a new superhero is created with a snapshot. This tells the `Camera` to add a new snapshot for this instance

`var superman = new Superhero("Clark Kent").TakeSnapshot<Superhero>();`

After the name property changes a few times, we want to grab all the Snapshots the camera took.

`var snapshots = _camera.GetAllSnapshots(superman);`

We can see that every time Clark became Superman, and vice-versa, that we captured the superhero object and the moment in time when it changed.            

`Assert.True(snapshots.Count == 6);`


## `TakeTypeSnapshot<T>` Example
We can go deeper. Snapshots can also be taken of a type. So let’s say we want to know not just when Clark changes to Superman, but also when Bruce Wayne becomes Batman. Instead of having an observer for each, you can simply observe the type. This means that any type of the same kind, that takes a snapshot of itself, will fall into the type collection. 


```
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
    Assert.True(typeSnapshots.Count == 6);

    // only the specific instance snapshots are returned
    Assert.True(batmanShots.Count == 3);

    // we also have all the Superman ones
    Assert.True(supermanShots.Count == 3);

}
```
We create a superhero instance and simultaneously invoke the `TakeTypeSnapshot`

`var superman = new Superhero("Clark Kent").TakeTypeSnapshot<Superhero>();`

We also create a superhero instance and simultaneously invoke the `TakeSnapshot`

`var batman = new Superhero("Bruce").TakeSnapshot<Superhero>();`

Now we need to get all the type snapshots:

`var typeSnapshots = _camera.GetSnapShotTypeCollection<Superhero>();`

And we also want just the `batman` specific snapshots:

`var batmanShots = _camera.GetAllSnapshots(batman);`

The type snapshot collection contains all of the snapshots taken for the ‘Superhero` type

`Assert.True(typeSnapshots.Count == 6);`

And the `batman` specific collection only contains, yup--you guessed it, `batman` specific snapshots

`Assert.True(batmanShots.Count == 3);`

We can also grab only the `superman` ones too!

`Assert.True(supermanShots.Count == 3);`



It’s a pretty cool way to capture all instantiated types and at the same be able to grab specific sets of snapshots on the fly. 

## `TakePrivateSnapshot<T>` Example
But, what if you don’t want your type being captured? Catwoman isn’t always a hero and might not want her actions known. Luckily for her, we can hide them. Simply use `TakePrivateSnapshot` when the `catwoman` object is created. This allows `catwoman` snapshots to happen without TypeSnapshots picking up the changes




```
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
    Assert.True(typeSnapshots.Count == 6);

    // the catwoman shot will contain all snapshots of the catwoman instance.
    Assert.True(catwomanShots.Count == 2);
}
```
This is very similar to our last example, except for one thing. The `catwoman` object uses `TakePrivateSnapshot<T>`.   

If you’ve watched the count of objects and property changes you would assume that `typeSnapshots` would have a count of eight. But since `catwoman` is private, it never reaches the type collection. And luckily, you can still get all snapshots of `catwoman`.

## Manual Snapshots - Using `ISnapshot`
Now, adding the `INotifyPropertyChanged` interface might not be something you want to do, or need to. But yet, you still want the ability to take a snapshot. Implement `ISnapshot` and you’ll be able to take as many snapshots as you want, just like our `SuperVillian` class.
```
public class SuperVillian : ISnapshot
{
    public SuperVillian(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public bool HasEvilPlan { get; set; }      
}
```

## `TakeSnapshot<T>` Manual Example
We can take a snapshot on any object that implements `ISnapshot`.
```
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

```
Since `SuperVillian` doesn't implement `IAutoSnapshot`, snapshots are not taken automatically.
```
var lexLuthor = new SuperVillian("Lex Luther").TakeSnapshot<SuperVillian>();
lexLuthor.HasEvilPlan = true;
```
Now if we get all the snapshots for `lexLuthor` we’ll only have one item in the collection. To get the updated object in the collection you must call one of the Snapshot extensions, like this:

`lexLuthor.TakeSnapshot<SuperVillian>();`

Now the collection will have both versions of `lexLuthor`.






