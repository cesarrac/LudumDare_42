using UnityEngine;
using System.Collections;

public struct LevelPrototype
{
    public string Name;
    public int difficultyLevel;
    public int darknessLevel;
    public LevelEnemies[] levelEnemies;

    public LevelPrototype(string name, int level, int darknessLevel, LevelEnemies[] levelEnemies)
    {
        this.Name = name;
        this.difficultyLevel = level;
        this.darknessLevel = darknessLevel;
        this.levelEnemies = levelEnemies;
    }
}

public struct LevelEnemies
{
    public string enemyPrototypeName;
    public int count;

    public LevelEnemies(string enemyPrototypeName, int count)
    {
        this.enemyPrototypeName = enemyPrototypeName;
        this.count = count;
    }
}
