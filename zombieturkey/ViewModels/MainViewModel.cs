using Avalonia;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace zombieturkey.ViewModels;

public class MainViewModel : ViewModelBase
{
    ObservableCollection<Sprite> Sprites { get; } = new ObservableCollection<Sprite>();
    private readonly List<Zombie> zombies = new();
    private readonly List<Swamp> swamps = new();
    private readonly Donkey donkey = new() { Diameter=20};
    private readonly Sprite carrot = new() { Diameter=3};
    private double zombiespeed = 2;
    private double donkeyspeed = 3;

    private int _level = 1;
    public int Level
    {
        get { return _level; }
        set { this.RaiseAndSetIfChanged(ref _level, value); }
    }
    
    bool _running = false;
    public bool Running
    {
        get { return _running; }
        set { this.RaiseAndSetIfChanged(ref _running, value); }
    }
    
    string _status = "Start";
    public string Status
    {
        get { return _status; }
        set { this.RaiseAndSetIfChanged(ref _status, value); }
    }
   
    public MainViewModel()
    {
        Level = 1;
        GenerateLevel();
    }

    public void Tick()
    {
        if (!Running) return;

        foreach (var zombie in zombies)
        {
            if (zombie.IsAlive)
            {
                zombie.MoveTowards(donkey, zombiespeed);
            }
        }
        donkey.MoveTowards(carrot, donkeyspeed);

        bool stillZombies = false;
        foreach (var zombie in zombies)
        {
            if (zombie.IsAlive)
            {
                foreach (var swamp in swamps)
                {
                    if (zombie.Collided(swamp))
                    {
                        zombie.IsAlive = false;
                        break;
                    }
                    else
                    {
                        stillZombies = true;
                    }
                }
                if (zombie.IsAlive && zombie.Collided(donkey))
                {
                    StopGame("Retry");
                    break;
                }
            }
        }
        foreach (var swamp in swamps)
        {
            if (donkey.Collided(swamp))
            {
                StopGame("Retry");
                return;
            }
        }

        if (!stillZombies)
        {
            StopGame("Next Level");
            Level++;
        }
    }
    private void StopGame(string status)
    {
        Status = status;
        Running = false;
    }

    public void NewGame()
    {
        GenerateLevel();
        Running = true;
    }
    private void GenerateLevel()
    {
        var random = new Random(Level);
        // todo make levels > 10 get harder
        zombiespeed = Math.Min(2.0 + (double)Level / 10.0, donkeyspeed);

        Sprites.Clear();
        zombies.Clear();
        swamps.Clear();
      
        donkey.X = 400;
        donkey.Y = 250;

        carrot.X = donkey.X;
        carrot.Y = donkey.Y;

        for (int i = 0; i < 16; i++)
        {
            Zombie zombie;
            do
            {
                zombie = new Zombie() { Diameter = 25,
                    X = random.NextDouble() * 770 + 15, 
                    Y = random.NextDouble() * 570 + 15 };
            } while (zombie.Distance(donkey) < 200);
            zombies.Add(zombie);
            Sprites.Add(zombie);
        }

        for (int i = 0; i < 16; i++)
        {
            Swamp swamp;
            do
            {
                swamp = new Swamp() { Diameter = 20 + random.NextDouble() * 20, 
                    X = random.NextDouble() * 700 + 50,
                    Y = random.NextDouble() * 500 + 5 };

            } while (swamp.Distance(donkey) < 50);
            swamps.Add(swamp);
            Sprites.Add(swamp);
        }

        Sprites.Add(donkey);
        Sprites.Add(carrot);

    }
    public void SetPointer(Point pos)
    {
       carrot.TopLeft = pos;
    }
}
class Sprite : ViewModelBase
{
    private double _diameter;
    public double Diameter
    {
        get { return _diameter; }
        set { this.RaiseAndSetIfChanged(ref _diameter, value); }
    }

    private Point _TopLeft;
    public Point TopLeft
    {
        get { return _TopLeft; }
        set { this.RaiseAndSetIfChanged(ref _TopLeft, value); }
    }
    public double X
    {
        set { TopLeft = TopLeft.WithX(value - Diameter / 2); }
        get { return TopLeft.X + Diameter / 2; }
    }
    public double Y
    {
        set { TopLeft = TopLeft.WithY(value - Diameter / 2); }
        get { return TopLeft.Y + Diameter / 2; }
    }
    public bool Collided(Sprite other)
    {
        return Distance(other) < (Diameter + other.Diameter) / 2;
    }
    public double Distance(Sprite other)
    {
        return Math.Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));
    }
    public void MoveTowards(Sprite target, double speed)
    {
        double angle = Math.Atan2(target.X - X, target.Y - Y);
        X += Math.Sin(angle) * speed;
        Y += Math.Cos(angle) * speed;
    }
}
class Zombie : Sprite
{
    public bool _IsAlive = true;
    public bool IsAlive
    {
        get { return _IsAlive; }
        set { this.RaiseAndSetIfChanged(ref _IsAlive, value); }
    }
}
class Donkey : Sprite
{

}
class Swamp : Sprite
{

}