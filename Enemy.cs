class Enemy
{
    public string Name { get; set; }
    public int Hp { get; set; }
    public int Damage { get; set; }
    public int Reward { get; set; }
    public bool IsStunning { get; set; }

    public Enemy(string name, int hp, int damage, int reward, bool isStunning = false)
    {
        Name = name;
        Hp = hp;
        Damage = damage;
        Reward = reward;
        IsStunning = isStunning;
    }
}